using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Serialization;

public class RegionManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private SceneReference sceneReference;
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [Header("References")]
    [SerializeField] private RegionUI regionUI;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private AllocationSystem allocationSystem;
    [SerializeField] private AllocationInfo allocationInfo;
    private List<CombatLandz> combatLandzs = new List<CombatLandz>();
    [Header("Goldz Cost")] private const float attackAllGoldzCost = 0.25f;

    private void OnEnable()
    {
        regionUI.BtnReturnToWorldMap.onClick.AddListener(OnReturnToWorldMap);
        regionUI.BtnRechargeAll.onClick.AddListener(OnTryRechargeAllLandz);
        battleManager.onSingleAttackEnded.AddListener(() => regionUI.UpdateUserInfo(userSessionController));
        battleManager.onAttackAllEnded.AddListener(() => regionUI.UpdateUserInfo(userSessionController));
        battleManager.onSingleAttackEnded.AddListener(() => regionUI.UpdateUserLandzBuilds(userSessionController, combatLandzs));
        battleManager.onAttackAllEnded.AddListener(() => regionUI.UpdateUserLandzBuilds(userSessionController, combatLandzs));
        battleManager.RegisterTryAttackWithAllLandzListener(regionUI.BtnAttackAll.onClick, attackAllGoldzCost);
        allocationSystem.onHeroAllocationCompleted += () => regionUI.InfoManager.UpdateUserHeroes(userSessionController);
        allocationSystem.onBuildAllocationCompleted += () => regionUI.InfoManager.UpdateUserInfo(userSessionController);
        allocationInfo.onBuildAllocatedRemoved += () => regionUI.InfoManager.UpdateUserInfo(userSessionController);
        InfoUIProductionManager.UpdateLandz.AddListener(() => regionUI.UpdateUserLandzBuilds(userSessionController, combatLandzs));
        InfoUIProductionManager.UpdateLandz.AddListener(() => regionUI.UpdateUserHeroes(userSessionController));
        InfoUIProductionManager.UpdateUserInfo.AddListener(() => regionUI.InfoManager.SetUserInfo(userSessionController));
        BezierCoin.OnInsertCoins += OnGoldzReceived;
    }
    private void OnDisable()
    {
        battleManager.onSingleAttackEnded.RemoveAllListeners();
        battleManager.onAttackAllEnded.RemoveAllListeners();
        BezierCoin.OnInsertCoins -= OnGoldzReceived;
        InfoUIProductionManager.UpdateLandz.RemoveAllListeners();
        InfoUIProductionManager.UpdateUserInfo.RemoveAllListeners();
        battleManager.RemoveTryAttackWithAllLandzListener(regionUI.BtnAttackAll.onClick);
        regionUI.BtnRechargeAll.onClick.RemoveListener(OnTryRechargeAllLandz);
        RemoveLandzListeners(combatLandzs);
    }
    private void Start()
    {
        regionUI.InfoManager.SetAllUserInfo(userSessionController);
        if (userSessionController.DataController.GetLandzByBiome(userSessionController.CurrentUserLandzBioma).Count <= 0)
            regionUI.ShowNoLandzWarning();
        else
        {
            regionUI.LandzPanel.CreateLandzOnPanel(userSessionController);
            combatLandzs = regionUI.LandzPanel.CombatLandz;
            battleManager.SetCombatLandz(combatLandzs);
            AddLandzListeners(combatLandzs);
        }
        regionUI.CheckAttackAllButton(userSessionController, attackAllGoldzCost, combatLandzs);
        regionUI.BiomeInfo.SetAllAttackInfo(userSessionController);
        regionUI.Effects.PlayFallingFeedback();
    }
    private void OnTryFixLandz(CombatLandz _landz, float _goldzCost, bool goldz)
    {
        var resourceCost = new int[] {userSessionController.DataController.HealLandzItems.weapons,
        userSessionController.DataController.HealLandzItems.armors,
        userSessionController.DataController.HealLandzItems.foodCrate};
        regionUI.PopupController.EnableFixConfirmationPoup((goldz) => OnFixLandzConfirmation(_landz, _goldzCost, goldz), _goldzCost, OnFixLandzCanceled, goldz, resourceCost);
    }
    private async void OnFixLandzConfirmation(CombatLandz _landz, float _goldzCost, bool goldz)
    {
        regionUI.ChangeInputLockState(_isToLock: true);
        try
        {
            await APIServices.DatabaseServer.FixLandz(userSessionController.Token, _landz.TokenId, goldz);
            _landz.OnFixLandzSuccessful();
            userSessionController.DataController.UpdateTokenBalance(-_goldzCost);
            userSessionController.UpdateInventoryItem("Weapons", userSessionController.DataController.HealLandzItems.weapons);
            userSessionController.UpdateInventoryItem("Armors", userSessionController.DataController.HealLandzItems.armors);
            userSessionController.UpdateInventoryItem("Food Crate", userSessionController.DataController.HealLandzItems.foodCrate);
            userSessionController.SetLandzHp(_landz.TokenId, _landz.LandzHp);
            userSessionController.SetTotalAttacks();
            userSessionController.SetBiomeAttacks();
            regionUI.BiomeInfo.SetAllAttackInfo(userSessionController);
            regionUI.InfoManager.UpdateResources(userSessionController);
            OnGoldzReceived(-_goldzCost);
        }
        catch (WebRequestException ex)
        {
            if (ex.Message.Contains("enough consumables"))
                regionUI.PopupController.ErrorHandler("Resource");
            else if (userSessionController.DataController.TokenBalance < _goldzCost)
                regionUI.PopupController.ErrorHandler("Goldz");
            else
                regionUI.PopupController.ErrorHandler("Request");
        }
        finally
        {
            regionUI.PopupController.DisableFixConfirmationPoup();
            regionUI.ChangeInputLockState(_isToLock: false);
        }
    }
    private void OnFixLandzCanceled()
    {
        regionUI.PopupController.DisableFixConfirmationPoup();
    }
    private async void OnTryRechargeAllLandz()
    {
        regionUI.ChangeInputLockState(_isToLock: true);
        if (SkipPopup.GetSettings())
        {
            regionUI.ChangeInputLockState(_isToLock: false);
            OnRechargeAllConfirmation();
        }
        else
        {
            try
            {
                CheckRechargeAll rechargAllCost = null;
                rechargAllCost = await APIServices.DatabaseServer.CheckRechargeAllCost(userSessionController.Token, userSessionController.CurrentUserLandzBioma);
                regionUI.PopupController.EnableRechargeAllPoup(OnRechargeAllConfirmation, OnRechargeAllCanceled, rechargAllCost.rechargeCost);
            }
            catch (WebRequestException e)
            {
                if (e.Message.Contains("can't recharge"))
                    regionUI.PopupController.ErrorHandler("Goldz");
                else if (e.Message.Contains("available recharges"))
                    regionUI.PopupController.ErrorHandler("Avaiable Recharge");
                else
                    regionUI.PopupController.ErrorHandler("Request");
            }
            finally
            {
                regionUI.ChangeInputLockState(_isToLock: false);
            }
        }
    }
    private async void OnRechargeAllConfirmation()
    {
        regionUI.ChangeInputLockState(_isToLock: true);
        try
        {
            RechargeAll rechargeAllResult = null;
            rechargeAllResult = await APIServices.DatabaseServer.RechargeAllLandz(userSessionController.Token, userSessionController.CurrentUserLandzBioma);
            userSessionController.DataController.UpdateTokenBalance(-rechargeAllResult.rechargeCost);
            DateTimeOffset.TryParse(rechargeAllResult.timestamp, out var rechargeTimeStamp);
            foreach (var landzID in rechargeAllResult.rechargedLandzID)
            {
                var currentLandz = combatLandzs.Find(landz => landz.TokenId == landzID);
                currentLandz.OnRechargeSuccessful(rechargeTimeStamp);
                userSessionController.RechargeLandz(landzID, rechargeTimeStamp);
                regionUI.BiomeInfo.SetAllAttackInfo(userSessionController);
                regionUI.CheckAttackAllButton(userSessionController, attackAllGoldzCost, combatLandzs);
            }
            OnGoldzReceived(-rechargeAllResult.rechargeCost);
        }
        catch (WebRequestException e)
        {
            regionUI.PopupController.ErrorHandler(e.Message.Contains("can't recharge") ? "Generic Recharge" : "Request");
        }
        finally
        {
            regionUI.PopupController.DisableRechargeAllPopup();
            regionUI.ChangeInputLockState(_isToLock: false);
        }
    }
    private void OnRechargeAllCanceled()
    {
        regionUI.PopupController.DisableRechargeAllPopup();
    }
    private async void OnReturnToWorldMap()
    {
        regionUI.ShowLoadingScreen();
        await new WaitUntil(() => TaskManager.GetRegisteredTasks().Count <= 0);
        SceneManager.LoadScene(sceneReference);
    }
    private void OnGoldzReceived(float _quantity)
    {
        regionUI.InfoManager.UpdateUserTokenBalance(_quantity);
        regionUI.CheckAttackAllButton(userSessionController, attackAllGoldzCost, combatLandzs);
    }
    private void AddLandzListeners(List<CombatLandz> _combatLandzs)
    {
        foreach (var landz in _combatLandzs)
        {
            landz.onTryFixLandz += OnTryFixLandz;
            landz.onOpenAllocationMenu += allocationSystem.OpenAllocationPanel;
            landz.onOpenHeroInfoPopup += allocationInfo.OpenAllocatedHeroInfo;
            landz.onOpenBuildInfoPopup += allocationInfo.OpenAllocatedBuildInfo;
        }
    }
    private void RemoveLandzListeners(List<CombatLandz> _combatLandzs)
    {
        foreach (var landz in _combatLandzs)
        {
            landz.onTryFixLandz -= OnTryFixLandz;
            landz.onOpenAllocationMenu -= allocationSystem.OpenAllocationPanel;
            landz.onOpenHeroInfoPopup -= allocationInfo.OpenAllocatedHeroInfo;
            landz.onOpenBuildInfoPopup -= allocationInfo.OpenAllocatedBuildInfo;
        }
    }
}
