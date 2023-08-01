using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simplerotate : MonoBehaviour
{

    private RectTransform rectComponent;
    private RectTransform parentRect;
    private Image imageComp;
    public float rotateSpeed = 200f;
    private float currentvalue;

    float x;
    float y;

    // Use this for initialization
    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currentvalue = currentvalue + (Time.deltaTime * rotateSpeed);

        x = parentRect.transform.eulerAngles.x;
        y = parentRect.transform.eulerAngles.y;

        //Debug.Log("currnet Value is " + currentvalue);
        parentRect = transform.parent.GetComponent<RectTransform>();
        rectComponent.transform.rotation = Quaternion.Euler(x, y, -72f * (int)currentvalue);
    }
}