using TMPro;
using UnityEngine;

public class RegionBiomeInfo : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI biomeAttackInfo;
    [SerializeField] private TextMeshProUGUI worldAttackInfo;
    public void SetBiomeAttackInfo(int _remainingAttacks, int _maxAttacks)
    {
        biomeAttackInfo.text = "Biome Attacks\n" + _remainingAttacks + "/" + _maxAttacks;
    }
    public void SetWorldAttackInfo(int _remainingAttacks, int _maxAttacks)
    {
        worldAttackInfo.text = "World Attacks\n" + _remainingAttacks + "/" + _maxAttacks;
    }
    public void SetAllAttackInfo(UserSessionController userSessionController)
    {
        biomeAttackInfo.text = "Biome Attacks\n" + userSessionController.CurrentBiomeAttacksAvailable + "/" + userSessionController.CurrentBiomeTotalAttacks;
        worldAttackInfo.text = "World Attacks\n" + userSessionController.TotalAttacksAvailable + "/" + userSessionController.TotalAttacks;
    }
}
