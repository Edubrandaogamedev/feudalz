using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsControl : MonoBehaviour
{
    [SerializeField] private Slider battleAnimations;
    private void Start()
    {
        battleAnimations.onValueChanged.AddListener(BattleAnimToggle);
        battleAnimations.value = PlayerPrefs.GetInt("battleAnim",1);
    }
    private void BattleAnimToggle(float _setBattleAnim)
    {
        PlayerPrefs.SetInt("battleAnim", (int)_setBattleAnim);
    }
    private void OnDisable()
    {
        battleAnimations.onValueChanged.RemoveAllListeners();
    }
}
