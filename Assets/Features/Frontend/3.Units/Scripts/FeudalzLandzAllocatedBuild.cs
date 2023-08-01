using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudalzLandzAllocatedBuild
{
    public string type { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public float durability { get; set; }
    public float maxDurability { get; set; }
    public int charges { get; set; }
    public int maxCharges { get; set; }
    public Sprite buildImage { get; set; }
    public string position { get; set; }
    public AllowedCrafting[] allowedCraftings { get; set; }
    public AllowedCrafting currentProduction { get; set; }
    public string lastStartedCycleTimestamp { get; set; }
    public bool autoRebuild { get; set; }
    public string category { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }

    public FeudalzLandzAllocatedBuild(AllocatedBuild _allocatedBuild, BuildingData _data, string _position, FeudalzLandzBuild _build)
    {
        type = _allocatedBuild.type;
        durability = _allocatedBuild.durability;
        maxDurability = _allocatedBuild.maxDurability;
        charges = _allocatedBuild.charges;
        maxCharges = _allocatedBuild.maxCharges;
        currentProduction = _allocatedBuild.currentProduction;
        lastStartedCycleTimestamp = _allocatedBuild.lastStartedCycleTimestamp;
        description = _build?.description;
        buildImage = _data.BuildingImg;
        position = _position;
        autoRebuild = _allocatedBuild.autoRebuild;
        category = _allocatedBuild.category;
        attack = _allocatedBuild.attack;
        defense = _allocatedBuild.defense;
        status = $"<color=yellow>Attack:</color> {attack} <color=yellow>Defense:</color> {defense}.";
    }
    public FeudalzLandzAllocatedBuild(FeudalzLandzBuild _build, bool _autoRebuild = false)
    {
        currentProduction = null;
        lastStartedCycleTimestamp = "";
        type = _build.type;
        durability = _build.maxDurability;
        maxDurability = _build.maxDurability;
        charges = _build.maxCharges;
        maxCharges = _build.maxCharges;
        description = _build.description;
        buildImage = _build.buildImage;
        position = _build.position;
        category = _build.category;
        attack = _build.attack;
        defense = _build.defense;
        status = $"<color=yellow>Attack:</color> {attack} <color=yellow>Defense:</color> {defense}.";
        autoRebuild = _autoRebuild;
    }
    public FeudalzLandzAllocatedBuild(string _type) //ONLY FOR UNICORN UNIQUE BUILD
    {
        type = _type;
    }
}
