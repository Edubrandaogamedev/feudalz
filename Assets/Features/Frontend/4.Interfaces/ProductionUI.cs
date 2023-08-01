using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class ProductionUI : MonoBehaviour
{
    [Header("Templates")]
    [SerializeField] private GameObject feedbackClaimPrefab;
    [SerializeField] private GameObject canvasScene;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemQuantityText;
    [Header("Images")]
    [SerializeField] private Image itemImage;
    [SerializeField] private Image timerBar;
    [Header("Buttons")]
    [SerializeField] private Button claimButton;
    [SerializeField] private Button cancelButton;
    [Header("Slider")]
    [SerializeField] private Slider autoCraftSlider;
    
    private string batchId;
    private string batchItemName;
    private int batchQuantity;
    private string feedbackQuantity;
    private DateTimeOffset lastCycleProduction;
    private DateTimeOffset claimTime;
    private float percentage = 1f;
    public string BatchId => batchId;
    public int BatchQuantity => batchQuantity;
    public string BatchItemName => batchItemName;
    public bool canRecharge { get; private set; }
    public GameObject CanvasScene { get => canvasScene; set => canvasScene = value; }

    public TextMeshProUGUI ItemQuantityText { get => itemQuantityText; set => itemQuantityText = value; }
    public Image ItemImage { get => itemImage; set => itemImage = value; }
    public Button ClaimButton { get => claimButton; set => claimButton = value; }
    public string FeedbackQuantity { get => feedbackQuantity; set => feedbackQuantity = value; }
    
    public void Setup(Batch _batchProduction, List<ResourceData> _datas)
    {
        batchId = _batchProduction.batchId;
        batchQuantity = _batchProduction.productQuantity;
        batchItemName = _batchProduction.productType;
        autoCraftSlider.value = _batchProduction.autoCraft ? 1 : 0;
        var data = _datas.Find(resource => resource.ResourceName == _batchProduction.productType);
        itemQuantityText.text = "x" + _batchProduction.productQuantity;
        itemImage.sprite = data.ResourceImg;
        itemNameText.text = data.ResourceName;
        if (data.ResourceName == "randomHero")
        {
            itemNameText.text = "Random Hero";
            cancelButton.gameObject.SetActive(false);
        }
        lastCycleProduction = DateTimeOffset.Parse(_batchProduction.timestamp);
#if DEVELOP
        AddTime(out claimTime, 5, TimeType.Minute);
#else
        AddTime(out claimTime,24,TimeType.Hour);
#endif
    }

    public void SetupFeedbackQuantity(HarvestResult _harvestResult)
    {
        if (_harvestResult?.harvestResults != null && _harvestResult?.harvestResults.Count >0)
            feedbackQuantity = DefineQuantityText(_harvestResult.harvestResults.ElementAt(0));
    }

    private string DefineQuantityText(KeyValuePair<string, int> _harvestItem)
    {
        if (_harvestItem.Key.Contains("babyDragon"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "babyDragon");
        else if (_harvestItem.Key.Contains("etherman"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "etherman");
        else if (_harvestItem.Key.Contains("godjira"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "godjira");
        else if (_harvestItem.Key.Contains("lys"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "lys");
        else if (_harvestItem.Key.Contains("urzog"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "urzog");
        else if (_harvestItem.Key.Contains("yokai"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "yokai");
        else if (_harvestItem.Key.Contains("guttx"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "guttx");
        else
            return "x" + _harvestItem.Value.ToString();
    }

    private string DefineHeroTextInQuantity(string _response, string _hero)
    {
        string _heroRarity = _response.Replace(_hero, "");

        return _heroRarity switch
        {
            "common" => $"Common {_hero}",
            "uncommon" => $"<color=#1eff00>Uncommon</color> {_hero}",
            "rare" => $"<color=#0070dd>Rare</color> {_hero}",
            "epic" => $"<color=#a335ee>Epic</color> {_hero}",
            _ => $""
        };
    }
    public void RegisterListeners(UnityAction<ProductionUI> _onTryClaimCallback, UnityAction<ProductionUI> _onTryCancelCallback)
    {
        ClaimButton.onClick.AddListener(() => _onTryClaimCallback(this));
        cancelButton.onClick.AddListener(() => _onTryCancelCallback(this));
    }

    public void RegisterAutoCraftListener(UnityAction<ProductionUI, float> _onChangeAutoCraftSettings)
    {
        autoCraftSlider.onValueChanged.RemoveAllListeners();
        autoCraftSlider.onValueChanged.AddListener((value) => _onChangeAutoCraftSettings(this, autoCraftSlider.value));
    }
    private void UpdateCooldown(DateTimeOffset _claimTime)
    {
        var remainingTime = _claimTime - DateTimeOffset.UtcNow;
        if (remainingTime.Seconds > 0)
        {
            percentage = (float)(1 - (remainingTime.TotalSeconds / ((claimTime - lastCycleProduction).TotalSeconds)));
        }
        timerBar.fillAmount = percentage;
        timerText.text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
        CheckClaimCooldown(remainingTime);
    }
    public void SetupClaimFeedback(Transform _button)
    {
        var feedback = Instantiate(feedbackClaimPrefab, _button.transform.position, _button.transform.rotation, canvasScene.transform);
        FeedbackAnimHandler feedbackClass = feedback.GetComponentInChildren<FeedbackAnimHandler>();
        feedbackClass.Setup(feedbackQuantity, itemImage.sprite);
    }

    public void LockAllButtons()
    {
        claimButton.interactable = false;
        cancelButton.interactable = false;
    }
    public void UnlockAllButtons()
    {
        claimButton.interactable = true;
        cancelButton.interactable = true;
    }
    private void CheckClaimCooldown(TimeSpan remainingTime)
    {
        timerText.gameObject.SetActive(remainingTime.TotalSeconds >= 0);
        timerBar.transform.parent.gameObject.SetActive(remainingTime.TotalSeconds >= 0);
        ClaimButton.interactable = remainingTime.TotalSeconds < 0;
        canRecharge = remainingTime.TotalSeconds < 0;
    }
    public void Update()
    {
        UpdateCooldown(claimTime);
    }
    private void AddTime(out DateTimeOffset _nextRechargeTime, int _timeToAdd, TimeType _timeType = TimeType.Day)
    {
        _nextRechargeTime = _timeType switch
        {
            TimeType.Day => lastCycleProduction.AddDays(_timeToAdd),
            TimeType.Hour => lastCycleProduction.AddHours(_timeToAdd),
            TimeType.Minute => lastCycleProduction.AddMinutes(_timeToAdd),
            _ => lastCycleProduction.AddSeconds(_timeToAdd)
        };
    }
    private enum TimeType { Day, Hour, Minute, Second }
}
