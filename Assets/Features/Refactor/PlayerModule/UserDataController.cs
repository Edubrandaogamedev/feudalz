using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.Refactor
{
    public class UserDataController : MonoBehaviour
    {
        [SerializeField] 
        private List<HeroezLocalData> _heroezLocalData = new List<HeroezLocalData>();
        [SerializeField] 
        private List<HeroezLocalData> _uniqueHeroezData = new List<HeroezLocalData>();
        [SerializeField] 
        private List<BuildingData> _buildingDatas = new List<BuildingData>();
        [SerializeField] 
        private List<ResourceData> _buildingResourceData = new List<ResourceData>();
        [SerializeField] 
        private List<int> _uniqueLandzID = new List<int>();
        
        private Dictionary<UnitType, List<FeudalzUnit>> _unitsData;
        private List<FeudalzHeroez> _heroez;
        private List<FeudalzCombatLandz> _combatLandzs; 
        private FeudalzBonus _feudalzBonus;
        private HealLandzCost _healLandzCost;
        
        public List<Batch> ProductionItems { get; private set;}
        public List<InventoryItem> Consumables { get; private set; }
        public float TokenBalance { get; private set; }
        public int BonusChargeLeft => _feudalzBonus.chargesLeft;
        public int BonusMaxCharge => _feudalzBonus.chargesMax;
        public string LastBonusReset => _feudalzBonus.timestamp;
        public ConsumablesCost HealLandzItems => _healLandzCost.consumables;
        public float HealLandzGoldz => _healLandzCost.goldz;
        public List<ResourceData> BuildingResourceData => _buildingResourceData;

        public List<BuildingData> BuildingDatas => _buildingDatas;

        private List<InventoryItem> ProcessConsumablesData(Dictionary<string, InventoryItem> items)
        {
            var postProcessData = new List<InventoryItem>();
            foreach (var item in items)
            {
                var newItem = new InventoryItem(item.Key, item.Value.itemId, item.Value.quantity);
                postProcessData.Add(newItem);
            }
            return postProcessData; 
        }
        
        private List<FeudalzHeroez> ProcessHeroezData(List<HerozData> heroezServerData, List<HeroezLocalData> _heroezLocalData)
        {
            List<FeudalzHeroez> postProcessData = new List<FeudalzHeroez>();
            foreach (var herozServerData in heroezServerData)
            {
                HeroezLocalData heroLocalData = _heroezLocalData.Find(hero => hero.HeroName == herozServerData.name);
                if (heroLocalData == null || herozServerData.quantity == 0)
                {
                    continue;
                }
                var newHeroz = new FeudalzHeroez(herozServerData, heroLocalData);
                postProcessData.Add(newHeroz);
            }
            return postProcessData;
        }
        
        private List<FeudalzCombatLandz> ProcessCombatLandzsData(List<LandzCombatInfo> landzServerData)
        {
            List<FeudalzCombatLandz> postProcessData = new List<FeudalzCombatLandz>();
            foreach (LandzCombatInfo landz in landzServerData)
            {
                FeudalzCombatLandz combatLandz = new FeudalzCombatLandz(landz,_healLandzCost.goldz);
                combatLandz.SetupHeroUnit(landz.hero, _heroezLocalData);
                combatLandz.SetupAllowedLandzBuildings(landz.allowedBuildings, _buildingDatas,Consumables);
                combatLandz.SetupAllocatedBuilds(landz.allocatedBuildings, _buildingDatas);
                if (_uniqueLandzID.Contains(landz.tokenId))
                {
                    combatLandz.SetupUniqueHeroUnit(combatLandz.DefineUniqueHeroez(landz.tokenId), _uniqueHeroezData);
                    combatLandz.SetupLandzUnique();
                }
                var foundedLandz = _unitsData[UnitType.Landz].Find(landz => landz.token_id == combatLandz.tokenId);
                combatLandz.nft_sprite = foundedLandz.nft_sprite;
                combatLandz.traits = foundedLandz.traits;
                combatLandz.name = foundedLandz.name;
                combatLandz.sprite_url = foundedLandz.img_url;
                postProcessData.Add(combatLandz);
            }

            return postProcessData;
        }
        
        public void SetUserData(UserInfo userData)
        {
            _unitsData = userData.userFeudalz.Units;
            _feudalzBonus = userData.feudalzBonus;
            _healLandzCost = userData.healLandCost;
            _heroez = ProcessHeroezData(userData.heroez,_heroezLocalData);
            _combatLandzs = ProcessCombatLandzsData(userData.landz);
            
            TokenBalance = userData.goldz;
            ProductionItems = userData.batches;
            Consumables = ProcessConsumablesData(userData.consumables);
        }

        public Dictionary<UnitType, List<FeudalzUnit>> GetAllUnits()
        {
            return _unitsData;
        }
        
        public List<FeudalzUnit> GetAllUnitsByType(UnitType unitType)
        {
            if (!_unitsData.ContainsKey(unitType))
            {
                new Exception("Unit type do not exist on data");
            }

            return _unitsData[unitType];
        }

        public List<FeudalzCombatLandz> GetLandzByBiome(string region)
        {
            var biomeLandzs = new List<FeudalzCombatLandz>();
            foreach (var landz in _combatLandzs)
            {
                Trait biomeTrait = landz.traits.Find(trait => trait.trait_type.Contains("biome"));
                if (biomeTrait == null && _uniqueLandzID.Contains(landz.tokenId)) //process missing metadata information on landz 
                {
                    biomeTrait = new Trait("biome", landz.DefineUniqueBiome(landz.tokenId), 0);
                }
                if (biomeTrait.value.Contains(region))
                {
                    biomeLandzs.Add(landz);
                }
            }
            return biomeLandzs;
        }

        public List<FeudalzCombatLandz> GetAllLandz()
        {
            return _combatLandzs;
        }
        
        public List<FeudalzHeroez> GetAllHeroez()
        {
            return _heroez;
        }

        public List<FeudalzHeroez> GetAllHeroezByRarity(string rarity)
        {
            return _heroez.FindAll(heroez => heroez.heroRarity == rarity);
        }
        
        public bool IsUniqueLandzUnique(int id)
        {
            return _uniqueLandzID.Contains(id);
        }
        
        public void UpdateTokenBalance(float value)
        {
            TokenBalance += value;
        }

        public void UpdateProductionItems(List<Batch> productionItems)
        {
            ProductionItems = productionItems;
        }

        public void UpdateFeudalzBonus(FeudalzBonus bonus)
        {
            _feudalzBonus = bonus;
        }

        public void AddHeroez(string heroToAdd, string rarity)
        {
            var hero = _heroez.Find(hero => (hero.name == heroToAdd && hero.heroRarity == rarity));
            if (hero != null)
            {
                hero.heroQuantity++;
            }
            else
            {
                var newHero = new HerozData(heroToAdd, rarity, 10) { quantity = 1 };
                var heroLocalData = _heroezLocalData.Find(searchingHero => searchingHero.HeroName == heroToAdd);
                var feudalzHeroez = new FeudalzHeroez(newHero,heroLocalData);
                _heroez.Add(feudalzHeroez);
            }
        }
    }
}