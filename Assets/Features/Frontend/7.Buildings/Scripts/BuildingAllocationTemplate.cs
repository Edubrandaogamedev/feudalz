using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class BuildingAllocationTemplate : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingDescription;
    [SerializeField] private TextMeshProUGUI statusDescription;
    [SerializeField] private TextMeshProUGUI goldzValue;
    [Header("References")]
    [SerializeField] private ScrollRect scrollViewDescription;
    [Header("Images")]
    [SerializeField] private Image buildingSprite;
    [SerializeField] private Image goldzSprite;
    [SerializeField] private Image resourceSprite;
    [SerializeField] private Image blockBuildImg;
    [Header("Slider")]
    [SerializeField] private Slider autoRebuildSlider;
    [Header("Buttons")]
    [SerializeField] private Button buyBtn;
    [Header("Properties")]
    private FeudalzLandzBuild _buildData;
    private float resourceCost = 0;
    private string resourceType = "";

    public FeudalzLandzBuild BuildData => _buildData;
    public string ResourceType => resourceType;
    public float ResourceCost => resourceCost;

    private void OnDisable()
    {
        buyBtn.onClick.RemoveAllListeners();
    }
    public void Setup(FeudalzLandzBuild _buildData, string _allocationType, List<ResourceData> resourceDatas)
    {
        this._buildData = _buildData;
        buildingName.text = _buildData.type;
        buildingDescription.text = _buildData.description;
        scrollViewDescription.verticalScrollbar.value = 1f;
        statusDescription.text = _buildData.status;
        buildingSprite.sprite = _buildData.buildImage;
        resourceCost = _buildData.cost;
        resourceType = _buildData.costType;
        blockBuildImg.gameObject.SetActive(false);
        SetResourceCostImage(_buildData, resourceDatas.Find(resource => resource.ResourceName == _buildData.costType));
        SetPosition(_allocationType);

    }
    private void SetPosition(string _position)
    {
        if (_position.Contains("Top"))
        {
            _buildData.position = "top";
        }
        else if (_position.Contains("Left"))
        {
            _buildData.position = "left";
        }
        else if (_position.Contains("Right"))
        {
            _buildData.position = "right";
        }
    }

    private void SetResourceCostImage(FeudalzLandzBuild _buildData, ResourceData _resourceData)
    {
        if (_buildData.costType == "goldz")
        {
            goldzSprite.gameObject.SetActive(true);
            resourceSprite.gameObject.SetActive(false);
            goldzValue.text = _buildData.cost.ToString();
        }
        else
        {
            goldzSprite.gameObject.SetActive(false);
            resourceSprite.gameObject.SetActive(true);
            resourceSprite.sprite = _resourceData.ResourceImg;
            var foundedResource = _buildData.inventoryItem.Find(resource => resource.name == _buildData.costType);
            if (foundedResource != null)
                if (_buildData.cost > foundedResource.quantity)
                    goldzValue.text = $"<color=red>{_buildData.cost}/{foundedResource.quantity.ToString()}</color>";
                else
                    goldzValue.text = $"<color=green>{_buildData.cost}/{foundedResource.quantity.ToString()}</color>";
            else
                goldzValue.text = $"<color=red>{_buildData.cost}/0</color>";
        }

    }
    public void BlockBuild()
    {
        blockBuildImg.gameObject.SetActive(true);
    }
    public void RegisterBuyListener(UnityAction<BuildingAllocationTemplate> _onTryBuyCallback)
    {
        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => _onTryBuyCallback(this));
    }
    public void LockInput()
    {
        if (buyBtn != null)
            buyBtn.interactable = false;
    }
    public void UnlockInput()
    {
        if (buyBtn != null)
            buyBtn.interactable = true;
    }
    public bool GetAutoRebuilderInfo()
    {
        return autoRebuildSlider.value != 0;
    }
}
