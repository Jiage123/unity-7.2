using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotceUI : MonoBehaviour
{
    public GameObject uiPanel; // 需要在Inspector中分配的UI Panel 
    // Start is called before the first frame update
    // 调用这个函数来切换UI Panel的显示与隐藏  

    private void Start()
    {
        
    }
    public void TogglePanel()
    {
        // 切换uiPanel的active状态  
        uiPanel.SetActive(!uiPanel.activeInHierarchy);
    }

    // 如果你是通过Button的OnClick()来调用这个方法，不需要其他逻辑  
}
