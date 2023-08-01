using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldMapPopupController : PoupController
{
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController popupPanel;
    [SerializeField] private CanvasGroupController goldzPanel;
    [SerializeField] private CanvasGroupController resourcePanel;
    [SerializeField] private CanvasGroupController errorPopupPanel;
    [SerializeField] private CanvasGroupController successPopupPanel;
    [SerializeField] private CanvasGroupController allBattleResultScrollViewPopup;
    [SerializeField] private CanvasGroupController resultMainPopup;
    [SerializeField] private CanvasGroupController resultWinnerObj;
    [SerializeField] private CanvasGroupController resultLoserObj;
    [Header("Button")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button auxButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button errorConfirmButton;
    [SerializeField] private Button successConfirmButton;
    [SerializeField] private Button resultConfirmButton;
    [SerializeField] private Button attackAllResultConfirmButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private TextMeshProUGUI popupGoldzCostText;
    [SerializeField] private TextMeshProUGUI popupResourcesCostText;
    [SerializeField] private TextMeshProUGUI weaponResourceCostText;
    [SerializeField] private TextMeshProUGUI armorResourceCostText;
    [SerializeField] private TextMeshProUGUI foodResourceCostText;
    [SerializeField] private TextMeshProUGUI popupErrorText;
    [SerializeField] private TextMeshProUGUI popupSuccessText;
    [SerializeField] private TextMeshProUGUI resultHeaderText;
    [SerializeField] private TextMeshProUGUI resultInfoText;
    [SerializeField] private TextMeshProUGUI resultGoldzText;
    [Header("Button Texts")]
    [SerializeField] private TextMeshProUGUI confirmButtonText;
    [SerializeField] private TextMeshProUGUI auxButtonText;
    public void EnableRechargeAllPoup(UnityAction _onConfirmationCallback, float _goldzCost)
    {
        popUpText.text = "This will recharge all allowed landz. Are you sure you want to continue?";
        popupGoldzCostText.text = "Cost: " + _goldzCost + " GOLDZ";
        popupPanel.Enable();
        goldzPanel.Enable();
        goldzPanel.gameObject.SetActive(true);
        AddConfirmationButtonListeners(_onConfirmationCallback,confirmButton);
        AddCancelButtonListeners(DisableRechargeAllPopup,cancelButton);
    }
    public void DisableRechargeAllPopup()
    {
        popupPanel.Disable();
        goldzPanel.Disable();
        goldzPanel.gameObject.SetActive(false);
        popupGoldzCostText.text = "";
        popupGoldzCostText.text = "";
        RemoveAllButtonListeners(confirmButton,cancelButton);
    }
    public void EnableFixConfirmationPoup(UnityAction<bool> _onConfirmationCallback, float _goldzCost,int[] resourceCost)
    {
        popUpText.text = "This will heal your troops. Are you sure you want to continue?";
        popupGoldzCostText.text = "Cost with GOLDZ: " + _goldzCost;
        popupResourcesCostText.text = "Cost with RESOURCES:";
        weaponResourceCostText.text = "x" + resourceCost[0];
        armorResourceCostText.text = "x" + resourceCost[1];
        foodResourceCostText.text = "x" + resourceCost[2];
        popupPanel.Enable();
        goldzPanel.Enable();
        resourcePanel.Enable();
        goldzPanel.gameObject.SetActive(true);
        resourcePanel.gameObject.SetActive(true);
        auxButton.gameObject.SetActive(true);
        confirmButtonText.text = "Pay with Goldz";
        auxButtonText.text = "Pay with Resources";
        AddConfirmationButtonListeners(() => _onConfirmationCallback(true), confirmButton);
        AddConfirmationButtonListeners(() => _onConfirmationCallback(false), auxButton);
        AddCancelButtonListeners(DisableFixConfirmationPoup,cancelButton);
    }
    public void DisableFixConfirmationPoup()
    {
        popupResourcesCostText.gameObject.SetActive(false);
        popupPanel.Disable();
        goldzPanel.Disable();
        resourcePanel.Disable();
        resourcePanel.gameObject.SetActive(false);
        auxButton.gameObject.SetActive(false);
        goldzPanel.gameObject.SetActive(false);
        popUpText.text = "";
        popupGoldzCostText.text = "";
        auxButtonText.text = "";
        confirmButtonText.text = "Proceed";
        weaponResourceCostText.text = "";
        armorResourceCostText.text = "";
        foodResourceCostText.text = "";
        RemoveAllButtonListeners(confirmButton,cancelButton, auxButton);
    }
    public void EnableAttackWithAllLandzConfirmationPopup(float _goldzCost, UnityAction<float> _onConfirmationCallback)
    {
        popUpText.text = "This will consume all the attacks. Are you sure you want to continue?";
        popupGoldzCostText.text = "Cost: " + _goldzCost + " GOLDZ";
        popupPanel.Enable();
        goldzPanel.Enable();
        goldzPanel.gameObject.SetActive(true);
        AddConfirmationButtonListeners(() => _onConfirmationCallback(_goldzCost), confirmButton);
        AddCancelButtonListeners(DisableAttackWithAllLandzConfirmationPopup, cancelButton);
    }
    public void DisableAttackWithAllLandzConfirmationPopup()
    {
        popupPanel.Disable();
        goldzPanel.Disable();
        goldzPanel.gameObject.SetActive(false);
        popupGoldzCostText.text = "";
        popupGoldzCostText.text = "";
        RemoveAllButtonListeners(confirmButton,cancelButton);
    }
    public void EnableAttackWithAllLandzResultPopup(UnityAction _onConfirmationCallback)
    {
        allBattleResultScrollViewPopup.Enable();
        allBattleResultScrollViewPopup.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
        AddConfirmationButtonListeners(_onConfirmationCallback, attackAllResultConfirmButton);
    }
    public void DisableAttackWithAllLandzResultPopup()
    {
        allBattleResultScrollViewPopup.Disable();
        RemoveAllButtonListeners(attackAllResultConfirmButton);
    }
    public void EnableResultPopup(UnityAction onConfirmationCallback, float goldzReceived, int _winCount)
    {
        if (_winCount > 0)
            resultWinnerObj.Enable();
        else 
            resultLoserObj.Enable();
        resultHeaderText.text = "RESULTS";
        resultInfoText.text = DefineResultAllAttackText(_winCount);
        resultGoldzText.text = $"RECEIVED {goldzReceived}";
        resultMainPopup.Enable();
        AddConfirmationButtonListeners(DisableResultPopup,resultConfirmButton);
        AddConfirmationButtonListeners(onConfirmationCallback,resultConfirmButton);
    }

    private void DisableResultPopup()
    {
        resultLoserObj.Disable();
        resultWinnerObj.Disable();
        resultMainPopup.Disable();
        RemoveAllButtonListeners(resultConfirmButton);
    }
    private string DefineResultAllAttackText(float _winCount)
    {
        return _winCount > 0 ? "You succeeded in some attacks and won some goldz." : "You failed in all attacks and received only few goldz.";
    }
    public void OpenSuccessPopupPanel(string _successType)
    {
        popupSuccessText.text = _successType;
        successPopupPanel.Enable();
        AddConfirmationButtonListeners(CloseSuccessPopupPanel,successConfirmButton);
    }

    private void CloseSuccessPopupPanel()
    {
        successPopupPanel.Disable();
        popupSuccessText.text = "";
        RemoveAllButtonListeners(successConfirmButton);
    }
    public void ErrorHandler(string _errorType)
    {
        switch (_errorType)
        {
            case "Generic Recharge":
                EnableErrorPopup(DisableErrorPopup,"You don't have enough goldz or there are no landz avaiable to recharge");
                break;
            case "Avaiable Recharge":
                EnableErrorPopup(DisableErrorPopup,"There are no landz avaiable to recharge");
                break;
            case "Request":
                EnableErrorPopup(DisableErrorPopup, "Too many requests at moment. Wait a while and try again!");
                break;
            case "Resources":
                EnableErrorPopup(DisableErrorPopup, "You don't have enough resources");
                break;
            case "Goldz":
                EnableErrorPopup(DisableErrorPopup, "You don't have enough goldz");
                break;
            case "Attack":
                EnableErrorPopup(DisableErrorPopup,"There was an error while processing all attacks, but some of them were counted so please refresh the game.");
                break;
        }
    }
    private void EnableErrorPopup(UnityAction _onConfirmationCallback, string errorText = "")
    {
        errorPopupPanel.Enable();
        popupErrorText.text = errorText;
        AddConfirmationButtonListeners(_onConfirmationCallback,errorConfirmButton);
    }
    private void DisableErrorPopup()
    {
        errorPopupPanel.Disable();
        popupErrorText.text = "Too many requests at moment. Wait a while and try again!";
        RemoveAllButtonListeners(errorConfirmButton);
    }
}
