using System;
using System.Collections;
using System.Collections.Generic;
using Features.Refactor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private UserService _userService;
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [SerializeField] private HeroezResponseFilter heroezResponseFilter;
    [Header("References")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private BattleUI battleUI;
    [Header("Internal")]
    private List<CombatLandz> combatLandzs = new List<CombatLandz>();
    public UnityEvent onSingleAttackEnded = new UnityEvent();
    public UnityEvent onAttackAllEnded = new UnityEvent();
    private void OnEnable()
    {
        battleController.onSingleAttackResultCalculated += OnSingleAttackEnded;
        battleController.onAttackAllResultCalculated += OnAttackWithAllLandzEnded;
        battleController.onError += OnAttackError;
    }
    private void OnDisable()
    {
        RemoveCombatLandzListeners();
        battleController.onSingleAttackResultCalculated -= OnSingleAttackEnded;
        battleController.onError -= OnAttackError;
        battleController.onAttackAllResultCalculated -= OnAttackWithAllLandzEnded;
    }
    public void SetCombatLandz(List<CombatLandz> _combatLandz)
    {
        combatLandzs = _combatLandz;
        AddCombatLandzListeners();
    }
    public void RegisterTryAttackWithAllLandzListener(UnityEvent _event, float _goldzCost)
    {
        _event.AddListener(() => OnTryAttackWithAllLandz(_goldzCost));
    }
    public void RemoveTryAttackWithAllLandzListener(UnityEvent _event)
    {
        _event.RemoveAllListeners();
    }
    private void AddCombatLandzListeners()
    {
        foreach (CombatLandz landz in combatLandzs)
        {
            landz.onTryAttack += OnTrySingleAttack;
        }
    }
    private void RemoveCombatLandzListeners()
    {
        foreach (CombatLandz landz in combatLandzs)
        {
            landz.onTryAttack -= OnTrySingleAttack;
        }
    }
    private void OnTryAttackWithAllLandz(float _goldzCost)
    {
        float totalCost = 0;
        foreach (CombatLandz landz in combatLandzs)
        {
            if (landz.Attacks > 0 && landz.LandzHp > 0)
                totalCost += landz.Attacks * _goldzCost;
        }
        if (SkipPopup.GetSettings())
            OnAttackWithAllLandzConfirmation(totalCost);
        else
            battleUI.PopupController.EnableAttackWithAllLandzConfirmationPopup(totalCost, OnAttackWithAllLandzConfirmation, OnAttackWithAllLandzCanceled);
    }
    private async void OnAttackWithAllLandzConfirmation(float _goldzCost)
    {
        battleUI.PopupController.DisableAttackWithAllLandzConfirmationPopup();
        battleUI.OpenAllBattleUI();
        await battleController.AttackWithAllLands(userSessionController.Token, userSessionController.CurrentUserLandzBioma, _goldzCost);
    }
    private void OnAttackWithAllLandzCanceled()
    {
        battleUI.PopupController.DisableAttackWithAllLandzConfirmationPopup();
    }
    private void OnAttackWithAllLandzEnded(AttackAll _attackAllResult, float _goldzCost)
    {
        battleUI.OpenAttackAllResultScreen(_attackAllResult, userSessionController);
        foreach (AttackResult result in _attackAllResult.results)
        {
            userSessionController.ReduceAttackFromLandz(result.attacker.land.id);
            userSessionController.SetLandzHp(result.attacker.land.id, result.attacker.land.health);
            var foundLandz = combatLandzs.Find(landz => landz.TokenId == result.attacker.land.id);
            foundLandz.OnAttackSucessful(result.attacker.land.health);
            if (result.attacker.hero != null && result.attacker.hero.bonus)
                heroezResponseFilter.OnHeroSkillActivated(userSessionController, result.attacker.hero.name, foundLandz);
        }
        userSessionController.DataController.UpdateTokenBalance(-_goldzCost);
        onAttackAllEnded?.Invoke();
        userSessionController.DataController.UpdateTokenBalance(_attackAllResult.goldz);
    }
    private void OnTrySingleAttack(CombatLandz _landz)
    {
        if (SkipPopup.GetSettings())
            OnSingleAttackConfirmation(_landz);
        else
            battleUI.PopupController.EnableLandzAttackConfirmationPopup(OnSingleAttackConfirmation, _landz, OnSingleAttackCanceled);
    }
    private async void OnSingleAttackConfirmation(CombatLandz _landz)
    {
        battleUI.PopupController.DisableLandzAttackConfirmationPopup();
        battleUI.OpenSingleBattleInterface();
        await battleController.AttackLandz(_landz, userSessionController.Token);
        _userService.ReloadUserInformation();
    }
    private async void OnSingleAttackEnded(CombatLandz _landz, AttackResult _result)
    {
        await battleUI.SetBattleResult(_landz, _result, userSessionController);
        await battleUI.PlayCombatAnimation(_result.winner == _result.attacker.address, _result.attacker.hero.bonus, _landz);
        battleUI.CloseSingleBattleInterface();
        battleUI.OpenResultPopup(_winCount: 0, _result.attacker.goldz, _isSingularAttack: true, _result, _attackAllResult: null);
        userSessionController.ReduceAttackFromLandz(_result.attacker.land.id);
        userSessionController.SetLandzHp(_result.attacker.land.id, _result.attacker.land.health);
        _landz.OnAttackSucessful(_result.attacker.land.health);
        if (_result.attacker.hero != null && _result.attacker.hero.bonus)
            heroezResponseFilter.OnHeroSkillActivated(userSessionController, _result.attacker.hero.name, _landz);
        onSingleAttackEnded?.Invoke();
        userSessionController.DataController.UpdateTokenBalance(_result.attacker.goldz);
    }
    private void OnSingleAttackCanceled()
    {
        battleUI.PopupController.DisableLandzAttackConfirmationPopup();
    }
    private void OnAttackError()
    {
        battleUI.OpenErrorPopup();
        AudioManager.instance.Stop("battle_bgm");
        AudioManager.instance.Play("worldmap_bgm");
    }
}
