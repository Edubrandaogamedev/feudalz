using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    public event UnityAction<CombatLandz,AttackResult> onSingleAttackResultCalculated;
    public event UnityAction<AttackAll,float> onAttackAllResultCalculated;
    public event UnityAction onError;
    public event UnityAction<string> onErrorWithMessage;
    public async Task AttackLandz(CombatLandz _landz, string _playerToken, int _requestRetries = 0)
    {
        int maxRequestRetries = 3;
        try
        {
            AttackResult result = await APIServices.DatabaseServer.LandzAttack(_playerToken, _landz.TokenId);
            onSingleAttackResultCalculated?.Invoke(_landz,result);
        }
        catch (Exception e)
        {
            //Debug.Log("Erro: " + e.Message);
            if (_requestRetries <= maxRequestRetries)
            {
                await new WaitForSeconds(UnityEngine.Random.Range(3, 10));
                await AttackLandz(_landz, _playerToken, _requestRetries + 1);
            }
            else
            {
                onError?.Invoke();
            }
        }
    }
    public async Task AttackWithAllLands(string _playerToken, string _currentPlayerBiome,float _goldzCost,int _requestRetries = 0)
    {
        //Debug.Log("Attack with all lands");
        int maxRequestRetries = 2;
        try
        {
            AttackAll response = await APIServices.DatabaseServer.AttackWithAllLandz(_playerToken,_currentPlayerBiome);
            onAttackAllResultCalculated?.Invoke(response,_goldzCost);
        }
        catch (Exception e)
        {
            string message = e.Message;
            //Debug.Log("Erro: " + e.Message + "Retry: " + _requestRetries);
            if (_requestRetries <= maxRequestRetries)
            {
                await new WaitForSeconds(UnityEngine.Random.Range(3, 10));
                await AttackWithAllLands(_playerToken, _currentPlayerBiome,_goldzCost,_requestRetries + 1);
            }
            else
            {
                //Debug.Log("EVOCANDO ERRO");
                onError?.Invoke();
                onErrorWithMessage?.Invoke(message.Contains("enough attack") ? "Attack" : "Request");
            }
        }
    }
}
