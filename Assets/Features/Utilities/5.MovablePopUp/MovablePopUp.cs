using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MovablePopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string textToPopUp;
    [SerializeField] private GameObject popUpPanel;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private RectTransform canvasPreparation; //Referência do rect transform do canvas para posicionamento da popup.
    [SerializeField] private GameObject objectToHighlight;

    public GameObject PopUpPanel { get => popUpPanel; set => popUpPanel = value; }
    public TextMeshProUGUI PopUpText { get => popUpText; set => popUpText = value; }
    public RectTransform CanvasPreparation { get => canvasPreparation; set => canvasPreparation = value; }
    public GameObject ObjectToHighlight { get => objectToHighlight; set => objectToHighlight = value; }
    public string TextToPopUp { get => textToPopUp; set => textToPopUp = value; }

    private void SpawnExplanation(PointerEventData eventData)
    {
        popUpPanel.SetActive(true);
        popUpText.text = textToPopUp;
        popUpPanel.transform.position = this.gameObject.transform.position;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popUpPanel != null && objectToHighlight != null && eventData.pointerCurrentRaycast.gameObject.name.Contains(objectToHighlight.name))
        {
            SpawnExplanation(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (popUpPanel != null)
            popUpPanel.SetActive(false);
    }
}
