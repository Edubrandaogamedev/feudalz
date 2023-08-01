using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegionPopupController : PoupController
{
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController popupPanel;
    [SerializeField] private CanvasGroupController goldzPanel;
    [SerializeField] private CanvasGroupController resourcePanel;
    [SerializeField] private CanvasGroupController errorPopupPanel;
    [SerializeField] private CanvasGroupController lockInputPanel;
    [Header("Button")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button auxButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button errorConfirmButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private TextMeshProUGUI popupGoldzCostText;
    [SerializeField] private TextMeshProUGUI weaponResourceCostText;
    [SerializeField] private TextMeshProUGUI armorResourceCostText;
    [SerializeField] private TextMeshProUGUI foodResourceCostText;
    [SerializeField] private TextMeshProUGUI popupResourcesCostText;
    [SerializeField] private TextMeshProUGUI popupErrorText;
    [Header("Button Texts")]
    [SerializeField] private TextMeshProUGUI confirmButtonText;
    [SerializeField] private TextMeshProUGUI auxButtonText;

    public void EnableFixConfirmationPoup(UnityAction<bool> _onConfirmationCallback, float _goldzCost, UnityAction _onCancelCallback, bool goldz, int[] resourceCost)
    {
        popUpText.text = "This will heal your troops. Are you sure you want to continue?";
        popupGoldzCostText.text = "Cost with GOLDZ: " + _goldzCost;
        popupResourcesCostText.text = "Cost with RESOURCES: ";
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
        AddCancelButtonListeners(_onCancelCallback, cancelButton);
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
        RemoveAllButtonListeners(confirmButton, cancelButton, auxButton);
    }
    public void EnableLockPanel()
    {
        lockInputPanel.Enable();
    }
    public void DisableLockPanel()
    {
        lockInputPanel.Disable();
    }
    public void EnableRechargeAllPoup(UnityAction _onConfirmationCallback, UnityAction _onCancelCallback, float _goldzCost)
    {
        popUpText.text = "This will recharge all allowed landz from this biome. Are you sure you want to continue?";
        popupGoldzCostText.text = "Cost: " + _goldzCost + " GOLDZ";
        popupPanel.Enable();
        goldzPanel.Enable();
        goldzPanel.gameObject.SetActive(true);
        AddConfirmationButtonListeners(_onConfirmationCallback, confirmButton);
        AddCancelButtonListeners(_onCancelCallback, cancelButton);
    }
    public void DisableRechargeAllPopup()
    {
        popupPanel.Disable();
        goldzPanel.Disable();
        goldzPanel.gameObject.SetActive(false);
        popupGoldzCostText.text = "";
        popupGoldzCostText.text = "";
        RemoveAllButtonListeners(confirmButton, cancelButton);
    }
    public void ErrorHandler(string _errorType)
    {
        switch (_errorType)
        {
            case "Generic Recharge":
                EnableErrorPopup(DisableErrorPopup, "You don't have enough goldz or there are no landz avaiable to recharge");
                break;
            case "Avaiable Recharge":
                EnableErrorPopup(DisableErrorPopup, "There are no landz avaiable to recharge");
                break;
            case "Request":
                EnableErrorPopup(DisableErrorPopup, "Too many requests at moment. Wait a while and try again!");
                break;
            case "Goldz":
                EnableErrorPopup(DisableErrorPopup, "You don't have enough goldz");
                break;
            case "Resource":
                EnableErrorPopup(DisableErrorPopup, "You don't have enough resources");
                break;
        }
    }
    private void EnableErrorPopup(UnityAction _onConfirmationCallback, string errorText = "")
    {
        errorPopupPanel.Enable();
        popupErrorText.text = errorText;
        AddConfirmationButtonListeners(_onConfirmationCallback, errorConfirmButton);
    }
    private void DisableErrorPopup()
    {
        errorPopupPanel.Disable();
        popupErrorText.text = "Too many requests at moment. Wait a while and try again!";
        RemoveAllButtonListeners(errorConfirmButton);
    }
}
