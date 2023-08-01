using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudalzLandzBuild
{
    public string type { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string costType { get; set; }
    public float cost { get; set; }
    public float maxDurability { get; set; }
    public int maxCharges { get; set; }
    public Sprite buildImage { get; set; }
    public string position { get; set; }
    public string category { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
    public List<InventoryItem> inventoryItem;
    public AllowedCrafting[] allowedCraftings { get; set; }
    public FeudalzLandzBuild(Build _build, BuildingData _data, List<InventoryItem> inventoryItems)
    {
        type = _build.type;
        costType = _build.costType;
        cost = _build.cost;
        maxDurability = _build.maxDurability;
        maxCharges = _build.maxCharges;
        allowedCraftings = _build.allowedCraftings;
        category = _build.category;
        attack = _build.attack;
        defense = _build.defense;
        inventoryItem = inventoryItems;
        description = $"{_data.BuildingDescription}\n{GetCraftingDescription(_build.allowedCraftings, inventoryItems)}";
        status = $"<color=yellow>Attack:</color> {attack} <color=yellow>Defense:</color> {defense}.";
        buildImage = _data.BuildingImg;
    }

    private string GetCraftingDescription(AllowedCrafting[] _allowedCraftings, List<InventoryItem> inventoryItems)
    {
        var craftingDescription = "";
        var itens = new List<string>();
        foreach (var item in _allowedCraftings)
        {
            if (itens.Contains(item.productType)) continue;
            string fullText = "";
            fullText = $"<color=yellow>{item.productType}</color> x {item.productQuantity}:\n";
            for (var i = 0; i < item.rawCost.Length; i++)
            {
                fullText += $"<color=yellow>Resources Needed:</color> {item.rawCostType[i]} x{item.rawCost[i]}.";
                var foundedResource = inventoryItems.Find(resource => resource.name == item.rawCostType[i]);
                if (foundedResource != null)
                    if (item.rawCost[i] > foundedResource.quantity)
                        fullText += $"\n You have <color=red>x{foundedResource.quantity}</color>\n";
                    else
                        fullText += $"\n You have <color=green>x{foundedResource.quantity}</color>\n";
                else
                    fullText += "\n You have <color=red>x0</color>\n";
            }

            itens.Add(fullText);
        }
        for (var i = 0; i < itens.Count; i++)
        {
            craftingDescription += itens[i] + "\n";
        }
        return craftingDescription;
    }
}
