using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public delegate void Mute();
    public static event Mute OnMute;

    [SerializeField] private Sprite[] btnSprites;
    private AudioManager audioManager;
    private Image btnOwnImage;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => InvokeMute());
        audioManager = FindObjectOfType<AudioManager>();
        btnOwnImage = GetComponent<Image>();
        SetupBtn();
        OnMute?.Invoke();
    }

    void InvokeMute()
    {
        if (PlayerPrefs.GetString("mute").Contains("false"))
        {
            PlayerPrefs.SetString("mute", "true");
        }
        else
        {
            PlayerPrefs.SetString("mute", "false");
        }

        SetupBtn();
        OnMute?.Invoke();
    }

    void SetupBtn()
    {
        if (PlayerPrefs.GetString("mute").Contains("false"))
            btnOwnImage.sprite = btnSprites[1];
        else
            btnOwnImage.sprite = btnSprites[0];
    }

    void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
