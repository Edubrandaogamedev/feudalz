using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class InfoUIBattleLog : MonoBehaviour
{
    [Header("Template")]
    [SerializeField] private BattleResultUI battleLogTemplate;
    [Header("Contents")]
    [SerializeField] private  RectTransform attackLogContent;
    [SerializeField] private  RectTransform defenseLogContent;
    [Header("Lists")]
    private readonly List<BattleResultUI> attackLogElements = new List<BattleResultUI>();
    private readonly List<BattleResultUI> defenseLogElements = new List<BattleResultUI>();
    private void UpdateDefenseLog(CombatInfo[] _combatLog, string _playerAddress)
    {   
        for (int i = 0; i < _combatLog.Length; i++)
        {
            bool isPlayerWinner = _combatLog[i].winner.Contains(_playerAddress);
            if (i >= defenseLogElements.Count)
            {
                BattleResultUI clone = Instantiate(battleLogTemplate,defenseLogContent);
                defenseLogElements.Add(clone);
                clone.SetupDefenseLog(_combatLog[i],isPlayerWinner);
            }
            else
                defenseLogElements[i].SetupDefenseLog(_combatLog[i],isPlayerWinner);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(defenseLogContent);
    }
    private void UpdateAttackLog(CombatInfo[] _combatLog,string _playerAddress)
    {
        for (int i = 0; i < _combatLog.Length; i++)
        {
            bool isPlayerWinner = _combatLog[i].winner.Contains(_playerAddress);
            if (i >= attackLogElements.Count)
            {
                BattleResultUI clone = Instantiate(battleLogTemplate,attackLogContent);
                attackLogElements.Add(clone);
                clone.SetupAttackLog(_combatLog[i],isPlayerWinner);
            }
            else
            {
                attackLogElements[i].SetupAttackLog(_combatLog[i],isPlayerWinner);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(attackLogContent);
    }
    public async void UpdateCombatLog(string _playerAddress, string _playerToken,string _biome)
    {
        var response = await APIServices.DatabaseServer.BattleLog(_playerToken,_biome);
        UpdateAttackLog(response.filteredAttackHistory,_playerAddress);
        UpdateDefenseLog(response.filteredDefenseHistory,_playerAddress);
    }
}
