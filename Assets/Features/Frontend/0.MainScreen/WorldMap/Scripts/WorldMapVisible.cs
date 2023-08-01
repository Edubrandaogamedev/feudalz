using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapVisible : MonoBehaviour
{
    [SerializeField] private GameObject[] boards;
    [SerializeField] private Button btnVisibility;
    [SerializeField] private Image visibilityBtnImage;
    [SerializeField] private Sprite hideIcon;
    [SerializeField] private Sprite showIcon;

    private void OnEnable() {
        btnVisibility.onClick.AddListener(ToogleBoardVisibility);
        SetBoardVisibility();
    }
    private void OnDisable()
    {
        btnVisibility.onClick.RemoveAllListeners();
    }
    private void SetBoardVisibility()
    {
        bool isToShow = PlayerPrefs.GetString("visibilityState","hide") == "show";
        if (isToShow)
            visibilityBtnImage.sprite = showIcon;
        else
            visibilityBtnImage.sprite = hideIcon;
        foreach(GameObject obj in boards)
        {
            obj.gameObject.SetActive(isToShow);
        }
    }
    private void ToogleBoardVisibility()
    {
        bool isToHide = PlayerPrefs.GetString("visibilityState","hide") == "hide";
        if (PlayerPrefs.GetString("visibilityState","hide") == "hide")
            PlayerPrefs.SetString("visibilityState","show");
        else
            PlayerPrefs.SetString("visibilityState","hide");
        SetBoardVisibility();
    }
}
