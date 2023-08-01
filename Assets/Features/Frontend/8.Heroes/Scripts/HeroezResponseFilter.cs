using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroezResponseFilter : MonoBehaviour
{
    public void OnHeroSkillActivated(UserSessionController userSessionController, string _heroezType, CombatLandz _combatLandz)
    {
        switch (_heroezType)
        {
            case "Guttx":
            userSessionController.IncreaseAttackFromLandz(_combatLandz.TokenId);
            _combatLandz.IncreaseAttackValue();
            break;
        }
    }

    public void OnHeroSkillActivated(UserSessionController userSessionController, string _heroezType, int landId)
    {
        switch (_heroezType)
        {
            case "Guttx":
                userSessionController.IncreaseAttackFromLandz(landId);
                break;
        }
    }
}
