using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudalzHeroez
{
    public string name {get;set;}
    public string heroRarity {get;set;}
    public string heroDescription {get;set;}
    public string heroSkillName { get; set; }
    public int heroCharges {get;set;}
    public int heroQuantity { get; set; }
    public float heroProbability { get; set; }
    public Sprite rarityBorderImage {get;set;}
    public Sprite rarityCardImage { get; set; }
    public Sprite heroImage {get;set;}
    public Sprite heroCard { get; set; }
    public FeudalzHeroez(HerozData _hero, HeroezLocalData _heroData)
    {
        name = _hero.name;
        heroRarity = _hero.rarity;
        heroCharges = (int)_hero.charges;
        heroQuantity = _hero.quantity;
        heroProbability = _hero.probability;
        if (_hero.charges == 0)
        {
            heroCharges = 10;
        }
        heroImage = _heroData.HeroImg;
        heroCard = _heroData.HeroCard;
        rarityCardImage = _heroData.GetRarityCardData(_hero.rarity).RarityBoard;
        rarityBorderImage = _heroData.GetRarityFrameData(_hero.rarity).RarityBoard;
        heroDescription = _heroData.HeroDescription;
        heroSkillName = _heroData.HeroSkillName;
    }
    public FeudalzHeroez(string _hero, HeroezLocalData _heroData)
    {
        name = _hero;
        heroImage = _heroData.HeroImg;
        heroRarity = "unique";
        heroDescription = _heroData.HeroDescription;
        rarityBorderImage = _heroData.GetRarityFrameData("unique").RarityBoard;
        heroCard = _heroData.HeroCard;
        rarityCardImage = _heroData.GetRarityCardData("unique").RarityBoard;
        rarityBorderImage = _heroData.GetRarityFrameData("unique").RarityBoard;
        heroDescription = _heroData.HeroDescription;
        heroSkillName = _heroData.HeroSkillName;
    }
    public Color GetRarityBackground()
    {
        var rarityBackground = new Color();
        switch (heroRarity)
        {
            case "common":
                ColorUtility.TryParseHtmlString("#453d38", out rarityBackground);
                break;
            case "uncommon":
                ColorUtility.TryParseHtmlString("#165a4c", out rarityBackground);
                break;
            case "rare":
                ColorUtility.TryParseHtmlString("#323353", out rarityBackground);
                break;
            case "epic":
                ColorUtility.TryParseHtmlString("#3f2350", out rarityBackground);
                break;
            case "unique":
                ColorUtility.TryParseHtmlString("#7a4843", out rarityBackground);
                break;
        }
        return rarityBackground;
    }
}
