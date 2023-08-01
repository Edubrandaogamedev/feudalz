using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapSettings : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnClose;
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController settingsPanel;
    [Header("Objects")] 
    [SerializeField] private GameObject fadeBackground;
    private void OnEnable()
    {
        RefreshNftsBehavior.OnRefreshedStarted += CloseSettingsPanel;
        btnSettings.onClick.AddListener(OpenSettingsPanel);
        btnClose.onClick.AddListener(CloseSettingsPanel);
    }
    private void OnDisable()
    {
        RefreshNftsBehavior.OnRefreshedStarted -= CloseSettingsPanel;
        btnSettings.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
    }
    private void OpenSettingsPanel()
    {
        settingsPanel.Enable();
        fadeBackground.SetActive(true);
    }
    private void CloseSettingsPanel()
    {
        settingsPanel.Disable();
        fadeBackground.SetActive(false);
    }
}
