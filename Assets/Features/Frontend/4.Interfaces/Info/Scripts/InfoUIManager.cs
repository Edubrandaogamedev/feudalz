using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoUIManager : MonoBehaviour
{
    public delegate void PanelChange(CanvasGroupController _panelSelected);
    public static event PanelChange OnPanelChange;
    [Header("References")]
    [SerializeField] private InfoUIUnitsManager unitsUIManager;
    [SerializeField] private InfoUIBattleLog battleLogUI;
    [SerializeField] private InfoUIRankingLog rankingLogUI;
    [SerializeField] private InfoUIHeroes heroesUI;
    [FormerlySerializedAs("buildingsUI")][SerializeField] private InfoUIProductionManager productionUI;
    [SerializeField] private InfoUIResource resourceUI;
    [SerializeField] private InfoUIUser userUIInfo;
    [Header("Scroll Rect")]
    [SerializeField] private ScrollRect scrollContentReference;
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController pnlInfo;
    [SerializeField] private CanvasGroupController pnlBattleLog;
    [SerializeField] private CanvasGroupController pnlUnits;
    [SerializeField] private CanvasGroupController pnlInventory;
    [SerializeField] private CanvasGroupController pnlSettings;
    [SerializeField] private CanvasGroupController pnlRanking;
    [SerializeField] private CanvasGroupController pnlHeroes;
    [SerializeField] private CanvasGroupController pnlResources;
    [SerializeField] private CanvasGroupController pnlBuildings;
    [SerializeField] private CanvasGroupController pnlCraftings;
    [Header("Buttons")]
    [SerializeField] private Button btnInfo;
    [SerializeField] private Button btnBattleLog;
    [SerializeField] private Button btnUnits;
    [SerializeField] private Button btnInventory;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnRanking;
    [SerializeField] private Button btnHeroes;
    [SerializeField] private Button btnResources;
    [SerializeField] private Button btnBuildings;
    [SerializeField] private PressHandler btnMarketplace;
    private Button currentSelectedButton;
    [Header("Lists")]
    private List<CanvasGroupController> allPanels = new List<CanvasGroupController>();
    private void OnEnable()
    {
        btnInfo.onClick?.AddListener(() => { OnButtonSelected(pnlInfo); HighligthtSelectedButton(btnInfo); });
        btnBattleLog.onClick?.AddListener(() => { OnButtonSelected(pnlBattleLog); HighligthtSelectedButton(btnBattleLog); });
        btnUnits.onClick?.AddListener(() => { OnButtonSelected(pnlUnits); HighligthtSelectedButton(btnUnits); });
        btnSettings.onClick?.AddListener(() => { OnButtonSelected(pnlSettings); HighligthtSelectedButton(btnSettings); });
        btnRanking.onClick?.AddListener(() => { OnButtonSelected(pnlRanking); HighligthtSelectedButton(btnRanking); });
        btnHeroes.onClick?.AddListener(() => { OnButtonSelected(pnlHeroes); HighligthtSelectedButton(btnHeroes); });
        btnResources.onClick?.AddListener(() => { OnButtonSelected(pnlResources); HighligthtSelectedButton(btnResources); });
        btnBuildings.onClick?.AddListener(() => { OnButtonSelected(pnlBuildings); HighligthtSelectedButton(btnBuildings); });
        btnMarketplace.OnPress.AddListener(() => { Application.OpenURL("https://feudalz.io/#/marketplace");});
        allPanels = new List<CanvasGroupController>() { pnlInfo, pnlBattleLog, pnlUnits, pnlInventory, pnlSettings, pnlRanking, pnlHeroes, pnlResources, pnlBuildings };
    }
    private void OnDisable()
    {
        btnInfo.onClick?.RemoveListener(() => { OnButtonSelected(pnlInfo); HighligthtSelectedButton(btnInfo); });
        btnBattleLog.onClick?.RemoveListener(() => { OnButtonSelected(pnlBattleLog); HighligthtSelectedButton(btnBattleLog); });
        btnUnits.onClick?.RemoveListener(() => { OnButtonSelected(pnlUnits); HighligthtSelectedButton(btnUnits); });
        btnSettings.onClick?.RemoveListener(() => { OnButtonSelected(pnlSettings); HighligthtSelectedButton(btnSettings); });
        btnRanking.onClick?.RemoveListener(() => { OnButtonSelected(pnlRanking); HighligthtSelectedButton(btnRanking); });
        btnHeroes.onClick?.RemoveListener(() => { OnButtonSelected(pnlHeroes); HighligthtSelectedButton(btnHeroes); });
        btnResources.onClick?.RemoveListener(() => { OnButtonSelected(pnlResources); HighligthtSelectedButton(btnResources); });
        btnBuildings.onClick?.RemoveListener(() => { OnButtonSelected(pnlBuildings); HighligthtSelectedButton(btnBuildings); });
        btnMarketplace.OnPress.RemoveAllListeners();
    }
    private void OnButtonSelected(CanvasGroupController _selectedPanel)
    {
        _selectedPanel.Enable();
        _selectedPanel.gameObject.SetActive(true);
        OnPanelChange?.Invoke(_selectedPanel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_selectedPanel.GetComponent<RectTransform>());
        DisableNoSelectedPanels(_selectedPanel);
        scrollContentReference.content = (RectTransform)_selectedPanel.transform;
    }
    private void HighligthtSelectedButton(Button _selectedButton)
    {
        if (currentSelectedButton != null)
        {
            currentSelectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        _selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;
        currentSelectedButton = _selectedButton;
    }
    private void DisableNoSelectedPanels(CanvasGroupController excludedPanel)
    {
        foreach (CanvasGroupController panel in allPanels)
        {
            if (panel == excludedPanel) continue;
            panel.Disable();
            panel.gameObject.SetActive(false);
        }
    }
    public void SetAllUserInfo(UserSessionController userSessionController)
    {
        unitsUIManager.SetUnitsInfo(userSessionController);
        userUIInfo.UpdateUserInfo(userSessionController);
        battleLogUI.UpdateCombatLog(userSessionController.UserMaticAddress, userSessionController.Token, userSessionController.CurrentUserLandzBioma);
        heroesUI.UpdateHeroInfo(userSessionController);
        resourceUI.SetResourcesInfo(userSessionController);
        productionUI.UpdateBuildingPanel();
        productionUI.UpdateProductionPanel();
        rankingLogUI.UpdateRankingLog();
    }
    public void UpdateUserInfo(UserSessionController userSessionController)
    {
        userUIInfo.UpdateUserInfo(userSessionController);
        resourceUI.UpdateAllResources(userSessionController);
        productionUI.UpdateBuildingPanel();
        productionUI.UpdateProductionPanel();
        rankingLogUI.UpdateRankingLog();
        LayoutRebuilder.ForceRebuildLayoutImmediate(pnlInfo.GetComponent<RectTransform>());
    }

    public void SetUserInfo(UserSessionController userSessionController)
    {
        userUIInfo.UpdateUserInfo(userSessionController);
    }
    public void UpdateUserTokenBalance(float _quantity)
    {
        userUIInfo.ChangeUserTokensValue(_quantity);
    }
    public void UpdateUserRanking()
    {
        rankingLogUI.UpdateRankingLog();
    }
    public void UpdateCombatLog(string _playerAddress, string _playerToken, string _biome)
    {
        battleLogUI.UpdateCombatLog(_playerAddress, _playerToken, _biome);
    }
    public void UpdateUserHeroes(UserSessionController userSessionController)
    {
        heroesUI.UpdateHeroInfo(userSessionController);
    }

    public void UpdateResources(UserSessionController userSessionController)
    {
        resourceUI.UpdateAllResources(userSessionController);
    }
}
