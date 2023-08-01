using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUIHeroes : MonoBehaviour
{
    [Header("Template")]
    [SerializeField] private HeroezAllocationTemplate heroezTemplate;
    [SerializeField] private RectTransform epicContent;
    [SerializeField] private RectTransform rareContent;
    [SerializeField] private RectTransform uncommonContent;
    [SerializeField] private RectTransform commonContent;
    private readonly List<HeroezAllocationTemplate> commonHeroez = new List<HeroezAllocationTemplate>();
    private readonly List<HeroezAllocationTemplate> uncommonHeroez = new List<HeroezAllocationTemplate>();
    private readonly List<HeroezAllocationTemplate> rareHeroez = new List<HeroezAllocationTemplate>();
    private readonly List<HeroezAllocationTemplate> epicHeroez = new List<HeroezAllocationTemplate>();

    public void UpdateHeroInfo(UserSessionController userSessionController)
    {
        var heroezByRarity = userSessionController.DataController.GetAllHeroezByRarity("common");
        SetHeroesPanelByRarity(heroezByRarity, commonContent, commonHeroez);
        heroezByRarity = userSessionController.DataController.GetAllHeroezByRarity("uncommon");
        SetHeroesPanelByRarity(heroezByRarity, uncommonContent, uncommonHeroez);
        heroezByRarity = userSessionController.DataController.GetAllHeroezByRarity("rare");
        SetHeroesPanelByRarity(heroezByRarity, rareContent, rareHeroez);
        heroezByRarity = userSessionController.DataController.GetAllHeroezByRarity("epic");
        SetHeroesPanelByRarity(heroezByRarity, epicContent, epicHeroez);
    }

    private void SetHeroesPanelByRarity(List<FeudalzHeroez> heroezList, RectTransform content, List<HeroezAllocationTemplate> poolingList)
    {
        var index = 0;
        foreach (var heroez in heroezList)
        {
            if (index < poolingList.Count)
            {
                poolingList[index].gameObject.SetActive(true);
                poolingList[index].Setup(heroez);
            }
            else
            {
                var clone = Instantiate(heroezTemplate, content);
                clone.Setup(heroez);
                poolingList.Add(clone);
            }
            index++;
        }
        if (poolingList.Count > heroezList.Count)
        {
            for (var i = heroezList.Count; i < poolingList.Count; i++)
            {
                poolingList[i].gameObject.SetActive(false);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
