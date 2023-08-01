using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CombatLandz : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CombatLandzUI combatLandzUI;
    private readonly CombatLandzData combatLandzData = new CombatLandzData();
    private DateTimeOffset lastRechargeTime;
    private DateTimeOffset nextRechargeTime;
    public int TokenId => combatLandzData.TokenId;
    public List<FeudalzLandzBuild> AllowedBuilds => combatLandzData.AllowedBuilds;
    public List<FeudalzLandzAllocatedBuild> AllocatedBuilds => combatLandzData.Builds;
    public FeudalzHeroez hero => combatLandzData.Heroez;
    public FeudalzHeroez uniqueHero => combatLandzData.UniqueHeroez;
    public int Attacks => combatLandzData.Attacks;
    public float LandzAttackBonus => combatLandzData.AttackBonus;
    public float LandzHp => combatLandzData.LandzHp;
    public string LandzName => combatLandzData.LandzName;
    public Sprite LandzSprite => combatLandzUI.LandzImg.sprite;
    public bool LandzUnique => combatLandzData.LandzUnique;
    public string LandzUniqueImgURL => combatLandzUI.GifTextureUnique.m_GifFileNameOrUrl;
    public event UnityAction<CombatLandz> onTryAttack;
    public event UnityAction<CombatLandz, float, bool> onTryFixLandz;
    public event UnityAction<CombatLandz,string> onOpenAllocationMenu;
    public event UnityAction<CombatLandz, FeudalzHeroez> onOpenHeroInfoPopup;
    public event UnityAction<CombatLandz, FeudalzLandzAllocatedBuild> onOpenBuildInfoPopup; 

    public async void Setup(string _playerToken, FeudalzCombatLandz _combatUnit)
    {
        combatLandzData.Setup(_combatUnit,_playerToken);
        combatLandzUI.RegisterListeners(OnTryAttack, OnTryFixLandz,OnOpenAllocationMenu,OnOpenAllocationInfo);
        lastRechargeTime = DateTimeOffset.Parse(combatLandzData.TimeStamp);
#if DEVELOP
        AddTime(out nextRechargeTime, 30, TimeType.Minute);
#else
        AddTime(out nextRechargeTime,1,TimeType.Day);
#endif
        combatLandzUI.UpdateHealthBar(combatLandzData.LandzHp, combatLandzData.MaxLandzHp);
        combatLandzUI.UpdateAttackInfo(combatLandzData.Attacks, combatLandzData.MaxAttacks);
        combatLandzUI.CheckAttackButton(combatLandzData.Attacks, combatLandzData.LandzHp);
        combatLandzUI.CheckRechargeCooldown(nextRechargeTime);
        combatLandzUI.CheckFixButton(combatLandzData.LandzHp);
        combatLandzUI.CheckHeroezButton(combatLandzData.Heroez);
        combatLandzUI.CheckBuildButtons(combatLandzData.Builds);
        if (_combatUnit.nft_sprite == null || !_combatUnit.uniqueLandz)
            _combatUnit.nft_sprite = await combatLandzUI.LoadLandzImage(_combatUnit.sprite_url);
        combatLandzUI.CheckUniqueHeroezButton(combatLandzData.UniqueHeroez, _combatUnit);
        combatLandzUI.SetLandzInfo(_combatUnit.name,_combatUnit.nft_sprite, 
        _combatUnit.uniqueLandz, combatLandzUI.LoadLandzImageUniqueUrl(_combatUnit.sprite_url));
    }

    public void OnAttackSucessful(float _landzHpAfterBattle)
    {
        ReduceAttackValue();
        ReduceHeroezCharge();
        SetHpValue(_landzHpAfterBattle);
        combatLandzUI.CheckRechargeCooldown(nextRechargeTime);
        combatLandzUI.CheckHeroezButton(combatLandzData.Heroez);
    }
    public void OnFixLandzSuccessful()
    {
        combatLandzData.LandzHp = combatLandzData.MaxLandzHp;
        combatLandzUI.UpdateHealthBar(combatLandzData.LandzHp, combatLandzData.MaxLandzHp);
        combatLandzUI.CheckAttackButton(combatLandzData.Attacks, combatLandzData.LandzHp);
        combatLandzUI.ReturnInfoAfterFix();
    }
    public void OnHeroAllocationSuccessful(FeudalzHeroez _heroez)
    {
        combatLandzData.Heroez = _heroez;
        combatLandzUI.CheckHeroezButton(combatLandzData.Heroez);
    }
    public void OnHeroRemoveSuccessful()
    {
        combatLandzData.Heroez = null;
        combatLandzUI.CheckHeroezButton(combatLandzData.Heroez);
    }
    public void OnBuildAllocationSuccessful(FeudalzLandzBuild _build,bool _autoRebuild = false)
    {
        combatLandzData.Builds.Add(new FeudalzLandzAllocatedBuild(_build,_autoRebuild));
        combatLandzUI.CheckBuildButtons(combatLandzData.Builds);
    }
    public void OnBuildRemoveSuccessful(FeudalzLandzAllocatedBuild _build)
    {
        combatLandzData.Builds.Remove(_build);
        combatLandzUI.CheckBuildButtons(combatLandzData.Builds);
    }
    private void OnTryAttack()
    {
        onTryAttack?.Invoke(this);
    }
    private void OnTryFixLandz()
    {
        onTryFixLandz?.Invoke(this, combatLandzData.RepairCost, combatLandzData.goldz);
    }
    private void OnOpenAllocationMenu(string _allocateType)
    {
        onOpenAllocationMenu?.Invoke(this,_allocateType);
    }
    private void OnOpenAllocationInfo(string _allocateType)
    {
        if (_allocateType.Contains("Build"))
        {
            var feudalzLandzAllocatedBuild = combatLandzData.Builds.Find(build => _allocateType.ToLower().Contains(build.position));
            if (feudalzLandzAllocatedBuild != null)
                onOpenBuildInfoPopup?.Invoke(this,feudalzLandzAllocatedBuild);
        }
        else if (_allocateType.Contains("Hero"))
        {
            if (combatLandzData.Heroez != null)
                onOpenHeroInfoPopup?.Invoke(this,combatLandzData.Heroez);
        }
        else if (_allocateType.Contains("Unique"))
        {
            if (combatLandzData.UniqueHeroez != null)
                onOpenHeroInfoPopup?.Invoke(this,combatLandzData.UniqueHeroez);
        }
    }
    public void OnRechargeSuccessful(DateTimeOffset _timeStamp)
    {
        lastRechargeTime = _timeStamp;
#if DEVELOP
        AddTime(out nextRechargeTime, 30, TimeType.Minute);
#else
        AddTime(out nextRechargeTime,1,TimeType.Day);
#endif
        combatLandzData.Attacks = combatLandzData.MaxAttacks;
        combatLandzUI.UpdateAttackInfo(combatLandzData.Attacks, combatLandzData.MaxAttacks);
        combatLandzUI.CheckAttackButton(combatLandzData.Attacks, combatLandzData.LandzHp);
        combatLandzUI.CheckRechargeCooldown(nextRechargeTime);
    }
    private void ReduceAttackValue()
    {
        combatLandzData.Attacks = Mathf.Max(0,combatLandzData.Attacks-1);
        combatLandzUI.UpdateAttackInfo(combatLandzData.Attacks, combatLandzData.MaxAttacks);
        combatLandzUI.CheckAttackButton(combatLandzData.Attacks, combatLandzData.LandzHp);
    }
    public void IncreaseAttackValue()
    {
        combatLandzData.Attacks++;
        combatLandzUI.UpdateAttackInfo(combatLandzData.Attacks, combatLandzData.MaxAttacks);
        combatLandzUI.CheckAttackButton(combatLandzData.Attacks, combatLandzData.LandzHp);
    }
    private void ReduceHeroezCharge()
    {
        if (combatLandzData.Heroez == null) return;
        combatLandzData.Heroez.heroCharges --;
    }
    public void SetHpValue(float _currentLandzHp)
    {
        combatLandzData.LandzHp = _currentLandzHp;
        combatLandzUI.UpdateHealthBar(combatLandzData.LandzHp, combatLandzData.MaxLandzHp);
        combatLandzUI.CheckFixButton(combatLandzData.LandzHp);
    }
    private void AddTime(out DateTimeOffset _nextRechargeTime, int _timeToAdd, TimeType _timeType = TimeType.Day)
    {
        _nextRechargeTime = _timeType switch
        {
            TimeType.Day => lastRechargeTime.AddDays(_timeToAdd),
            TimeType.Hour => lastRechargeTime.AddHours(_timeToAdd),
            TimeType.Minute => lastRechargeTime.AddMinutes(_timeToAdd),
            _ => lastRechargeTime.AddSeconds(_timeToAdd)
        };
    }
    private void Update()
    {
        if (combatLandzUI.InitializeTimer)
        {
            combatLandzUI.UpdateRechargeCooldown(nextRechargeTime);
        }
    }
    private enum TimeType { Day, Hour, Minute, Second }
}
