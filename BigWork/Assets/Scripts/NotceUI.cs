using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotceUI : MonoBehaviour
{
    public GameObject uiPanel; // ��Ҫ��Inspector�з����UI Panel 
    // Start is called before the first frame update
    // ��������������л�UI Panel����ʾ������  

    private void Start()
    {
        
    }
    public void TogglePanel()
    {
        // �л�uiPanel��active״̬  
        uiPanel.SetActive(!uiPanel.activeInHierarchy);
    }

    // �������ͨ��Button��OnClick()�������������������Ҫ�����߼�  
}
