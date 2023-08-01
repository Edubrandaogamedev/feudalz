using System.Collections.Generic;
using UnityEngine;
//use web3.jslib
//[CreateAssetMenu(fileName = "New User Data", menuName = "User/Data")]
public class UserData : ScriptableObject
{
    public int totalAttacksAvaiable {get;set;}
    public int totalAttacks {get;set;}
    public int currentBiomeAttacksAvaiable {get;set;}
    public int currentBiomeTotalAttacks {get;set;}
    public HealLandzCost healCosts {get;set;}

    public List<FeudalzCombatLandz> combatLandzs = new List<FeudalzCombatLandz>();
    public List<FeudalzHeroez> heroezs = new List<FeudalzHeroez>();
    public List<InventoryItem> inventoryResources = new List<InventoryItem>();
    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    private void OnDisable()
    {
        ClearSO();
    }
    private void ClearSO()
    {
        
        totalAttacksAvaiable = 0;
        totalAttacks = 0;
        currentBiomeAttacksAvaiable = 0;
        currentBiomeTotalAttacks = 0;
        inventoryResources = null;
        healCosts = null;
        ClearAllLists();
    }
    private void ClearAllLists()
    {
        // elvezs.Clear();
        // orczs.Clear();
        // animalzs.Clear();
        // feudalzs.Clear();
        // landzs.Clear();
        // combatLandzs.Clear();
        // heroezs.Clear();
        // productionItens.Clear();
        
    }
}