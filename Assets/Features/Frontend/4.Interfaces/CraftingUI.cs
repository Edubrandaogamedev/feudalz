using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CraftingUI : MonoBehaviour
{
    public static event UnityAction<CraftingUI, bool> OnCraftOptionChosen;
    [Header("Templates")]
    [SerializeField] private ResourceUI resourcesTemplate;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resourceName;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI resourceNeededText;
    [Header("Toggle")]
    [SerializeField] private Toggle selectedToggle;
    [Header("Images")]
    [SerializeField] private Image resourceImg;

    [SerializeField] private Image blockResourceImg;
    [Header("Rect Transforms")]
    [SerializeField] private RectTransform resourcesNeededContent;
    [SerializeField] private RectTransform noCostPosition; //this set the position of production quantity text when there's no resource needed
    [SerializeField] private RectTransform costPosition; //this set the position of production quantity text when there's resource needed
    private readonly List<ResourceUI> resourcesNeeded = new List<ResourceUI>();
    [field: Header("Internal")]
    public int Index { get; private set; }
    public string Type { get; private set; }
    public List<ResourceUI> ResourcesNeeded => resourcesNeeded; 

    private void OnEnable()
    {
        selectedToggle.onValueChanged.AddListener(OnSelectCrafting);
    }
    private void OnDisable()
    {
        selectedToggle.onValueChanged.RemoveAllListeners();
    }

    private void OnSelectCrafting(bool _selected)
    {
        OnCraftOptionChosen?.Invoke(this, _selected);
    }
    public void Setup(AllowedCrafting _craft, List<ResourceData> _datas, int optionIndex, UserSessionController userSessionController)
    {
        Index = optionIndex;
        var foundData = _datas.Find(resource => resource.ResourceName == _craft.productType);
        if (foundData == null) return;
        Type = _craft.productType;
        resourceName.text = _craft.productType;
        quantityText.text = "Quantity Produced: " + _craft.productQuantity;
        if (_craft.rawCostType.Length <= 0)
        {
            quantityText.transform.position = noCostPosition.transform.position;
            resourcesNeededContent.gameObject.SetActive(false);
            resourceNeededText.gameObject.SetActive(false);
        }
        else
        {
            quantityText.transform.position = costPosition.transform.position;
            resourcesNeededContent.gameObject.SetActive(true);
            resourceNeededText.gameObject.SetActive(true);
        }
        resourceImg.sprite = foundData.ResourceImg;
        blockResourceImg.gameObject.SetActive(false);
        CreateResourceCost(_craft, _datas, userSessionController);
    }

    public void UpdateResourceQuantity(AllowedCrafting _craft, int _value, UserSessionController userSessionController)
    {
        quantityText.text = "Quantity Produced: " + _craft.productQuantity * _value;
        foreach (var resource  in resourcesNeeded)
        {
            resource.UpdateNeededResource(_value, userSessionController);
        }
    }
    public void BlockResource()
    {
        blockResourceImg.gameObject.SetActive(true);
    }
    private void CreateResourceCost(AllowedCrafting _craft, List<ResourceData> _datas, UserSessionController userSessionController)
    {      
        for (var index = 0; index < _craft.rawCostType.Length; index++)
        {
            var rawCost = _craft.rawCostType[index];
            var foundData = _datas.Find(resource => resource.ResourceName == rawCost);
            if (foundData == null) continue;
            var clone = Instantiate(resourcesTemplate, resourcesNeededContent);
            clone.SetupNeededResource(_craft.rawCostType[index],foundData.ResourceImg, (int)_craft.rawCost[index], userSessionController);
            resourcesNeeded.Add(clone);
        }
    }
}
