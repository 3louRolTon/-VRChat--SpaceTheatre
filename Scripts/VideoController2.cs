
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

public class VideoController2 : UdonSharpBehaviour
{

    public BaseVRCVideoPlayer avProVideoPlayer;
    public BaseVRCVideoPlayer unityVideoPlayer;

    private BaseVRCVideoPlayer baseVideoPlayer;
    //public VRCUrlInputField[] videoURLInputFields;
    public VRCUrlInputField videoURLInputField;

    public float syncFrequency = 5;
    public float syncThreshold = 1;

    private float _lastSyncTime = 0;
    [UdonSynced] private float _videoStartNetworkTime = 0;
    [UdonSynced] private VRCUrl _syncedURL = VRCUrl.Empty;
    [UdonSynced] private int _videoNumber = 0;
    private int _loadedVideoNumber = 0;
    [UdonSynced] private bool _ownerPlaying = false;
    private bool _waitForSync = false;

    [UdonSynced] private bool _videoIsPause = false;
    [UdonSynced] private float _currentVideoTime = 0;

    public Text[] _currentVideoTimeTexts;
    public Text[] _fullVideoTimeTexts;
    public Slider[] _videoTimeSliders;

    public GameObject[] AVProObjects;
    public GameObject[] UnityObjects;

    [UdonSynced] private string _syncedOwnerName = "";
    private string _ownerName = "";
    public Text _ownerText;

    private bool avProPlayer = true;

    public Image UnityPlayerButton;
    public Image AVProPlayerButton;

    private void Start()
    {
        baseVideoPlayer = avProVideoPlayer;
        baseVideoPlayer.Loop = false;
    }

    private void FixedUpdate()
    {
        if (_syncedOwnerName != _ownerName)
        {
            _ownerName = _syncedOwnerName;
            _ownerText.text = _ownerName;
        }

        if (Networking.IsOwner(this.gameObject) == true)
        {
            SyncVideoIfTime();
            _currentVideoTime = baseVideoPlayer.GetTime();
        }
        else
        {
            if (_waitForSync)
            {
                if (_ownerPlaying)
                {
                    baseVideoPlayer.Play();
                    _waitForSync = false;
                    SyncVideoIfTime();
                }
            }
            else SyncVideoIfTime();
        }

        var ts = TimeSpan.FromSeconds(baseVideoPlayer.GetTime());
        string currentTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

        foreach (var _currentVideoTimeText in _currentVideoTimeTexts)
            _currentVideoTimeText.text = currentTime;

        foreach (var _videoTimeSlider in _videoTimeSliders)
            _videoTimeSlider.value = baseVideoPlayer.GetTime();
    }

    public void OnUnityPlayer()
    {
        if (!avProPlayer) return;
        Debug.Log("OnUnityPlayer");
        ResetVideo();
        foreach (var obj in UnityObjects)
        {
            obj.SetActive(true);
        }
        foreach (var obj in AVProObjects)
        {
            obj.SetActive(false);
        }
        baseVideoPlayer = unityVideoPlayer;
        StartVideo();
        UnityPlayerButton.color = Color.green;
        AVProPlayerButton.color = Color.white;
        avProPlayer = !avProPlayer;
    }

    public void OnAVProPlayer()
    {
        if (avProPlayer) return;
        Debug.Log("OnAVProPlayer");
        ResetVideo();
        foreach (var obj in UnityObjects)
        {
            obj.SetActive(true);
        }
        foreach (var obj in AVProObjects)
        {
            obj.SetActive(false);
        }
        baseVideoPlayer = avProVideoPlayer;
        StartVideo();
        UnityPlayerButton.color = Color.white;
        AVProPlayerButton.color = Color.green;

        avProPlayer = !avProPlayer;
    }

    public void ResetVideo()
    {
        _loadedVideoNumber = -1;
        baseVideoPlayer.Stop();
    }

    public void SetMaster()
    {
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        _syncedOwnerName = Networking.LocalPlayer.displayName;

        Debug.Log(string.Format("Set Master {0}", Networking.LocalPlayer.displayName));
    }

    public void ChangeVideoTimeSlider()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            foreach (var _videoTimeSlider in _videoTimeSliders)
                if (Mathf.Abs(_currentVideoTime - _videoTimeSlider.value) > syncThreshold)
                {
                    baseVideoPlayer.SetTime(_videoTimeSlider.value);
                    _currentVideoTime = _videoTimeSlider.value;

                    foreach (var _slider in _videoTimeSliders)
                        _slider.value = _videoTimeSlider.value;
                }
        }
    }
    public void SyncVideoIfTime()
    {
        if (Time.realtimeSinceStartup - _lastSyncTime > syncFrequency)
        {
            Debug.Log("SyncVideoIfTime");
            _lastSyncTime = Time.realtimeSinceStartup;
            SyncVideo();
        }
    }

    public void SyncVideo()
    {
        Debug.Log("SyncVideo");
        //float temp = Mathf.Clamp((float)Networking.GetServerTimeInSeconds() - _videoStartNetworkTime, 0, baseVideoPlayer.GetDuration());
        if (Mathf.Abs(baseVideoPlayer.GetTime() - _currentVideoTime) > syncThreshold)
        {
            baseVideoPlayer.SetTime(_currentVideoTime);
            Debug.Log(string.Format("Syncing Video to {0:N2}", _currentVideoTime));
        }
    }

    //public void OnURLChanged()
    //{
    //    if (Networking.IsOwner(this.gameObject))
    //    {
    //        foreach(var _vrcUrl in videoURLInputFields)
    //        {
    //            if (_vrcUrl.GetUrl().Get() != null || _vrcUrl.GetUrl().Get() != "")
    //                _syncedURL = _vrcUrl.GetUrl();
    //            _vrcUrl.SetUrl(VRCUrl.Empty);
    //        }
    //        StartVideo();
    //        Debug.Log(string.Format("Video URL Changed to {0}", _syncedURL));
    //    }
    //}

    public void OnURLChanged()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            _syncedURL = videoURLInputField.GetUrl();
            StartVideo();
            videoURLInputField.SetUrl(VRCUrl.Empty);
            Debug.Log(string.Format("Video URL Changed to {0}", _syncedURL));
        }
    }

    private void OnVideoReady()
    {
        Debug.Log("OnVideoReady");
        if (Networking.IsOwner(this.gameObject))
        {
            baseVideoPlayer.Play();
        }
        else
        {
            if (_ownerPlaying)
            {
                baseVideoPlayer.Play();
                SyncVideo();
            }
            else _waitForSync = true;
        }
    }

    private void OnVideoStart()
    {
        Debug.Log("OnVideoStart");
        if (Networking.IsOwner(this.gameObject))
        {
            _videoStartNetworkTime = (float)Networking.GetServerTimeInSeconds();
            _ownerPlaying = true;
        }
        else
        {
            if (_ownerPlaying)
            {
                baseVideoPlayer.Pause();
                _waitForSync = true;
            }
        }

        var ts = TimeSpan.FromSeconds(baseVideoPlayer.GetDuration());
        string currentTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

        foreach (var _fullVideoTimeText in _fullVideoTimeTexts)
            _fullVideoTimeText.text = currentTime;

        foreach (var _videoTimeSlider in _videoTimeSliders)
            _videoTimeSlider.maxValue = baseVideoPlayer.GetDuration();
    }

    private void OnVideoEnd()
    {
        Debug.Log("OnVideoEnd");
        if (Networking.IsOwner(this.gameObject))
        {
            _videoStartNetworkTime = 0;
            _ownerPlaying = false;
        }
    }

    private void OnDeserialization()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            if (_videoNumber != _loadedVideoNumber)
            {
                if (Time.timeSinceLevelLoad > 10)
                {
                    if (_syncedURL.Get() != null || _syncedURL.Get() != "")
                    {
                        baseVideoPlayer.Stop();
                        baseVideoPlayer.LoadURL(_syncedURL);
                        SyncVideo();
                        _loadedVideoNumber = _videoNumber;
                        Debug.Log(string.Format("Playing synced: {0}", _syncedURL));
                    }
                }
            }
            else
            {
                if (_ownerPlaying)
                {
                    if (_videoIsPause == baseVideoPlayer.IsPlaying)
                    {
                        if (_videoIsPause)
                            baseVideoPlayer.Pause();
                        else baseVideoPlayer.Play();
                    }
                }
            }
        }
    }

    private void OnVideoError(VideoError videoError)
    {
        Debug.Log("OnVideoError");
        baseVideoPlayer.Stop();
        Debug.Log(string.Format("Video failed: {0}", _syncedURL));
    }

    public void StopVideo()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            _videoStartNetworkTime = 0;
            _ownerPlaying = false;
            _videoIsPause = false;
            baseVideoPlayer.Stop();
            _syncedURL = VRCUrl.Empty;
        }
    }
    public void PauseVideo()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            if (_ownerPlaying)
            {
                if (_videoIsPause)
                {
                    baseVideoPlayer.Play();
                    _videoIsPause = false;
                }
                else
                {
                    baseVideoPlayer.Pause();
                    _videoIsPause = true;
                }
            }
        }
    }
    public void StartVideo()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            //_syncedURL = videoURLInputField.GetUrl();
            _videoNumber++;
            _loadedVideoNumber = _videoNumber;
            baseVideoPlayer.Stop();
            baseVideoPlayer.LoadURL(_syncedURL);
            foreach (var _videoTimeSlider in _videoTimeSliders)
                _videoTimeSlider.value = 0;
            _ownerPlaying = false;
            _videoIsPause = false;
            _videoStartNetworkTime = float.MaxValue;
            Debug.Log(string.Format("Video Start {0}", _syncedURL));
        }
    }

    public void VideoRewind(float sec)
    {
        baseVideoPlayer.SetTime(baseVideoPlayer.GetTime() + sec);
    }
    public void VideoRewindAdd10S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(10);
    }
    public void VideoRewindAdd20S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(20);
    }
    public void VideoRewindAdd30S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(30);
    }
    public void VideoRewindAdd1M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(60);
    }
    public void VideoRewindAdd5M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(300);
    }
    public void VideoRewindAdd10M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(600);
    }
    public void VideoRewindTake10S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-10);
    }
    public void VideoRewindTake20S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-20);
    }
    public void VideoRewindTake30S()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-30);
    }
    public void VideoRewindTake1M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-60);
    }
    public void VideoRewindTake5M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-300);
    }
    public void VideoRewindTake10M()
    {
        if (Networking.IsOwner(this.gameObject))
            VideoRewind(-600);
    }
}
