
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class VolumeController : UdonSharpBehaviour
{

    public AudioSource[] _audioSources;

    public Slider[] _volumeSliders;

    public float _currentVolume = 0.3f;

    public void ChangeVolume()
    {
        foreach(var _volumeSlider in _volumeSliders)
        {
            if (_currentVolume - _volumeSlider.value != 0)
            {
                _currentVolume = _volumeSlider.value;
                foreach (var _audioSource in _audioSources)
                    _audioSource.volume = _volumeSlider.value;

                foreach (var _slider in _volumeSliders)
                    _slider.value = _volumeSlider.value;
            }
        }
    }
}
