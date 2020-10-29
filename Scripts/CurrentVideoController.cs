
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;
using VRC.Udon;

public class CurrentVideoController : UdonSharpBehaviour
{
    public BaseVRCVideoPlayer baseVideoPlayer;

    public VideoController videoController;

    public string playerName;
    private void OnVideoReady()
    {
        Debug.Log("OnVideoReady" + playerName);
        videoController._videoError = "";
        if (Networking.IsOwner(videoController.gameObject))
        {
            baseVideoPlayer.Play();
        }
        else
        {
            if (videoController._ownerPlaying)
            {
                baseVideoPlayer.Play();
                videoController.SyncVideo();
            }
            else videoController._waitForSync = true;
        }
    }

    private void OnVideoStart()
    {
        Debug.Log("OnVideoStart" + playerName);
        if (Networking.IsOwner(videoController.gameObject))
        {
            videoController._videoStartNetworkTime = (float)Networking.GetServerTimeInSeconds();
            videoController._ownerPlaying = true;
            videoController._startVideoPause = 0;
            videoController._videoError = "";
            /*if (videoController._tempCurrentTimeVideo != 0)
            {
                baseVideoPlayer.SetTime(videoController._tempCurrentTimeVideo);
                videoController._currentVideoTime = videoController._tempCurrentTimeVideo;
            }

            videoController._tempCurrentTimeVideo = 0;*/
        }
        else
        {
            if (videoController._ownerPlaying)
            {
                baseVideoPlayer.Pause();
                videoController._waitForSync = true;
            }
        }

        var ts = TimeSpan.FromSeconds(baseVideoPlayer.GetDuration());
        string currentTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

        foreach (var _fullVideoTimeText in videoController._fullVideoTimeTexts)
            _fullVideoTimeText.text = currentTime;

        foreach (var _videoTimeSlider in videoController._videoTimeSliders)
            _videoTimeSlider.maxValue = baseVideoPlayer.GetDuration();
    }

    private void OnVideoEnd()
    {
        Debug.Log("OnVideoEnd" + playerName);
        if (Networking.IsOwner(videoController.gameObject))
        {
            videoController._videoStartNetworkTime = 0;
            videoController._startVideoPause = 0;
           
            videoController._ownerPlaying = false;
        }
    }

    private void OnVideoError(VideoError videoError)
    {
        Debug.Log("OnVideoError" + playerName);
        videoController._videoError = videoError.ToString();
        baseVideoPlayer.Stop();
        Debug.Log(string.Format("Video failed: {0}", videoController._syncedURL));
    }
}
