using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingLogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI landzPos;
    [SerializeField] private TextMeshProUGUI landzName;
    [SerializeField] private TextMeshProUGUI landzGoldz;
    [SerializeField] private TextMeshProUGUI landzWinRate;
    [SerializeField] private Image logPanel;

    public void SetupRankingLog(RankingObj _log, int _rankingPos)
    {
        landzPos.text = _rankingPos.ToString();
        landzName.text = _log.name.ToString();
        landzGoldz.text = _log.cumulatedVictoryGold.ToString();
        landzWinRate.text = $"{((_log.attackSucceded * 100) / (_log.attackSucceded + _log.attackFailed)).ToString()}%";
    }
      public void SetupRankingLogPanel(Color _color)
    {
        logPanel.color = _color;
    }
}
