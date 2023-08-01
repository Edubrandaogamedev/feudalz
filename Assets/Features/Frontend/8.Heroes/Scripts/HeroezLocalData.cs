using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Heroez/Data", fileName = "NewHeroezData")]
public class HeroezLocalData : ScriptableObject
{
    [SerializeField] private string heroName;
    [SerializeField] private Sprite heroImg;
    [SerializeField] private Sprite heroCard;
    [SerializeField] private List<RarityFrameData> rarities = new List<RarityFrameData>();
    [SerializeField] private List<RarityFrameData> cards = new List<RarityFrameData>();
    [Multiline]
    [SerializeField] private string heroDescription;

    [SerializeField] private string heroSkillName;
    public Sprite HeroImg => heroImg;
    public Sprite HeroCard => heroCard;
    public string HeroDescription => heroDescription;
    public string HeroName => heroName;
    public string HeroSkillName => heroSkillName;

    public RarityFrameData GetRarityFrameData(string _rarity)
    {
        return rarities.Find(rarity => rarity.RarityName == _rarity);
    }
    public RarityFrameData GetRarityCardData(string _rarity)
    {
        return cards.Find(rarity => rarity.RarityName == _rarity);
    }
}
