using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RegionLandzPanel : MonoBehaviour
{
    [Header("Template")]
    [SerializeField] private CombatLandz landzTemplate;
    [Header("Contents")]
    [SerializeField] private  RectTransform landzContent;
    private List<CombatLandz> combatLandz;
    public List<CombatLandz> CombatLandz { get => combatLandz;}
    public void CreateLandzOnPanel(UserSessionController userSessionController)
    {
        combatLandz = new List<CombatLandz>();
        int index = 0;
        foreach (FeudalzCombatLandz landz in userSessionController.DataController.GetLandzByBiome(userSessionController.CurrentUserLandzBioma))
        {
            CombatLandz clone = Instantiate(landzTemplate,landzContent.transform);
            clone.gameObject.name = "CombatLandz" + index;
            var foundedCombatLandz = userSessionController.CombatLandzs.Find(land => land.tokenId == landz.tokenId);
            clone.Setup(userSessionController.Token, landz);
            combatLandz.Add(clone);
            index ++;
        }
    }
}
