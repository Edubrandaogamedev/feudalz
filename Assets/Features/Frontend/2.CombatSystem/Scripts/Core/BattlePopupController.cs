using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePopupController : PoupController
{
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController landzAttackConfirmationPopup;
    [SerializeField] private CanvasGroupController allBattleResultScrollViewPopup;
    [SerializeField] private CanvasGroupController attackErrorPopup;
    [SerializeField] private CanvasGroupController goldzCostObj;
    [SerializeField] private CanvasGroupController resultMainPopup;
    [SerializeField] private CanvasGroupController resultWinnerObj;
    [SerializeField] private CanvasGroupController resultLoserObj;
    [Header("Button")]
    [SerializeField] private Button attackConfirmButton;
    [SerializeField] private Button attackCancelButton;
    [SerializeField] private Button resultConfirmButton;
    [SerializeField] private Button attackAllResultConfirmButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resultHeaderText;
    [SerializeField] private TextMeshProUGUI resultInfoText;
    [SerializeField] private TextMeshProUGUI resultGoldzText;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private TextMeshProUGUI goldzCostText;
    public void EnableLandzAttackConfirmationPopup(UnityAction<CombatLandz> _onConfirmationCallback, CombatLandz _landz, UnityAction _onCancelCallback)
    {
        landzAttackConfirmationPopup.Enable();
        popUpText.text = "Are you sure you want to select this landz to raid a enemy?";
        AddConfirmationButtonListeners(() => _onConfirmationCallback(_landz), attackConfirmButton);
        AddCancelButtonListeners(_onCancelCallback, attackCancelButton);
    }
    public void DisableLandzAttackConfirmationPopup()
    {
        landzAttackConfirmationPopup.Disable();
        popUpText.text = "";
        RemoveAllButtonListeners(attackConfirmButton, attackCancelButton);
    }
    public void EnableAttackWithAllLandzConfirmationPopup(float _goldzCost, UnityAction<float> _onConfirmationCallback, UnityAction _onCancelCallback)
    {
        popUpText.text = "This will consume all the attacks from this biome. Are you sure you want to continue?";
        goldzCostText.text = "Cost: " + _goldzCost + " GOLDZ";
        landzAttackConfirmationPopup.Enable();
        goldzCostObj.Enable();
        AddConfirmationButtonListeners(() => _onConfirmationCallback(_goldzCost), attackConfirmButton);
        AddCancelButtonListeners(_onCancelCallback, attackCancelButton);
    }
    public void DisableAttackWithAllLandzConfirmationPopup()
    {
        landzAttackConfirmationPopup.Disable();
        goldzCostObj.Disable();
        popUpText.text = "";
        goldzCostText.text = "";
        RemoveAllButtonListeners(attackConfirmButton, attackCancelButton);
    }
    public void EnableAttackErrorPopup()
    {
        attackErrorPopup.Enable();
    }
    public void DisableAttackErrorPopup()
    {
        attackErrorPopup.Disable();
    }
    public void EnableAttackWithAllLandzResultPopup(UnityAction _onConfirmationCallback, UnityAction _confirmationSecondCallback)
    {
        allBattleResultScrollViewPopup.Enable();
        allBattleResultScrollViewPopup.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
        AddConfirmationButtonListeners(_onConfirmationCallback, attackAllResultConfirmButton);
        AddConfirmationButtonListeners(_confirmationSecondCallback, attackAllResultConfirmButton);
    }
    public void DisableAttackWithAllLandzResultPopup()
    {
        allBattleResultScrollViewPopup.Disable();
        RemoveAllButtonListeners(attackAllResultConfirmButton);
    }
    public void EnablePopup(float goldzReceived, UnityAction<float> _onConfirmationCallback, bool _isSingularAttack, int _winCount, AttackResult _attackResult, AttackResult[] _attackAllResult)
    {
        if (_isSingularAttack)
        {
            resultInfoText.text = DefineResultSingularText(goldzReceived);
            if (goldzReceived >= 3)
            {
                resultHeaderText.text = "VICTORY";
                resultWinnerObj.Enable();
            }
            else
            {
                resultHeaderText.text = "LOSE";
                resultLoserObj.Enable();
            }
            if (_attackResult.attacker.land.unique != null)
            {
                resultGoldzText.text = _attackResult.attacker.land.unique.Contains("Kreaken") ? $"RECEIVED {goldzReceived - _attackResult.attacker.land.goldzBonus}\nBONUS {_attackResult.attacker.land.goldzBonus}" : $"RECEIVED {goldzReceived}";
            }
            else
            {
                resultGoldzText.text = $"RECEIVED {goldzReceived}";
            }
        }
        else
        {
            if (_winCount > 0)
                resultWinnerObj.Enable();
            else
                resultLoserObj.Enable();
            resultHeaderText.text = "RESULTS";
            resultInfoText.text = DefineResultAllAttackText(_winCount);
            resultGoldzText.text = $"RECEIVED {goldzReceived}";
        }

        resultMainPopup.Enable();
        AddConfirmationButtonListeners(() => _onConfirmationCallback(goldzReceived), resultConfirmButton);
    }

    public void DisableResultPopup()
    {
        resultLoserObj.Disable();
        resultWinnerObj.Disable();
        resultMainPopup.Disable();
        RemoveAllButtonListeners(resultConfirmButton);
    }
    private string DefineResultSingularText(float _goldzWon)
    {
        if (_goldzWon >= 7)
            return "Your attack was extremely effective and you did a massive damage to opponent landz.";
        else if (_goldzWon < 7 && _goldzWon >= 5)
            return "Your attack was effective and you did great damage to the opponent landz.";
        else if (_goldzWon < 5 && _goldzWon >= 3)
            return "Your attack was succesful and you damaged opponent landz";
        else if (_goldzWon < 3 && _goldzWon >= 1.5f)
            return "Your attack was not so effective and you did minor damage to opponent landz.";
        else
            return "You failed to attack and didn't do any damage to opponent landz. One of your orcz found a coin in the ground.";
    }
    private string DefineResultAllAttackText(float _winCount)
    {
        if (_winCount > 0)
            return "You succeeded in some attacks and won some goldz.";
        else
            return "You failed in all attacks and received only few goldz.";
    }
}
