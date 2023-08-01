using System.Collections.Generic;
using System.Threading.Tasks;
using Features.Refactor;
using Newtonsoft.Json;

public class APIServices
{
    public class DatabaseServer
    {
        private static List<string> errors = new List<string>();
        public static async Task<Nonce> GetNonce(string _userAddress)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.NONCE_CONTEXT;
            var bodyData = new PublicAddress(_userAddress);
            var requestResponse = await ServerRequest.HTTPRequestPost(serviceUrl_, bodyData);
            return (JsonConvert.DeserializeObject<Nonce>(requestResponse));
        }

        public static async Task<AuthenticationToken> LoginAuthentication(string _userAddress, string _signature)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.LOGIN_CONTEXT;
            var bodyData = new Login(_userAddress, _signature);
            var requestResponse = await ServerRequest.HTTPRequestPost(serviceUrl_, bodyData);
            return (JsonConvert.DeserializeObject<AuthenticationToken>(requestResponse));
        }
        public static async Task<UserInfo> GetUserInfo(string _token)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.INFO_CONTEXT;
            var header = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_, header);
            return (JsonConvert.DeserializeObject<UserInfo>(requestResponse));
        }

        public static async Task<UserInfo> GetUserFastInfo(string _token)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.INFO_FAST_CONTEXT;
            var header = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_, header);
            return (JsonConvert.DeserializeObject<UserInfo>(requestResponse));
        }
        public static async Task<UserInfo> AdminGetUserInfo(string _userAddress)
        {
            var serviceUrl_ = $"https://feudalz-game-server-djaxojxmla-uc.a.run.app/admin/info/{_userAddress}";
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_);
            return (JsonConvert.DeserializeObject<UserInfo>(requestResponse));
        }
        public static async Task<AttackResult> LandzAttack(string _token, int _myLandzId)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.ATTACK_CONTEXT;
            var header = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new LandzBody(_myLandzId);
            var requestResponse = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header);
            return (JsonConvert.DeserializeObject<AttackResult>(requestResponse));
        }

        public static async Task<AttackAll> AttackWithAllLandz(string _token, string _currentPlayerBiome)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.ATTACK_ALL_CONTEXT;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new CurrentBiomeBody(_currentPlayerBiome);
            var requestResponse = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
            return (JsonConvert.DeserializeObject<AttackAll>(requestResponse));
        }

        public static async Task<BattleLog> BattleLog(string _token, string _biome)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.BATTLE_LOG_CONTEXT + "?biome=" + _biome;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_, header_);
            return (JsonConvert.DeserializeObject<BattleLog>(requestResponse));
        }

        public static async Task<RankingObj[]> RankingLog(string _token, string _biome)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.RANKING_LOG_CONTEXT + "?biome=" + _biome;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_, header_);
            return (JsonConvert.DeserializeObject<RankingObj[]>(requestResponse));
        }
        public static async Task<BattleLog> AdminBattleLog(string _userAddress)
        {
            var serviceUrl_ = $"https://feudalz-game-server-djaxojxmla-uc.a.run.app/admin/history/{_userAddress}";
            var requestResponse = await ServerRequest.HTTPRequestGet(serviceUrl_);
            return (JsonConvert.DeserializeObject<BattleLog>(requestResponse));
        }
        public static async Task FixLandz(string _token, int _myLandzId, bool goldz)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.FIXLANDZ_CONTEXT;
            var header = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new FixBody(_myLandzId, goldz);
            await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header);
        }
        public static async Task<AllFixedLandz> FixAllLandz(string _token, bool _goldz)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.FIXLANDZ_ALL_CONTEXT;
            var header = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new FixAllBody(_goldz);
            var requestResponse = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header);
            return (JsonConvert.DeserializeObject<AllFixedLandz>(requestResponse));
        }
        public static async Task<RechargeAll> RechargeAllLandz(string _token, string _currentPlayerBiome)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.RECHARGE_ALL_CONTEXT;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new CurrentBiomeBody(_currentPlayerBiome);
            var requestResponse = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
            return (JsonConvert.DeserializeObject<RechargeAll>(requestResponse));
        }

        public static async Task<CheckRechargeAll> CheckRechargeAllCost(string _token, string _currentPlayerBiome)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.CHECK_RECHARGE_ALL_COST_CONTEXT;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new CurrentBiomeBody(_currentPlayerBiome);
            var requestResponse = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
            return (JsonConvert.DeserializeObject<CheckRechargeAll>(requestResponse));
        }
        public static async Task InstallHero(string _token, int tokenId, string _heroName, string _rarity)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.INSTALL_HERO;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new InstallHeroBody(tokenId, _heroName, _rarity);
            await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
        }
        public static async Task RemoveAllocatedHero(string _token, int tokenId)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.REMOVE_HERO;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new LandzBody(tokenId);
            await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
        }
        public static async Task AllocateBuild(string _token, int _tokenId, string _position, string _type, bool _autoRebuild)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.ALLOCATE_BUILD;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData_ = new AllocateBuild(_tokenId, _position, _type, _autoRebuild);
            await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData_, header_);
        }
        public static async Task<RemoveBuildingResponse> RemoveAllocatedBuild(string _token, int _tokenId, string _position)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.REMOVE_ALLOCATED_BUILD;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new RemoveAllocatedBuildBody(_tokenId, _position);
            var response = await ServerRequest.HTTPRequestDelete(serviceUrl_, bodyData, header_);
            return (JsonConvert.DeserializeObject<RemoveBuildingResponse>(response));
        }
        public static async Task<ProductionStarted> StartProduction(string _token, string _building, int _buildingQuantity, int _craftIndex, bool _autoCraft)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.START_PRODUCTION;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new StartProdBody(_building, _buildingQuantity, _craftIndex, _autoCraft);
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
            return (JsonConvert.DeserializeObject<ProductionStarted>(response));
        }

        public static async Task<HarvestResult> CancelProduction(string _token, string _batchId)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.CANCEL_PRODUCTION;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new HarvestBody(_batchId);
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
            return (JsonConvert.DeserializeObject<HarvestResult>(response));
        }
        public static async Task<HarvestResult> Harvest(string _token, string _batchId)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.HARVEST;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new HarvestBody(_batchId);
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
            return (JsonConvert.DeserializeObject<HarvestResult>(response));
        }
        public static async Task<HarvestResult> HarvestAll(string _token)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.HARVEST_ALL;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new HarvestBody("");
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
            return (JsonConvert.DeserializeObject<HarvestResult>(response));
        }
        public static async Task SwitchAutoRebuild(string _token, int _tokenId, string _position, bool _newStatus)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.AUTO_REBUILD_CONTEXT;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new SwitchAutoRebuildBody(_tokenId, _position, _newStatus);
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
        }
        public static async Task SwitchAutoCraft(string _token, string _batchId, bool _newStatus)
        {
            var serviceUrl_ = APIURLs.DataBaseUrls.DATABASE_URL + APIURLs.DataBaseUrls.AUTO_CRAFT_CONTEXT;
            var header_ = new List<ServerRequest.RequestHeader>() { new ServerRequest.RequestHeader("x-access-token", $"{_token}") };
            var bodyData = new SwitchAutoCraftBody(_batchId, _newStatus);
            var response = await ServerRequest.HTTPRequestPut(true, serviceUrl_, bodyData, header_);
        }
    }
}

public class UserInfo
{
    public float goldz { get; set; }
    public List<LandzCombatInfo> landz { get; set; }
    public UserFeudalz userFeudalz { get; set; }
    public List<HerozData> heroez { get; set; }
    public Dictionary<string, InventoryItem> consumables;
    public List<Batch> batches { get; set; }
    public HealLandzCost healLandCost { get; set; }
    public FeudalzBonus feudalzBonus;
}

public class FeudalzBonus
{
    public int chargesLeft;
    public int chargesMax;
    public string timestamp;
}
public class HealLandzCost
{
    public float goldz { get; set; }
    public ConsumablesCost consumables { get; set; }
}

public class ConsumablesCost
{
    [JsonProperty(PropertyName = "Food Crate")]
    public int foodCrate { get; set; }
    [JsonProperty(PropertyName = "Armors")]
    public int armors { get; set; }
    [JsonProperty(PropertyName = "Weapons")]
    public int weapons { get; set; }
}

public class InventoryItem
{
    public string name;
    public int itemId;
    public int quantity;
    public InventoryItem(string name, int itemId, int quantity)
    {
        this.name = name;
        this.itemId = itemId;
        this.quantity = quantity;
    }
}
public class UserFeudalz
{
    private List<FeudalzUnit> Feudalz { get; set; }
    private List<FeudalzUnit> Orcz { get; set; }
    private List<FeudalzUnit> Elvez { get; set; }
    private List<FeudalzUnit> Animalz { get; set; }
    private List<FeudalzUnit> Landz { get; set; }

    public readonly Dictionary<UnitType, List<FeudalzUnit>> Units = new Dictionary<UnitType, List<FeudalzUnit>>();
    
    [JsonConstructor]
    public UserFeudalz(List<FeudalzUnit> feudalz, List<FeudalzUnit> orcz, List<FeudalzUnit> elvez, List<FeudalzUnit> animalz, List<FeudalzUnit> landz)
    {
        Feudalz = feudalz;
        Orcz = orcz;
        Elvez = elvez;
        Animalz = animalz;
        Landz = landz;
        Units = new Dictionary<UnitType, List<FeudalzUnit>>()
        {
            {UnitType.Feudalz, feudalz},
            {UnitType.Orcz, orcz},
            {UnitType.Elvez, elvez},
            {UnitType.Animalz, animalz},
            {UnitType.Landz, landz}
        };
    }
}
public class FeudalzBonusStats
{
    public float attack { get; set; }
    public float defense { get; set; }
}
public class Trait
{
    public string trait_type { get; set; }
    public string value { get; set; }
    public int trait_count { get; set; }

    public Trait(string traitType, string value, int traitCount)
    {
        trait_type = traitType;
        trait_count = traitCount;
        this.value = value;
    }
}
public class LandzCombatInfo
{
    public LandzCombatInfo(int tokenId, int attacks, int maxAttacks, string timestamp, float health, float maxHealth, string biome, HerozData hero, float defenseBonus, Build[] allowedBuildings, AllocatedBuildings allocatedBuilding)
    {
        this.tokenId = tokenId;
        this.attacks = attacks;
        this.maxAttacks = maxAttacks;
        this.timestamp = timestamp;
        this.health = health;
        this.maxHealth = maxHealth;
        this.biome = biome;
        this.hero = hero;
        this.defenseBonus = defenseBonus;
        this.allowedBuildings = allowedBuildings;
        this.allocatedBuildings = allocatedBuilding;
    }
    public int tokenId { get; set; }
    public int attacks { get; set; }
    public int maxAttacks { get; set; }
    public string timestamp { get; set; }
    public float health { get; set; }
    public float maxHealth { get; set; }
    public string biome { get; set; }
    public HerozData hero { get; set; }
    public float attackBonus { get; set; }
    public float defenseBonus { get; set; }
    public Build[] allowedBuildings { get; set; }
    public AllocatedBuildings allocatedBuildings { get; set; }
}
public class Build
{
    public string type { get; set; }
    public string costType { get; set; }
    public float cost { get; set; }
    public string productType { get; set; }
    public float productQuantity { get; set; }
    public AllowedCrafting[] allowedCraftings { get; set; }
    public float maxDurability { get; set; }
    public int maxCharges { get; set; }
    public string category { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
}
public class AllocateBuild
{
    public int tokenId;
    public string position;
    public string type;
    public bool autoRebuild;

    public AllocateBuild(int tokenId, string position, string type, bool autoRebuild)
    {
        this.tokenId = tokenId;
        this.position = position;
        this.type = type;
        this.autoRebuild = autoRebuild;
    }
}
public class AllocatedBuild
{
    public string type { get; set; }
    public float durability { get; set; }
    public float maxDurability { get; set; }
    public int charges { get; set; }
    public int maxCharges { get; set; }
    public AllowedCrafting currentProduction { get; set; }
    public string lastStartedCycleTimestamp { get; set; }
    public bool autoRebuild { get; set; }
    public string category { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
}
public class AllocatedBuildings
{
    public AllocatedBuild top { get; set; }
    public AllocatedBuild left { get; set; }
    public AllocatedBuild right { get; set; }
}
public class RemoveAllocatedBuildBody
{
    public RemoveAllocatedBuildBody(int tokenId, string position)
    {
        this.tokenId = tokenId;
        this.position = position;
    }
    public int tokenId { get; set; }
    public string position { get; set; }
}
public class AllowedCrafting
{
    public float[] rawCost { get; set; }
    public string[] rawCostType { get; set; }
    public string productType { get; set; }
    public float productQuantity { get; set; }
}
public class PublicAddress
{
    public PublicAddress(string publicAddress)
    {
        this.publicAddress = publicAddress;
    }

    public string publicAddress { get; set; }
}
public class Nonce
{
    public int nonce { get; set; }
}
public class Login
{
    public Login(string publicAddress, string signature)
    {
        this.publicAddress = publicAddress;
        this.signature = signature;
    }
    public string publicAddress { get; set; }
    public string signature { get; set; }
}
public class AuthenticationToken
{
    public string token { get; set; }
}
public class LandzBody
{
    public LandzBody(int tokenId)
    {
        this.tokenId = tokenId;
    }

    public int tokenId { get; set; }
}
public class FixBody
{
    public FixBody(int tokenId, bool goldz)
    {
        this.tokenId = tokenId;
        this.goldz = goldz;
    }

    public int tokenId { get; set; }
    public bool goldz { get; set; }
}

public class FixAllBody
{
    public FixAllBody(bool goldz)
    {
        this.goldz = goldz;
    }
    public bool goldz { get; set; }
}

public class AllFixedLandz
{
    public int[] healedLandzTokens { get; set; }
}
public class RechargeAll
{
    public string timestamp { get; set; }
    public int[] rechargedLandzID { get; set; }
    public float rechargeCost { get; set; }
}
public class CheckRechargeAll
{
    public float rechargeCost { get; set; }
}
public class CurrentBiomeBody
{
    public string currentPlayerBiome { get; set; }
    public CurrentBiomeBody(string currentPlayerBiome)
    {
        this.currentPlayerBiome = currentPlayerBiome;
    }
}
#region Battle Log

public class BattleLog
{
    public CombatInfo[] filteredAttackHistory { get; set; }
    public CombatInfo[] filteredDefenseHistory { get; set; }
}
public class CombatInfo
{
    public string winner { get; set; }
    public int attackerLandTokenId { get; set; }
    public int defenderLandTokenId { get; set; }
    public string playerLandName { get; set; }
    public string enemyLandName { get; set; }
    public int attackerDiceResult { get; set; }
    public int defenderDiceResult { get; set; }
    public float attackerBonus { get; set; }
    public float defenderBonus { get; set; }
    public float coins { get; set; }
}
#endregion
#region Ranking
public class RankingObj
{
    public string name { get; set; }
    //public string publicAddress { get; set; }
    //public int tokenId { get; set; }
    public float cumulatedVictoryGold { get; set; }
    public int attackSucceded { get; set; }
    public int attackFailed { get; set; }
}
#endregion
#region Landz Combat
public class AttackAll
{
    public AttackResult[] results { get; set; }
    public float goldz { get; set; }
}
public class AttackResult
{
    public string winner { get; set; }
    public AttackerPlayer attacker { get; set; }
    public DefenderPlayer defender { get; set; }
}
public class AttackerPlayer
{
    public string address { get; set; }
    public float defense { get; set; }
    public AttackerLandz land { get; set; }
    public HerozData hero { get; set; }
    public float dice { get; set; }
    public float total { get; set; }
    public float goldz { get; set; }
    public float percentage { get; set; }
    public float selfDamange { get; set; }
}
public class DefenderPlayer
{
    public string address { get; set; }
    public DefenderLandz land { get; set; }
    public float dice { get; set; }
    public float total { get; set; }
}
public class AttackerLandz
{
    public int id { get; set; }
    public string name { get; set; }
    public float health { get; set; }
    public string unique { get; set; }
    public float defense { get; set; }
    public float goldzBonus { get; set; }
    public float diceBonus { get; set; }
    public float percentage { get; set; }
    public float stolenDice { get; set; }
    public float attackTotal { get; set; }
    public float defenseTotal { get; set; }
}
public class DefenderLandz
{
    public string name { get; set; }
    public int tokenId { get; set; }
    public string image { get; set; }
}
#endregion
public class HerozData
{
    public HerozData(string name, string rarity, int? charges)
    {
        this.name = name;
        this.rarity = rarity;
        this.charges = charges.GetValueOrDefault();
    }
    public int quantity { get; set; }
    public string name { get; set; }
    public string rarity { get; set; }
    public int? charges { get; set; }
    public bool bonus { get; set; }
    public float probability { get; set; }
    public float[] dices { get; set; }
}
public class InstallHeroBody
{
    public int tokenId { get; set; }
    public string name { get; set; }
    public string rarity { get; set; }

    public InstallHeroBody(int tokenId, string name, string rarity)
    {
        this.tokenId = tokenId;
        this.name = name;
        this.rarity = rarity;
    }
}

public class RemoveBuildingResponse
{
    public Batch[] batches;
}
public class StartProdBody
{
    public string building;
    public int buildingsQuantity;
    public int craftIndex;
    public bool autoCraft;
    public StartProdBody(string building, int buildingsQuantity, int craftIndex, bool autoCraft)
    {
        this.building = building;
        this.buildingsQuantity = buildingsQuantity;
        this.craftIndex = craftIndex;
        this.autoCraft = autoCraft;
    }
}
public class ProductionStarted
{
    public List<LandzCombatInfo> affectedLandz;
    public Batch[] batches;
}
public class Batch
{
    public string batchId;
    public string productType;
    public int productQuantity;
    public AllocateBuild[] buildings;
    public string timestamp;
    public bool autoCraft;
}
public class HarvestResult
{
    public Dictionary<string, int> harvestResults;
    public Batch[] batches;
    public List<LandzCombatInfo> landz;
    public FeudalzBonus feudalzBonus;
}

public class HarvestBody
{
    public string batchId { get; set; }

    public HarvestBody(string _batchId)
    {
        batchId = _batchId;
    }
}
public class SwitchAutoRebuildBody
{
    public int tokenId;
    public string position;
    public bool newStatus;

    public SwitchAutoRebuildBody(int tokenId, string position, bool newStatus)
    {
        this.tokenId = tokenId;
        this.position = position;
        this.newStatus = newStatus;
    }
}

public class SwitchAutoCraftBody
{
    public string batchId;
    public bool autoCraft;
    public SwitchAutoCraftBody(string batchId, bool autoCraft)
    {
        this.batchId = batchId;
        this.autoCraft = autoCraft;
    }
}