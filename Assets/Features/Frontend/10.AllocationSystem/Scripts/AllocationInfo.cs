using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AllocationInfo : MonoBehaviour
{
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController mainPanel;

    [SerializeField] private CanvasGroupController lockInputPanel;

    [Header("References")]
    [SerializeField] private AllocationInfoPopupController popupController;
    [SerializeField] private HeroezAllocationTemplate allocatedHero;
    [SerializeField] private BuildingAllocatedTemplate allocatedBuild;
    [Header("Cache")]
    private CombatLandz cachedLandz;
    private FeudalzHeroez cachedHero;
    private FeudalzLandzAllocatedBuild cachedBuild;
    public event UnityAction onBuildAllocatedRemoved;
    public static readonly UnityEvent onUpdateProductionPanel = new UnityEvent();
    private void OnEnable()
    {
        allocatedHero.RegisterCloseListener(CloseInfoPanel);
        allocatedBuild.RegisterCloseListener(CloseInfoPanel);
    }
    public void OpenAllocatedHeroInfo(CombatLandz _landz, FeudalzHeroez _hero)
    {
        cachedLandz = _landz;
        cachedHero = _hero;
        allocatedHero.gameObject.SetActive(true);
        if (_hero.heroRarity != "unique")
            allocatedHero.Setup(_hero);
        else
            allocatedHero.SetupUnique(_hero);
        allocatedHero.RegisterRemoveHeroListener(OnTryRemoveHero);
        mainPanel.Enable();
    }
    public void OpenAllocatedBuildInfo(CombatLandz _landz, FeudalzLandzAllocatedBuild _build)
    {
        cachedLandz = _landz;
        cachedBuild = _build;
        allocatedBuild.gameObject.SetActive(true);
        allocatedBuild.Setup(_build);
        allocatedBuild.RegisterRemoveBuildListener(OnTryRemoveBuild);
        allocatedBuild.RegisterAutoRebuildListener(ChangeAutoRebuildSettings);
        mainPanel.Enable(); ;
    }
    public void CloseInfoPanel()
    {
        mainPanel.Disable();
        allocatedHero.gameObject.SetActive(false);
        allocatedBuild.gameObject.SetActive(false);
        cachedLandz = null;
    }

    private async void ChangeAutoRebuildSettings(float _value)
    {
        lockInputPanel.Enable();
        allocatedBuild.LockInput();
        await APIServices.DatabaseServer.SwitchAutoRebuild(userSessionController.Token,cachedLandz.TokenId,cachedBuild.position,_value != 0);
        allocatedBuild.UnlockInput();
        lockInputPanel.Disable();
    }
    private void OnTryRemoveHero()
    {

        popupController.EnableHeroRemovePopup(OnRemoveHeroConfirmed);
    }
    private async void OnRemoveHeroConfirmed()
    {
        allocatedHero.LockInput();
        lockInputPanel.Enable();
        await APIServices.DatabaseServer.RemoveAllocatedHero(userSessionController.Token, cachedLandz.TokenId);
        lockInputPanel.Disable();
        allocatedHero.UnlockInput();
        userSessionController.CombatLandzs.Find(land => land.tokenId == cachedLandz.TokenId).heroez = null;
        cachedLandz.OnHeroRemoveSuccessful();
        popupController.DisablePopup();
        CloseInfoPanel();
        ClearCache();
    }

    private void OnTryRemoveBuild()
    {
        popupController.EnableBuildRemovePopup(OnRemoveBuildConfirmed);
    }
    private async void OnRemoveBuildConfirmed()
    {
        lockInputPanel.Enable();
        allocatedBuild.LockInput();
        var response = await APIServices.DatabaseServer.RemoveAllocatedBuild(userSessionController.Token, cachedLandz.TokenId, cachedBuild.position);
        cachedLandz.OnBuildRemoveSuccessful(cachedBuild);
        userSessionController.UpdateLandzBuildsInfo(cachedLandz);
        userSessionController.DataController.UpdateProductionItems(response.batches.ToList());
        allocatedBuild.UnlockInput();
        lockInputPanel.Disable();
        popupController.DisablePopup();
        onBuildAllocatedRemoved?.Invoke();
        onUpdateProductionPanel?.Invoke();
        CloseInfoPanel();
        ClearCache();
    }
    private void ClearCache()
    {
        cachedHero = null;
        cachedLandz = null;
        cachedBuild = null;
    }
}
