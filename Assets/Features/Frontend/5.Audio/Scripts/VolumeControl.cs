using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    public delegate void VolumeChanged(float _volume);
    public static event VolumeChanged OnVolumeChanged;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(VolumeSlider);
        if (PlayerPrefs.HasKey("volume"))
            volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void VolumeSlider(float _volume)
    {
        PlayerPrefs.SetFloat("volume", _volume);
        OnVolumeChanged?.Invoke(_volume);
    }

    void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveAllListeners();
    }
}
