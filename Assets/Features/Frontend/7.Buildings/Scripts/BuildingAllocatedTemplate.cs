using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuildingAllocatedTemplate : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingDescription;
    [SerializeField] private TextMeshProUGUI statusDescription;
    [SerializeField] private TextMeshProUGUI durabilityInfo;
    [SerializeField] private TextMeshProUGUI chargeInfo;
    [Header("References")]
    [SerializeField] private ScrollRect scrollViewDescription;
    [Header("Images")]
    [SerializeField] private Image buildingSprite;
    [SerializeField] private Image durabilityBar;
    [Header("Buttons")]
    [SerializeField] private Button removeBtn;
    [SerializeField] private Button closeBtn;
    [Header("Slider")]
    [SerializeField] private Slider autoRebuilding;
    private void OnDisable()
    {
        if (removeBtn != null)
            removeBtn.onClick.RemoveAllListeners();
        autoRebuilding.onValueChanged.RemoveAllListeners();
    }
    public void Setup(FeudalzLandzAllocatedBuild _build)
    {
        buildingName.text = _build.type;
        buildingDescription.text = _build.description;
        scrollViewDescription.verticalScrollbar.value = 1f;
        statusDescription.text = _build.status;
        buildingSprite.sprite = _build.buildImage;
        UpdateDurabilityBar(_build.durability, _build.maxDurability);
        UpdateBuildCharge(_build.charges, _build.maxCharges);
        autoRebuilding.value = _build.autoRebuild ? 1 : 0;
    }
    public void UpdateDurabilityBar(float _currentDurability, float _maxDurability)
    {
        durabilityInfo.text = _currentDurability + "/" + _maxDurability;
        durabilityBar.fillAmount = _currentDurability / _maxDurability;
    }
    public void UpdateBuildCharge(int _currentCharge, int _maxCharge)
    {
        chargeInfo.text = "charges " + _currentCharge + "/" + _maxCharge;
    }
    public void RegisterRemoveBuildListener(UnityAction _onTryRemoveBuildCallback)
    {
        removeBtn.onClick.AddListener(_onTryRemoveBuildCallback);
    }
    public void RegisterAutoRebuildListener(UnityAction<float> onChangeAutoRebuildSettingsCallback)
    {
        autoRebuilding.onValueChanged.AddListener(onChangeAutoRebuildSettingsCallback);
    }
    public void RegisterCloseListener(UnityAction _onCloseCallback)
    {
        closeBtn.onClick.AddListener(_onCloseCallback);
    }
    public void LockInput()
    {
        if (removeBtn != null)
            removeBtn.interactable = false;
        if (closeBtn != null)
            closeBtn.interactable = false;
    }
    public void UnlockInput()
    {
        if (removeBtn != null)
            removeBtn.interactable = true;
        if (closeBtn != null)
            closeBtn.interactable = true;
    }
}
