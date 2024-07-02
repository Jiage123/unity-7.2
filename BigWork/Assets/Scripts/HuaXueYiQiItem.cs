using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HuaXueYiQiItem : MonoBehaviour
{
    public GameObject uiPanel;
    public VideoPlayer videoPlayer;
    private bool isPlay = false;
    void Start()
    {
        //isPlay = false;
        uiPanel.SetActive(false);
    }
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                if (hit.collider.gameObject.name== "zhuixingping")
                {
                    isPlay = false;
                    uiPanel.SetActive(true);
                }
            }
        }
    }

    public void UIClose()
    {
        isPlay = false;
        uiPanel.SetActive(false);
    }

    public void PlayVideo()
    {
        isPlay = true;
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        isPlay = false;
        videoPlayer.Stop();
    }

    public void PauseVideo()
    {
        isPlay = false;
        videoPlayer.Pause();
    }
}
