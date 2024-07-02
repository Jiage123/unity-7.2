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
    //���������

    AssetBundle CubeBundle;
    AssetBundle SphereBundle;

    GameObject SampleObject;

    public Button LoadAssetBundleButton;
    public Button LoadAssetButton;
    public Button UnLoadFalseButton;
    public Button UnLoadTrueButton;

    //����İ���������ҪEditor���������Դ������ҲҪ���ʣ��ʷ��ڴ˴�
    //  public static string MainAssetBundleName = "SampleAssetBundle";

    //���������⣬ʵ�ʰ�����Сд
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
        ////     //����¼��޷�ֱ�Ӵ���������ķ�����ʹ����������
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
        Debug.Log($"���ؽ���{progress * 100}%,��ǰ���س���{currentLength * 1.0f / 1024 / 1024}M,�ļ��ܳ��ȣ�{totalLength * 1.0f / 1024 / 1024}M");
        if (currentLength!=totalLength)
        {
            if (currentLength>totalLength/10.0f*downloadCount)
            {
                Debug.Log(currentLength);
                Debug.Log(totalLength);
                downloadCount++;
                //SampleDownloader.Dispose();
                Debug.Log("�����жϲ���");
            }
        }
    }

    void onError(ErrorCode errorCode,string message)
    {
        Debug.LogError(message);
    }
    void CreateAssetBundleDownloadList()
    {
        Debug.Log("���");
        string assetBundleHashsPath = Path.Combine(DownloadVersionPath, "AssetBundleHashs");
        string assetBundleHashsString = File.ReadAllText(assetBundleHashsPath);

        //Զ�˰��б�
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
            Debug.Log("���ر��ȡʧ�ܣ�����Զ�˱�");
            downloadAssetNames = remoteAssetsBundleHashs.ToList();
        }
        else
        {
            AssetBundleVersionDiffence diffence = ContrastAssetBundleVersion(localAssetBundleHash, remoteAssetsBundleHashs);
            downloadAssetNames = diffence.AdditionAssetBundles;
        }

        //�����������
        downloadAssetNames.Add("LocalAssets");
        Downloader downloader = null;
        foreach (string assetBundleName in downloadAssetNames)
        {
            string fileName = assetBundleName;
            if (assetBundleName.Contains("_"))
            {
                //�»��ߺ����һλ��AssetbundleName
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
                
                onCompleted(fileName, "�����Ѵ���");
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
                onCompleted(packageName, "�����Ѵ���");
                
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
            onCompleted("AssetBundleHashs", "�����Ѵ���");
            
        }
    }
    BuildInfo RemoteBuildInfo;

    IEnumerator GetRemoteVersion()
    {
        #region ��ȡԶ�˰汾��
        string remoteVersionFilePath = Path.Combine(HTTPAddress, "BuildOutput", "BuildVersion.version");
        UnityWebRequest request = UnityWebRequest.Get(remoteVersionFilePath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // ����NULL����ȴ�һ֡
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
        //ʹ�ñ�������Զ�˰汾
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
      
        Debug.Log($"Զ����Դ�汾Ϊ{version}");

        #region ��ȡԶ��buildInfo
        string downloadPath = Path.Combine
            (AssetManagerRuntime.Instance.DownLoadPath, AssetManagerRuntime.Instance.RemoteAssetVersion.ToString());

        string remoteBuildInfoPath = Path.Combine(HTTPAddress, "BuildOutput", version.ToString(), "BuildInfo");

        request= UnityWebRequest.Get(remoteBuildInfoPath);

        request.SendWebRequest();

        while (!request.isDone)
        {
            // ����NULL����ȴ�һ֡
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
            // ����NULL����ȴ�һ֡
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

        Debug.Log($"Package������� {packageSavePath}");

        List<string> packageNames = JsonConvert.DeserializeObject<List<string>>(allPackagesString);

        foreach (string packageName in packageNames)
        {
            remotePackagePath = Path.Combine
                (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), packageName);

            request= UnityWebRequest.Get(remotePackagePath);

            request.SendWebRequest();

            while (!request.isDone)
            {
                // ����NULL����ȴ�һ֡
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

            Debug.Log($"package�������{packageName}");

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
            // ����NULL����ȴ�һ֡
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

        Debug.Log($"AssetBundleHash�б��������{ hashString}");

        CreateDownloadList();
        yield return null;
    }

    void CreateDownloadList()
    {
        //���ȶ�ȡ���ص������б�
        
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
        //��������Allpackages��Packages
        //��Ҫ�ж�Allpackages�Ƿ��Ѿ�����
        if (CurrentDownloadInfo.DownloadFileNames.Contains("AllPackages"))
        {
            onCompleted("AllPackages", "�Ѿ��ڱ��ش���");
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
        //    Debug.Log($"Զ�˱��ȡʧ��{remoteAssetBundleHashPath}");
        //    return;
        //}


        ////��Ҫ���ص�AB������
        //List<string> assetBundleNames = null;
        //if (localAssetBundleHash==null)
        //{
        //    Debug.LogWarning("���ر��ȡʧ��");
        //    assetBundleNames = remoteAssetBundleHash.ToList();
           

        //}
        //else
        //{
        //    AssetBundleVersionDiffence versionDiffence = ContrastAssetBundleVersion(localAssetBundleHash, remoteAssetBundleHash);
        //    //����AB���б���ǽ�Ҫ���ص��ļ��б�
        //    assetBundleNames = versionDiffence.AdditionAssetBundles;
        //}
        //if (assetBundleNames!=null&&assetBundleNames.Count>0)
        //{
        //    // ���������
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
                //�»��ߺ����һλ��AssetbundleName
                int startIndex = fileName.IndexOf("_") + 1;
                assetBundleName = fileName.Substring(startIndex);
            }
            string assetBundleDownloadPath= Path.Combine
           (HTTPAddress, "BuildOutput", AssetManagerRuntime.Instance.RemoteAssetVersion.ToString(), assetBundleName);


            UnityWebRequest request = UnityWebRequest.Get(assetBundleDownloadPath);

            request.SendWebRequest();

            while (!request.isDone)
            {
                // ����NULL����ȴ�һ֡
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
            Debug.Log($"AssetBundle�������{assetBundleName}");
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
    //ȷ�ϼ����ĸ�·���µİ�
    //void CheckAssetBundleLoadPath()
    //{
    //    switch (LoadPattern)
    //    {
    //        case AssetBundlePattern.EditorSimultion:
    //            break;
    //        case AssetBundlePattern.Local:
    //            AssetBundleLoadPath = Path.Combine(Application.streamingAssetsPath);
    //            Debug.Log("���ؼ���");
    //            break;
    //        case AssetBundlePattern.Remote:
    //            //������HTTP������·��
    //            HTTPAddressBundlePath = Path.Combine(HTTPAddress);
    //            DownloadPath = Path.Combine(Application.persistentDataPath, "DownloadAssetBundle");

    //            //��һ���ļ��ṹ���ʹ�������µ�·�����ֿ���
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

    //����һ��Э��
    //IEnumerator DownloadFile(string fileName, Action callBack, bool isSaveFile = true)
    //{


    //    string assetBundleDownLoadPath = Path.Combine(HTTPAddressBundlePath, fileName);
    //    Debug.Log(assetBundleDownLoadPath);
    //    UnityWebRequest webRequest = UnityWebRequest.Get(assetBundleDownLoadPath);
    //    //����������󲢷���
    //    webRequest.SendWebRequest();

    //    while (!webRequest.isDone)
    //    {

    //        //�����ֽ���
    //        Debug.Log(webRequest.downloadedBytes);
    //        //���ؽ��� 
    //        Debug.Log(webRequest.downloadProgress);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    //ȡ������
    //    //AssetBundle mainBundle = DownloadHandlerAssetBundle.GetContent(webRequest);

    //    //Debug.Log(mainBundle.name);

    //    //AssetBundleManifest manifest = mainBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
    //    //foreach (string bundleName in manifest.GetAllAssetBundles())
    //    //{
    //    //    Debug.Log(bundleName);
    //    //}
    //    //Debug.Log(webRequest.downloadedBytes);

    //    //�����ĸ��ļ������������Ǹ��ļ���
    //    string fileSavePath = Path.Combine(AssetBundleLoadPath, fileName);
    //    Debug.Log(webRequest.downloadHandler.data.Length);
    //    if (isSaveFile)
    //    {
    //        yield return SaveFile(fileSavePath, webRequest.downloadHandler.data, callBack);
    //    }
    //    else
    //    {
    //        //��Ŀ������ж������Ƿ�Ϊ��
    //        //��Ϊ�վ�ִ��
    //        callBack?.Invoke();
    //    }

    //}

    IEnumerator SaveFile(string savePath, byte[] bytes, Action callBack)
    {
        //���ļ���
        FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
        //����д���ļ�����
        yield return fileStream.WriteAsync(bytes, 0, bytes.Length);

        //�ͷ��ļ�����ʹ�ÿɱ��������̶�ȡ
        fileStream.Flush();
        fileStream.Close();
        fileStream.Dispose();
        callBack?.Invoke();
        Debug.Log($"{savePath}�ļ��������");

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
        Debug.Log("����ʵ��");

        //string mainBundleName = "Bundles";


        //string assetBundlePath = Path.Combine(Application.dataPath, "Bundles", "prefabs");
        //������
        //Application.dataPath:����·����
        //�����ļ��У�ͬʱҲ�ǰ�������
        //Ҫ��ѹ�İ���
        //string assetBundlePath = Path.Combine(Application.persistentDataPath, mainBundleName, mainBundleName);
        //Application.persistentDataPath:�ⲿ·��
        //���������ƶ��˿ɶ���д������
        //Զ�����صĵ�AB�����ܷ����ڸ�·����

        string assetBundlePath = Path.Combine(AssetBundleLoadPath, "");


        //�����嵥�����/����
        AssetBundle mainAB = AssetBundle.LoadFromFile(assetBundlePath);

        AssetBundleManifest assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));

        //��ʵ��Cube��sphere�����ù�ϵһ�£�ֻ�ü���Cube��������
        //foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies("Cube"))
        foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies("0"))
        //GetAllDependencies���������˸�����������AB����
        {
            Debug.Log(depAssetBundleName);
            assetBundlePath = Path.Combine(AssetBundleLoadPath, depAssetBundleName);
            //��������AB���е���Դ ���ɲ��ñ��������ʵ��������ʵ�����ɻ�������ڴ���
            AssetBundle.LoadFromFile(assetBundlePath);
        }

        //�Ѿ�������������Ҫ�İ������Լ��ظ�����İ���
        //����Ϊ������·����������İ���
        //assetBundlePath = Path.Combine(AssetBundleLoadPath, "Cube");
        assetBundlePath = Path.Combine(AssetBundleLoadPath, "0");


        CubeBundle = AssetBundle.LoadFromFile(assetBundlePath);
        //����AB�� �ĺ���


        //assetBundlePath = Path.Combine(AssetBundleLoadPath, "Sphere");
        assetBundlePath = Path.Combine(AssetBundleLoadPath, "1");


        SphereBundle = AssetBundle.LoadFromFile(assetBundlePath);
        //����AB�� �ĺ���
    }

    //void LoadAsset()
    //{
    //    GameObject cubeObject = CubeBundle.LoadAsset<GameObject>("Cube");
    //    //(������Ϊ��������֮���õĺ�׺)

    //    Instantiate(cubeObject);
    //    //SampleObject����ʵ������Դ
    //    //����

    //    GameObject SphereObject = SphereBundle.LoadAsset<GameObject>("Sphere");
    //    //(������Ϊ��������֮���õĺ�׺)

    //    Instantiate(SphereObject);
    //    //SampleObject����ʵ������Դ
    //    //����
    //}

    void UnloadAssetBundle(bool isTrue)
    {
        Debug.Log(isTrue);
        DestroyImmediate(gameObject);
        //�����ص�����ݻ�
        CubeBundle.Unload(isTrue);
        //Unload(false)һ�㲻���������������ֻ��ж��AB������
        //Unload(true)����ȷ�������ظ�
        //ֻ�ǻ����˲�������ͼ�����屾����Ȼ����

        //Unload(false)�ܲ��ƻ���ǰ���е�Ч��
        //�����Դ��AB����������δ��������Դ����Ȼ��ʹ�õ�AB����Unload(true)ж��
        //ʹ�õ�ǰ����ʱ��ʧAB���е���Դ
        //����ʹ��false,����UnloadUnusedAssets()��ƨ��
        //���ж��ڴ�Ĳ�����ռ��CPUʹ�ã������CPUʹ������ϵ�ʱ���ǿ�ƻ���
        //�������������س���ʱ
        Resources.UnloadUnusedAssets();
    }
}
