using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class HelloWorld : MonoBehaviour
{
    public AssetBundlePattern LoadPattern;

    //AssetBundle SampleBundle;
    //最早的主包

    AssetBundle CubeBundle;
    AssetBundle SphereBundle;

    GameObject SampleObject;

    public Button LoadAssetBundleButton;
    public Button LoadAssetButton;
    public Button UnLoadFalseButton;
    public Button UnLoadTrueButton;

    //打包的包名按理需要Editor类管理，但资源加载类也要访问，故放在此处
    //  public static string MainAssetBundleName = "SampleAssetBundle";

    //除了主包外，实际包名都小写
    // public static string ObjectAssetBundleName = "resoursesbundle";

    public string AssetBundleLoadPath;

    public string HTTPAddress = "http://192.168.203.37:8080/";

    string RemoteVersionPath;
    string DownloadVersionPath;


    //public string HTTPAddressBundlePath;

    //public string DownloadPath;
    // Start is called before the first frame update
    void Start()
    {

        AssetManagerRuntime.AssetManagerInit(LoadPattern);
        if (LoadPattern == AssetBundlePattern.Remote)
        {
            StartCoroutine(GetRemoteVersion());
        }
        else
        {
            LoadAsset();
        }


        //AssetPackage assetPackage = AssetManagerRuntime.Instance.LoadPackage("AA");
        //Debug.Log(assetPackage.PackageName);

        //GameObject sampleObject = assetPackage.LoadAsset<GameObject>("Assets/Resources/Capsule.prefab");
        //Instantiate(sampleObject);


        //// CheckAssetBundleLoadPath();
        //// LoadAssetBundleButton.onClick.AddListener(CheckAssetBundlePattern);

        //// LoadAssetButton.onClick.AddListener(LoadAsset);
        //// UnLoadFalseButton.onClick.AddListener(()=>
        //// {
        ////     //点击事件无法直接传入带参数的方法，使用命名方法
        ////     UnloadAssetBundle(false);

        //// });
        //// UnLoadTrueButton.onClick.AddListener(() =>
        ////{
        ////    UnloadAssetBundle(true);


        ////});

    }

    Downloader SampleDownloader;
    string FileSavePath;
    string FilesURL;

    void DownloaderTest()
    {
        SampleDownloader = new Downloader(FilesURL, FileSavePath, onCompleted, onProgress, onError);

        SampleDownloader.Start();
    }

    void LoadAsset()
    {
        AssetPackage package = AssetManagerRuntime.Instance.LoadPackage("A");
        GameObject obj = package.LoadAsset<GameObject>("Assets/Resources/Cube.prefab");

        Instantiate(obj);
    }

    DownloadInfo CurrentDownloadInfo;
    void onCompleted(string fileName, string message)
    { 
        if (!CurrentDownloadInfo.DownloadFileNames.Contains(fileName))
        {
           
            CurrentDownloadInfo.DownloadFileNames.Add(fileName);
        }
        
        switch (fileName)
        {
            case"AllPackages":
                
                CreatePackagesownloadList();
                break;
            case "AssetBundleHashs":
                CreateAssetBundleDownloadList();
                break;
        }
        string downloadInfoString = JsonConvert.SerializeObject(CurrentDownloadInfo);
        string downloadSavePath = Path.Combine(AssetManagerRuntime.Instance.DownLoadPath, "TempDownloadInfo");

        File.WriteAllText(downloadSavePath, downloadInfoString);
        Debug.Log($"{fileName}:{message}");
    }

    int downloadCount =1;
    void onProgress(float progress,long currentLength,long totalLength)
    {
        Debug.Log($"下载进度{progress * 100}%,当前下载长度{currentLength * 1.0f / 1024 / 1024}M,文件总长度：{totalLength * 1.0f / 1024 / 1024}M");
        if (currentLength!=totalLength)
        {
            if (currentLength>totalLength/10.0f*downloadCount)
            {
                Debug.Log(currentLength);
                Debug.Log(totalLength);
                downloadCount++;
                //SampleDownloader.Dispose();
                Debug.Log("下载中断测试");
            }
        }
    }

    void onError(ErrorCode errorCode,string message)
    {
        Debug.LogError(message);
    }
    void CreateAssetBundleDownloadList()
    {
        Debug.Log("你好");
        string assetBundleHashsPath = Path.Combine(DownloadVersionPath, "AssetBundleHashs");
        string assetBundleHashsString = File.ReadAllText(assetBundleHashsPath);

        //远端包列表
        string[] remoteAssetsBundleHashs = JsonConvert.DeserializeObject<string[]>(assetBundleHashsString);


        string localAssetBundleHashPath = Path.Combine(AssetManagerRuntime.Instance.AssetBundlePath, "AssetBundleHashs");
        string assetBundleHashString = "";

        string[] localAssetBundleHash = null;
        if (File.Exists(localAssetBundleHashPath))
        {
            assetBundleHashString = File.ReadAllText(localAssetBundleHashPath);

            localAssetBundleHash = JsonConvert.DeserializeObject<string[]>(assetBundleHashString);

        }
        List<string> downloadAssetNames = null;
        if (localAssetBundleHash==null)
        {
            Debug.Log("本地表读取失败，下载远端表");
            downloadAssetNames = remoteAssetsBundleHashs.ToList();
        }
        else
        {
            AssetBundleVersionDiffence diffence = ContrastAssetBundleVersion(localAssetBundleHash, remoteAssetsBundleHashs);
            downloadAssetNames = diffence.AdditionAssetBundles;
        }

        //添加主包包名
        downloadAssetNames.Add("LocalAssets");
        Downloader downloader = null;
        foreach (string assetBundleName in downloadAssetNames)
        {
            string fileName = assetBundleName;
            if (assetBundleName.Contains("_"))
            {
                //下划线后最后一位是AssetbundleName
                int startIndex = assetBundleName.IndexOf("_") + 1;
                fileName = assetBundleName.Substring(startIndex);
            }
            if (!CurrentDownloadInfo.DownloadFileNames.Contains(fileName))
            {
                string fileURL = Path.Combine(RemoteVersionPath, fileName);
                string fileSavePath = Path.Combine(DownloadVersionPath, fileName);
                downloader = new Downloader(fileURL, fileSavePath, onCompleted, onProgress, onError);
                downloader.Start();
            }
            else
            {
                
                onCompleted(fileName, "本地已存在");
            }
        }
        CopyDownloadAssetsToLocalPath();
        AssetManagerRuntime.Instance.UpdateLocalAssetVersion();
        LoadAsset();

    }
    void CreatePackagesownloadList()
    {
        string allPackagesPath = Path.Combine(DownloadVersionPath, "AllPackages");
        string allPackagesString = File.ReadAllText(allPackagesPath);
        List<string> allPackages = JsonConvert.DeserializeObject<List<string>>(allPackagesString);

        Downloader downloader = null;
        foreach (string packageName in allPackages)
        {
            if (!CurrentDownloadInfo.DownloadFileNames.Contains(packageName))
            {
                string remotePackageePath = Path.Combine(RemoteVersionPath, packageName);
                string remotePackagesSavePath = Path.Combine(DownloadVersionPath, packageName);
                downloader = new Downloader(remotePackageePath, remotePackagesSavePath, onCompleted, onProgress, onError);
                downloader.Start();
                
            }
            else
            {
                onCompleted(packageName, "本地已存在");
                
            }
        }
        if (!CurrentDownloadInfo.DownloadFileNames.Contains("AssetBundleHashs"))
        {
            string remoteFilePath = Path.Combine(RemoteVersionPath, "AssetBundleHashs");
            string remoteFileSavePath = Path.Combine(DownloadVersionPath, "AssetBundleHashs");
            downloader = new Downloader(remoteFilePath, remoteFileSavePath, onCompleted, onProgress, onError);
            downloader.Start();
            
        }
        else
        {
            onCompleted("AssetBundleHashs", "本地已存在");
            
        }
    }
    BuildInfo RemoteBuildInfo;

    IEnumerator GetRemoteVersion()
    {
        #region 获取远端版本号
        string remoteVersionFilePath = Path.Combine(HTTPAddress, "BuildOutput", "BuildVersion.version");
        UnityWebRequest request = UnityWebRequest.Get(remoteVersionFilePath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // 返回NULL代表等待一帧
            yield return null;

        }
        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.error);
            yield break;
        }

        int version = int.Parse(request.downloadHandler.text);

        if (AssetManagerRuntime.Instance.LocalAssetVersion == version)
        {
            LoadAsset();
            yield break;
        }
        //使用变量保存远端版本
        AssetManagerRuntime.Instance.RemoteAssetVersion = version;

        #endregion

        RemoteVersionPath = Path.Combine(HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());
        DownloadVersionPath = Path.Combine(AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());

        if (!Directory.Exists(DownloadVersionPath))
        {
            Directory.CreateDirectory(DownloadVersionPath);
        }

        //StartCoroutine(GetRemotePackages());

        Debug.Log(DownloadVersionPath);
      
        Debug.Log($"远端资源版本为{version}");

        #region 获取远端buildInfo
        string downloadPath = Path.Combine
            (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());

        string remoteBuildInfoPath = Path.Combine(HTTPAddress, "BuildOutput", version.ToString(), "BuildInfo");

        request= UnityWebRequest.Get(remoteBuildInfoPath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // 返回NULL代表等待一帧
            yield return null;

        }
        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.error);
            yield break;
        }

        string buildInfoString = request.downloadHandler.text;
        RemoteBuildInfo = JsonConvert.DeserializeObject<BuildInfo>(buildInfoString);

        if (RemoteBuildInfo==null||RemoteBuildInfo.FizeTotalSize<=0)
        {
            yield break;
        }
        #endregion
        CreateDownloadList();


        //if (!Directory.Exists(downloadPath))
        //{
        //    Directory.CreateDirectory(downloadPath);
        //}
        //if (AssetManagerRuntime.Instance.LocalAssetVersion!=AssetManagerRuntime.Instance.RemoteAssetVersion)
        //{
        //    StartCoroutine(GetRemotePackages());
        //}
        //yield return null;
    }
    IEnumerator GetRemotePackages()
    {
        string remotePackagePath = Path.Combine
            (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), "AllPackages");
        UnityWebRequest request = UnityWebRequest.Get(remotePackagePath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // 返回NULL代表等待一帧
            yield return null;

        }
        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.error);
            yield break;
        }

        string allPackagesString = request.downloadHandler.text;


        string packageSavePath = Path.Combine
            (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), "AllPackages");
        File.WriteAllText(packageSavePath, allPackagesString);

        Debug.Log($"Package下载完毕 {packageSavePath}");

        List<string> packageNames = JsonConvert.DeserializeObject<List<string>>(allPackagesString);

        foreach (string packageName in packageNames)
        {
            remotePackagePath = Path.Combine
                (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), packageName);

            request= UnityWebRequest.Get(remotePackagePath);

            request.SendWebRequest();

            while (!request.isDone)
            {
                // 返回NULL代表等待一帧
                yield return null;

            }
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.error);
                yield break;
            }

            string packageString = request.downloadHandler.text;

            packageSavePath = Path.Combine
                (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), packageName);

            File.WriteAllText(packageSavePath, packageString);

            Debug.Log($"package下载完毕{packageName}");

        }
        StartCoroutine(GetRemoteAssetBundleHash());
        yield return null;
    }

    IEnumerator GetRemoteAssetBundleHash()
    {
        string remoteHashPath = Path.Combine
           (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), "AssetBundleHashs");

        UnityWebRequest request = UnityWebRequest.Get(remoteHashPath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // 返回NULL代表等待一帧
            yield return null;

        }
        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.LogError(request.error);
            yield break;
        }

        string hashString = request.downloadHandler.text;
        string hashSavePath=Path.Combine
                (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), "AssetBundleHashs");

        File.WriteAllText(hashSavePath, hashString);

        Debug.Log($"AssetBundleHash列表下载完成{ hashString}");

        CreateDownloadList();
        yield return null;
    }

    void CreateDownloadList()
    {
        //首先读取本地的下载列表
        
        string downloadInfoPath = Path.Combine(AssetManagerRuntime.Instance.DownLoadPath, "TempDownloadInfo");
        if (File.Exists(downloadInfoPath))
        {
            string downloadInfoString = File.ReadAllText(downloadInfoPath);
            CurrentDownloadInfo = JsonConvert.DeserializeObject<DownloadInfo>(downloadInfoString);

        }
        else
        {
            CurrentDownloadInfo = new DownloadInfo();
        }
        //首先下载Allpackages和Packages
        //需要判断Allpackages是否已经下载
        if (CurrentDownloadInfo.DownloadFileNames.Contains("AllPackages"))
        {
            onCompleted("AllPackages", "已经在本地存在");
        }

        else
        {
            string filePath = Path.Combine(RemoteVersionPath,"AllPackages");
            string savePath = Path.Combine(DownloadVersionPath, "AllPackages");
            Downloader downloader = new Downloader(filePath, savePath, onCompleted, onProgress, onError);
            downloader.Start();
        }
        

        //string localAssetBundleHashPath = Path.Combine(AssetManagerRuntime.Instance.AssetBundlePath, "AssetBundleHashs");


        //string assetBundleHashString = "";

        //string[] localAssetBundleHash = null;
        //if (File.Exists(localAssetBundleHashPath))
        //{
        //    assetBundleHashString = File.ReadAllText(localAssetBundleHashPath);

        //    localAssetBundleHash = JsonConvert.DeserializeObject<string[]>(assetBundleHashString);

        //}
        //string remoteAssetBundleHashPath = Path.Combine
        //        (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), "AssetBundleHashs");

        //string remoteAssetBundleHashString= "";
        
        //string[] remoteAssetBundleHash = null;

        //if (File.Exists(remoteAssetBundleHashPath))
        //{
        //    remoteAssetBundleHashString = File.ReadAllText(remoteAssetBundleHashPath);


        //    remoteAssetBundleHash = JsonConvert.DeserializeObject<string[]>(remoteAssetBundleHashString);
        //}
        //if (remoteAssetBundleHash==null)
        //{
        //    Debug.Log($"远端表读取失败{remoteAssetBundleHashPath}");
        //    return;
        //}


        ////将要下载的AB包名称
        //List<string> assetBundleNames = null;
        //if (localAssetBundleHash==null)
        //{
        //    Debug.LogWarning("本地表读取失败");
        //    assetBundleNames = remoteAssetBundleHash.ToList();
           

        //}
        //else
        //{
        //    AssetBundleVersionDiffence versionDiffence = ContrastAssetBundleVersion(localAssetBundleHash, remoteAssetBundleHash);
        //    //新增AB包列表就是将要下载的文件列表
        //    assetBundleNames = versionDiffence.AdditionAssetBundles;
        //}
        //if (assetBundleNames!=null&&assetBundleNames.Count>0)
        //{
        //    // 添加主包名
        //    assetBundleNames.Add("LocalAssets");

        //    StartCoroutine(DownloadAssetsBundle(assetBundleNames,()=> 
        //    { CopyDownloadAssetsToLocalPath();
        //        AssetManagerRuntime.Instance.UpdateLocalAssetVersion();
        //    }));

        //}
    }
    void CopyDownloadAssetsToLocalPath()
    {
        //string downloadAssetVersionPath= Path.Combine
        //        (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());

        DirectoryInfo directoryInfo = new DirectoryInfo(DownloadVersionPath);

        string localVersionPath = Path.Combine
            (AssetManagerRuntime.Instance.LocalAssetPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());

        directoryInfo.MoveTo(localVersionPath);
    }
    IEnumerator DownloadAssetsBundle(List<string>fileNames,Action callBack=null)
    {
        foreach (string fileName in fileNames)
        {
            string assetBundleName = fileName;
            if (fileName.Contains("_"))
            {
                //下划线后最后一位是AssetbundleName
                int startIndex = fileName.IndexOf("_") + 1;
                assetBundleName = fileName.Substring(startIndex);
            }
            string assetBundleDownloadPath= Path.Combine
           (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), assetBundleName);


            UnityWebRequest request = UnityWebRequest.Get(assetBundleDownloadPath);

            request.SendWebRequest();

            while (!request.isDone)
            {
                // 返回NULL代表等待一帧
                yield return null;

            }
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.error);
                yield break;
            }

            string assetBundleSavePath = Path.Combine
                (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), assetBundleName);

            File.WriteAllBytes(assetBundleSavePath, request.downloadHandler.data);
            Debug.Log($"AssetBundle下载完毕{assetBundleName}");
        }

        callBack?.Invoke();
        //if (callBack==null)
        //{
        //    callBack?.Invoke();
        //}
        yield return null;
    }

     AssetBundleVersionDiffence ContrastAssetBundleVersion(string[] oldVersionAssets, string[] newVersionAssets)
    {
        AssetBundleVersionDiffence diffence = new AssetBundleVersionDiffence();
        foreach (var assetName in oldVersionAssets)
        {
            if (!newVersionAssets.Contains(assetName))
            {
                diffence.ReduceAssetBunddles.Add(assetName);
            }
        }
        foreach (var assetName in newVersionAssets)
        {
            if (!oldVersionAssets.Contains(assetName))
            {
                diffence.AdditionAssetBundles.Add(assetName);
            }
        }
        return diffence;
    }
    //确认加载哪个路径下的包
    //void CheckAssetBundleLoadPath()
    //{
    //    switch (LoadPattern)
    //    {
    //        case AssetBundlePattern.EditorSimultion:
    //            break;
    //        case AssetBundlePattern.Local:
    //            AssetBundleLoadPath = Path.Combine(Application.streamingAssetsPath);
    //            Debug.Log("本地加载");
    //            break;
    //        case AssetBundlePattern.Remote:
    //            //依旧是HTTP加主包路径
    //            HTTPAddressBundlePath = Path.Combine(HTTPAddress);
    //            DownloadPath = Path.Combine(Application.persistentDataPath, "DownloadAssetBundle");

    //            //加一层文件结构，和打包下载下的路径区分开来
    //            AssetBundleLoadPath = Path.Combine(DownloadPath);
    //            if (!Directory.Exists(AssetBundleLoadPath))
    //            {
    //                Directory.CreateDirectory(AssetBundleLoadPath);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    // Update is called once per frame

    //声明一个协程
    //IEnumerator DownloadFile(string fileName, Action callBack, bool isSaveFile = true)
    //{


    //    string assetBundleDownLoadPath = Path.Combine(HTTPAddressBundlePath, fileName);
    //    Debug.Log(assetBundleDownLoadPath);
    //    UnityWebRequest webRequest = UnityWebRequest.Get(assetBundleDownLoadPath);
    //    //返回这个请求并发送
    //    webRequest.SendWebRequest();

    //    while (!webRequest.isDone)
    //    {

    //        //下载字节数
    //        Debug.Log(webRequest.downloadedBytes);
    //        //下载进度 
    //        Debug.Log(webRequest.downloadProgress);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    //取出主包
    //    //AssetBundle mainBundle = DownloadHandlerAssetBundle.GetContent(webRequest);

    //    //Debug.Log(mainBundle.name);

    //    //AssetBundleManifest manifest = mainBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
    //    //foreach (string bundleName in manifest.GetAllAssetBundles())
    //    //{
    //    //    Debug.Log(bundleName);
    //    //}
    //    //Debug.Log(webRequest.downloadedBytes);

    //    //存在哪个文件名，就下载那个文件名
    //    string fileSavePath = Path.Combine(AssetBundleLoadPath, fileName);
    //    Debug.Log(webRequest.downloadHandler.data.Length);
    //    if (isSaveFile)
    //    {
    //        yield return SaveFile(fileSavePath, webRequest.downloadHandler.data, callBack);
    //    }
    //    else
    //    {
    //        //三目运算符判读对象是否为空
    //        //不为空就执行
    //        callBack?.Invoke();
    //    }

    //}

    IEnumerator SaveFile(string savePath, byte[] bytes, Action callBack)
    {
        //打开文件流
        FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
        //完整写入文件内容
        yield return fileStream.WriteAsync(bytes, 0, bytes.Length);

        //释放文件流，使得可被其他进程读取
        fileStream.Flush();
        fileStream.Close();
        fileStream.Dispose();
        callBack?.Invoke();
        Debug.Log($"{savePath}文件保存完成");

    }

    //void CheckAssetBundlePattern()
    //{
    //    if (LoadPattern == AssetBundlePattern.Remote)
    //    {
    //        StartCoroutine(DownloadFile("", LoadAssetBundle));
    //    }
    //    else
    //    {
    //        LoadAssetBundle();
    //    }
    //}
    void LoadAssetBundle()
    {
        Debug.Log("正在实现");

        //string mainBundleName = "Bundles";


        //string assetBundlePath = Path.Combine(Application.dataPath, "Bundles", "prefabs");
        //参数：
        //Application.dataPath:现有路径，
        //主包文件夹（同时也是包名），
        //要解压的包名
        //string assetBundlePath = Path.Combine(Application.persistentDataPath, mainBundleName, mainBundleName);
        //Application.persistentDataPath:外部路径
        //由于其在移动端可读可写的特性
        //远程下载的的AB包都能放置在该路径下

        string assetBundlePath = Path.Combine(AssetBundleLoadPath, "");


        //加载清单捆绑包/主包
        AssetBundle mainAB = AssetBundle.LoadFromFile(assetBundlePath);

        AssetBundleManifest assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));

        //事实上Cube和sphere的引用关系一致，只用加载Cube的依赖包
        //foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies("Cube"))
        foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies("0"))
        //GetAllDependencies方法返回了父物体依赖的AB包名
        {
            Debug.Log(depAssetBundleName);
            assetBundlePath = Path.Combine(AssetBundleLoadPath, depAssetBundleName);
            //倘若无需AB包中的资源 ，可不用变量储存该实例，但该实例依旧会存在于内存中
            AssetBundle.LoadFromFile(assetBundlePath);
        }

        //已经加载依赖所需要的包，可以加载父物体的包了
        //参数为：加载路径，对象包的包名
        //assetBundlePath = Path.Combine(AssetBundleLoadPath, "Cube");
        assetBundlePath = Path.Combine(AssetBundleLoadPath, "0");


        CubeBundle = AssetBundle.LoadFromFile(assetBundlePath);
        //加载AB包 的函数


        //assetBundlePath = Path.Combine(AssetBundleLoadPath, "Sphere");
        assetBundlePath = Path.Combine(AssetBundleLoadPath, "1");


        SphereBundle = AssetBundle.LoadFromFile(assetBundlePath);
        //加载AB包 的函数
    }

    //void LoadAsset()
    //{
    //    GameObject cubeObject = CubeBundle.LoadAsset<GameObject>("Cube");
    //    //(括号内为传入物体之后获得的后缀)

    //    Instantiate(cubeObject);
    //    //SampleObject用来实例化资源
    //    //出现

    //    GameObject SphereObject = SphereBundle.LoadAsset<GameObject>("Sphere");
    //    //(括号内为传入物体之后获得的后缀)

    //    Instantiate(SphereObject);
    //    //SampleObject用来实例化资源
    //    //出现
    //}

    void UnloadAssetBundle(bool isTrue)
    {
        Debug.Log(isTrue);
        DestroyImmediate(gameObject);
        //将加载的物体摧毁
        CubeBundle.Unload(isTrue);
        //Unload(false)一般不会获得理想情况，它只会卸载AB包本身
        //Unload(true)可以确保对象不重复
        //只是回收了材质与贴图，物体本身依然存在

        //Unload(false)能不破坏当前运行的效果
        //如果资源是AB包创建，但未被管理，资源会仍然被使用但AB包被Unload(true)卸载
        //使得当前运行时丢失AB包中的资源
        //于是使用false,并用UnloadUnusedAssets()擦屁股
        //所有对内存的操作会占据CPU使用，最好在CPU使用情况较低时完成强制回收
        //过场动画，加载场景时
        Resources.UnloadUnusedAssets();
    }
}
