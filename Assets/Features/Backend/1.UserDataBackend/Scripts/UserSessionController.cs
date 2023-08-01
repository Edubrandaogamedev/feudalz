using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;
using EnhancedScrollerDemos.ExpandingCells;
using Features.Refactor;

//[CreateAssetMenu(fileName = "UserSessionDataManager", menuName = "User/UserSessionDataManager")]
public class UserSessionController : ScriptableObject
{
    private readonly UserDataController _userDataController;
    public UserDataController DataController => _userDataController;
    public bool Initialized { get; private set; }
    // User Account Info Scope
    public string UserMaticAddress { get; private set;}
    public string Token {get; private set;}
    public string CurrentUserLandzBioma { get; private set;}
    public int TotalAttacksAvailable { get; private set; }
    public int TotalAttacks { get; private set; }
    public int CurrentBiomeAttacksAvailable { get; private set;}
    public int CurrentBiomeTotalAttacks { get; private set; }
    //events
    public event UnityAction onInitalized;
    public event UnityAction onRetryLogin;
    
    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    
    private void OnDisable()
    {
        ClearData();
    }
    private void ClearData()
    {
        Initialized = false;
    }
    
    public void InitializeSession(UserInfo userData)
    {
        _userDataController.SetUserData(userData);
        SetTotalAttacks();
        Initialized = true;
        onInitalized?.Invoke();
    }
    
    public void SetTotalAttacks()
    {
        TotalAttacksAvailable = 0;
        TotalAttacks = 0;
        foreach (var landz in DataController.GetAllLandz())
        {
            if (!(landz.health > 0)) continue;
            TotalAttacksAvailable += landz.attacks;
            TotalAttacks += landz.maxAttacks;
        }
    }
    public void SetBiomeAttacks()
    {
        var landzFromBiome = DataController.GetLandzByBiome(CurrentUserLandzBioma);
        CurrentBiomeAttacksAvailable = 0;
        CurrentBiomeTotalAttacks = 0;
        foreach (var landz in landzFromBiome)
        {
            if (!(landz.health > 0)) continue;
            CurrentBiomeAttacksAvailable += landz.attacks;
            CurrentBiomeTotalAttacks += landz.maxAttacks;
        }
    }
    private void ChangeUserAttacks(int value)
    {
        TotalAttacksAvailable = Mathf.Max(0,TotalAttacksAvailable + value);
        CurrentBiomeAttacksAvailable = Mathf.Max(0,CurrentBiomeAttacksAvailable + value);
    }
    public void ReduceAttackFromLandz(int landId)
    {
        var foundedLandz = DataController.GetAllLandz().Find(landz => landz.tokenId == landId);
        if (foundedLandz == null) return;
        foundedLandz.attacks = Mathf.Max(0,foundedLandz.attacks-1);
        ChangeUserAttacks(-1);
    }
    public void IncreaseAttackFromLandz(int landId)
    {
        var foundedLandz = DataController.GetAllLandz().Find(landz => landz.tokenId == landId);
        if (foundedLandz == null) return;
        foundedLandz.attacks++;
        ChangeUserAttacks(1);
    }
    public void SetLandzHp(int landId, float _currentHealth)
    {
        var foundedLandz = DataController.GetAllLandz().Find(landz => landz.tokenId == landId);
        if (foundedLandz != null)
        {
            foundedLandz.health = _currentHealth;
        }
    }
    public void RechargeLandz(int _landId, DateTimeOffset _lastRechargeTimeStamp)
    {
        var foundedLandz = DataController.GetAllLandz().Find(landz => landz.tokenId == _landId);
        if (foundedLandz == null) return;
        ChangeUserAttacks(foundedLandz.maxAttacks - foundedLandz.attacks);
        foundedLandz.attacks = foundedLandz.maxAttacks;
        foundedLandz.timeStamp = _lastRechargeTimeStamp.ToString();
    }
    
    public void UpdateInventoryItem(string _itemName, int _quantity)
    {
        var targetItem = DataController.Consumables.Find(item => item.name == _itemName);
        if (targetItem == null) return;
        targetItem.quantity += _quantity;
    }
    
    public void SetLoginCredentials(string _userMaticAddress, string _token)
    {
        UserMaticAddress = _userMaticAddress;
        Token = _token;
    }
    
    public void UpdateLandzBuildsInfo(List<LandzCombatInfo> _newInfoLandz)
    {
        foreach (var info in _newInfoLandz)
        {
            var foundLand = DataController.GetAllLandz().Find(land => land.tokenId == info.tokenId);
            foundLand?.SetupAllocatedBuilds(info.allocatedBuildings, DataController.BuildingDatas);
        }
    }
    public void UpdateLandzBuildsInfo(CombatLandz _newInfoLandz)
    {
        var foundLand = DataController.GetAllLandz().Find(land => land.tokenId == _newInfoLandz.TokenId);
        foundLand.allocatedBuilds = _newInfoLandz.AllocatedBuilds;
    }
    
    public void RetryLogin()
    {
        //TODO CHANGE TO THE PROPERLY LOCAL TO USE USER SERVICE
    }
    
    public void OnRegionEntered(string _region)
    {
        CurrentUserLandzBioma = _region;
        SetBiomeAttacks();
    }
}