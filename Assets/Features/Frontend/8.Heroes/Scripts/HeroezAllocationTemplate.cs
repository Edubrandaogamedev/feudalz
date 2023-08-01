using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class HeroezAllocationTemplate : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroRarity;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private TextMeshProUGUI heroQuantity;
    [SerializeField] private TextMeshProUGUI chargeValue;
    [Header("Images")]
    [SerializeField] private Image heroSprite;
    [SerializeField] private Image rarityFrame;
    [SerializeField] private Image heroRarityBackground;
    [Header("Buttons")]
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button removeBtn;
    [SerializeField] private Button closeBtn;
    [Header("Heroez")]
    private FeudalzHeroez heroez;
    private void OnDisable()
    {
        if (selectBtn != null)
            selectBtn.onClick.RemoveAllListeners();
        if (removeBtn != null)
            removeBtn.onClick.RemoveAllListeners();
    }
    public void Setup(FeudalzHeroez _hero)
    {
        heroez = _hero;
        heroName.text = _hero.name.ToUpper();
        heroRarity.text = _hero.heroRarity.ToUpper();
        if (heroQuantity != null) heroQuantity.text = $"x{_hero.heroQuantity}";
        SetRarityColor(_hero.heroRarity);
        chargeValue.text = "Charges x" + _hero.heroCharges.ToString();
        if (_hero.name == "yokai")
            heroDescription.text = _hero.heroDescription + $" {_hero.heroProbability}";
        else
            heroDescription.text = _hero.heroDescription + $" {_hero.heroProbability}%";
        heroSprite.sprite = _hero.heroImage;
        rarityFrame.sprite = _hero.rarityBorderImage;
        SetupPopup(false);
    }

    public void SetupUnique(FeudalzHeroez _hero)
    {
        heroez = _hero;
        heroName.text = _hero.name.ToUpper();
        heroDescription.text = _hero.heroDescription;
        heroSprite.sprite = _hero.heroImage;
        rarityFrame.sprite = _hero.rarityBorderImage;
        heroRarity.text = "Unique";
        SetRarityColor("unique");
        SetupPopup(true);
    }

    public void SetupPopup(bool _unique)
    {
        if (removeBtn != null)
        {
            removeBtn.gameObject.SetActive(!_unique);
        }
        if (heroQuantity != null)
        {
            heroQuantity.gameObject.SetActive(!_unique);
        }
        if (chargeValue != null)
        {
            chargeValue.gameObject.SetActive(!_unique);
        }
    }
    public void RegisterSelectListener(UnityAction<FeudalzHeroez> _onTryAllocateCallback)
    {
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(() => _onTryAllocateCallback(heroez));
    }
    public void RegisterRemoveHeroListener(UnityAction _onTryRemoveHeroCallback)
    {
        removeBtn.onClick.AddListener(_onTryRemoveHeroCallback);
    }
    public void RegisterCloseListener(UnityAction _onCloseCallback)
    {
        closeBtn.onClick.AddListener(_onCloseCallback);
    }
    public void LockInput()
    {
        if (selectBtn != null)
            selectBtn.interactable = false;
        if (removeBtn != null)
            removeBtn.interactable = false;
        if (closeBtn != null)
            closeBtn.interactable = false;
    }
    public void UnlockInput()
    {
        if (selectBtn != null)
            selectBtn.interactable = true;
        if (removeBtn != null)
            removeBtn.interactable = true;
        if (closeBtn != null)
            closeBtn.interactable = true;
    }
    private void SetRarityColor(string _rarity)
    {
        var rarityColor = new Color();
        var rarityBackground = new Color();
        switch (_rarity)
        {
            case "common":
                rarityColor = Color.white;
                ColorUtility.TryParseHtmlString("#453d38", out rarityBackground);
                break;
            case "uncommon":
                ColorUtility.TryParseHtmlString("#1eff00", out rarityColor);
                ColorUtility.TryParseHtmlString("#165a4c", out rarityBackground);
                break;
            case "rare":
                ColorUtility.TryParseHtmlString("#0070dd", out rarityColor);
                ColorUtility.TryParseHtmlString("#323353", out rarityBackground);
                break;
            case "epic":
                ColorUtility.TryParseHtmlString("#a335ee", out rarityColor);
                ColorUtility.TryParseHtmlString("#3f2350", out rarityBackground);
                break;
            case "unique":
                ColorUtility.TryParseHtmlString("#ffba00", out rarityColor);
                ColorUtility.TryParseHtmlString("#7a4843", out rarityBackground);
                break;
        }
        heroRarity.color = rarityColor;
        heroRarityBackground.color = rarityBackground;
    }
}