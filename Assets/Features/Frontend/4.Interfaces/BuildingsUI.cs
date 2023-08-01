using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BuildingsUI : MonoBehaviour
{
    public static event UnityAction<BuildingsUI, int> OnBuildAdded;
    public static event UnityAction<BuildingsUI, int> OnBuildRemoved;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingAllocatedQuantityText;
    [Header("Image")]
    [SerializeField] private Image buildingSprite;
    [SerializeField] private Button plusBtn;
    [SerializeField] private Button minusBtn;
    public List<AllowedCrafting> CraftingOptions { get; private set; }
    public List<AllowedCrafting> OriginalCraftingOptions { get; private set; }
    public string BuildingType { get; private set; }
    public string ProductType { get; private set; }
    private int chosenBuildQuantity;
    private int maxBuildQuantity;

    public void Setup(AllowedCrafting _product, List<ResourceData> _datas, FeudalzLandzAllocatedBuild _build, List<AllowedCrafting> _originalList)
    {
        var foundData = _datas.Find(resource => resource.ResourceName == _product.productType);
        if (foundData == null) return;
        OriginalCraftingOptions = _originalList;
        ProductType = _product.productType;
        CraftingOptions = _build.allowedCraftings.ToList().FindAll(option => option.productType == _product.productType);
        BuildingType = _build.type;
        maxBuildQuantity = 1;
        buildingSprite.sprite = foundData.ResourceImg;
        buildingName.text = _product.productType;
        ControlBuildQuantityText(0);
    }
    public void Setup(FeudalzLandzAllocatedBuild _build)
    {
        buildingSprite.sprite = _build.buildImage;
        maxBuildQuantity = 1;
        buildingName.text = _build.type;
        BuildingType = _build.type;
        CraftingOptions = _build.allowedCraftings.ToList();
        ControlBuildQuantityText(0);
    }
    public void SetLockButtonState(bool _unlock)
    {
        plusBtn.interactable = _unlock;
        minusBtn.interactable = _unlock;
    }
    public void ResetStatus()
    {
        chosenBuildQuantity = 0;
        maxBuildQuantity = 0;
    }
    public void IncreaseMaxQuantity()
    {
        maxBuildQuantity += 1;
        ControlBuildQuantityText(chosenBuildQuantity);
    }
    private void OnEnable()
    {
        plusBtn.onClick.AddListener(SelectPlusBuilding);
        minusBtn.onClick.AddListener(SelectMinusBuilding);
    }
    private void OnDisable()
    {
        plusBtn.onClick.RemoveAllListeners();
        minusBtn.onClick.RemoveAllListeners();
    }
    private void SelectPlusBuilding()
    {
        if (chosenBuildQuantity >= maxBuildQuantity) return;
        ControlBuildQuantityText(chosenBuildQuantity + 1);
        OnBuildAdded?.Invoke(this,chosenBuildQuantity);
    }
    private void SelectMinusBuilding()
    {
        if (chosenBuildQuantity <= 0) return;
        ControlBuildQuantityText(chosenBuildQuantity - 1);
        OnBuildRemoved?.Invoke(this,chosenBuildQuantity);
    }
    private void ControlBuildQuantityText(int value)
    {
        chosenBuildQuantity = value;
        buildingAllocatedQuantityText.text = $"{chosenBuildQuantity.ToString()} / {maxBuildQuantity.ToString()}";
    }
}
