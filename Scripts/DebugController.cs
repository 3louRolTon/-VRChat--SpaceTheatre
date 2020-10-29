
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class DebugController : UdonSharpBehaviour
{

    public VideoController _videoPlayerSettings;

    public Text[] _ownerNameTexts;

    public Text _syncedURLText;

    public Text _videoStartNetworkTimeText;
    public Text _videoStartNetworkPauseTimeText;

    public Text _videoNumberText;
    public Text _loadedVideoNumberText;

    public Text _ownerPlayingText;

    public Text _waitForSyncText;

    public Text _videoIsReadyText;
    public Text _videoIsPauseText;

    public Text _videoErrorText;

    private void FixedUpdate()
    {
        _syncedURLText.text = _videoPlayerSettings._syncedURL.Get();

        _videoNumberText.text = _videoPlayerSettings._videoNumber.ToString();
        _loadedVideoNumberText.text = _videoPlayerSettings._loadedVideoNumber.ToString();

        _videoStartNetworkTimeText.text = _videoPlayerSettings._videoStartNetworkTime.ToString();
        _videoStartNetworkPauseTimeText.text = _videoPlayerSettings._startVideoPause.ToString();

        _waitForSyncText.text = _videoPlayerSettings._waitForSync.ToString();
        _videoIsReadyText.text = _videoPlayerSettings.baseVideoPlayer.baseVideoPlayer.IsReady.ToString();
        _videoIsPauseText.text = _videoPlayerSettings._videoIsPause.ToString();

        _ownerPlayingText.text = _videoPlayerSettings._ownerPlaying.ToString();

        _videoErrorText.text = _videoPlayerSettings._videoError;

        foreach (var _ownerNameText in _ownerNameTexts)
        {
            _ownerNameText.text = _videoPlayerSettings._syncedOwnerName;
        }
    }
}
