using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AllocationPopupController : PoupController
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
    public void EnableBuildingAllocationPopup(UnityAction _onConfirmationCallback)
    {
        popUpText.text = "Are you sure you want to buy this build?";
        popupPanel.Enable();
        AddConfirmationButtonListeners(_onConfirmationCallback,confirmButton);
        AddCancelButtonListeners(DisableAllocationPopup,cancelButton);
    }
    public void EnableHeroAllocationPopup(UnityAction _onConfirmationCallback)
    {
        popUpText.text = "Are you sure you want to allocate this hero?";
        popupPanel.Enable();
        AddConfirmationButtonListeners(_onConfirmationCallback,confirmButton);
        AddCancelButtonListeners(DisableAllocationPopup,cancelButton);
    }
    public void ErrorHandler(string _errorType)
    {
        if (_errorType == "Build")
            EnableErrorPopup(DisableErrorPopup,"You already have a build in this position");
        else if (_errorType == "Goldz")
            EnableErrorPopup(DisableErrorPopup,"You do not have enough goldz to build");
        else if (_errorType == "Resources")
            EnableErrorPopup(DisableErrorPopup,"You do not have enough resources to build");
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
    public void DisableAllocationPopup()
    {
        popupPanel.Disable();
        popUpText.text = "";
        RemoveAllButtonListeners(confirmButton,cancelButton);
    }
}
