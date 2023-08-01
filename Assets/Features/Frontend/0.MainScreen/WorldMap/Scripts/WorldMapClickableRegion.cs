using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class WorldMapClickableRegion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Bioma")]
    [SerializeField] private FeudalzBiomas bioma;
    [Header("Button")]
    [Space]
    [SerializeField] private Button regionButton;
    [SerializeField] private Image regionImage;
    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks zoomInFeedback;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI biomeDescription;
    [SerializeField] private TextMeshProUGUI biomeLandsQuantity;
    [SerializeField] private TextMeshProUGUI biomeLandsAttacks;
    [SerializeField] private float textFontScaleEffect;
    private float originalFontSize;
    [Header("Shadow Effect")]
    [SerializeField] private Shadow shadowEffect;
    private int cachedOriginalHieraquyIndex;
    bool isLocked;
    public event UnityAction<FeudalzBiomas> onRegionClicked;
    private void OnEnable()
    {
        regionButton.onClick?.AddListener(OnRegionClicked);
        originalFontSize = biomeDescription.fontSize;
    }
    private void OnDisable()
    {
        regionButton.onClick?.RemoveListener(OnRegionClicked);
    }
    private void Start()
    {
         regionImage.alphaHitTestMinimumThreshold = 1f;
    }
    private void OnRegionClicked()
    {
        if (isLocked) return;
        onRegionClicked?.Invoke(bioma);
        this.transform.SetAsLastSibling();
        zoomInFeedback.PlayFeedbacks();
        isLocked = true;
        AudioManager.instance.Play("natural_gust_sfx");
    }
    public void LockButton()
    {
        isLocked = true;
    }
    public void UnlockButton()
    {
        isLocked = false;
    }

    public void SetUserLands(UserSessionController userSessionController, List<FeudalzCombatLandz> landz)
    {
        int landsCounter = 0;
        foreach (var land in landz)
        {
            var biomeTrait = land.traits.Find(trait => trait.trait_type.Contains("biome"));
            if (biomeTrait == null && userSessionController.DataController.IsUniqueLandzUnique(land.tokenId))
                biomeTrait = new Trait("biome", land.DefineUniqueBiome(land.tokenId), 0);
            if (biomeTrait.value.Contains(bioma.ToString()))
                landsCounter++;
        }
        biomeLandsQuantity.text = landsCounter.ToString();
        regionButton.interactable = landsCounter > 0;
        regionImage.raycastTarget = landsCounter > 0;
    }

    public void SetUserLandsAttacks(UserSessionController userSessionController, List<FeudalzCombatLandz> landz)
    {
        if (biomeLandsAttacks == null) return;
        int landsAttacks = 0;
        foreach (var land in landz)
        {
            var biomeTrait = land.traits.Find(trait => trait.trait_type.Contains("biome"));
            if (biomeTrait == null && userSessionController.DataController.IsUniqueLandzUnique(land.tokenId))
                biomeTrait = new Trait("biome", land.DefineUniqueBiome(land.tokenId), 0);
            if (biomeTrait.value.Contains(bioma.ToString()) && land.health > 0)
                landsAttacks += land.attacks;
        }
        biomeLandsAttacks.text = landsAttacks.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked) return;
        this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        cachedOriginalHieraquyIndex = transform.GetSiblingIndex();
        this.transform.SetAsLastSibling();
        biomeDescription.fontSize = textFontScaleEffect;
        shadowEffect.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isLocked) return;
        this.transform.localScale = new Vector3(1, 1, 1);
        biomeDescription.fontSize = originalFontSize;
        shadowEffect.enabled = false;
        this.transform.SetSiblingIndex(cachedOriginalHieraquyIndex);
    }
}
