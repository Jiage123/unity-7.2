
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



//���ϰ汾AB���Ա�
public class AssetBundleVersionDiffence
{
    //������Դ��
    public List<string> AdditionAssetBundles = new List<string>();
    //�Ƴ���Դ��
    public List<string> ReduceAssetBunddles = new List<string>();
}
public enum AssetBundlePattern
{
    //�༭��ģ����أ�ʹ��AssetDatabase���м��أ����ô��
    EditorSimultion,
    //���ؼ���ģʽ����������ػ���StreamingAssets·���£������·������
    Local,
    //Զ�˼���ģʽ�������������Դ��������ַ��ͨ������������أ����ص�ɳ��·��persistDataPath�����
    Remote
}


//ѹ������ʽ
public enum AssetBundleCompressionPattern
{
    LZMA,
    LZ4,
    None
}

//Package������¼����Ϣ
public class PackageBuildInfo
{

    public string PackageName;
    public List<AssetBuildInfo> AssetInfos = new List<AssetBuildInfo>();
    public List<string> PackageDenpdecies = new List<string>();

    //�����Ƿ�Ϊ��ʼ��
    public bool IsSourcePackage = false;
}

//Package�е�Asset������¼����Ϣ
public class AssetBuildInfo
{
    //��Դ����
    //����Ҫ������Դʱ��Ӧ�ú͸��ַ�����ͬ
    public string AssetName;

    //����Դ������һ��AB��
    public string AssetBundleName;
}


//�Ƿ����������
//�κ�BuildOption���ڷ�ForceBuild�¶���Ϊ�������
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
            Debug.LogError($"{assetName}δ��{PackageName}���ҵ�");
        }

        return assetObject;
    }
}
public class AssetManagerRuntime 
{
    //��ǰ��Դ��ģʽ
    AssetBundlePattern CurrentPattern;

    //��ǰ��ĵ���
    public static AssetManagerRuntime Instance;

    //���б���Asset����·��
    //Ӧ����AssetBundleLoadPath����һ��
    public string LocalAssetPath;

    //AB������·��
    public string AssetBundlePath;

    //��Դ����·��
    //������ɺ�Ҫ����Դ����LocalAssetPath��
    public string DownLoadPath;

    //���ڶԱȱ�����Դ�汾��Զ�˰汾
    public int LocalAssetVersion;

    //�������з��ʵ�����Դ�����
    public int RemoteAssetVersion;

    //��������Package��Ϣ
    List<string> PackageNames;

    //���������Ѽ��ص�Package
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

        Debug.Log($"���ذ汾�������{LocalAssetVersion}");
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
            Debug.LogError($"{packageName}�����ذ��б�����");
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
            Debug.LogWarning($"{packageName}�Ѿ�����");
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
