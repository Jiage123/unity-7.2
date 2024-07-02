using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "AssetManagerEditorWindowConfig", menuName = "AssetManager/CreateWindowConfig")]
public class AssetManagerEditorWindowConfigSO : ScriptableObject
{
    public GUIStyle TitleTextStyle;
    public GUIStyle VersionTextStyle;
    public Texture2D LogeTexture;
    public GUIStyle LogeTextureTextStyle;

    //private void Awake()
    //{
       

    //    TitleTextStyle = new GUIStyle();
    //    TitleTextStyle.fontSize = 26;
    //    TitleTextStyle.normal.textColor = Color.red;
    //    TitleTextStyle.alignment = TextAnchor.MiddleCenter;

    //    VersionTextStyle = new GUIStyle();
    //    VersionTextStyle.fontSize = 20;
    //    VersionTextStyle.normal.textColor = Color.cyan;
    //    VersionTextStyle.alignment = TextAnchor.MiddleRight;

    //    LogeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/leader3.png");
    //    LogeTextureTextStyle = new GUIStyle();
    //    LogeTextureTextStyle.alignment = TextAnchor.MiddleCenter;
    //}
}
