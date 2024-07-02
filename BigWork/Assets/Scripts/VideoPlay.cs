using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool isPlay = false;

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

    public void ToggleVideo()
    {
        if (isPlay)
        {
            StopVideo();
        }
        else
        {
            PlayVideo();
        }
    }

    
}