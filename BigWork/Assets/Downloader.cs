using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
 public class DownloadInfo
     {
        public List<string> DownloadFileNames=new List<string> ();
        }
public class Downloader
{
    //文件服务器地址
    string URL = null;

    //文件保存路径
    string SavePath = null;

    //具体的下载类型示例
    UnityWebRequest request = null;

    //由我们自己实现的下载处理类
    DownloadHeader downloadHandler = null;

    ErrorEventHandler OnError = null;

    ProgressEventHandler OnProgress = null;

    CompleteEventHandler OnComplete = null;

   
    public Downloader(string url, string savePath, CompleteEventHandler OnComplete, ProgressEventHandler OnProgress, ErrorEventHandler OnError)
    {
        this.URL = url;
        this.SavePath = savePath;
        this.OnComplete = OnComplete;
        this.OnProgress = OnProgress;
        this.OnError = OnError;


    }

    public void Start()
    {
        request = UnityWebRequest.Get(URL);
        if (!string.IsNullOrEmpty(SavePath))
        {
            request.timeout = 30;
            request.disposeDownloadHandlerOnDispose = true;
            downloadHandler = new DownloadHeader(SavePath, OnComplete, OnProgress, OnError);

            //currentLength会实例化同时在收到服务器数据时更新，故可以表达本地文件长度；
            request.SetRequestHeader("range", $"btyes={downloadHandler.CurrentLength}-");

            request.downloadHandler = downloadHandler;
        }
        request.SendWebRequest();
    }

    public  void Dispose()
    {
        OnError = null;
        OnComplete = null;
        OnProgress = null;
        if (request!=null)
        {
            if (!request.isDone)
            {
                request.Abort();
            }
            request.Dispose();
            request = null;
        }
    }
}
