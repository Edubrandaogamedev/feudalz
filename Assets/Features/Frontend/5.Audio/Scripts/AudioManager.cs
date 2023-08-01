using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Audio[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Audio s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("volume"))
            Volume(PlayerPrefs.GetFloat("volume"));   
    }

    public void Play(string sound)
    {
        Audio s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (!s.source.isPlaying && s.bgm)
            s.source.Play();
        else if (s.sfx)
            s.source.Play();
    }

    public void Stop(string sound)
    {
        Audio s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    // public void Mute()
    // {
    //     foreach (Audio s in sounds)
    //     {
    //         if (PlayerPrefs.GetString("mute").Contains("false"))
    //         {
    //             s.source.mute = true;
    //         }
    //         else
    //         {
    //             s.source.mute = false;
    //         }
    //     }
    // }

    public void Volume(float _volume)
    {
        foreach (Audio s in sounds)
        {
            s.source.volume = _volume;
        }
    }

    #region Inscrição e trancamento nos eventos

    void OnEnable()
    {
        VolumeControl.OnVolumeChanged += Volume;
    }

    void OnDisable()
    {
        VolumeControl.OnVolumeChanged -= Volume;
    }
    #endregion
}
