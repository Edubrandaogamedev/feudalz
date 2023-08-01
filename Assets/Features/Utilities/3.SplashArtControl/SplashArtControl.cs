using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashArtControl : MonoBehaviour
{
    [SerializeField] private SceneReference worldMapScene;
    public void SplashArtFinished()
    {
        SceneManager.LoadScene(worldMapScene);
    }
}
