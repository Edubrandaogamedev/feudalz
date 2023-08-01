using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownButton : MonoBehaviour
{
    [SerializeField] private Button btnReference;
    [SerializeField] private float cooldownTimer;
    private Image btnImage;

    void OnEnable()
    {
        btnReference.onClick.AddListener(InvokeCooldown);
        btnImage = btnReference.GetComponent<Image>();
    }

    void OnDisable()
    {
        btnReference.onClick.RemoveAllListeners();
    }

    void InvokeCooldown()
    {
        StartCoroutine(ButtonTimer());
    }

    IEnumerator ButtonTimer()
    {
        btnImage.fillAmount = 0;
        btnReference.interactable = false;

        bool clicked = true;
        float timer = 0;

        while (clicked)
        {
            timer += Time.deltaTime;

            btnImage.fillAmount = timer / cooldownTimer;
            yield return null;

            if (timer >= cooldownTimer)
            {
                btnReference.interactable = true;
                clicked = false;
            }
            yield return null;
        }
    }
}
