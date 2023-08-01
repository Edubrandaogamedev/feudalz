using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WebGLSignMessageExample : MonoBehaviour
{
    public Text text;
    async public void OnSignMessage()
    {
        try {
            string message = "hello";
            string response = await Web3GL.Sign(message);
            text.text = response;
            Debug.Log(response);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }
    }
}