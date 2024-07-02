using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName ="AssetManagerConfig",menuName ="AssetManager/CreateManagerConfig")]
public class AssetManagerConfigScriptableObject : ScriptableObject
{
  

    


    //打包逻辑，决定了打包内容输出到哪个文件夹

    //编辑器模拟加载，使用AssetDatabase进行加载，不用打包

    //本地加载模式，打包到本地或者StreamingAssets路径下，从这个路径加载

    //远端加载模式，打包到任意资源服务器地址，通过网络进行下载，下载到沙盒路径（该案例中为）persistDataPath后加载
    public AssetBundlePattern BuildingPattern;

    //AB包压缩格式
    public AssetBundleCompressionPattern CompressionPattern;

    //是否使用增量打包
    public IncrementalBuildMode _IncrementalBuildMode;

    //资源管理器工具的版本 
    public int AssetManagerVersion = 100;

    //资源打包的版本
    public int CurrentBuildVersion = 100;


    public List<PackageInfoEditor> packageInfoEditors=new List<PackageInfoEditor>();

   
  
    //获取当前文件夹下所有资源的函数


 

    //需要在打包时排除的拓展名
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
