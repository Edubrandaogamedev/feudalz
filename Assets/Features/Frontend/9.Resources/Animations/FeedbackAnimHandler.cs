using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackAnimHandler : MonoBehaviour
{
    [SerializeField] private GameObject _reference;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image imgResource;
    
    public void Setup(string _qnty, Sprite _img)
    {
        quantityText.text = _qnty;
        imgResource.sprite = _img;
    }

    public void OnFinishingAnim()
    {
        Destroy(_reference);
    }
}
