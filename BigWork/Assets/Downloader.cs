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
    //�ļ���������ַ
    string URL = null;

    //�ļ�����·��
    string SavePath = null;

    //�������������ʾ��
    UnityWebRequest request = null;

    //�������Լ�ʵ�ֵ����ش�����
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

            //currentLength��ʵ����ͬʱ���յ�����������ʱ���£��ʿ��Ա�ﱾ���ļ����ȣ�
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
