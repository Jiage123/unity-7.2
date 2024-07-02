using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoUI : MonoBehaviour
{
    public GameObject uiPanel;
    

    void Start()
    {
        uiPanel.SetActive(false);
    }

    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiPanel.SetActive(false);
        }
    }

    
}