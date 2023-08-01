using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerPrefs.DeleteKey("global");
    }
}
