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

//һ�������ռ���Editor�����´��ڵ�Package��Ϣ
//���Ǵ��֮�����Ϣ

[Serializable]

public class PackageInfoEditor
{

    //��ǰ��������
    //�������ڱ��������������ɶ���
    public string PackageName;

    //���ڵ�ǰ���е���Դ�б�
    //�������ڱ��������������ɶ���
    public List<UnityEngine.Object> AssetList = new List<UnityEngine.Object>();
}

public class AssetBundleEdge
{
    public List<AssetBundleNode> Nodes = new List<AssetBundleNode>();
}

public class AssetBundleNode
{
    public string AssetName;

    //�����ж��Ƿ���sourceAsset
    //���Ϊ-1����ΪDeriveedAsset
    public int SourceIndex = -1;

    //��ǰNode��Index�б�
    //ͨ������OutEdge���д���
    public List<int> SourceIndices = new List<int>();

    


    //��ǰ��Դ�����Ǹ�Package
    //���ܶ��SourceAssetӵ��ͬһ��PackageName
    public string PackageName;

    //��ǰ��Դ���ĸ�Package����
    public List<string> PackageNames = new List<string>();

    //���õ�ǰnodes��nodes
    public AssetBundleEdge InEdge;


    //��ǰnodes�����õ�nodes
    public AssetBundleEdge OutEdge;



}


public class AssetManagerEditor
{



    public static AssetManagerConfigScriptableObject AssetManagerConfig;





    //��Դ����İ汾

    //���δ������AB�����·��
    //��Ҫ�������������������������
    //AB�����·��
    public static string AssetBundleOutoutPath;


    //��������ļ������·�� 
    public static string BuildOutputPath;


    //List���������ͣ������޸Ļ�Ӧ���ڶ�Ӧ����ֵ��
    public static List<GUID> ContrastDependenciesFromGUID(List<GUID> setsA, List<GUID> setsB)
    {
        List<GUID> newDependenics = new List<GUID>();

        //ȡ����
        foreach (var assetGUID in setsA)
        {
            if (setsB.Contains(assetGUID))
            {
                newDependenics.Add(assetGUID);
            }
        }

        //ȡ�
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


    //��Դ���ϴ����
    //���Ĳ���
    //public static void BuildAssetBundleFromSets()
    //{
    //    CheckBuildOutpitPath();

    //    //��ѡ�еĴ����Դ�б�,�б�A
    //    List<string> selectedAssets = new List<string>();
    //    //�����б�L
    //    List<List<GUID>> selectedAssetDependenices = new List<List<GUID>>();

    //    //��������ѡ���sourseAsset�Լ���������ü����б�L

    //    foreach (string selectedAsset in selectedAssets)
    //    {
    //        //��ȡSourseAsset��DerivedAsset�������Ѿ�������SourseAsset
    //        string[] assetDeps = AssetDatabase.GetDependencies(selectedAsset, true);
    //        List<GUID> assetGUIDs = new List<GUID>();
    //        foreach (string assetdep in assetDeps)
    //        {
    //            GUID assetGUID = AssetDatabase.GUIDFromAssetPath(assetdep);
    //            assetGUIDs.Add(assetGUID);

    //        }
    //        //��������SourseAsset�Լ�DriveedAsset�ļ�����ӵ�����L��

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


    //        Debug.Log($"�Ա�֮ǰ{selectedAssetDependenices[i].Count}");
    //        Debug.Log($"�Ա�֮ǰ{selectedAssetDependenices[nextIndex].Count}");

    //        for (int j = 0; j <= i; j++)
    //        {
    //            List<GUID> newDenpendenies = ContrastDependenciesFromGUID(selectedAssetDependenices[j], selectedAssetDependenices[nextIndex]);
    //            //��Snew������ӵ������б�L��
    //            if (newDenpendenies != null && newDenpendenies.Count > 0)
    //            {
    //                selectedAssetDependenices.Add(newDenpendenies);
    //            }

    //        }

    //        Debug.Log($"�Ա�֮��{selectedAssetDependenices[i].Count}");
    //        Debug.Log($"�Ա�֮��{selectedAssetDependenices[nextIndex].Count}");

    //    }
    //    AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[selectedAssetDependenices.Count];
    //    for (int i = 0; i < assetBundleBuilds.Length; i++)
    //    {

    //        // assetBundleBuilds[i].assetBundleName = i.ToString();



    //        string[] assetNames = new string[selectedAssetDependenices[i].Count];

    //        //ע�⣬��ĳЩ���в���������ʱ��Ӧ�������ų�
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

    //    //�汾����
    //    AssetManagerConfig. CurrentBuildVersion++;


    //    //���ø÷����ڴ��֮�����
    //    //BuildAssetBundleHashTable(assetBundleBuilds);
    //    AssetDatabase.Refresh();
    //}

    public static void BuildAssetBundleFromDirectedGraph()
    {
        CheckBuildOutpitPath();

        List<AssetBundleNode> allNobes = new List<AssetBundleNode>();

        int sourceIndex = 0;

        Dictionary<string, PackageBuildInfo> packageInfoDic = new Dictionary<string, PackageBuildInfo>();
        #region ����ͼ����
        for (int i = 0; i < AssetManagerConfig.packageInfoEditors.Count; i++)
        {
            PackageBuildInfo packageBuildInfo = new PackageBuildInfo();
            packageBuildInfo.PackageName = AssetManagerConfig.packageInfoEditors[i].PackageName;

            packageBuildInfo.IsSourcePackage = true;

            packageInfoDic.Add(packageBuildInfo.PackageName, packageBuildInfo);


            //��ǰ��ѡ�е���Դ������SourceAsset
            //�����������SourceAsset��Node
            foreach (UnityEngine.Object asset in AssetManagerConfig.packageInfoEditors[i].AssetList)
            {

                AssetBundleNode currentNode = null;

                //����Դ�ľ���·����Ϊ��Դ����
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
        #region ����ͼ���ִ������
        Dictionary<List<int>, List<AssetBundleNode>> assetBundleNodeDic = new Dictionary<List<int>, List<AssetBundleNode>>();
        foreach (AssetBundleNode node in allNobes)
        {
            StringBuilder packNameString = new StringBuilder();

            //�������ǿջ��ޣ�˵����һ��SourceAsset,������Ѿ��ڱ༭���������
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

                //�˴�����˶�Ӧ�İ��Ͱ���
                //û����Ӿ�����ж�Ӧ��Asset
                //Asset���ʱ��ҪAssetBundleName,����ֻ��������AssetBundleName�����Asset
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
            //��������key
            //ͨ�����ַ�ʽȷ����ͬlist֮�������Ƿ�һ��
            foreach (List<int> key in assetBundleNodeDic.Keys)
            {
                //�ж�key�ĳ����Ƿ�͵�ǰnode��SourceInndices�������
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
            //���ѭ����ͬ��һ����ֵ�Ի�ȡnode
            //���Ǵ�SourceIndices��ͬ�ļ����У���ȡ��ӦNode�����Asset
            foreach (AssetBundleNode node in assetBundleNodeDic[key])
            {
                assetNames.Add(node.AssetName);

                //�����һ����ʼ��������PackageNameֻ�����Լ�
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
                //��Ϊ������DerivePackage
                //�˴�����ȷ��ÿ��Node����һ������
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

        //�����汾·��
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

        //��ȡAB������ļ�����Ϣ
        DirectoryInfo directoryInfo = new DirectoryInfo(versionPath);

        //��ȡ���ļ����������ļ���Ϣ
        FileInfo[] fileInfos = directoryInfo.GetFiles();

        //�������ļ����������ļ����ռ������ļ��ĳ���
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
    /// <param name="packages">Package���ֵ䣬key�ǰ���</param>
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
        //��AssetBundle���·���¶�ȡ���б�
        string[] assetNames = ReadAssetBundleHashTale(AssetBundleOutoutPath);

       
        //���������ļ�
        string mainBundleOriginPath = Path.Combine(AssetBundleOutoutPath, OutputBundleName);
        string mainBundleVersionPath = Path.Combine(versionPath, OutputBundleName);
        File.Copy(mainBundleOriginPath, mainBundleVersionPath,true);

        //����PackageInfo
        //string packageInfoPath = Path.Combine(outputPath, PackageTableName);
        //string packageInfoVersionPath = Path.Combine(assetBundleVersionPath,PackageTableName);
        //File.Copy(mainBundleOriginPath, mainBundleVersionPath, true);

        foreach (var assetName in assetNames)
        {
            string assetHashName = assetName.Substring(assetName.IndexOf("_") + 1);

            string assetOriginPath = Path.Combine(AssetBundleOutoutPath, assetHashName);
            //assetName�ǰ�������չ�����ļ���
            string assetVersionPath = Path.Combine(versionPath, assetHashName);
            //assetName�ǰ�����Ŀ¼�������ļ�����·��
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
    ////�Ƚϰ�����Դ�仯 
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
    //            Debug.Log($"��ǰ�汾������Դ{assetName}");
    //        }
    //        foreach (var assetName in diffence.ReduceAssetBunddles)
    //        {
    //            Debug.Log($"��ǰ�汾������Դ{assetName}");
    //        }
    //    }
    //}

    public static void GetNodesFromDependencies(AssetBundleNode lastNode, List<AssetBundleNode> allNodes)
    {
        //����ͼһ��㽨��������ϵ����˲���ֱ�ӻ�ȡ��ǰ��Դ��ȫ������
        //ֻ��ȡ��ǰ��Դ��ֱ������

        string[] assetNames = AssetDatabase.GetDependencies(lastNode.AssetName, false);

        if (assetNames.Length==0)
        {
            //����ͼ�����յ�
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
                //�����ǰ��Դ�����ѱ�ĳ��Nodeʹ��
                //�ж�Ϊ��ͬ����Դ��ֱ��ʹ���Ѵ��ڵ�Node
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

            
            //�����Լ�������Դ������ ��ͬ��ͨ������ͼ����
            if (!string.IsNullOrEmpty(lastNode.PackageName))
            {
                if (!currentNode.PackageNames.Contains(lastNode.PackageName))
                {
                    currentNode.PackageNames.Add(lastNode.PackageName);
                }


            }
            else//DerivedAsset,ֱ�ӻ�ȡlastNode��SourceIndices���
            {
                foreach (string packageNames in lastNode.PackageNames)
                {
                    if (!currentNode.PackageNames.Contains(packageNames))
                    {
                        currentNode.PackageNames.Add(packageNames);
                    }
                }
            }
            //���lastNode��SourceAsset����ǰNodeֱ����� lastNode��Index
            //��ΪList���������ͣ�SourceAsset��SourceIndices�������ݺ�Derivedһ����Ҳ��Ϊ��List

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
    //    //    Debug.LogError("�����ڴ��Ŀ¼");
    //    //    return;
    //    //}
    //    //��ѡ�еĽ�Ҫ�������Դ�б�
    //    List<string> selectedAssets = new List<string>();


    //    //ѡ�ж�����Դ���ʹ�����ٸ�AB��
    //    AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[selectedAssets.Count];
    //    //��ȡ�ļ�·��
    //    //string directoryPath = AssetDatabase.GetAssetPath(AssetManagerConfig.AssetBundleDirectory);
    //    for (int i = 0; i < assetBundleBuilds.Length; i++)
    //    {
    //        //���ļ�Ŀ¼�滻Ϊ��
    //        //string bundleName = selectedAssets[i].Replace($@"{directoryPath}\", string.Empty);
    //        //unity�ڵ���prefab�ļ�ʱ��Ĭ��ʹ�õ�������AB����Ԥ���壬�ᵼ�±���
    //        //bundleName = bundleName.Replace(".prefab", string.Empty);

    //        //�����Asset�����һ��AB��
    //        //assetBundleNameҪ����İ���
    //        //�������滻�����ļ���
    //        //assetBundleBuilds[i].assetBundleName = bundleName;
    //        //��Դʵ���ϵ�·��
    //        assetBundleBuilds[i].assetNames = new string[] { selectedAssets[i] };

    //    }


    //    BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);

    //    //���ø÷����ڴ��֮�����
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
        //Application.persistentDataPath��ʹ�������ʹ���ⲿ·��������ʹ�ù����ڲ��Ŀռ�
        //�÷�����unityά��

        if (!Directory.Exists(AssetBundleOutoutPath))
        {
            Directory.CreateDirectory(AssetBundleOutoutPath);
        }



        BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, CheckCompressionPattern(), EditorUserBuildSettings.activeBuildTarget);
        //ִ�д������
        // BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);




        ///BuildAssetBundles����:
        ////outputPath_������ AssetBundle Ҫ�������Ŀ¼�����Խ������Ϊ������κ����Ŀ¼,���繤����·����ֻ��ȷ���ڳ��Թ���֮ǰ�ļ���ʵ�ʴ��ڡ�

        ////BuildAssetBundleOptions:
        ////����ָ���������и���Ч���Ĳ�ͬ 
        ////��Ҫ����ָ��AssetBundle��ѹ����ʽ,Unity����LZMA,LZ4����ѹ����ʽ
        ///
        ///none:ʹ��LZMAѹ��
        ///UncompressedAssetBundle:��ѹ��
        ///chunkbasedcompression:ʹ��LZ4ѹ��(���У����Ƽ�)


        ////BuildTarget
        ////BuildTarget.Standalone���������Ǹ��߹������ߣ�����Ҫ����Щ AssetBundle ������ЩĿ��ƽ̨�������ڹ��� BuildTarget �Ľű� API �ο����ҵ�������ʽ����Ŀ����б����ǣ���������ڹ���Ŀ���н���Ӳ���룬�������� EditorUserBuildSettings.activeBuildTarget�������Զ��ҵ���ǰ���õ�Ŀ�깹��ƽ̨�������ݸ�Ŀ�깹�� AssetBundle

        ////��ͬ��ƽ̨֮�䶼���뵥������AsseetBundle,�����ܻ���ͨ��

        ////EditorUserBuildSettings.activeBuildTarget
        ///���ȷ��ƽ̨����

        Debug.Log(AssetBundleOutoutPath);
        Debug.Log("������");
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
        //ʹ��AssetDataBase������Դ��ֻҪ����AssetĿ¼�µ�·������

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
        //ʹ��AssetDataBase������Դ��ֻҪ����AssetĿ¼�µ�·������

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
        //����ScriptableObjecct���͵�ʵ��
        //����������Json��ĳ����ʵ�����Ĺ���
        AssetManagerConfigScriptableObject config = ScriptableObject.CreateInstance<AssetManagerConfigScriptableObject>();

        AssetDatabase.CreateAsset(config, "Assets/Editor/AssetManagerConfig.asset");

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }


    //����GUID��һ��ǩ����ʹ��MD5�㷨�õ���Hashֵ�������Ĵ�����
    //����һ����������Asset��GUID�б���MD5�㷨�õ���Hash�ַ���
    //���GUID�б��䣬�������������
    //�õ����ַ�����һ�µ�
    static string ComputeAssetSetSignature(IEnumerable<string> assetNames)
    {
        var assetGUIDs = assetNames.Select(AssetDatabase.AssetPathToGUID);

        MD5 currentMD5 = MD5.Create();
        foreach (var assetGUID in assetGUIDs.OrderBy(x => x))
        {
            byte[] bytes = Encoding.ASCII.GetBytes(assetGUID);

            //ʹ��MD5�����ַ����ֽ�����
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
            //תʮ��λ����
            stringBuilder.Append(aByte.ToString("x2"));

        }
        return stringBuilder.ToString();
    }

    static string[] BuildAssetBundleHashTable(AssetBundleBuild[] assetBundleBuilds,string versionPath)
    {
        //��ĳ�����AB��������һ��
        string[] assetBundleHashs = new string[assetBundleBuilds.Length];
        for (int i = 0; i < assetBundleBuilds.Length; i++)
        {
            if (string.IsNullOrEmpty(assetBundleBuilds[i].assetBundleName))
            {
                continue;
            }
            string assetBundlePath = Path.Combine(AssetBundleOutoutPath, assetBundleBuilds[i].assetBundleName);
            FileInfo info = new FileInfo(assetBundlePath);

            //���м�¼����һ���ļ���AssetBundle���ȣ��Լ������ݵ�Hash
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


    //��ȡѡ�������������
    public static List<string> GetSelectedAssetsDependencies()
    {
        List<string> dependenices = new List<string>();
        List<string> selectedAAssets = new List<string>();
        for (int i = 0; i < selectedAAssets.Count; i++)
        {
            //����ͨ���÷�����ȡ�����飬����Ϊ�����б��е�Ԫ��
            string[] deps = AssetDatabase.GetDependencies(selectedAAssets[i], true);
            foreach (string depName in deps)
            {
                Debug.Log(depName);
                Debug.Log("���");
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

    //���ָ���ļ�����������ԴΪAB��
    public static void BuildAssetBundleFromDirectory()
    {
        CheckBuildOutpitPath();
        //��ȡ�ļ�·��
        //if (AssetManagerConfig.AssetBundleDirectory == null)
        //{
        //    Debug.LogError("���·��������");
        //    return;
        //}
        //string directoryPath = AssetDatabase.GetAssetPath(AssetManagerEditor.AssetBundleDirectory);

        //string[] assetPaths = FindAllNameFromDirectory(directoryPath).ToArray();


        AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[1];

        //ʵ�ʵĴ�������İ���
        assetBundleBuilds[0].assetBundleName = "Local";

        //��Ȼ����names, ʵ��������Դ�ڹ����µ�·��
        //assetBundleBuilds[0].assetNames = AssetManagerConfig. CurrentAllAssets.ToArray();

        //�����жϣ�����������ļ�·��������
        if (string.IsNullOrEmpty(AssetBundleOutoutPath))
        {
            Debug.LogError("���·��Ϊ��");
            return;
        }
        else if (!Directory.Exists(AssetBundleOutoutPath))
        {
            Directory.CreateDirectory(AssetBundleOutoutPath);
        }

        //�����Inspector�����õ�AssetBundle��Ϣ��ʵ���Ͼ���BuildAssetBundles�Ľṹ��
        //�����Խ���ͬ�Ѿ����õ�Asset������Զ���İ���

        BuildPipeline.BuildAssetBundles(AssetBundleOutoutPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);

        Debug.Log(AssetBundleOutoutPath);

        //ˢ�� ���̽��棬������������ڲ����Բ�ִ��
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
