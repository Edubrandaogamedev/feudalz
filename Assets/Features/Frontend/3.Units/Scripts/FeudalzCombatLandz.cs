using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudalzCombatLandz
{
    public int tokenId { get; set; }
    public string timeStamp { get; set; }
    public string name { get; set; }
    public int attacks { get; set; }
    public int maxAttacks { get; set; }
    public float maxHealth { get; set; }
    public float health { get; set; }
    public float attackBonus { get; set; }
    public float defenseBonus { get; set; }
    public string sprite_url { get; set; }
    public Sprite nft_sprite { get; set; }
    public List<Trait> traits { get; set; }
    public FeudalzHeroez heroez { get; set; }
    public FeudalzHeroez uniqueHeroez { get; set; }
    public bool uniqueLandz { get; set; }
    public float repairCost { get; set; }
    public List<FeudalzLandzBuild> allowedBuilds = new List<FeudalzLandzBuild>();
    public List<FeudalzLandzAllocatedBuild> allocatedBuilds = new List<FeudalzLandzAllocatedBuild>();
    public FeudalzCombatLandz(LandzCombatInfo data, float _repairCost)
    {
        tokenId = data.tokenId;
        attackBonus = data.attackBonus;
        defenseBonus = data.defenseBonus;
        attacks = data.attacks;
        maxAttacks = data.maxAttacks;
        health = data.health;
        maxHealth = data.maxHealth;
        timeStamp = data.timestamp;
        repairCost = _repairCost;
    }
    public void SetupLandzUnique()
    {
        uniqueLandz = true;
    }

    public void SetupHeroUnit(HerozData _hero, List<HeroezLocalData> _data)
    {
        if (_hero?.name == null) return;
        var heroData = _data.Find(hero => hero.HeroName == _hero.name);
        heroez = new FeudalzHeroez(_hero,heroData);
    }
    public void SetupUniqueHeroUnit(string _hero, List<HeroezLocalData> _data)
    {
        if (_hero == null) return;
        uniqueHeroez = new FeudalzHeroez(_hero, _data.Find(hero => hero.HeroName == _hero));
    }
    public void SetupAllocatedBuilds(AllocatedBuildings _allocatedBuildings, List<BuildingData> _data)
    {
        
        if (_allocatedBuildings == null) return;
        allocatedBuilds = new List<FeudalzLandzAllocatedBuild>();
        if (_allocatedBuildings.top != null)
        {
            allocatedBuilds.Add(new FeudalzLandzAllocatedBuild(_allocatedBuildings.top,
            _data.Find(build => _allocatedBuildings.top.type == build.BuildingName),
            "top",
            allowedBuilds.Find(build => _allocatedBuildings.top.type == build.type)));
        }
        if (_allocatedBuildings.left != null)
        {
            allocatedBuilds.Add(new FeudalzLandzAllocatedBuild(_allocatedBuildings.left,
            _data.Find(build => _allocatedBuildings.left.type == build.BuildingName),
            "left",
            allowedBuilds.Find(build => _allocatedBuildings.left.type == build.type)));
            
        }
        if (_allocatedBuildings.right != null)
        {
            allocatedBuilds.Add(new FeudalzLandzAllocatedBuild(_allocatedBuildings.right,
            _data.Find(build => _allocatedBuildings.right.type == build.BuildingName),
            "right",
            allowedBuilds.Find(build => _allocatedBuildings.right.type == build.type)));
        }
    }
    public void SetupAllowedLandzBuildings(Build[] _buildings, List<BuildingData> _data, List<InventoryItem> inventoryItems)
    {
        allowedBuilds = new List<FeudalzLandzBuild>();
        foreach (var build in _buildings)
        {
            var buildData = _data.Find(buildData => build.type == buildData.BuildingName);
            if (buildData == null) continue;
            var newBuild = new FeudalzLandzBuild(build, buildData, inventoryItems);
            allowedBuilds.Add(newBuild);
        }
    }

    public string DefineUniqueHeroez(int _tokenId)
    {
        string heroName = "";
        switch (_tokenId)
        {
            case 4071:
                heroName = "Death Prophet";
                break;
            case 4072:
                heroName = "Sky-Ogre Steward";
                break;
            case 4073:
                heroName = "Cursed Captain";
                break;
            case 4074:
                heroName = "Old Berserker";
                break;
            case 4075:
                heroName = "Ghoul Knight";
                break;
            case 4076:
                heroName = "Thief of Fates";
                break;
            case 4077:
                heroName = "Ghost Mason";
                break;
            case 4078:
                heroName = "Dragon Knight";
                break;
            case 4079:
                heroName = "Living Spellbook";
                break;
            case 4080:
                heroName = "Dreamwalker Fairy";
                break;
        }
        return heroName;
    }

    public string DefineUniqueBiome(int _tokenId)
    {
        string biomeName = "";
        switch (_tokenId)
        {
            case 4071:
                biomeName = "Vanthera";
                break;
            case 4072:
                biomeName = "Vundt";
                break;
            case 4073:
                biomeName = "Jamana";
                break;
            case 4074:
                biomeName = "Vikraam";
                break;
            case 4075:
                biomeName = "Swundvand";
                break;
            case 4076:
                biomeName = "Vallados";
                break;
            case 4077:
                biomeName = "Maradurk";
                break;
            case 4078:
                biomeName = "Gwinland";
                break;
            case 4079:
                biomeName = "Sahadak";
                break;
            case 4080:
                biomeName = "Xue";
                break;
        }
        return biomeName;
    }
}
