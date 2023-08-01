using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipPopup : MonoBehaviour
{
    [SerializeField] private Slider skipPopup;
    private void OnEnable()
    {
        skipPopup.onValueChanged.AddListener(SetSkipPopup);
        skipPopup.value = PlayerPrefs.GetInt("skipPopup",0);
    }
    private void OnDisable()
    {
        skipPopup.onValueChanged.RemoveAllListeners();
    }
    private void SetSkipPopup(float _setBattleAnim)
    {
        PlayerPrefs.SetInt("skipPopup", (int)_setBattleAnim);
    }
    public static bool GetSettings()
    {
        // 1 = true
        if (PlayerPrefs.GetInt("skipPopup",0) == 1)
            return true;
        return false;
    }
}
