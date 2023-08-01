using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ZestriaLandzManager : MonoBehaviour
{
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [SerializeField] private WorldMapPopupController popupController;
    [SerializeField] private BattleController battleController;
    [SerializeField] private HeroezResponseFilter heroezResponseFilter;
    private readonly LoadTexture loadTexture = new LoadTexture();
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController managerPanel;
    [SerializeField] private CanvasGroupController loadPanel;
    [SerializeField] private CanvasGroupController allBattlePanel;
    [Header("GameObject")]
    [SerializeField] private GameObject fadeBackground;
    [SerializeField] private GameObject loadingAttackAll;
    [Header("Template")]
    [SerializeField] private AllLandzBattleUI allLandzBattlePrefab;
    [SerializeField] private RectTransform allLandzBattleContent;
    [Header("Buttons")]
    [SerializeField] private Button btnopenManager;
    [SerializeField] private Button btncloseManager;
    [SerializeField] private Button btnAttackAll;
    [SerializeField] private Button btnRechargeAll;
    [SerializeField] private Button btnHealAll;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI attackAvaiablesText;
    [SerializeField] private TextMeshProUGUI rechargeAvaiablesText;
    [SerializeField] private TextMeshProUGUI healAvaiablesText;
    [SerializeField] private TextMeshProUGUI goldzQuantity;
    [SerializeField] private TextMeshProUGUI weaponsQuantity;
    [SerializeField] private TextMeshProUGUI armorsQuantity;
    [SerializeField] private TextMeshProUGUI foodQuantity;
    [Header("Image")]
    [SerializeField] private Image attackAvaiableBar;
    [SerializeField] private Image rechargeAvaiableBar;
    [SerializeField] private Image healAvaiableBar;
    public event UnityAction OnPlayerInfoUpdated;
    private void OnEnable()
    {
        userSessionController.onInitalized += SetControllerInfos;
        battleController.onAttackAllResultCalculated += OnAttackWithAllLandzEnded;
        battleController.onErrorWithMessage += popupController.ErrorHandler;
        btnopenManager.onClick.AddListener(OpenManagerPanel);
        btncloseManager.onClick.AddListener(CloseManagerPanel);
        btnAttackAll.onClick.AddListener(TryAttackAll);
        btnRechargeAll.onClick.AddListener(TryRechargeAll);
        btnHealAll.onClick.AddListener(TryHealAll);
    }

    private void OnDisable()
    {
        userSessionController.onInitalized -= SetControllerInfos;
        battleController.onAttackAllResultCalculated -= OnAttackWithAllLandzEnded;
        battleController.onErrorWithMessage -= popupController.ErrorHandler;
        btnopenManager.onClick.RemoveListener(OpenManagerPanel);
        btncloseManager.onClick.RemoveListener(CloseManagerPanel);
        btnAttackAll.onClick.RemoveListener(TryAttackAll);
        btnRechargeAll.onClick.RemoveListener(TryRechargeAll);
        btnHealAll.onClick.RemoveListener(TryHealAll);
    }
    private void Start()
    {
        if (userSessionController.Initialized)
            SetControllerInfos();
    }
    private void OpenManagerPanel()
    {
        managerPanel.Enable();
        fadeBackground.SetActive(true);
    }
    private void CloseManagerPanel()
    {
        managerPanel.Disable();
        fadeBackground.SetActive(false);
    }
    private void TryAttackAll()
    {
        var goldzCost = 0.25f;
        var totalCost = userSessionController.CombatLandzs.Where(landz => landz.attacks > 0 && landz.health > 0).Sum(landz => landz.attacks * goldzCost);
        if (SkipPopup.GetSettings())
            OnAttackWithAllLandzConfirmation(totalCost);
        else
            popupController.EnableAttackWithAllLandzConfirmationPopup(totalCost, OnAttackWithAllLandzConfirmation);
    }
    private async void OnAttackWithAllLandzConfirmation(float totalCost)
    {
        popupController.DisableAttackWithAllLandzConfirmationPopup();
        OpenAllBattleUI();
        await battleController.AttackWithAllLands(userSessionController.Token, "", totalCost);
    }
    private void OnAttackWithAllLandzEnded(AttackAll _attackAllResult, float _goldzCost)
    {
        OpenAttackAllResultScreen(_attackAllResult, userSessionController);
        foreach (var result in _attackAllResult.results)
        {
            userSessionController.ReduceAttackFromLandz(result.attacker.land.id);
            userSessionController.SetLandzHp(result.attacker.land.id, result.attacker.land.health);
            if (result.attacker.hero != null && result.attacker.hero.bonus)
                heroezResponseFilter.OnHeroSkillActivated(userSessionController, result.attacker.hero.name, result.attacker.land.id);
        }
        userSessionController.ChangeGoldz(-_goldzCost);
        userSessionController.ChangeGoldz(_attackAllResult.goldz);
        userSessionController.SetTotalAttacks();
        SetControllerInfos();
    }

    public void OpenAllBattleUI()
    {
        allBattlePanel.Enable();
        loadingAttackAll.SetActive(true);
    }
    public async void OpenAttackAllResultScreen(AttackAll _attackAllResult, UserSessionController userSessionController)
    {
        var winCount = 0;
        foreach (var result in _attackAllResult.results)
        {
            if (result.winner == result.attacker.address)
                winCount++;
        }
        List<AllLandzBattleUI> panelLandz = new List<AllLandzBattleUI>();
        await ConstructAllResultPanel(_attackAllResult.results, userSessionController, panelLandz);
        loadingAttackAll.SetActive(false);
        popupController.EnableAttackWithAllLandzResultPopup(() => CloseAttackAllResultScreen(panelLandz, winCount, _attackAllResult.goldz));
    }
    private void CloseAttackAllResultScreen(List<AllLandzBattleUI> _panelList, int _winCount, float _wonGoldz)
    {
        allBattlePanel.Disable();
        loadingAttackAll.SetActive(false);
        popupController.DisableAttackWithAllLandzResultPopup();
        managerPanel.Disable();
        popupController.EnableResultPopup(OpenManagerPanel, _wonGoldz, _winCount);
        DestroyResultPanel(_panelList);
    }
    private async Task ConstructAllResultPanel(AttackResult[] _results, UserSessionController userSessionController, List<AllLandzBattleUI> panelLandz)
    {
        panelLandz.Clear();
        await LoadAllPlayerLandzTexture(this.userSessionController);
        foreach (var item in _results)
        {
            var obj = Instantiate(allLandzBattlePrefab, allLandzBattleContent.transform);
            obj.Setup(item, userSessionController);
            panelLandz.Add(obj);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(allLandzBattleContent);
    }
    private async Task LoadAllPlayerLandzTexture(UserSessionController userSessionController)
    {
        foreach (var landz in userSessionController.CombatLandzs)
        {
            try
            {
                landz.nft_sprite = await loadTexture.GetTexture(landz.sprite_url);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
    private void DestroyResultPanel(List<AllLandzBattleUI> _panelLandz)
    {
        foreach (var landz in _panelLandz)
        {
            landz.DestroyObject();
        }
    }
    private async void TryRechargeAll()
    {
        loadPanel.Enable();
        if (SkipPopup.GetSettings())
            OnRechargeAllConfirmation();
        else
        {
            try
            {
                CheckRechargeAll rechargAllCost = null;
                rechargAllCost = await APIServices.DatabaseServer.CheckRechargeAllCost(userSessionController.Token, "");
                popupController.EnableRechargeAllPoup(OnRechargeAllConfirmation, rechargAllCost.rechargeCost);
            }
            catch (WebRequestException e)
            {
                if (e.Message.Contains("can't recharge"))
                    popupController.ErrorHandler("Goldz");
                else if (e.Message.Contains("available recharges"))
                    popupController.ErrorHandler("Avaiable Recharge");
                else
                    popupController.ErrorHandler("Request");
            }
            finally
            {
                loadPanel.Disable();
            }
        }
    }
    private async void OnRechargeAllConfirmation()
    {
        loadPanel.Enable();
        try
        {
            RechargeAll rechargeAllResult = null;
            rechargeAllResult = await APIServices.DatabaseServer.RechargeAllLandz(userSessionController.Token, "");
            userSessionController.ChangeGoldz(-rechargeAllResult.rechargeCost);
            DateTimeOffset.TryParse(rechargeAllResult.timestamp, out var rechargeTimeStamp);
            foreach (var landzID in rechargeAllResult.rechargedLandzID)
                userSessionController.RechargeLandz(landzID, rechargeTimeStamp);
            SetControllerInfos();
            popupController.OpenSuccessPopupPanel("You landz are recharged");
        }
        catch (WebRequestException e)
        {
            popupController.ErrorHandler(e.Message.Contains("can't recharge") ? "Generic Recharge" : "Request");
        }
        finally
        {
            loadPanel.Disable();
            popupController.DisableRechargeAllPopup();
        }
    }
    private void TryHealAll()
    {
        var noHealthLandzCounter = userSessionController.CombatLandzs.Count(landz => landz.health <= 0);
        var goldzCost = userSessionController.HealCosts.goldz * noHealthLandzCounter;
        var resourceCost = new int[] {userSessionController.HealCosts.consumables.weapons*noHealthLandzCounter,
        userSessionController.HealCosts.consumables.armors*noHealthLandzCounter,
        userSessionController.HealCosts.consumables.foodCrate*noHealthLandzCounter};
        popupController.EnableFixConfirmationPoup((goldz) => OnHealAllConfirmation(goldzCost, resourceCost, goldz), goldzCost, resourceCost);
    }
    private async void OnHealAllConfirmation(float _goldzCost, int[] resourceCost, bool _goldz)
    {
        loadPanel.Enable();
        try
        {
            var response = await APIServices.DatabaseServer.FixAllLandz(userSessionController.Token, _goldz);
            if (_goldz)
                userSessionController.ChangeGoldz(-_goldzCost);
            else
            {
                userSessionController.UpdateInventoryItem("Weapons", -resourceCost[0]);
                userSessionController.UpdateInventoryItem("Armors", -resourceCost[1]);
                userSessionController.UpdateInventoryItem("Food Crate", -resourceCost[2]);
            }
            foreach (var landzId in response.healedLandzTokens)
            {
                userSessionController.SetLandzHp(landzId, 1000);
            }
            userSessionController.SetTotalAttacks();
            SetControllerInfos();
            popupController.OpenSuccessPopupPanel("You landz are healed");
        }
        catch (WebRequestException e)
        {
            if (_goldz)
                popupController.ErrorHandler(userSessionController.DataController.TokenBalance < _goldzCost ? "Goldz" : "Request");
            else
            {
                var weapon = userSessionController.InventoryItens.Find(weapon => weapon.name == "Weapons");
                var armors = userSessionController.InventoryItens.Find(armor => armor.name == "Armors");
                var foodCrate = userSessionController.InventoryItens.Find(food => food.name == "Food Crate");
                if (weapon == null || armors == null || foodCrate == null || weapon?.quantity < resourceCost[0] || armors?.quantity < resourceCost[1] || foodCrate.quantity < resourceCost[2])
                    popupController.ErrorHandler("Resources");
                else
                    popupController.ErrorHandler("Request");
            }
        }
        finally
        {
            loadPanel.Disable();
            popupController.DisableFixConfirmationPoup();

        }
    }
    private void SetControllerInfos()
    {
        SetAvaiablesAttack();
        SetAvaiablesHealLandz();
        SetAvaiableRechargeLandz();
        SetResourcesInfo();
        OnPlayerInfoUpdated?.Invoke();
    }
    private void SetAvaiablesAttack()
    {
        var attackCounter = 0;
        foreach (var landz in userSessionController.CombatLandzs)
        {
            attackCounter += landz.maxAttacks;
        }
        attackAvaiablesText.text = userSessionController.TotalAttacksAvailable + "/" + attackCounter;
        attackAvaiableBar.fillAmount = (float)userSessionController.TotalAttacksAvailable / attackCounter;
        CheckAttackAllButton();
    }
    private void SetAvaiableRechargeLandz()
    {
        var avaiableRechargeLandz = 0;
        foreach (var landz in userSessionController.CombatLandzs)
        {
            if (landz.attacks > 0 || landz.health <= 0) continue;
            var lastRechargeTime = DateTimeOffset.Parse(landz.timeStamp);
#if DEVELOP
            AddTime(lastRechargeTime, out var nextRechargeTime, 30, TimeType.Minute);
#else
            AddTime(lastRechargeTime, out var nextRechargeTime,1,TimeType.Day);
#endif
            if ((nextRechargeTime - DateTimeOffset.UtcNow).Seconds < 0)
                avaiableRechargeLandz++;
        }
        rechargeAvaiablesText.text = avaiableRechargeLandz + "/" + userSessionController.CombatLandzs.Count;
        rechargeAvaiableBar.fillAmount = (float)avaiableRechargeLandz / userSessionController.CombatLandzs.Count;
        btnRechargeAll.interactable = avaiableRechargeLandz > 0;
    }
    private void SetAvaiablesHealLandz()
    {
        var noHealthLandzCounter = userSessionController.CombatLandzs.Count(landz => landz.health <= 0);
        healAvaiablesText.text = noHealthLandzCounter + "/" + userSessionController.CombatLandzs.Count;
        healAvaiableBar.fillAmount = (float)noHealthLandzCounter / userSessionController.CombatLandzs.Count;
        btnHealAll.interactable = noHealthLandzCounter > 0;
    }
    private void CheckAttackAllButton()
    {
        btnAttackAll.interactable = userSessionController.TotalAttacksAvailable > 0;
    }

    private void SetResourcesInfo()
    {
        goldzQuantity.text = "Total Amount\n" + userSessionController.DataController.TokenBalance;
        weaponsQuantity.text = "Total Amount\n" + GetResourcesInfo("Weapons", userSessionController.InventoryItens).ToString();
        armorsQuantity.text = "Total Amount:\n" + GetResourcesInfo("Armors", userSessionController.InventoryItens).ToString();
        foodQuantity.text = "Total Amount\n" + GetResourcesInfo("Food Crate", userSessionController.InventoryItens).ToString();
    }
    private int GetResourcesInfo(string resourceType, List<InventoryItem> inventoryItems)
    {
        var item = inventoryItems.Find(resource => resource.name == resourceType);
        return item != null ? item.quantity : 0;
    }
    private void AddTime(DateTimeOffset _lastRechargeTime, out DateTimeOffset _nextRechargeTime, int _timeToAdd, TimeType _timeType = TimeType.Day)
    {
        _nextRechargeTime = _timeType switch
        {
            TimeType.Day => _lastRechargeTime.AddDays(_timeToAdd),
            TimeType.Hour => _lastRechargeTime.AddHours(_timeToAdd),
            TimeType.Minute => _lastRechargeTime.AddMinutes(_timeToAdd),
            _ => _lastRechargeTime.AddSeconds(_timeToAdd)
        };
    }
    private enum TimeType { Day, Hour, Minute, Second }
}
