using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

public enum FeudalzBiomas { Solfar, Myleane, Oriannus, Jarladre, Ethos, Leonis, 
Lyis, Kroom, Azurye, Vanthera, Vundt, Jamana, Vikraam, Swundvand, Vallados, Maradurk, Gwinland, Sahadak, Xue,
Bracquia, Mandoss }
public class WorldMapManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private string regionScene;
    [FormerlySerializedAs("userSessionManager")] [FormerlySerializedAs("userSessionDataManager")] [SerializeField] private UserSessionController userSessionController;
    [SerializeField] private ZestriaLandzManager zestriaLandzManager;
    [Header("Region Buttons")]
    [SerializeField] private WorldMapClickableRegion[] clickableRegions;
    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks feedbackFade;
    private void OnEnable()
    {
        SubscribeClickableRegionListeners();
        feedbackFade.Events.OnComplete.AddListener(LoadBiomaRegion);
        zestriaLandzManager.OnPlayerInfoUpdated += SetupLandzInfo;
        userSessionController.onInitalized += SetupLandzInfo;

    }
    private void OnDisable()
    {
        UnsubscribeClickableRegionListeners();
        feedbackFade.Events.OnComplete.RemoveListener(LoadBiomaRegion);
        userSessionController.onInitalized -= SetupLandzInfo;
        zestriaLandzManager.OnPlayerInfoUpdated -= SetupLandzInfo;
    }
    private void SubscribeClickableRegionListeners()
    {
        for (int i = 0; i < clickableRegions.Length; i++)
        {
            clickableRegions[i].onRegionClicked += ChangeToBioma;
        }
    }
    private void UnsubscribeClickableRegionListeners()
    {
        for (int i = 0; i < clickableRegions.Length; i++)
        {
            clickableRegions[i].onRegionClicked -= ChangeToBioma;
        }
    }
    private void Start()
    {
        if (userSessionController.Initialized)
        {
            SetupLandzInfo();
        }
    }
    private void ChangeToBioma(FeudalzBiomas _region)
    {
        foreach (WorldMapClickableRegion regionButton in clickableRegions)
        {
            regionButton.LockButton();
        }
        userSessionController.OnRegionEntered(_region.ToString());
        feedbackFade.PlayFeedbacks();
    }
    private void LoadBiomaRegion()
    {
        SceneManager.LoadScene(regionScene);
    }
    private void SetupLandzInfo()
    {
        foreach (WorldMapClickableRegion regionButton in clickableRegions)
        {
            regionButton.SetUserLands(userSessionController, userSessionController.CombatLandzs);
            regionButton.SetUserLandsAttacks(userSessionController, userSessionController.CombatLandzs);
        }
    }
}