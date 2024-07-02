using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//Unity中不包含的错误类型
public enum ErrorCode
{
    //下载内容为空
    DownloadFileEmpty,

    //临时文件丢失
    TempFileMissing
}

//无参数，无返回值的委托
//实质是声明一种特定返回值，特定参数的函数，但不指定具体哪一个
//任何符合规则的函数都可以是委托
//任何符合规则的函数，都可以委托给某个委托实例
public delegate void SampleDelegate();


//下载错误执行
public delegate void ErrorEventHandler(ErrorCode errorCode, string message);
//下载完成执行
public delegate void CompleteEventHandler(string fileName, string message);
//下载进度更新执行
public delegate void ProgressEventHandler(float progree,long currentLength,long totalLength);
public class DownloadHeader : DownloadHandlerScript
{
    //下载后保存路径
    string SavePath;

    //临时文件存储路径
    string TempPath;

    //临时文件的大小
    //是本次下载的起始位置
    long currentLength = 0;

    //文件总大小
    long totalLength = 0;

    //本次需要下载的长度（字节）
    long contentLength = 0;

    //文件读写流
    FileStream fileStream = null;

    //报错时候回调
    //委托
    ErrorEventHandler onError = null;

    //下载完成执行回调
    CompleteEventHandler onCompleted = null;

    //下载更新进度时回调
    ProgressEventHandler OnProgress = null;

    public long CurrentLength { get { return currentLength; } }

    public long TotalLength
    {
        get
        {
            return totalLength;
        }
    }

    public DownloadHeader(string savePath, CompleteEventHandler onComplete, ProgressEventHandler onProcess, ErrorEventHandler onError) : base(new byte[1024 * 1024])
    {
        this.SavePath = savePath;

        //原本的文件路径下，创建一个.temp文件
        this.TempPath = savePath + ".temp";
        this.onCompleted = onComplete;
        this.onError = onError;
        this.OnProgress = onProcess;

        fileStream = new FileStream(this.TempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        //如果是create长度为0
        this.currentLength = fileStream.Length;

        //除了下载之外，写入文件也从最大的长度往下写
        fileStream.Position = this.currentLength;
    }

    //设置Header时调用
     protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        this.contentLength = (long)contentLength;

        //总长度=已下载+未下载
        this.totalLength = this.contentLength+currentLength;
    }


    //每次从服务器上收到消息调用
    protected override bool ReceiveData(byte[]datas,int dataLength)
    {
        if (contentLength<=0||datas==null||datas.Length<=0)
        {
            return false;
        }
        //0和length都是datas的位置
        this.fileStream.Write(datas, 0, dataLength);

        currentLength += dataLength;

        //1.0f隐式转换float
        ///
        ///
        OnProgress?.Invoke(currentLength * 1.0f / totalLength, currentLength, totalLength);

        return true;
    }

    protected override void CompleteContent()
    {
        FileStreamClose();
        if (contentLength<=0)
        {
            onError.Invoke(ErrorCode.DownloadFileEmpty, "下载长度为0");
            return;
        }
        if (!File.Exists(TempPath))
        {
            onError.Invoke(ErrorCode.TempFileMissing, "临时文件丢失 ");
            return;
        }
        //保存地址的文件删除
        if (File.Exists(SavePath))

        {
            File.Delete(SavePath);
        }

        //move有重新命名的功能
        //path中要求指定路径名称
        File.Move(TempPath, SavePath);

        FileInfo fileInfo = new FileInfo(SavePath);
        onCompleted.Invoke(fileInfo.Name, "下载完成");
    }

    public override void Dispose()
    {
        base.Dispose();
        FileStreamClose();
    }

    void FileStreamClose()
    {
        if (fileStream==null)
        {
            return;
        }
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
    }
}
