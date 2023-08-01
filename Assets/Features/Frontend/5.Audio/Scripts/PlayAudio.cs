using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public void PlayAudioClip(string clipName)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play(clipName);
    }

    public void StopAudioClip(string clipName)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Stop(clipName);
    }
}
