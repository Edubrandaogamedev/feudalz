using System;
using Features.Refactor;
using UnityEngine;
using TMPro;
public class InfoUIUser : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI userTokenQuantity;
    [SerializeField] private TextMeshProUGUI feudalzUnits;
    [SerializeField] private TextMeshProUGUI orczUnits;
    [SerializeField] private TextMeshProUGUI elvezUnits;
    [SerializeField] private TextMeshProUGUI animalzUnits;
    [SerializeField] private TextMeshProUGUI feudalzBonus;
    [SerializeField] private TextMeshProUGUI timeToBonusReset;
    private DateTimeOffset lastBonusReset;
    private DateTimeOffset nextBonusReset;
    private int bonusMaxCharge;
    public void UpdateUserInfo(UserSessionController userSessionController)
    {
        userTokenQuantity.text = userSessionController.DataController.TokenBalance.ToString();
        feudalzBonus.text = "Feudalz Bonus:\n" + userSessionController.DataController.BonusChargeLeft + "/" + userSessionController.DataController.BonusMaxCharge;
        feudalzUnits.text = "Humanz: " + (userSessionController.DataController.GetAllUnitsByType(UnitType.Feudalz).Count);
        orczUnits.text = "Orcz: " + userSessionController.DataController.GetAllUnitsByType(UnitType.Orcz).Count;
        elvezUnits.text = "Elvez: " + userSessionController.DataController.GetAllUnitsByType(UnitType.Elvez).Count;
        animalzUnits.text = "Animalz: " + userSessionController.DataController.GetAllUnitsByType(UnitType.Animalz).Count;
        bonusMaxCharge = userSessionController.DataController.BonusMaxCharge;
        lastBonusReset = DateTimeOffset.Parse(userSessionController.DataController.LastBonusReset);
#if DEVELOP
        AddTime(lastBonusReset, out nextBonusReset, 5, TimeType.Minute);
#else
        AddTime(lastBonusReset, out nextBonusReset,1,TimeType.Day);
#endif
    }
    public void ChangeUserTokensValue(float quantity)
    {
        userTokenQuantity.text = (float.Parse(userTokenQuantity.text) + quantity).ToString();
    }
    private void Update()
    {
        var duration = nextBonusReset - DateTimeOffset.UtcNow;
        if ((duration).Seconds >= 0)
            timeToBonusReset.text = "Time until bonus reset:\n" + $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        else
        {
            feudalzBonus.text = "Feudalz Bonus:\n" + bonusMaxCharge + "/" + bonusMaxCharge;
            timeToBonusReset.text = "";
            lastBonusReset = nextBonusReset;
            #if DEVELOP
                    AddTime(lastBonusReset, out nextBonusReset, 5, TimeType.Minute);
            #else
                    AddTime(lastBonusReset, out nextBonusReset,1,TimeType.Day);
            #endif
        }
    }
    private void AddTime(DateTimeOffset _lastRechargeTime, out DateTimeOffset _nextRechargeTime, int _timeToAdd, TimeType _timeType = TimeType.Day)
    {
        _nextRechargeTime = _timeType switch
        {
            TimeType.Day => _lastRechargeTime.AddDays(_timeToAdd),
            TimeType.Hour => _lastRechargeTime.AddHours(_timeToAdd),
            TimeType.Minute => _lastRechargeTime.AddMinutes(_timeToAdd),
            _ => _lastRechargeTime.AddSeconds(_timeToAdd)
        };
    }
    private enum TimeType { Day, Hour, Minute, Second }
}
