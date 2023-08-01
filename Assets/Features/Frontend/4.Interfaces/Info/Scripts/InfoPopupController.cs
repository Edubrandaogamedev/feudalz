using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoPopupController : PoupController
{
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController popupPanel;
    [SerializeField] private CanvasGroupController errorPopupPanel;
    [SerializeField] private CanvasGroupController goldzPanel;
    [Header("Button")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button errorConfirmButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private TextMeshProUGUI popupErrorText;
    
    public void EnableCancelProductionPopup(UnityAction _onConfirmationCallback)
    {
        popUpText.text = "You will lose all items from this production.Are you sure you want to continue? ";
        goldzPanel.Enable();
        popupPanel.Enable();
        AddConfirmationButtonListeners(_onConfirmationCallback,confirmButton);
        AddCancelButtonListeners(DisablePopup,cancelButton);
    }
    public void DisablePopup()
    {
        popupPanel.Disable();
        popUpText.text = "";
        RemoveAllButtonListeners(confirmButton,cancelButton);
    }
    public void ErrorHandler(string _errorType)
    {
        if (_errorType.Contains("consumables"))
            EnableErrorPopup(DisableErrorPopup,"You do not have enough resources to start this production");
        else
            EnableErrorPopup(DisableErrorPopup,"An error has occurred try again later");
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
        popupErrorText.text = "";
        RemoveAllButtonListeners(errorConfirmButton);
    }
    
}
