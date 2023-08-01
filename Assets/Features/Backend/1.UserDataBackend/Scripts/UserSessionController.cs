using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Linq;
using System;
using EnhancedScrollerDemos.ExpandingCells;
using Features.Refactor;

//[CreateAssetMenu(fileName = "UserSessionDataManager", menuName = "User/UserSessionDataManager")]
public class UserSessionController : ScriptableObject
{
    private readonly UserDataController _userDataController;
    public UserDataController DataController => _userDataController;
    [SerializeField] private UserData userData;
    [SerializeField] private List<BuildingData> buildingDatas = new List<BuildingData>();
    [SerializeField] private List<ResourceData> buildingResourceData = new List<ResourceData>();
    public bool Initialized { get; private set; }
    // User Account Info Scope
    public string UserMaticAddress { get; private set;}
    public string Token {get; private set;}
    public string CurrentUserLandzBioma { get; private set;}
    public int TotalAttacksAvailable { get => userData.totalAttacksAvaiable; private set => userData.totalAttacksAvaiable = value; }
    public int TotalAttacks { get; private set; }
    public int CurrentBiomeAttacksAvailable { get => userData.currentBiomeAttacksAvaiable; private set => userData.currentBiomeAttacksAvaiable = value; }
    public int CurrentBiomeTotalAttacks { get => userData.currentBiomeTotalAttacks; private set => userData.currentBiomeTotalAttacks = value; }
    public HealLandzCost HealCosts { get => userData.healCosts; private set => userData.healCosts = value; }
    // User Account Units Info Scope
    public List<FeudalzCombatLandz> CombatLandzs { get => userData.combatLandzs; private set => userData.combatLandzs = value; }
    public List<ResourceData> BuildingResourceData => buildingResourceData;
    public List<InventoryItem> InventoryItens { get => userData.inventoryResources; private set => userData.inventoryResources = value; }
    //internal use
    private LoadNFTData loadNFTDataController = new LoadNFTData();
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
        loadNFTDataController = null;
    }
    
    public void InitializeSession(UserInfo userData)
    {
        _userDataController.SetUserData(userData);
        SetTotalAttacks();
        LoadAllNFTImages();
        Initialized = true;
        onInitalized?.Invoke();
    }
    
    public async void LoadAllNFTImages()
    {
        await loadNFTDataController.LoadNFTImagePerBatch(DataController.GetAllUnitsByType(UnitType.Elvez), 50);
        await loadNFTDataController.LoadNFTImagePerBatch(DataController.GetAllUnitsByType(UnitType.Orcz), 50);
        await loadNFTDataController.LoadNFTImagePerBatch(DataController.GetAllUnitsByType(UnitType.Animalz), 50);
        await loadNFTDataController.LoadNFTImagePerBatch(DataController.GetAllUnitsByType(UnitType.Feudalz), 50);
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
        var foundedLandz = CombatLandzs.Find(landz => landz.tokenId == landId);
        if (foundedLandz == null) return;
        foundedLandz.attacks = Mathf.Max(0,foundedLandz.attacks-1);
        ChangeUserAttacks(-1);
    }
    public void IncreaseAttackFromLandz(int landId)
    {
        var foundedLandz = CombatLandzs.Find(landz => landz.tokenId == landId);
        if (foundedLandz == null) return;
        foundedLandz.attacks++;
        ChangeUserAttacks(1);
    }
    public void SetLandzHp(int landId, float _currentHealth)
    {
        var foundedLandz = CombatLandzs.Find(landz => landz.tokenId == landId);
        if (foundedLandz != null)
        {
            foundedLandz.health = _currentHealth;
        }
    }
    public void RechargeLandz(int _landId, DateTimeOffset _lastRechargeTimeStamp)
    {
        var foundedLandz = CombatLandzs.Find(landz => landz.tokenId == _landId);
        if (foundedLandz == null) return;
        ChangeUserAttacks(foundedLandz.maxAttacks - foundedLandz.attacks);
        foundedLandz.attacks = foundedLandz.maxAttacks;
        foundedLandz.timeStamp = _lastRechargeTimeStamp.ToString();
    }
    public void ChangeGoldz(float _goldzQuantity)
    {
        DataController.UpdateTokenBalance(_goldzQuantity);
    }
    public void UpdateInventoryItem(string _itemName, int _quantity)
    {
        var targetItem = InventoryItens.Find(item => item.name == _itemName);
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
            var foundLand = CombatLandzs.Find(land => land.tokenId == info.tokenId);
            foundLand?.SetupAllocatedBuilds(info.allocatedBuildings, buildingDatas);
        }
    }
    public void UpdateLandzBuildsInfo(CombatLandz _newInfoLandz)
    {
        var foundLand = CombatLandzs.Find(land => land.tokenId == _newInfoLandz.TokenId);
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
public class LoadNFTData
{
    internal async Task LoadNFTImagePerBatch(List<FeudalzUnit> units, int _maxBatchSize)
    {
        //Debug.Log("<color=orange> HERO TYPE: </color>" + type);
        var batchCounter = 0;
        for (var i = 0; i < units.Count; i++)
        {
            if (batchCounter < _maxBatchSize)
            {
                //Debug.Log("LOADING IMAGE OF UNIT ID: " + units[i].token_id);
                //Debug.Log(units[i].img_url);
                units[i].LoadNFTImage();
                batchCounter++;
            }
            else
            {
                units[i].LoadNFTImage();
                await new WaitForSeconds(10f);
                batchCounter = 0;
                //Debug.Log("<color=red> NEXT BATCH </color>");
            }
        }
    }
}