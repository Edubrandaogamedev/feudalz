using Features.Refactor;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

public class MenuManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] MetamaskLogin loginAPI;
    [FormerlySerializedAs("userSessionManager")] [FormerlySerializedAs("userSessionDataManager")] [SerializeField] UserSessionController userSessionController;
    [SerializeField] private IUserService _userService;
    [Header("CanvasGroupController")]
    [SerializeField] private CanvasGroupController cgThisPanel;
    [SerializeField] private CanvasGroupController cgWorldMapPanel;
    [SerializeField] private CanvasGroupController popUpErrorLogin;  
    [Header("Feedbacks")]
    [SerializeField] MMFeedbacks feedbacksOpening;
    [SerializeField] MMFeedbacks feedbackshowingJoinButton;
    [SerializeField] MMFeedbacks feedbackLoading;
    [Header("Buttons")]
    [SerializeField] Button btnJoinWorld;
    [SerializeField] Button popUpErrorLoginButton;
    [Header("Objects Reference")]
    [SerializeField] GameObject waterShaderGO;
    private void OnEnable()
    {
        userSessionController.onInitalized += OnLoginSuccessful;
        userSessionController.onRetryLogin += ShowErrorPopUp;
        SubscribeFeedbackListeners();
        SubscribeButtonsListeners();
    }
    private void OnDisable()
    {
        userSessionController.onInitalized -= OnLoginSuccessful;
        userSessionController.onRetryLogin -= ShowErrorPopUp;
        RemoveFeedbackListeners();
        RemoveButtonsListeners();
    }
    private void SubscribeFeedbackListeners()
    {
        feedbacksOpening.Events.OnComplete.AddListener(OnOpeningFeedbackEnded);
    }
    private void RemoveFeedbackListeners()
    {
        feedbacksOpening.Events.OnComplete.RemoveListener(OnOpeningFeedbackEnded);
    }
    private void SubscribeButtonsListeners()
    {
        btnJoinWorld.onClick.AddListener(JoinWorld);
        popUpErrorLoginButton.onClick.AddListener(InvokeNewRetry);
    }
    private void RemoveButtonsListeners()
    {
        btnJoinWorld.onClick.RemoveListener(JoinWorld);
        popUpErrorLoginButton.onClick.RemoveListener(InvokeNewRetry);
    }
    private void Start()
    {
        if (_userService.IsUserSessionInitialized())
        {
            JoinWorld();
        }
        else
        {
            feedbacksOpening.PlayFeedbacks();
        }     
    }
    private void OnOpeningFeedbackEnded()
    {
        if (userSessionController.Initialized == false)
        {
            loginAPI.TryLogin();
        }
    }
    private void OnLoginSuccessful()
    {
        feedbackLoading.ResumeFeedbacks();
        feedbackshowingJoinButton.PlayFeedbacks();
    }
    private void ShowErrorPopUp()
    {
        popUpErrorLogin.Enable();
    }
    private void InvokeNewRetry()
    {
        userSessionController.RetryLogin();
    }
    private void EnableWorldMap()
    {
        cgThisPanel.Disable();
        cgWorldMapPanel.Enable();
    }
    private void JoinWorld()
    {
        AudioManager.instance.Play("worldmap_bgm");
        EnableWorldMap();
        waterShaderGO.SetActive(true);
    }
}