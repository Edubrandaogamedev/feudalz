using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Heroez/Rarity", fileName = "NewRarityBoard")]
public class RarityFrameData : ScriptableObject
{
    [SerializeField] private string rarity;
    [SerializeField] private Sprite rarityBoard;
    public string RarityName { get => rarity;}
    public Sprite RarityBoard { get => rarityBoard;}
}
