using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class RegionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InfoUIManager infoManager;
    public InfoUIManager InfoManager => infoManager;
    [SerializeField] private RegionLandzPanel regionLandzPanel;
    public RegionLandzPanel LandzPanel => regionLandzPanel;
    [SerializeField] private RegionBiomeInfo regionBiomeInfo;
    public RegionBiomeInfo BiomeInfo => regionBiomeInfo;
    [SerializeField] private RegionEffects regionFeedbacks;
    public RegionEffects Effects => regionFeedbacks;
    [SerializeField] private RegionPopupController regionPopupController;
    public RegionPopupController PopupController => regionPopupController;

    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController pnlNoLandzWarning;
    [SerializeField] private CanvasGroupController loadingPanel;
    [Header("Buttons")]
    [SerializeField] private Button btnReturnToWorldMap;
    [SerializeField] private Button btnAttackAll;
    [SerializeField] private Button btnRechargeAll;
    public Button BtnAttackAll => btnAttackAll;
    public Button BtnRechargeAll => btnRechargeAll;
    public Button BtnReturnToWorldMap => btnReturnToWorldMap;

    private void OnDisable()
    {
        btnReturnToWorldMap.onClick.RemoveAllListeners();
    }
    public void ShowNoLandzWarning()
    {
        pnlNoLandzWarning.EnableImage();
    }
    public void ShowLoadingScreen()
    {
        loadingPanel.Enable();
    }
    public void UpdateUserInfo(UserSessionController userSessionController)
    {
        infoManager.SetUserInfo(userSessionController);
        infoManager.UpdateUserRanking();
        infoManager.UpdateCombatLog(userSessionController.UserMaticAddress,userSessionController.Token,userSessionController.CurrentUserLandzBioma);
        regionBiomeInfo.SetAllAttackInfo(userSessionController);
    }

    public void UpdateUserLandzBuilds(UserSessionController userSessionController, List<CombatLandz> targetLandzs)
    {
        foreach (var land in targetLandzs)
        {
            var foundLand = userSessionController.DataController.GetAllLandz().Find(searchingLand => searchingLand.tokenId == land.TokenId);
            land.Setup(userSessionController.Token,foundLand);
        }
    }

    public void UpdateUserHeroes(UserSessionController userSessionController)
    {
        infoManager.UpdateUserHeroes(userSessionController);
    }
    public void CheckAttackAllButton(UserSessionController userSessionController, float goldzCost,List<CombatLandz> landzs)
    {
        float totalCost = 0;
        totalCost = landzs.Where(landz => landz.Attacks > 0 && landz.LandzHp > 0).Sum(landz => landz.Attacks * goldzCost);
        if (userSessionController.CurrentBiomeAttacksAvailable <= 0 || userSessionController.DataController.TokenBalance < totalCost || totalCost == 0)
            BtnAttackAll.interactable = false;
        else
            BtnAttackAll.interactable = true;
    }
    public void ChangeInputLockState(bool _isToLock)
    {
        if (_isToLock)
            regionPopupController.EnableLockPanel();
        else
            regionPopupController.DisableLockPanel();
    }
}
