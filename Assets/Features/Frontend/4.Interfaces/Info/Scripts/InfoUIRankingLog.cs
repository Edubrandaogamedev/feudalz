using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoUIRankingLog : MonoBehaviour
{
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [Header("Buttons")]
    [SerializeField] private Button refreshButton;
    [Header("GameObject")]
    [SerializeField] private GameObject loadingObj;
    [Header("Template")]
    [SerializeField] private RankingLogUI rankingLogTemplate;
    [Header("Contents")]
    [SerializeField] private RectTransform rankingLogContent;
    [Header("CanvasGroupController")]
    [SerializeField] private CanvasGroupController rankingCanvasController;
    [Header("Lists")]
    private List<RankingLogUI> rankingLogElements = new List<RankingLogUI>();
    private void OnEnable()
    {
        refreshButton.onClick.AddListener(RefreshRankingLog);

    }
    private void OnDisable()
    {
        refreshButton.onClick.RemoveAllListeners();
    }
    private void SetRankingLog(RankingObj[] _rankingLog)
    {
        int controlPanelColor = 0;
        for (int i = 0; i < _rankingLog.Length; i++)
        {
            if (i >= rankingLogElements.Count)
            {
                RankingLogUI clone = Instantiate(rankingLogTemplate, rankingLogContent);
                rankingLogElements.Add(clone);
                clone.SetupRankingLog(_rankingLog[i], i + 1);
                if (controlPanelColor % 2 != 0)
                    clone.SetupRankingLogPanel(Color.grey);
                controlPanelColor++;
            }
            else
            {
                rankingLogElements[i].SetupRankingLog(_rankingLog[i], i + 1);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rankingLogContent);
    }

    public async void UpdateRankingLog()
    {
        var response = await APIServices.DatabaseServer.RankingLog(userSessionController.Token, userSessionController.CurrentUserLandzBioma);
        SetRankingLog(response);
        if (loadingObj != null)
            loadingObj.SetActive(false);
        if (rankingCanvasController != null)
            rankingCanvasController.Enable();
    }
    private void RefreshRankingLog()
    {
        rankingCanvasController.Disable();
        loadingObj.SetActive(true);
        UpdateRankingLog();
    }
}
