using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName ="AssetManagerConfig",menuName ="AssetManager/CreateManagerConfig")]
public class AssetManagerConfigScriptableObject : ScriptableObject
{
  

    


    //����߼��������˴������������ĸ��ļ���

    //�༭��ģ����أ�ʹ��AssetDatabase���м��أ����ô��

    //���ؼ���ģʽ����������ػ���StreamingAssets·���£������·������

    //Զ�˼���ģʽ�������������Դ��������ַ��ͨ������������أ����ص�ɳ��·�����ð�����Ϊ��persistDataPath�����
    public AssetBundlePattern BuildingPattern;

    //AB��ѹ����ʽ
    public AssetBundleCompressionPattern CompressionPattern;

    //�Ƿ�ʹ���������
    public IncrementalBuildMode _IncrementalBuildMode;

    //��Դ���������ߵİ汾 
    public int AssetManagerVersion = 100;

    //��Դ����İ汾
    public int CurrentBuildVersion = 100;


    public List<PackageInfoEditor> packageInfoEditors=new List<PackageInfoEditor>();

   
  
    //��ȡ��ǰ�ļ�����������Դ�ĺ���


 

    //��Ҫ�ڴ��ʱ�ų�����չ��
    public string[] InvalidExtensionNames = new string[] { ".meta", ".cs" };

    public bool isValidExtentionName(string fileName)
    {
        bool isValid = true;
        foreach (string invalidName in InvalidExtensionNames)
        {
            if (fileName.Contains(invalidName))
            {
                isValid = false;
                return isValid;
            }
        }
        return isValid;

    }



}
