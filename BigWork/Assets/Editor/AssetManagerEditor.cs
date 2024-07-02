using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System;

//一个用于收集在Editor环境下存在的Package信息
//不是打包之后的信息

[Serializable]

public class PackageInfoEditor
{

    //当前包的名称
    //开发者在编译器窗口中自由定义
    public string PackageName;

    //属于当前包中的资源列表
    //开发者在编译器窗口中自由定义
    public List<UnityEngine.Object> AssetList = new List<UnityEngine.Object>();
}

public class AssetBundleEdge
{
    public List<AssetBundleNode> Nodes = new List<AssetBundleNode>();
}

public class AssetBundleNode
{
    public string AssetName;

    //用于判断是否是sourceAsset
    //如果为-1，则为DeriveedAsset
    public int SourceIndex = -1;

    //当前Node的Index列表
    //通过自生OutEdge进行传递
    public List<int> SourceIndices = new List<int>();

    


    //当前资源属于那个Package
    //可能多个SourceAsset拥有同一个PackageName
    public string PackageName;

    //当前资源被哪个Package引用
    public List<string> PackageNames = new List<string>();

    //引用当前nodes的nodes
    public AssetBundleEdge InEdge;


    //当前nodes所引用的nodes
    public AssetBundleEdge OutEdge;



}


public class AssetManagerEditor
{



    public static AssetManagerConfigScriptableObject AssetManagerConfig;





    //资源打包的版本

    //本次打包所有AB的输出路径
    //需要包含主包包名，适配增量打包
    //AB包输出路径
    public static string AssetBundleOutoutPath;


    //整个打包文件的输出路径 
    public static string BuildOutputPath;


    //List是引用类型，对其修改会应用在对应的数值上
    public static List<GUID> ContrastDependenciesFromGUID(List<GUID> setsA, List<GUID> setsB)
    {
        List<GUID> newDependenics = new List<GUID>();

        //取交集
        foreach (var assetGUID in setsA)
        {
            if (setsB.Contains(assetGUID))
            {
                newDependenics.Add(assetGUID);
            }
        }

        //取差集
        foreach (var assetGUID in newDependenics)
        {
            if (setsA.Contains(assetGUID))
            {
                setsA.Remove(assetGUID);
            }
            if (setsB.Contains(assetGUID))
            {
                setsB.Remove(assetGUID);
            }
        }
        return newDependenics;
    }


    //资源集合打包法
    //核心部分
    //public static void BuildAssetBundleFromSets()
    //{
    //    CheckBuildOutpitPath();

    //    //被选中的打包资源列表,列表A
    //    List<string> selectedAssets = new List<string>();
    //    //集合列表L
    //    List<List<GUID>> selectedAssetDependenices = new List<List<GUID>>();

    //    //遍历所有选择的sourseAsset以及依赖，获得集合列表L

    //    foreach (string selectedAsset in selectedAssets)
    //    {
    //        //获取SourseAsset的DerivedAsset，其中已经包含了SourseAsset
    //        string[] assetDeps = AssetDatabase.GetDependencies(selectedAsset, true);
    //        List<GUID> assetGUIDs = new List<GUID>();
    //        foreach (string assetdep in assetDeps)
    //        {
    //            GUID assetGUID = AssetDatabase.GUIDFromAssetPath(assetdep);
    //            assetGUIDs.Add(assetGUID);

    //        }
    //        //将包含了SourseAsset以及DriveedAsset的集合添加到集合L中

    //        if (assetGUIDs.Count == 0)
    //        {
    //            continue;
    //        }
    //        selectedAssetDependenices.Add(assetGUIDs);
    //    }

    //    for (int i = 0; i < selectedAssetDependenices.Count; i++)
    //    {

    //        int nextIndex = i + 1;
    //        if (nextIndex >= selectedAssetDependenices.Count)

    //        {

    //            break;
    //        };


    //        Debug.Log($"对比之前{selectedAssetDependenices[i].Count}");
    //        Debug.Log($"对比之前{selectedAssetDependenices[nextIndex].Count}");

    //        for (int j = 0; j <= i; j++)
    //        {
    //            List<GUID> newDenpendenies = ContrastDependenciesFromGUID(selectedAssetDependenices[j], selectedAssetDependenices[nextIndex]);
    //            //将Snew集合添加到集合列表L中
    //            if (newDenpendenies != null && newDenpendenies.Count > 0)
    //            {
    //                selectedAssetDependenices.Add(newDenpendenies);
    //            }

    //        }

    //        Debug.Log($"对比之后{selectedAssetDependenices[i].Count}");
    //        Debug.Log($"对比之后{selectedAssetDependenices[nextIndex].Count}");

    //    }
    //    AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[selectedAssetDependenices.Count];
    //    for (int i = 0; i < assetBundleBuilds.Length; i++)
    //    {

    //        // assetBundleBuilds[i].assetBundleName = i.ToString();



    //        string[] assetNames = new string[selectedAssetDependenices[i].Count];

    //        //注意，当某些包中不存在依赖时，应当将其排除
    //        if (assetNames.Length == 0)
    //        {
    //            continue;
    //        }

    //        List<GUID> assetGUIDs = selectedAssetDependenices[i];

    //        for (int j = 0; j < assetNames.Length; j++)
    //        {
    //            string assetName = AssetDatabase.GUIDToAssetPath(assetGUIDs[j]);
    //            if (assetName.Contains(".cs"))
    //            {
    //                continue;
    //            }
    //            assetNames[j] = assetName;
    //        }
    //        string[] assetNamesArray = assetNames.ToArray();
    //        assetBundleBuilds[i].assetBundleName = ComputeAssetSetSignature(assetNamesArray);
    //        assetBundleBuilds[i].assetNames = assetNames;
    //    }
    //    //BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);
    //    BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckIncrementalBuildMode(), BuildTarget.StandaloneWindows);

    //    string[] currentVersionAssettHashs = BuildAssetBundleHashTable(assetBundleBuilds);
    //    CopyAssetBundleToVersionFolder();

    //     GetVersionDifference(currentVersionAssettHashs);

    //    //版本自增
    //    AssetManagerConfig. CurrentBuildVersion++;


    //    //调用该方法在打包之后调用
    //    //BuildAssetBundleHashTable(assetBundleBuilds);
    //    AssetDatabase.Refresh();
    //}

    public static void BuildAssetBundleFromDirectedGraph()
    {
        CheckBuildOutpitPath();

        List<AssetBundleNode> allNobes = new List<AssetBundleNode>();

        int sourceIndex = 0;

        Dictionary<string, PackageBuildInfo> packageInfoDic = new Dictionary<string, PackageBuildInfo>();
        #region 有向图构建
        for (int i = 0; i < AssetManagerConfig.packageInfoEditors.Count; i++)
        {
            PackageBuildInfo packageBuildInfo = new PackageBuildInfo();
            packageBuildInfo.PackageName = AssetManagerConfig.packageInfoEditors[i].PackageName;

            packageBuildInfo.IsSourcePackage = true;

            packageInfoDic.Add(packageBuildInfo.PackageName, packageBuildInfo);


            //当前所选中的资源，就是SourceAsset
            //所以首先添加SourceAsset的Node
            foreach (UnityEngine.Object asset in AssetManagerConfig.packageInfoEditors[i].AssetList)
            {

                AssetBundleNode currentNode = null;

                //以资源的具体路径作为资源名称
                string AssetNamePath = AssetDatabase.GetAssetPath(asset);
                foreach (AssetBundleNode node in allNobes)
                {
                    if (node.AssetName==AssetNamePath)
                    {
                        currentNode = node;
                        currentNode.PackageName = packageBuildInfo.PackageName;
                        break;
                    }
                }

                if (currentNode==null)
                {
                    currentNode = new AssetBundleNode();
                    currentNode.AssetName = AssetNamePath;

                    currentNode.SourceIndex = sourceIndex;
                    currentNode.SourceIndices = new List<int>() { sourceIndex };

                    currentNode.PackageName = packageBuildInfo.PackageName;
                    currentNode.PackageNames.Add(currentNode.PackageName);

                    currentNode.InEdge = new AssetBundleEdge();
                    allNobes.Add(currentNode);

                }
                

                GetNodesFromDependencies(currentNode, allNobes);

                sourceIndex++;
            }
        }
        #endregion
        #region 有向图区分打包集合
        Dictionary<List<int>, List<AssetBundleNode>> assetBundleNodeDic = new Dictionary<List<int>, List<AssetBundleNode>>();
        foreach (AssetBundleNode node in allNobes)
        {
            StringBuilder packNameString = new StringBuilder();

            //包名不是空或无，说明是一个SourceAsset,其包名已经在编辑器窗口添加
            if (string.IsNullOrEmpty(node.PackageName))
            {
                for (int i = 0; i < node.PackageNames.Count; i++)
                {
                    packNameString.Append(node.PackageNames[i]);
                    if (i<node.PackageNames.Count-1)
                    {
                        packNameString.Append("_");
                    }
                }
                string packageName = packNameString.ToString();
                node.PackageName = packageName;

                //此处添加了对应的包和包名
                //没有添加具体包中对应的Asset
                //Asset添加时需要AssetBundleName,所以只能在生成AssetBundleName处添加Asset
                if (!packageInfoDic.ContainsKey(packageName))
                {
                    PackageBuildInfo packageBuildInfo = new PackageBuildInfo();
                    packageBuildInfo.PackageName = packageName;
                    packageBuildInfo.IsSourcePackage = false;
                    packageInfoDic.Add(packageBuildInfo.PackageName, packageBuildInfo);
                }
            }

            bool isEquals = false;
            List<int> keyList = new List<int>();
            //遍历所有key
            //通过这种方式确保不同list之间内容是否一致
            foreach (List<int> key in assetBundleNodeDic.Keys)
            {
                //判断key的长度是否和当前node的SourceInndices长度相等
                isEquals = node.SourceIndices.Count == key.Count && node.SourceIndices.All(p => key.Any(k => k.Equals(p)));
                if (isEquals)
                {
                    keyList = key;
                    break;
                }
            }
            if (!isEquals)
            {
                keyList = node.SourceIndices;
                assetBundleNodeDic.Add(node.SourceIndices, new List<AssetBundleNode>());
            }

            assetBundleNodeDic[keyList].Add(node);
        }
        #endregion
        AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[assetBundleNodeDic.Count];
        int buildIndex = 0;

        foreach (List<int> key in assetBundleNodeDic.Keys)
        {
            List<string> assetNames = new List<string>();
            //这层循环，同从一个键值对获取node
            //就是从SourceIndices相同的集合中，获取对应Node代表队Asset
            foreach (AssetBundleNode node in assetBundleNodeDic[key])
            {
                assetNames.Add(node.AssetName);

                //如果是一个初始包，它的PackageName只有他自己
                foreach (string packageName in node.PackageNames)
                {
                    if (packageInfoDic.ContainsKey(packageName))
                    {
                        if (!packageInfoDic[packageName].PackageDenpdecies.Contains(packageName)
                            && !string.Equals(node.PackageName, packageInfoDic[packageName].PackageName) )
                        {
                            packageInfoDic[packageName].PackageDenpdecies.Add(node.PackageName);
                        }
                    }
                }
            }
            string[] assetNamesArrary = assetNames.ToArray();

            assetBundleBuilds[buildIndex].assetBundleName = ComputeAssetSetSignature(assetNamesArrary);
            assetBundleBuilds[buildIndex].assetNames = assetNamesArrary;

            
            foreach (AssetBundleNode node in assetBundleNodeDic[key])
            {
                //因为区分了DerivePackage
                //此处可以确保每个Node都有一个包名
                AssetBuildInfo assetBuildInfo = new AssetBuildInfo();

                    assetBuildInfo.AssetName = node.AssetName;
                    assetBuildInfo.AssetBundleName = assetBundleBuilds[buildIndex].assetBundleName;

                    packageInfoDic[node.PackageName].AssetInfos.Add(assetBuildInfo);

                
                    
                
            }
            buildIndex++;
        }
        

        BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckIncrementalBuildMode(), BuildTarget.StandaloneWindows);

        string buildVersionFilePath = Path.Combine(BuildOutputPath,"BuildVersion.version");
        File.WriteAllText(buildVersionFilePath, AssetManagerConfig.CurrentBuildVersion.ToString());

        //创建版本路径
        string versionPath = Path.Combine(BuildOutputPath, AssetManagerConfig.CurrentBuildVersion.ToString());

        if (!Directory.Exists(versionPath))
        {
            Directory.CreateDirectory(versionPath);
        }


        //string assetBundleVersionPath = Path.Combine(AssetBundleOutoutPath, AssetManagerConfig.CurrentBuildVersion.ToString());

        //if (!Directory.Exists(assetBundleVersionPath))
        //{
        //    Directory.CreateDirectory(assetBundleVersionPath);
        //}

        BuildAssetBundleHashTable(assetBundleBuilds, versionPath);

        CopyAssetBundleToVersionFolder(versionPath);

        BuildPackageTable(packageInfoDic,versionPath);

        CreateBuildInfo(versionPath);
        //GetVersionDifference(currentVersionAssetHashs);

        AssetManagerConfig.CurrentBuildVersion++;

        AssetDatabase.Refresh();
    }

    public static void CreateBuildInfo(string versionPath)
    {
        BuildInfo currentBuildInfo = new BuildInfo();
        currentBuildInfo.BuildVersion = AssetManagerConfig.CurrentBuildVersion;

        //获取AB包输出文件夹信息
        DirectoryInfo directoryInfo = new DirectoryInfo(versionPath);

        //获取该文件夹下所有文件信息
        FileInfo[] fileInfos = directoryInfo.GetFiles();

        //遍历该文件夹下所有文件，收集所有文件的长度
        foreach (FileInfo fileInfo in fileInfos)
        {
            currentBuildInfo.FileNames.Add(fileInfo.Name, (ulong)fileInfo.Length);
            currentBuildInfo.FizeTotalSize += (ulong)fileInfo.Length;
        }
        string buildInfoSavePath = Path.Combine(versionPath, "BuildInfo");
        string buildInfoString = JsonConvert.SerializeObject(currentBuildInfo);

        File.WriteAllTextAsync(buildInfoSavePath, buildInfoString);

    }

    public static string PackageTableName = "AllPackages";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="packages">Package是字典，key是包名</param>
    /// <param name="outputPath"></param>
    static void BuildPackageTable(Dictionary<string,PackageBuildInfo>packages,string versionPath)
    {

        string packagePath = Path.Combine(AssetBundleOutoutPath, PackageTableName);
        string packagesVersionPath = Path.Combine(versionPath, PackageTableName);

        string packagesJSON = JsonConvert.SerializeObject(packages.Keys);

        File.WriteAllText(packagePath, packagesJSON);
        File.WriteAllText(packagesVersionPath, packagesJSON);

        foreach (PackageBuildInfo package in packages.Values)
        {
            packagePath = Path.Combine(AssetBundleOutoutPath, package.PackageName);
            packagesJSON = JsonConvert.SerializeObject(package);
            packagesVersionPath = Path.Combine(versionPath, package.PackageName);

            File.WriteAllText(packagePath, packagesJSON);
            File.WriteAllText(packagesVersionPath, packagesJSON);
        }

        
    }

    static void CopyAssetBundleToVersionFolder(string versionPath)
    {
        //从AssetBundle输出路径下读取包列表
        string[] assetNames = ReadAssetBundleHashTale(AssetBundleOutoutPath);

       
        //复制主包文件
        string mainBundleOriginPath = Path.Combine(AssetBundleOutoutPath, OutputBundleName);
        string mainBundleVersionPath = Path.Combine(versionPath, OutputBundleName);
        File.Copy(mainBundleOriginPath, mainBundleVersionPath,true);

        //复制PackageInfo
        //string packageInfoPath = Path.Combine(outputPath, PackageTableName);
        //string packageInfoVersionPath = Path.Combine(assetBundleVersionPath,PackageTableName);
        //File.Copy(mainBundleOriginPath, mainBundleVersionPath, true);

        foreach (var assetName in assetNames)
        {
            string assetHashName = assetName.Substring(assetName.IndexOf("_") + 1);

            string assetOriginPath = Path.Combine(AssetBundleOutoutPath, assetHashName);
            //assetName是包含了拓展名的文件名
            string assetVersionPath = Path.Combine(versionPath, assetHashName);
            //assetName是包含了目录和完整文件名的路径
            File.Copy(assetOriginPath, assetVersionPath, true);
        }
    }

    static BuildAssetBundleOptions CheckIncrementalBuildMode()
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        switch (AssetManagerConfig._IncrementalBuildMode)
        {
            case IncrementalBuildMode.None:
                options = BuildAssetBundleOptions.None;
                break;
            case IncrementalBuildMode.IncreamentalBuild:
                options = BuildAssetBundleOptions.DeterministicAssetBundle;
                break;
            case IncrementalBuildMode.ForceBuild:
                options = BuildAssetBundleOptions.ForceRebuildAssetBundle;
                break;
            default:
                break;
        }
        return options;
    }

    static string[] ReadAssetBundleHashTale(string outputPath)
    {


        string VersionHashTablePath = Path.Combine(outputPath, "AssetBundleHashs");
        string VersionHashString = File.ReadAllText(VersionHashTablePath);
        string[] VersionAssetHashs = JsonConvert.DeserializeObject<string[]>(VersionHashString);

        return VersionAssetHashs;
    }
    ////比较包中资源变化 
    //public static void GetVersionDifference(string[] currentAssetHashs)
    //{
    //    if (AssetManagerConfig.CurrentBuildVersion >= 101)
    //    {
    //        int lastVersion = AssetManagerConfig.CurrentBuildVersion - 1;
    //        string versionString = lastVersion.ToString();
    //        for (int i = versionString.Length - 1; i >= 1; i--)
    //        {
    //            versionString = versionString.Insert(i, ".");
    //        }

    //        var lastOutoutPath = Path.Combine(Application.streamingAssetsPath, versionString, "Local");
    //        //CheckBuildOutpitPath();

    //        //string lastVersionHashTablePath = Path.Combine(lastOutoutPath, "AssetBundleHashs");
    //        //string lastVersionHashString = File.ReadAllText(lastVersionHashTablePath);
    //        string[] lastVersionAssetHashs = ReadAssetBundleHashTale(lastOutoutPath);

    //        AssetBundleVersionDiffence diffence = ContrastAssetBundleVersion(lastVersionAssetHashs, currentAssetHashs);
    //        foreach (var assetName in diffence.AdditionAssetBundles)
    //        {
    //            Debug.Log($"当前版本新增资源{assetName}");
    //        }
    //        foreach (var assetName in diffence.ReduceAssetBunddles)
    //        {
    //            Debug.Log($"当前版本减少资源{assetName}");
    //        }
    //    }
    //}

    public static void GetNodesFromDependencies(AssetBundleNode lastNode, List<AssetBundleNode> allNodes)
    {
        //有向图一层层建立依赖关系，因此不能直接获取当前资源的全部依赖
        //只获取当前资源的直接依赖

        string[] assetNames = AssetDatabase.GetDependencies(lastNode.AssetName, false);

        if (assetNames.Length==0)
        {
            //有向图到了终点
            return;
        }
        if (lastNode.OutEdge==null)
        {
            lastNode.OutEdge = new AssetBundleEdge();
        }
        foreach (string  assetName in assetNames)
        {
            if (!isValidExtentionName(assetName))
            {
                continue;
            }
            AssetBundleNode currentNode = null;
            foreach (AssetBundleNode existingNode in allNodes)
            {
                //如果当前资源名称已被某个Node使用
                //判断为相同的资源，直接使用已存在的Node
                if (existingNode.AssetName==assetName)
                {
                    currentNode = existingNode;
                    break;
                }

            }
            if (currentNode==null)
            {
                currentNode = new AssetBundleNode();
                currentNode.AssetName = assetName;
                currentNode.InEdge = new AssetBundleEdge();
                allNodes.Add(currentNode);
            }
            currentNode.InEdge.Nodes.Add(lastNode);
            lastNode.OutEdge.Nodes.Add(currentNode);

            
            //包名以及包对资源的引用 。同样通过有向图传递
            if (!string.IsNullOrEmpty(lastNode.PackageName))
            {
                if (!currentNode.PackageNames.Contains(lastNode.PackageName))
                {
                    currentNode.PackageNames.Add(lastNode.PackageName);
                }


            }
            else//DerivedAsset,直接获取lastNode的SourceIndices便可
            {
                foreach (string packageNames in lastNode.PackageNames)
                {
                    if (!currentNode.PackageNames.Contains(packageNames))
                    {
                        currentNode.PackageNames.Add(packageNames);
                    }
                }
            }
            //如果lastNode是SourceAsset，则当前Node直接添加 lastNode的Index
            //因为List是引用类型，SourceAsset的SourceIndices哪怕内容和Derived一样，也视为新List

            if (lastNode.SourceIndex>=0)
            {
                if (!currentNode.SourceIndices.Contains(lastNode.SourceIndex))
                {
                    currentNode.SourceIndices.Add(lastNode.SourceIndex);
                }
                

            }
            else
            {
                foreach (int index in lastNode.SourceIndices)
                {
                    if (!currentNode.SourceIndices.Contains(index))
                    {
                        currentNode.SourceIndices.Add(index);
                    }
                }
            }
            GetNodesFromDependencies(currentNode, allNodes);
        }
    }

    //public static void BuildAssetBundleFromEditorWindow()
    //{
    //    CheckBuildOutpitPath();
    //    //if (AssetManagerConfig.AssetBundleDirectory == null)
    //    //{
    //    //    Debug.LogError("不存在打包目录");
    //    //    return;
    //    //}
    //    //被选中的将要打包的资源列表
    //    List<string> selectedAssets = new List<string>();


    //    //选中多少资源，就打包多少个AB包
    //    AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[selectedAssets.Count];
    //    //获取文件路径
    //    //string directoryPath = AssetDatabase.GetAssetPath(AssetManagerConfig.AssetBundleDirectory);
    //    for (int i = 0; i < assetBundleBuilds.Length; i++)
    //    {
    //        //将文件目录替换为空
    //        //string bundleName = selectedAssets[i].Replace($@"{directoryPath}\", string.Empty);
    //        //unity在导入prefab文件时，默认使用导入器，AB不是预制体，会导致报错
    //        //bundleName = bundleName.Replace(".prefab", string.Empty);

    //        //将多个Asset打包至一个AB包
    //        //assetBundleName要打包的包名
    //        //现在是替换掉的文件名
    //        //assetBundleBuilds[i].assetBundleName = bundleName;
    //        //资源实际上的路径
    //        assetBundleBuilds[i].assetNames = new string[] { selectedAssets[i] };

    //    }


    //    BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);

    //    //调用该方法在打包之后调用
    //    BuildAssetBundleHashTable(assetBundleBuilds);

    //    AssetDatabase.Refresh();
    //}




    //public static string AssetBundleOutoutPath=Path.Combine(Application.persistentDataPath, MainAssetBundleName);


    [MenuItem(nameof(AssetManagerEditor) + "/" + nameof(BuildAssetBundle))]
    static void BuildAssetBundle()
    {
        CheckBuildOutpitPath();
        // string outputPath = Path.Combine(Application.dataPath, "Bundles");
        //string outputPath = Path.Combine(Application.persistentDataPath, "Bundles");
        //Application.persistentDataPath：使得你可以使用外部路径而不必使用工程内部的空间
        //该方法由unity维护

        if (!Directory.Exists(AssetBundleOutoutPath))
        {
            Directory.CreateDirectory(AssetBundleOutoutPath);
        }



        BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, CheckCompressionPattern(), EditorUserBuildSettings.activeBuildTarget);
        //执行打包工作
        // BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);




        ///BuildAssetBundles参数:
        ////outputPath_：这是 AssetBundle 要输出到的目录。可以将其更改为所需的任何输出目录,比如工程外路径，只需确保在尝试构建之前文件夹实际存在。

        ////BuildAssetBundleOptions:
        ////可以指定几个具有各种效果的不同 
        ////主要用于指定AssetBundle的压缩格式,Unity中有LZMA,LZ4两种压缩格式
        ///
        ///none:使用LZMA压缩
        ///UncompressedAssetBundle:不压缩
        ///chunkbasedcompression:使用LZ4压缩(适中，最推荐)


        ////BuildTarget
        ////BuildTarget.Standalone：这里我们告诉构建管线，我们要将这些 AssetBundle 用于哪些目标平台。可以在关于 BuildTarget 的脚本 API 参考中找到可用显式构建目标的列表。但是，如果不想在构建目标中进行硬编码，请充分利用 EditorUserBuildSettings.activeBuildTarget，它将自动找到当前设置的目标构建平台，并根据该目标构建 AssetBundle

        ////不同的平台之间都必须单独构建AsseetBundle,而不能互相通用

        ////EditorUserBuildSettings.activeBuildTarget
        ///灵活确定平台类型

        Debug.Log(AssetBundleOutoutPath);
        Debug.Log("打包完成");
    }

    public static void AddPackageInfoEditor()
    {
        AssetManagerConfig.packageInfoEditors.Add(new PackageInfoEditor());


    }


    public static void RemovePackageInfoEditor(PackageInfoEditor info)
    {
        if (AssetManagerConfig.packageInfoEditors.Contains(info))
        {
            AssetManagerConfig.packageInfoEditors.Remove(info);
        }
    }

    public static void AddAsset(PackageInfoEditor info)
    {
        info.AssetList.Add(null);
    }

    public static void RemoveAsset(PackageInfoEditor info, UnityEngine.Object asset)
    {
        if (info.AssetList.Contains(asset))
        {
            info.AssetList.Remove(asset);
        }
    }

    public static void LoadConfig(AssetManagerEditorWindow window)
    {
        if (AssetManagerConfig == null)
        {
            AssetManagerConfig = AssetDatabase.LoadAssetAtPath<AssetManagerConfigScriptableObject>("Assets/Editor/AssetManagerConfig.asset");
            window.VersionString = AssetManagerConfig.AssetManagerVersion.ToString();

            for (int i = window.VersionString.Length; i >= 1; i--)
            {
                window.VersionString = window.VersionString.Insert(i, ".");
            }
            //window.editorWindowDirectory = AssetManagerConfig.AssetBundleDirectory;
        }
        //使用AssetDataBase加载资源，只要传入Asset目录下的路径即可

    }
    public static void LoadWindowConfig(AssetManagerEditorWindow window)
    {
        if (window.windowConfig == null)
        {
            window.windowConfig = AssetDatabase.LoadAssetAtPath<AssetManagerEditorWindowConfigSO>("Assets/Editor/AssetManagerEditorWindowConfig.asset");
            window.windowConfig.TitleTextStyle = new GUIStyle();
            window.windowConfig.TitleTextStyle.fontSize = 26;
            window.windowConfig.TitleTextStyle.normal.textColor = Color.red;
            window.windowConfig.TitleTextStyle.alignment = TextAnchor.MiddleCenter;

            window.windowConfig.VersionTextStyle = new GUIStyle();
            window.windowConfig.VersionTextStyle.fontSize = 20;
            window.windowConfig.VersionTextStyle.normal.textColor = Color.cyan;
            window.windowConfig.VersionTextStyle.alignment = TextAnchor.MiddleRight;

            window.windowConfig.LogeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/leader3.png");
            window.windowConfig.LogeTextureTextStyle = new GUIStyle();
            window.windowConfig.LogeTextureTextStyle.alignment = TextAnchor.MiddleCenter;




            //window.VersionString = AssetManagerConfig.AssetManagerVersion.ToString();

            //for (int i = window.VersionString.Length; i >= 1; i--)
            //{
            //    window.VersionString = window.VersionString.Insert(i, ".");
            //}
            //window.editorWindowDirectory = AssetManagerConfig.AssetBundleDirectory;
        }
        //使用AssetDataBase加载资源，只要传入Asset目录下的路径即可

    }
    public static void LoadConfigFromJson()
    {


        string configPath = Path.Combine(Application.dataPath, "Editor/AssetManagerConfig.amc");

        string configString = File.ReadAllText(configPath);
        JsonUtility.FromJsonOverwrite(configString, AssetManagerConfig);


    }

    public static void SaveConfigToJson()
    {
        if (AssetManagerConfig != null)
        {
            string configString = JsonUtility.ToJson(AssetManagerConfig);
            string outputPath = Path.Combine(Application.dataPath, "Editor/AssetManagerConfig.amc");

            File.WriteAllText(outputPath, configString);
            AssetDatabase.Refresh();
        }
    }


    [MenuItem(nameof(AssetManagerEditor) + "/" + nameof(CreateConfig))]
    static void CreateConfig()
    {
        //声明ScriptableObjecct类型的实例
        //声明类似于Json将某个类实例化的过程
        AssetManagerConfigScriptableObject config = ScriptableObject.CreateInstance<AssetManagerConfigScriptableObject>();

        AssetDatabase.CreateAsset(config, "Assets/Editor/AssetManagerConfig.asset");

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }


    //基于GUID做一组签名，使用MD5算法得到的Hash值做命名的打包结果
    //返回一个包中所有Asset的GUID列表经过MD5算法得到的Hash字符串
    //如果GUID列表不变，方法与参数不变
    //得到的字符串是一致的
    static string ComputeAssetSetSignature(IEnumerable<string> assetNames)
    {
        var assetGUIDs = assetNames.Select(AssetDatabase.AssetPathToGUID);

        MD5 currentMD5 = MD5.Create();
        foreach (var assetGUID in assetGUIDs.OrderBy(x => x))
        {
            byte[] bytes = Encoding.ASCII.GetBytes(assetGUID);

            //使用MD5加密字符串字节数组
            currentMD5.TransformBlock(bytes, 0, bytes.Length, null, 0);

        }
        currentMD5.TransformFinalBlock(new byte[0], 0, 0);

        return BytesToHexString(currentMD5.Hash);
    }
    static string BytesToHexString(byte[] bytes)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var aByte in bytes)
        {
            //转十六位进制
            stringBuilder.Append(aByte.ToString("x2"));

        }
        return stringBuilder.ToString();
    }

    static string[] BuildAssetBundleHashTable(AssetBundleBuild[] assetBundleBuilds,string versionPath)
    {
        //表的长度与AB包的数量一致
        string[] assetBundleHashs = new string[assetBundleBuilds.Length];
        for (int i = 0; i < assetBundleBuilds.Length; i++)
        {
            if (string.IsNullOrEmpty(assetBundleBuilds[i].assetBundleName))
            {
                continue;
            }
            string assetBundlePath = Path.Combine(AssetBundleOutoutPath, assetBundleBuilds[i].assetBundleName);
            FileInfo info = new FileInfo(assetBundlePath);

            //表中记录的是一个文件的AssetBundle长度，以及其内容的Hash
            assetBundleHashs[i] = $"{info.Length}_{assetBundleBuilds[i].assetBundleName}";

        }
        string hashString = JsonConvert.SerializeObject(assetBundleHashs);
        string hashFilePath = Path.Combine(AssetBundleOutoutPath, "AssetBundleHashs");
        string hashFileVersionPath = Path.Combine(versionPath, "AssetBundleHashs");

        File.WriteAllText(hashFilePath, hashString);
        File.WriteAllText(hashFileVersionPath, hashString);
        

        //string hashString=JsonConvert
        return assetBundleHashs;
    }
    //static AssetBundleVersionDiffence ContrastAssetBundleVersion(string[] oldVersionAssets, string[] newVersionAssets)
    //{
    //    AssetBundleVersionDiffence diffence = new AssetBundleVersionDiffence();
    //    foreach (var assetName in oldVersionAssets)
    //    {
    //        if (!newVersionAssets.Contains(assetName))
    //        {
    //            diffence.ReduceAssetBunddles.Add(assetName);
    //        }
    //    }
    //    foreach (var assetName in newVersionAssets)
    //    {
    //        if (!oldVersionAssets.Contains(assetName))
    //        {
    //            diffence.AdditionAssetBundles.Add(assetName);
    //        }
    //    }
    //    return diffence;
    //}


    //获取选择物体的依赖项
    public static List<string> GetSelectedAssetsDependencies()
    {
        List<string> dependenices = new List<string>();
        List<string> selectedAAssets = new List<string>();
        for (int i = 0; i < selectedAAssets.Count; i++)
        {
            //所有通过该方法获取的数组，可视为集合列表中的元素
            string[] deps = AssetDatabase.GetDependencies(selectedAAssets[i], true);
            foreach (string depName in deps)
            {
                Debug.Log(depName);
                Debug.Log("输出");
            }
        }
        return dependenices;
    }


    static BuildAssetBundleOptions CheckCompressionPattern()
    {
        BuildAssetBundleOptions option = new BuildAssetBundleOptions();
        switch (AssetManagerConfig.CompressionPattern)
        {
            case AssetBundleCompressionPattern.LZMA:
                option = BuildAssetBundleOptions.None;

                break;
            case AssetBundleCompressionPattern.LZ4:
                option = BuildAssetBundleOptions.ChunkBasedCompression;
                break;
            case AssetBundleCompressionPattern.None:
                option = BuildAssetBundleOptions.UncompressedAssetBundle;
                break;


        }
        return option;
    }

    [MenuItem(nameof(AssetManagerEditor) + "/" + nameof(OpenAssetManagerWindow))]
    static void OpenAssetManagerWindow()
    {
        AssetManagerEditorWindow window = (AssetManagerEditorWindow)EditorWindow.GetWindow(typeof(AssetManagerEditorWindow)
            );
    }
    public static string OutputBundleName= "LocalAssets";
    static void CheckBuildOutpitPath()
    {
        //string versionString = CurrentBuildVersion.ToString();
        //for (int i = versionString.Length - 1; i >= 1; i--)
        //{
        //    versionString = versionString.Insert(i, ".");
        //}


        switch (AssetManagerConfig.BuildingPattern)
        {
            case AssetBundlePattern.EditorSimultion:
                //OutputBundleName = "Editor";
                break;

            case AssetBundlePattern.Local:
                //OutputBundleName = "BuildOutput";
                BuildOutputPath = Path.Combine(Application.streamingAssetsPath,"BuildOutput");
                break;

            case AssetBundlePattern.Remote:
                //OutputBundleName = "Remote";
                BuildOutputPath = Path.Combine(Application.persistentDataPath, "BuildOutput");

                break;
            default:
                break;
        }
         if (!Directory.Exists(BuildOutputPath))
        {
            Directory.CreateDirectory(BuildOutputPath);
        }
        AssetBundleOutoutPath = Path.Combine(BuildOutputPath, OutputBundleName); 
        if (!Directory.Exists(AssetBundleOutoutPath))
        {
            Directory.CreateDirectory(AssetBundleOutoutPath);
        }
    }

    //打包指定文件夹下所有资源为AB包
    public static void BuildAssetBundleFromDirectory()
    {
        CheckBuildOutpitPath();
        //获取文件路径
        //if (AssetManagerConfig.AssetBundleDirectory == null)
        //{
        //    Debug.LogError("打包路径不存在");
        //    return;
        //}
        //string directoryPath = AssetDatabase.GetAssetPath(AssetManagerEditor.AssetBundleDirectory);

        //string[] assetPaths = FindAllNameFromDirectory(directoryPath).ToArray();


        AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[1];

        //实际的打包出来的包名
        assetBundleBuilds[0].assetBundleName = "Local";

        //虽然叫做names, 实际上是资源在工程下的路径
        //assetBundleBuilds[0].assetNames = AssetManagerConfig. CurrentAllAssets.ToArray();

        //进行判断，看看输出的文件路径存在吗
        if (string.IsNullOrEmpty(AssetBundleOutoutPath))
        {
            Debug.LogError("输出路径为空");
            return;
        }
        else if (!Directory.Exists(AssetBundleOutoutPath))
        {
            Directory.CreateDirectory(AssetBundleOutoutPath);
        }

        //这里的Inspector所配置的AssetBundle信息，实际上就是BuildAssetBundles的结构体
        //它可以将不同已经设置的Asset打包到自定义的包中

        BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);

        Debug.Log(AssetBundleOutoutPath);

        //刷新 工程界面，不打包到工程内部可以不执行
        AssetDatabase.Refresh();
    }

    public static bool isValidExtentionName(string fileName)
    {
        bool isValid = true;
        foreach (string invalidName in AssetManagerConfig.InvalidExtensionNames)
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
