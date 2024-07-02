using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetManagerEditorWindow : EditorWindow
{
 
    public string VersionString;

    public AssetManagerEditorWindowConfigSO windowConfig;
    // Start is called before the first frame update

    private void Awake()
    {
        AssetManagerEditor.LoadConfig(this);
        AssetManagerEditor.LoadWindowConfig(this);
    }
    private void OnValidate()
    {
        AssetManagerEditor.LoadConfig(this);
        AssetManagerEditor.LoadWindowConfig(this);
    }
    private void OnInspectorUpdate()
    {
        AssetManagerEditor.LoadConfig(this);
        AssetManagerEditor.LoadWindowConfig(this);
    }
    private void OnEnable()
    {
        //AssetManagerEditor.AssetManagerConfig.GetCurrentDirectoryAllAssets();
    }

    //public DefaultAsset editorWindowDirectory=null;
    private void OnGUI()
    {
        //Ĭ�ϴ�ֱ�Ű�
        GUILayout.Space(20);


        if (windowConfig. LogeTexture != null)
        {
            GUILayout.Label(windowConfig.LogeTexture, windowConfig.LogeTextureTextStyle);
        }

        GUILayout.Label(nameof(AssetManagerEditor), windowConfig.TitleTextStyle);



        GUILayout.Space(20);
        GUILayout.Label(VersionString, windowConfig.VersionTextStyle);




        GUILayout.Space(20);
        AssetManagerEditor.AssetManagerConfig.BuildingPattern = (AssetBundlePattern)EditorGUILayout.EnumPopup
            ("���ģʽ", AssetManagerEditor.AssetManagerConfig.BuildingPattern);

        GUILayout.Space(20);
        AssetManagerEditor.AssetManagerConfig.CompressionPattern = (AssetBundleCompressionPattern)
        EditorGUILayout.EnumPopup("ѹ����ʽ", AssetManagerEditor.AssetManagerConfig.CompressionPattern);

        GUILayout.Space(20);
        AssetManagerEditor.AssetManagerConfig._IncrementalBuildMode = (IncrementalBuildMode)
            EditorGUILayout.EnumPopup("�������", AssetManagerEditor.AssetManagerConfig._IncrementalBuildMode);


        GUILayout.Space(20);

        GUILayout.BeginVertical("frameBox");
        GUILayout.Space(10);
        for (int i = 0; i < AssetManagerEditor.AssetManagerConfig.packageInfoEditors.Count; i++)
        {
            PackageInfoEditor packageInfo = AssetManagerEditor.AssetManagerConfig.packageInfoEditors[i];

            GUILayout.BeginVertical("frameBox");

            GUILayout.BeginHorizontal();
            packageInfo.PackageName = EditorGUILayout.TextField("PackageName", packageInfo.PackageName);

            if (GUILayout.Button("Remove"))
            {
                AssetManagerEditor.RemovePackageInfoEditor(packageInfo);
            }
            GUILayout.EndHorizontal();

            
            GUILayout.Space(10);

            for (int j = 0; j < packageInfo.AssetList.Count; j++)
            {
                GUILayout.BeginHorizontal();
                packageInfo.AssetList[j] = EditorGUILayout.ObjectField(packageInfo.AssetList[j], typeof(GameObject)) as GameObject;
                if (GUILayout.Button("Remove"))
                {
                    AssetManagerEditor.RemoveAsset(packageInfo, packageInfo.AssetList[j]);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.Space(5);
            if (GUILayout.Button("����Asset"))
            {
                AssetManagerEditor.AddAsset(packageInfo);
            }
            GUILayout.EndVertical();
            GUILayout.Space(20);
        }
        
        GUILayout.Space(10);

        if (GUILayout.Button("����Package"))
        {
            AssetManagerEditor.AddPackageInfoEditor();
        }
        

        GUILayout.EndVertical();

        //editorWindowDirectory = EditorGUILayout.ObjectField
        //    (editorWindowDirectory, typeof(DefaultAsset), true) as DefaultAsset;

        //if (AssetManagerEditor.AssetManagerConfig.AssetBundleDirectory!=editorWindowDirectory)
        //{
        //    if (editorWindowDirectory==null)
        //    {
        //        AssetManagerEditor.AssetManagerConfig.CurrentAllAssets.Clear();
        //    }
        //    AssetManagerEditor.AssetManagerConfig.AssetBundleDirectory = editorWindowDirectory;
        //    AssetManagerEditor.AssetManagerConfig.GetCurrentDirectoryAllAssets();
        //}

        //if (AssetManagerEditor.AssetManagerConfig.CurrentAllAssets != null)
        //{
        //    for (int i = 0; i < AssetManagerEditor.AssetManagerConfig.CurrentAllAssets.Count; i++)
        //    {
        //        AssetManagerEditor.AssetManagerConfig.CurreSelectAssets[i] =
        //EditorGUILayout.ToggleLeft
        //(AssetManagerEditor.AssetManagerConfig.CurrentAllAssets[i], AssetManagerEditor.AssetManagerConfig.CurreSelectAssets[i]);
        //    }
        //}


        GUILayout.Space(20);
        if (GUILayout.Button("���AB"))
        {
            AssetManagerEditor.BuildAssetBundleFromDirectedGraph();
            //AssetManagerEditor.BuildAssetBundleFromSets();
            //AssetManagerEditor.BuildAssetBundleFromEditorWindow();
            //AssetManagerEditor.BuildAssetBundleFromDirectory();
            Debug.Log("EditorButton����");
        }
        ;
        GUILayout.Space(20);
        if (GUILayout.Button("����Config"))
        {
            AssetManagerEditor.SaveConfigToJson();
            //AssetManagerEditor.BuildAssetBundleFromEditorWindow();
            //AssetManagerEditor.BuildAssetBundleFromDirectory();
            Debug.Log("SaveConfigToJson����");
        }

        GUILayout.Space(20);
        if (GUILayout.Button("��ȡConfigJson�ļ�"))
        {
            AssetManagerEditor.LoadConfigFromJson();
            //AssetManagerEditor.BuildAssetBundleFromEditorWindow();
            //AssetManagerEditor.BuildAssetBundleFromDirectory();
            //Debug.Log("SaveConfigToJson����");
        }
    }
}
