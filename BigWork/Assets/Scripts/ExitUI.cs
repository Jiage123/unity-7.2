using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnExitGame()
    {
#if UNITY_EDITOR //1在编辑器模式退出
        UnityEditor.EditorApplication.isPlaying = false;
#else
     Application.Quit();
#endif
    }
}
