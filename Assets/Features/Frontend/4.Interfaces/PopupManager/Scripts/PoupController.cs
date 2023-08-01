using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PoupController : MonoBehaviour
{
    protected void AddConfirmationButtonListeners(UnityAction _confirmationCallback,params Button[] _buttons)
    {
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(_confirmationCallback);
        }
    }
    protected void AddCancelButtonListeners(UnityAction _cancelCallback,params Button[] _buttons)
    {
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(_cancelCallback);
        }
    }
    public void RemoveAllButtonListeners(params Button[] _buttons)
    {
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
