using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleEffects : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks loadingSingleBattle;
    [SerializeField] private MMFeedbacks landzShockAnim;
    [SerializeField] private MMFeedbacks fadeAnim;
    [SerializeField] private MMFeedbacks goldzFeedbacks;
    [Header("Particles")]
    [SerializeField] private ParticleSystem background_particle;
    [Header("GameObjects")]
    [SerializeField] private GameObject playerLandzObj;
    [SerializeField] private GameObject enemyLandObj;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject walletSpriteTarget;
    [SerializeField] private GameObject coinsParent;
    [SerializeField] private GameObject loadingAttackAll;
    [Header("Animators")]
    [SerializeField] private Animator magicAttackAnim;
    [SerializeField] private Animator arrowAttackAnim;
    [Header("Heroes Cards")]
    [SerializeField] private RectTransform cardsContent;
    [SerializeField] private CanvasGroupController skillCanvasGroup;
    [SerializeField] private CanvasGroupController heroCardPrefab;
    [SerializeField] private MMFeedbacks heroCardFeedbacks;
    [SerializeField] private CanvasGroupController heroUniqueCardPrefab;
    [SerializeField] private MMFeedbacks heroUniqueCardFeedbacks;
    [SerializeField] private CanvasGroupController heroCenterCardPrefab;
    [SerializeField] private MMFeedbacks heroCenterCardFeedbacks;
    private Vector3 playerLandzOriginalPosition;
    private Vector3 enemyLandzOriginalPosition;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI vsTextColor;
    private void OnEnable()
    {
        playerLandzOriginalPosition = playerLandzObj.transform.position;
        enemyLandzOriginalPosition = enemyLandObj.transform.position;
    }
    public void PlaySearchingEnemyEffect()
    {
        loadingSingleBattle.PlayFeedbacks();
        background_particle.Play();
        mainCamera.orthographic = false;
        AudioManager.instance.Stop("worldmap_bgm");
        AudioManager.instance.Play("battle_bgm");
    }
    public async Task PlayCombatFeedback(bool _isUserWon, bool _heroActivated, CombatLandz _landz)
    {
        loadingSingleBattle.ResumeFeedbacks();
        landzShockAnim.PlayFeedbacks();
        await new WaitUntil(() => landzShockAnim.IsPlaying == false);
        await new WaitForSeconds(3);
        AudioManager.instance.Stop("battle_bgm");
        fadeAnim.PlayFeedbacks();
        await new WaitUntil(() => fadeAnim.IsPlaying == false);
        //TODO not the ideal way to do that fix that
        if (PlayerPrefs.GetInt("battleAnim") == 1 || !PlayerPrefs.HasKey("battleAnim"))
        {
            if (_heroActivated && _landz.LandzUnique)
            {
                skillCanvasGroup.Enable();
                heroCardPrefab.Enable();
                heroCardFeedbacks.PlayFeedbacks();
                heroUniqueCardPrefab.Enable();
                heroUniqueCardFeedbacks.PlayFeedbacks();
                await new WaitForSeconds(0.5f);
                heroCardFeedbacks.PlayFeedbacks();
                heroUniqueCardFeedbacks.PlayFeedbacks();
                await new WaitForSeconds(5f);
                heroCardFeedbacks.PlayFeedbacks();
                heroUniqueCardFeedbacks.PlayFeedbacks();
                LayoutRebuilder.ForceRebuildLayoutImmediate(cardsContent);
                await new WaitForSeconds(0.5f);
                heroCardFeedbacks.PlayFeedbacks();
                heroUniqueCardFeedbacks.PlayFeedbacks();
                heroCardPrefab.Disable();
                heroUniqueCardPrefab.Disable();
                skillCanvasGroup.Disable();
            }
            else if (_heroActivated || _landz.LandzUnique)
            {                
                skillCanvasGroup.Enable();
                heroCenterCardPrefab.Enable();
                heroCenterCardFeedbacks.PlayFeedbacks();
                await new WaitForSeconds(0.5f);
                heroCenterCardFeedbacks.PlayFeedbacks();
                await new WaitForSeconds(5f);
                heroCenterCardFeedbacks.PlayFeedbacks();
                LayoutRebuilder.ForceRebuildLayoutImmediate(cardsContent);
                await new WaitForSeconds(0.5f);
                heroCenterCardFeedbacks.PlayFeedbacks();
                heroCenterCardPrefab.Disable();
                skillCanvasGroup.Disable();
            }

            if (_isUserWon)
            {
                magicAttackAnim.gameObject.SetActive(true);
                await new WaitForSeconds(3.5f);
                magicAttackAnim.gameObject.SetActive(false);
            }
            else
            {
                arrowAttackAnim.gameObject.SetActive(true);
                await new WaitForSeconds(4f);
                arrowAttackAnim.gameObject.SetActive(false);
            }
        }

        AudioManager.instance.Play("worldmap_bgm");
    }
    public void ResetLoadingEffect()
    {
        loadingSingleBattle.ResumeFeedbacks();
        loadingSingleBattle.ResetFeedbacks();
    }
    public void ActivateLoadingEffect(bool _isToActivate)
    {
        loadingAttackAll.SetActive(_isToActivate);
    }
    public async void ReceiveCoinsFeedback(float _quantity, UnityAction _onCoinsFeedbackEnded)
    {
        await SpawnCoins(_quantity, walletSpriteTarget, _onCoinsFeedbackEnded); ;
    }
    public void GoldzSpinEffect()
    {
        goldzFeedbacks.PlayFeedbacks();
    }
    public void ResetGoldzSpinEffect()
    {
        goldzFeedbacks.StopFeedbacks();
        goldzFeedbacks.gameObject.transform.parent.rotation = Quaternion.identity;
    }
    public void ClearEffects()
    {
        background_particle.Stop();
        background_particle.Clear();
        mainCamera.orthographic = true;
        //TODO improve that reset feedbacks
        fadeAnim.GetComponent<CanvasGroup>().alpha = 0f;
        playerLandzObj.transform.position = playerLandzOriginalPosition;
        enemyLandObj.transform.position = enemyLandzOriginalPosition;
        vsTextColor.alpha = 0;
    }
    //TODO REFACTOR THIS
    private async Task SpawnCoins(float qnt, GameObject destination, UnityAction _onCoinsFeedbackEnded)
    {
        float splitMoney = qnt / 5;
        float control = 0;

        while (control < qnt)
        {
            await new WaitForSeconds(.05f);
            GameObject coin = Instantiate(coinPrefab, coinsParent.transform.position,
            coinsParent.transform.rotation, coinsParent.transform);
            coin.GetComponent<BezierCoin>().target = destination;
            coin.GetComponent<BezierCoin>().Ref1 = new Vector2(2, -3);
            coin.GetComponent<BezierCoin>().CoinsValue = splitMoney;
            control += splitMoney;
            Destroy(coin, 2f);
        }
        _onCoinsFeedbackEnded?.Invoke();
    }
}
