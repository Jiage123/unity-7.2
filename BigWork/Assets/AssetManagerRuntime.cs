
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BuildInfo 
{
    public int BuildVersion;
    public Dictionary<string, ulong> FileNames = new Dictionary<string, ulong>();
    public ulong FizeTotalSize;
}



//新老版本AB包对比
public class AssetBundleVersionDiffence
{
    //新增资源包
    public List<string> AdditionAssetBundles = new List<string>();
    //移除资源包
    public List<string> ReduceAssetBunddles = new List<string>();
}
public enum AssetBundlePattern
{
    //编辑器模拟加载，使用AssetDatabase进行加载，不用打包
    EditorSimultion,
    //本地加载模式，打包到本地或者StreamingAssets路径下，从这个路径加载
    Local,
    //远端加载模式，打包到任意资源服务器地址，通过网络进行下载，下载到沙盒路径persistDataPath后加载
    Remote
}


//压缩包格式
public enum AssetBundleCompressionPattern
{
    LZMA,
    LZ4,
    None
}

//Package打包后记录的信息
public class PackageBuildInfo
{

    public string PackageName;
    public List<AssetBuildInfo> AssetInfos = new List<AssetBuildInfo>();
    public List<string> PackageDenpdecies = new List<string>();

    //代表是否为起始包
    public bool IsSourcePackage = false;
}

//Package中的Asset打包后记录的信息
public class AssetBuildInfo
{
    //资源名称
    //当需要加载资源时，应该和该字符串相同
    public string AssetName;

    //该资源属于哪一个AB包
    public string AssetBundleName;
}


//是否是增量打包
//任何BuildOption处于非ForceBuild下都视为增量打包
public enum IncrementalBuildMode
{
    None,
    IncreamentalBuild,
    ForceBuild
}
public class AssetPackage {
    public PackageBuildInfo PackageInfo;
    public string PackageName { get { return PackageInfo.PackageName; } }

    Dictionary<string, Object> LoadedAssets = new Dictionary<string, Object>();

    public T LoadAsset<T>(string assetName)where T:Object
    {
        T assetObject = default;
        foreach (AssetBuildInfo info in PackageInfo.AssetInfos)
        {
            if (info.AssetName==assetName)
            {
                if (LoadedAssets.ContainsKey(assetName))
                {
                    assetObject = LoadedAssets[assetName] as T;
                    return assetObject;
                }
                foreach (string dependAssetName in AssetManagerRuntime.Instance.Manifest.GetAllDependencies(info.AssetBundleName)) 
                {
                    string dependAssetBundlePath = Path.Combine(AssetManagerRuntime.Instance.AssetBundlePath, dependAssetName);
                    AssetBundle.LoadFromFile(dependAssetBundlePath);
                }

                string assetBundlePath = Path.Combine(AssetManagerRuntime.Instance.AssetBundlePath, info.AssetBundleName);

                AssetBundle bundle = AssetBundle.LoadFromFile(assetBundlePath);
                assetObject = bundle.LoadAsset<T>(assetName);
            }
        }
        if (assetObject == null)
        {
            Debug.LogError($"{assetName}未在{PackageName}中找到");
        }

        return assetObject;
    }
}
public class AssetManagerRuntime 
{
    //当前资源包模式
    AssetBundlePattern CurrentPattern;

    //当前类的单例
    public static AssetManagerRuntime Instance;

    //所有本地Asset所处路径
    //应该在AssetBundleLoadPath的上一层
    public string LocalAssetPath;

    //AB包加载路径
    public string AssetBundlePath;

    //资源下载路径
    //下载完成后要将资源放在LocalAssetPath中
    public string DownLoadPath;

    //用于对比本地资源版本和远端版本
    public int LocalAssetVersion;

    //本次运行访问到的资源服务号
    public int RemoteAssetVersion;

    //本地所有Package信息
    List<string> PackageNames;

    //本地所有已加载的Package
    Dictionary<string, AssetPackage> LoadedAssetPackages = new Dictionary<string, AssetPackage>();

    public AssetBundleManifest Manifest;
    public static void AssetManagerInit( AssetBundlePattern pattern)
    {
        if (Instance == null)
        {
            Instance = new AssetManagerRuntime();
            Instance.CurrentPattern = pattern;
            Instance.CheckLocalAssetPath();
            Instance.CheckLocalAssetVersion();
            Instance.CheckAssetBundleLoadPath();
            
            
        }
    }

    void CheckLocalAssetPath()
    {
        switch (CurrentPattern)
        {
            case AssetBundlePattern.EditorSimultion:
                break;
            case AssetBundlePattern.Local:
                LocalAssetPath = Path.Combine(Application.streamingAssetsPath,"LocalAssets");
                break;
            case AssetBundlePattern.Remote:
                DownLoadPath = Path.Combine(Application.persistentDataPath, "DownLoadAssets");
                LocalAssetPath = Path.Combine(Application.persistentDataPath, "BuildOutput", "LocalAssets");
                break;
            default:
                break;
        }
    }
    void CheckLocalAssetVersion()
    {
        string versionFilePath = Path.Combine(LocalAssetPath, "LocalVersion.version");
        if (!File.Exists(versionFilePath))
        {
            LocalAssetVersion = 100;
            File.WriteAllText(versionFilePath, LocalAssetVersion.ToString());
        }
        LocalAssetVersion = int.Parse(File.ReadAllText(versionFilePath));
    }
    void CheckAssetBundleLoadPath()
    {
        AssetBundlePath = Path.Combine(LocalAssetPath, LocalAssetVersion.ToString());
    }

    public void UpdateLocalAssetVersion()
    {
        LocalAssetVersion = RemoteAssetVersion;
        string versionFilePath = Path.Combine(LocalAssetPath, "LocalVersion.version");

        File.WriteAllText(versionFilePath, LocalAssetVersion.ToString());

        CheckAssetBundleLoadPath();

        Debug.Log($"本地版本更新完成{LocalAssetVersion}");
    }

    public AssetPackage LoadPackage(string packageName)
    {
        string packagePath = null ;
        string packageString = null;
        if (PackageNames == null)
        {
            packagePath = Path.Combine(AssetBundlePath, "AllPackages");
            packageString = File.ReadAllText(packagePath);

            PackageNames = JsonConvert.DeserializeObject<List<string>>(packageString);
        }

        if (!PackageNames.Contains(packageName))
        {
            Debug.LogError($"{packageName}包本地包列表不存在");
            return null;
        }

        if (Manifest==null)
        {
            string mainBundlePath = Path.Combine(AssetBundlePath, "LocalAssets");

            AssetBundle mainBundle = AssetBundle.LoadFromFile(mainBundlePath);
            Manifest = mainBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
        }
        AssetPackage assetPackage = null;
        if (LoadedAssetPackages.ContainsKey(packageName)) 
        {
            assetPackage = LoadedAssetPackages[packageName];
            Debug.LogWarning($"{packageName}已经加载");
            return assetPackage;
        }
        assetPackage = new AssetPackage();


        packagePath= Path.Combine(AssetBundlePath, packageName);
        packageString = File.ReadAllText(packagePath);
        assetPackage.PackageInfo = JsonConvert.DeserializeObject<PackageBuildInfo>(packageString);

        LoadedAssetPackages.Add(assetPackage.PackageName, assetPackage);

        foreach (string denpendName in assetPackage.PackageInfo.PackageDenpdecies)
        {
            LoadPackage(denpendName);
        }
        return assetPackage;
    }
}
