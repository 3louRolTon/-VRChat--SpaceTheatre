
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

public class VideoController : UdonSharpBehaviour
{
    
    public CurrentVideoController avProVideoPlayer;
    public CurrentVideoController unityVideoPlayer;

    [HideInInspector]
    public CurrentVideoController baseVideoPlayer;
    
    public VRCUrlInputField videoURLInputField;

    public float syncFrequency = 5;
    public float syncThreshold = 1;

    [HideInInspector]
    public float _lastSyncTime = 0;
    [HideInInspector]
    [UdonSynced] public float _videoStartNetworkTime = 0;
    [HideInInspector]
    [UdonSynced] public VRCUrl _syncedURL = VRCUrl.Empty;
    [HideInInspector]
    [UdonSynced] public int _videoNumber = 0;
    [HideInInspector]
    public int _loadedVideoNumber = 0;
    [HideInInspector]
    [UdonSynced] public bool _ownerPlaying = false;
    [HideInInspector]
    public bool _waitForSync = false;

    [HideInInspector]
    [UdonSynced] public bool _videoIsPause = false;
    //[HideInInspector]
    //[UdonSynced] public float _currentVideoTime = 0;
    [HideInInspector]
    [UdonSynced] public float _startVideoPause = 0;

    public Text[] _currentVideoTimeTexts;
    public Text[] _fullVideoTimeTexts;
    public Slider[] _videoTimeSliders;

    public GameObject[] AVProObjects;
    public GameObject[] UnityObjects;

    [HideInInspector]
    [UdonSynced] public string _syncedOwnerName = "";
    [HideInInspector]
    public string _ownerName = "";
    public Text _ownerText;

    [HideInInspector]
    public bool avProPlayer = true;

    public Image UnityPlayerButton;
    public Image AVProPlayerButton;

    [HideInInspector]
    public string _videoError = "";

    private Vector4 green = new Vector4(17 / 255.0f, 160 / 255.0f, 0 / 255.0f, 1);
    private Vector4 gray = new Vector4(194 / 255.0f, 194 / 255.0f, 194 / 255.0f, 1);

    private void Start()
    {
        baseVideoPlayer = avProVideoPlayer;
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
            //_currentVideoTime = baseVideoPlayer.baseVideoPlayer.GetTime();
        }
        else
        {
            if (_waitForSync)
            {
                if (_ownerPlaying)
                {
                    baseVideoPlayer.baseVideoPlayer.Play();
                    _waitForSync = false;
                    SyncVideoIfTime();
                }
            }
            else SyncVideoIfTime();
        }

        var ts = TimeSpan.FromSeconds(baseVideoPlayer.baseVideoPlayer.GetTime());
        string currentTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

        foreach (var _currentVideoTimeText in _currentVideoTimeTexts)
            _currentVideoTimeText.text = currentTime;

        foreach (var _videoTimeSlider in _videoTimeSliders)
            _videoTimeSlider.value = baseVideoPlayer.baseVideoPlayer.GetTime();
    }

    public void OnUnityPlayer()
    {
        if (!avProPlayer) return;
        Debug.Log("OnUnityPlayer");
        ResetVideo();
        foreach(var obj in UnityObjects)
        {
            obj.SetActive(true);
        }
        foreach (var obj in AVProObjects)
        {
            obj.SetActive(false);
        }
        baseVideoPlayer = unityVideoPlayer;
        StartVideo();
        UnityPlayerButton.color = green;
        AVProPlayerButton.color = gray;
        avProPlayer = !avProPlayer;
    }

    public void OnAVProPlayer()
    {
        if (avProPlayer) return;
        Debug.Log("OnAVProPlayer");
        ResetVideo();
        foreach (var obj in UnityObjects)
        {
            obj.SetActive(false);
        }
        foreach (var obj in AVProObjects)
        {
            obj.SetActive(true);
        }
        baseVideoPlayer = avProVideoPlayer;
        StartVideo();
        UnityPlayerButton.color = gray;
        AVProPlayerButton.color = green;

        avProPlayer = !avProPlayer;
    }

    public void ResetVideo()
    {
        if(!Networking.IsOwner(this.gameObject))
            _loadedVideoNumber = -1;

        baseVideoPlayer.baseVideoPlayer.Stop();
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
                if (Mathf.Abs((float)Networking.GetServerTimeInSeconds() -_videoStartNetworkTime - _videoTimeSlider.value) > syncThreshold)
                {
                    //_currentVideoTime = _videoTimeSlider.value;
                    
                    _videoStartNetworkTime = (float)Networking.GetServerTimeInSeconds() - _videoTimeSlider.value;
                    SyncVideo();
                    //baseVideoPlayer.baseVideoPlayer.SetTime(Mathf.Clamp((float)Networking.GetServerTimeInSeconds() - _videoStartNetworkTime, 0, baseVideoPlayer.baseVideoPlayer.GetDuration()));

                    foreach (var _slider in _videoTimeSliders)
                        _slider.value = _videoTimeSlider.value;
                }
        }
    }
    public void SyncVideoIfTime()
    {
        if (Time.realtimeSinceStartup - _lastSyncTime > syncFrequency)
        {
            //Debug.Log("SyncVideoIfTime");
            _lastSyncTime = Time.realtimeSinceStartup;
            SyncVideo();
        }
    }

    public void SyncVideo()
    {
        //Debug.Log("SyncVideo");
        if (!_videoIsPause)
        {
            float temp = Mathf.Clamp((float)Networking.GetServerTimeInSeconds() - _videoStartNetworkTime, 0, baseVideoPlayer.baseVideoPlayer.GetDuration());
            if (Mathf.Abs(baseVideoPlayer.baseVideoPlayer.GetTime() - temp) > syncThreshold)
            {
                baseVideoPlayer.baseVideoPlayer.SetTime(temp);
                Debug.Log(string.Format("Syncing Video to {0:N2}", temp));
            }
        }
        else
        {
            float temp = Mathf.Clamp(_startVideoPause - _videoStartNetworkTime, 0, baseVideoPlayer.baseVideoPlayer.GetDuration());
            if (Mathf.Abs(baseVideoPlayer.baseVideoPlayer.GetTime() - temp) > syncThreshold)
            {
                baseVideoPlayer.baseVideoPlayer.SetTime(temp);
                Debug.Log(string.Format("Syncing Video to {0:N2}", temp));
            }
        }
    }

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
                        baseVideoPlayer.baseVideoPlayer.Stop();
                        baseVideoPlayer.baseVideoPlayer.LoadURL(_syncedURL);
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
                    if (_videoIsPause == baseVideoPlayer.baseVideoPlayer.IsPlaying)
                    {
                        if (_videoIsPause)
                            baseVideoPlayer.baseVideoPlayer.Pause();
                        else baseVideoPlayer.baseVideoPlayer.Play();
                    }
                }
            }
        }
    }

    public void StopVideo()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            _videoStartNetworkTime = 0;
            _startVideoPause = 0;
            //_tempCurrentTimeVideo = 0;
            //_currentVideoTime = 0;
            _ownerPlaying = false;
            _videoIsPause = false;
            baseVideoPlayer.baseVideoPlayer.Stop();
            _syncedURL = VRCUrl.Empty;
        }
    }
    public void PauseVideo()
    {
        if (Networking.IsOwner(this.gameObject))
        {
            if(_ownerPlaying)
            {
                if (_videoIsPause)
                {
                    _videoStartNetworkTime += (float)Networking.GetServerTimeInSeconds() - _startVideoPause;
                    _startVideoPause = 0;
                    baseVideoPlayer.baseVideoPlayer.Play();
                    _videoIsPause = false;
                }
                else
                {
                    _startVideoPause = (float)Networking.GetServerTimeInSeconds();
                    baseVideoPlayer.baseVideoPlayer.Pause();
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
            baseVideoPlayer.baseVideoPlayer.Stop();
            baseVideoPlayer.baseVideoPlayer.LoadURL(_syncedURL);
            foreach(var _videoTimeSlider in _videoTimeSliders)
                _videoTimeSlider.value = 0;
            _ownerPlaying = false;
            _videoIsPause = false;
            _videoStartNetworkTime = float.MaxValue;
            Debug.Log(string.Format("Video Start {0}", _syncedURL));
        }
    }

    public void VideoRewind(float sec)
    {
        //float temp = Mathf.Clamp((float)Networking.GetServerTimeInSeconds() - _videoStartNetworkTime + sec, 0, baseVideoPlayer.baseVideoPlayer.GetDuration());
        _videoStartNetworkTime -= sec;
        SyncVideo();
        //baseVideoPlayer.baseVideoPlayer.SetTime(temp);
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
