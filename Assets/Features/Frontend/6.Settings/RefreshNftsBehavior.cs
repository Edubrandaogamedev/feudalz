using Features.Refactor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RefreshNftsBehavior : MonoBehaviour
{
    
    [Header("Service")]
    [SerializeField] 
    private UserService _userService;
    [SerializeField] 
    private UserSessionController _userSessionController; 
    [Header("Buttons")]
    [SerializeField] private Button refreshBtn;
    [Header("Canvas Group")]
    [SerializeField] private CanvasGroupController loading;
    [Header("GameObject")]
    [SerializeField] private GameObject uiLandzPanel;
    [Header("Reload Scene")]
    [SerializeField] private string reloadScene;
    [Header("World")]
    [SerializeField] private bool isWorldScene;

    public static event UnityAction OnRefreshedStarted;
    private void OnEnable()
    {
        refreshBtn.onClick.AddListener(RefreshNFTs);
    }
    private async void RefreshNFTs()
    {
        if (isWorldScene)
        {
            OnRefreshedStarted?.Invoke();
            loading.Enable();
            _userService.ReloadUserInformation();
            SceneManager.LoadScene(reloadScene);
        }
        else
        {
            var rb = uiLandzPanel.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            await new WaitForSeconds(1f);
            loading.Enable();
            _userService.ReloadUserInformation();
            _userSessionController.OnRegionEntered(_userSessionController.CurrentUserLandzBioma);
            Destroy(rb);
            SceneManager.LoadScene(reloadScene);
        }
    }
    
    private void OnDisable()
    {
        refreshBtn.onClick.RemoveAllListeners();
    }
}
