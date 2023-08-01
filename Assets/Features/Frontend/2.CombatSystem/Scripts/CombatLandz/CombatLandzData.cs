using System.Collections.Generic;
using UnityEngine;
public class CombatLandzData
{
    public string PlayerToken {get;set;}
    public string TimeStamp {get;set;}
    public string LandzName {get;set;}
    public int Attacks {get;set;}
    public int MaxAttacks {get;set;}
    public int TokenId {get;set;}
    public float AttackBonus { get; set; }
    public float DefenseBonus {get;set;}
    public float LandzHp {get;set;}
    public float MaxLandzHp {get;set;}
    public FeudalzHeroez Heroez {get;set;}
    public FeudalzHeroez UniqueHeroez {get;set;}
    public bool LandzUnique {get;set;}
    public List<FeudalzLandzBuild> AllowedBuilds = new List<FeudalzLandzBuild>();
    public List<FeudalzLandzAllocatedBuild> Builds = new List<FeudalzLandzAllocatedBuild>();
    public float RepairCost {get;set;}
    public bool goldz;
    public void Setup(FeudalzCombatLandz _combatUnitData,string _playerToken)    
    {
        Attacks = _combatUnitData.attacks;
        MaxAttacks = _combatUnitData.maxAttacks;
        TimeStamp = _combatUnitData.timeStamp;
        TokenId = _combatUnitData.tokenId;
        LandzName = _combatUnitData.name;
        AttackBonus = _combatUnitData.attackBonus;
        DefenseBonus = _combatUnitData.defenseBonus;
        PlayerToken = _playerToken;
        LandzHp = _combatUnitData.health;
        MaxLandzHp = _combatUnitData.maxHealth;
        if (_combatUnitData.maxHealth == 0)
            MaxLandzHp = 1000;
        Heroez = _combatUnitData.heroez;
        UniqueHeroez = _combatUnitData.uniqueHeroez;
        AllowedBuilds = _combatUnitData.allowedBuilds;
        Builds = _combatUnitData.allocatedBuilds;  
        LandzUnique = _combatUnitData.uniqueLandz;  
        RepairCost = _combatUnitData.repairCost;  
    }
}
