using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitsUI : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    public int UnitID { get; private set;}
    public string UnitType { get; private set; }
    public void Setup(Sprite _unitSprite, int _id,string _type)
    {
        unitImage.sprite = _unitSprite;
        UnitID = _id;
        UnitType = _type;
    }
    public void Setup(int _id, string _type)
    {
        UnitID = _id;
        UnitType = _type;
    }
    public void Setup(Sprite _unitSprite)
    {
        unitImage.sprite = _unitSprite;
    }
}
