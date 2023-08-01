using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AllocationData : ScriptableObject
{
    [SerializeField] private List<BuildingData> buildingDatas = new List<BuildingData>();
    [SerializeField] private List<HeroezLocalData> heroezDatas = new List<HeroezLocalData>();
    [SerializeField] private List<RarityFrameData> rarityFrameDatas = new List<RarityFrameData>();
    public BuildingData GetBuildingByName(string _name)
    {
        return buildingDatas.Find(building => building.BuildingName == _name);
    }
    public HeroezLocalData GetHeroezDataByName(string _name)
    {
        return heroezDatas.Find(hero => hero.HeroName == _name);
    }
    public RarityFrameData GetRarityFrameData(string _rarity)
    {
        return rarityFrameDatas.Find(frame => frame.RarityName == _rarity);
    }
}
