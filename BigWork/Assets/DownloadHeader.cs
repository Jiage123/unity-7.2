using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//Unity�в������Ĵ�������
public enum ErrorCode
{
    //��������Ϊ��
    DownloadFileEmpty,

    //��ʱ�ļ���ʧ
    TempFileMissing
}

//�޲������޷���ֵ��ί��
//ʵ��������һ���ض�����ֵ���ض������ĺ���������ָ��������һ��
//�κη��Ϲ���ĺ�����������ί��
//�κη��Ϲ���ĺ�����������ί�и�ĳ��ί��ʵ��
public delegate void SampleDelegate();


//���ش���ִ��
public delegate void ErrorEventHandler(ErrorCode errorCode, string message);
//�������ִ��
public delegate void CompleteEventHandler(string fileName, string message);
//���ؽ��ȸ���ִ��
public delegate void ProgressEventHandler(float progree,long currentLength,long totalLength);
public class DownloadHeader : DownloadHandlerScript
{
    //���غ󱣴�·��
    string SavePath;

    //��ʱ�ļ��洢·��
    string TempPath;

    //��ʱ�ļ��Ĵ�С
    //�Ǳ������ص���ʼλ��
    long currentLength = 0;

    //�ļ��ܴ�С
    long totalLength = 0;

    //������Ҫ���صĳ��ȣ��ֽڣ�
    long contentLength = 0;

    //�ļ���д��
    FileStream fileStream = null;

    //����ʱ��ص�
    //ί��
    ErrorEventHandler onError = null;

    //�������ִ�лص�
    CompleteEventHandler onCompleted = null;

    //���ظ��½���ʱ�ص�
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

        //ԭ�����ļ�·���£�����һ��.temp�ļ�
        this.TempPath = savePath + ".temp";
        this.onCompleted = onComplete;
        this.onError = onError;
        this.OnProgress = onProcess;

        fileStream = new FileStream(this.TempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        //�����create����Ϊ0
        this.currentLength = fileStream.Length;

        //��������֮�⣬д���ļ�Ҳ�����ĳ�������д
        fileStream.Position = this.currentLength;
    }

    //����Headerʱ����
     protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        this.contentLength = (long)contentLength;

        //�ܳ���=������+δ����
        this.totalLength = this.contentLength+currentLength;
    }


    //ÿ�δӷ��������յ���Ϣ����
    protected override bool ReceiveData(byte[]datas,int dataLength)
    {
        if (contentLength<=0||datas==null||datas.Length<=0)
        {
            return false;
        }
        //0��length����datas��λ��
        this.fileStream.Write(datas, 0, dataLength);

        currentLength += dataLength;

        //1.0f��ʽת��float
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
            onError.Invoke(ErrorCode.DownloadFileEmpty, "���س���Ϊ0");
            return;
        }
        if (!File.Exists(TempPath))
        {
            onError.Invoke(ErrorCode.TempFileMissing, "��ʱ�ļ���ʧ ");
            return;
        }
        //�����ַ���ļ�ɾ��
        if (File.Exists(SavePath))

        {
            File.Delete(SavePath);
        }

        //move�����������Ĺ���
        //path��Ҫ��ָ��·������
        File.Move(TempPath, SavePath);

        FileInfo fileInfo = new FileInfo(SavePath);
        onCompleted.Invoke(fileInfo.Name, "�������");
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
