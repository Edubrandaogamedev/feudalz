using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
public class InfoUIResource : MonoBehaviour
{
    [Header("Template")]
    [SerializeField] private ResourceUI resourceInfoTemplate;

    [Header("GameObject")] 
    [SerializeField] private GameObject resourceHeader;
    [Header("Contents")]
    [SerializeField] private RectTransform resourceContent;
    [field: Header("Internal")]
    private readonly List<ResourceUI> resources = new List<ResourceUI>();

    private void OnEnable()
    {
        InfoUIProductionManager.OnResourceClaimed += UpdateResourceInfo;
    }
    private void OnDisable()
    {
        InfoUIProductionManager.OnResourceClaimed -= UpdateResourceInfo;
    }
    public void SetResourcesInfo(UserSessionController userSessionController)
    {
        foreach (var resource in userSessionController.DataController.BuildingResourceData)
        {
            if (resource.ResourceName == "Dragon" || resource.ResourceName == "randomHero") continue;
            var clone = Instantiate(resourceInfoTemplate, resourceContent);
            clone.Setup(resource, GetInventoryResourceQuantity(resource.ResourceName, userSessionController.DataController.Consumables));
            resources.Add(clone);
        }
        OrderByAmount();
    }
    private void OrderByAmount()
    {
        var orderedEnumerable = resources.OrderByDescending(amount => amount.TotalQuantity);
        var index = 0;
        foreach (var order in orderedEnumerable)
        {
            order.transform.SetSiblingIndex(index);
            index++;
        }
        resourceHeader.transform.SetAsFirstSibling();
    }
    public void UpdateAllResources(UserSessionController userSessionController)
    {
        foreach (var resource in resources)
        {
            if (resource.Type == "Dragon" || resource.Type == "Heroez") continue;
            var inventoryItem = userSessionController.DataController.Consumables.Find(item => item.name == resource.Type);
            if (inventoryItem != null)
                resource.UpdateQuantity(inventoryItem.quantity);
        }
        OrderByAmount();
    }
    public void UpdateResourceInfo(string _resourceName, int _quantity)
    {
        var resourceUI = resources.Find(resource => resource.Type == _resourceName);
        if (resourceUI != null)
            resourceUI.UpdateQuantity(resourceUI.TotalQuantity + _quantity);
        OrderByAmount();
    }
    private int GetInventoryResourceQuantity(string resourceType, List<InventoryItem> inventoryItems)
    {
        var item = inventoryItems.Find(resource => resource.name == resourceType);
        return item != null ? item.quantity : 0;
    }
}