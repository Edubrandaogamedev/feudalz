using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerDiceResult;
    [SerializeField] private TextMeshProUGUI enemyDiceResult;
    [SerializeField] private TextMeshProUGUI playerBonus;
    [SerializeField] private TextMeshProUGUI enemyBonus;
    [SerializeField] private TextMeshProUGUI playerTotalResult;
    [SerializeField] private TextMeshProUGUI enemyTotalResult;
    [SerializeField] private TextMeshProUGUI playerLandId;
    [SerializeField] private TextMeshProUGUI enemyLandId;
    [SerializeField] private TextMeshProUGUI goldzEarned;
    [SerializeField] private Image resultScreen;
    [SerializeField] private TextMeshProUGUI resultStatus;
    [SerializeField] private Sprite victoryScreen;
    [SerializeField] private Sprite loseScreen;
    [SerializeField] private Image goldzImage;
    public void SetupAttackLog(CombatInfo _log, bool _userWon)
    {
        playerDiceResult.text = "Dice Result\n" + _log.attackerDiceResult.ToString();
        enemyDiceResult.text = "Dice Result\n" + _log.defenderDiceResult.ToString();
        playerBonus.text = "Attack Bonus\n" + _log.attackerBonus.ToString();
        enemyBonus.text = "Defense Bonus\n" + _log.defenderBonus.ToString();
        playerTotalResult.text = "Attack Total\n" + (_log.attackerBonus + _log.attackerDiceResult).ToString();
        enemyTotalResult.text = "Defense Total\n" + (_log.defenderBonus + _log.defenderDiceResult).ToString();
        if (_log.playerLandName == null)
            playerLandId.text = "Your Land\n" + _log.attackerLandTokenId;
        else
            playerLandId.text = "Your Land\n" + _log.playerLandName;
        if (_log.enemyLandName == null)
            enemyLandId.text = "Enemy Land\n" + _log.defenderLandTokenId;
        else
            enemyLandId.text = "Enemy Land\n" + _log.enemyLandName;
        goldzEarned.text = "Goldz earned: " + _log.coins.ToString();
        if (_userWon)
        {
            resultScreen.sprite = victoryScreen;
            resultStatus.text = "Victory!";
        }
        else
        {
            resultScreen.sprite = loseScreen;
            resultStatus.text = "Defeat!";
        }
    }
    public void SetupDefenseLog(CombatInfo _log, bool _userWon)
    {
        if (_log.playerLandName == null)
            playerLandId.text = "Your Land\n" + _log.attackerLandTokenId;
        else
            playerLandId.text = "Your Land\n" + _log.playerLandName;
        if (_log.enemyLandName == null)
            enemyLandId.text = "Enemy Land\n" + _log.attackerLandTokenId;
        else
            enemyLandId.text = "Enemy Land\n" + _log.enemyLandName;
        playerBonus.text = "Defense Bonus\n" + _log.defenderBonus.ToString();
        enemyBonus.text = "Attack Bonus\n" + _log.attackerBonus.ToString();
        playerDiceResult.text = "Dice Result\n" + _log.defenderDiceResult.ToString();
        enemyDiceResult.text = "Dice Result\n" + _log.attackerDiceResult.ToString();
        playerTotalResult.text = "Defense Total\n" + (_log.defenderBonus + _log.defenderDiceResult).ToString();
        enemyTotalResult.text = "Attack Total\n" + (_log.attackerBonus + _log.attackerDiceResult).ToString();
        goldzEarned.text = "";
        goldzImage.gameObject.SetActive(false);
        if (_userWon)
        {
            resultScreen.sprite = victoryScreen;
            resultStatus.text = "Victory!";
        }
        else
        {
            resultScreen.sprite = loseScreen;
            resultStatus.text = "Defeat!";
        }
    }
}
