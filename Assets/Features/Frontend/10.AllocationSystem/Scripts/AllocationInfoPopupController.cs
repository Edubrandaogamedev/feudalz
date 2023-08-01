using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AllocationInfoPopupController : PoupController
{
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController popupPanel;
    [SerializeField] private CanvasGroupController errorPopupPanel;
    [Header("Button")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button errorConfirmButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private TextMeshProUGUI popupErrorText;
    
    public void EnableHeroRemovePopup(UnityAction _onConfirmationCallback)
    {
        popUpText.text = "This hero will be removed and destroyed. Are you sure you want to continue?";
        popupPanel.Enable();
        AddConfirmationButtonListeners(_onConfirmationCallback,confirmButton);
        AddCancelButtonListeners(DisablePopup,cancelButton);
    }
    public void EnableBuildRemovePopup(UnityAction _onConfirmationCallback)
    {
        popUpText.text = "This build will be removed and destroyed. Are you sure you want to continue?";
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
        // if (_errorType == "Build")
        //     EnableErrorPopup(DisableErrorPopup,"You already have a build in this position");
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
