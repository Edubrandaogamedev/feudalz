using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private Image resourceImg;
    [SerializeField] private TextMeshProUGUI resourceName;
    [SerializeField] private TextMeshProUGUI resourceQuantity;
    public string Type { get; private set; }
    public int TotalQuantity { get; private set; }
    public int BaseQuantity { get; private set; }
    public int CachedResourceQuantity { get; private set; }

    public void Setup(ResourceData _data, int quantity)
    {
        Type = _data.ResourceName;
        TotalQuantity = quantity;
        resourceImg.sprite = _data.ResourceImg;
        resourceName.text = _data.ResourceName;
        resourceQuantity.text = "Total Amount: " + quantity;
    }

    public void SetupNeededResource(string _resourceType, Sprite _resourceImg, int _quantity, UserSessionController userSessionController)
    {
        Type = _resourceType;
        BaseQuantity = _quantity;
        resourceImg.sprite = _resourceImg;
        // for (int i = 0; i < _userSessionDataManager.InventoryItens.Count; i++)
        // { 
        var foundedResource = userSessionController.DataController.Consumables.Find(item => item.name == _resourceType);
        if (foundedResource != null)
        {
            CachedResourceQuantity = foundedResource.quantity;
            if (_quantity > foundedResource.quantity)
                resourceQuantity.text = $"<color=red>{_quantity}/{foundedResource.quantity}</color>";
            else
                resourceQuantity.text = $"<color=green>{_quantity}/{foundedResource.quantity}</color>";
        }
        else
        {
            //Debug.Log("Não achei a chave");
            resourceQuantity.text = $"<color=red>{_quantity}/0</color>";
        }
        // }

    }
    public void UpdateNeededResource(int _multiplier, UserSessionController userSessionController)
    {
        if (BaseQuantity * _multiplier > CachedResourceQuantity)
            resourceQuantity.text = $"<color=red>{BaseQuantity * _multiplier}/{CachedResourceQuantity}</color>";
        else
            resourceQuantity.text = $"<color=green>{BaseQuantity * _multiplier}/{CachedResourceQuantity}</color>";
        //resourceQuantity.text = "x" + BaseQuantity * _multiplier;
    }
    public void UpdateQuantity(int _quantity)
    {
        TotalQuantity = _quantity;
        resourceQuantity.text = "Total Amount: " + _quantity;
    }
}
