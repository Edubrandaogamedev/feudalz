using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class BattleUI : MonoBehaviour
{
    [Header("References")]
    private LoadTexture loadTexture = new LoadTexture();
    [SerializeField] private BattleEffects battleEffects;
    [SerializeField] private BattlePopupController popupController;
    public BattlePopupController PopupController { get => popupController; }
    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController singleBattlePanel;
    [SerializeField] private CanvasGroupController allBattlePanel;
    [Header("Images")]
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Image playerLandzImg;
    [SerializeField] private Image playerLandzUniqueImg;
    [SerializeField] private GifTexture playerGifTextureUnique;
    [SerializeField] private Image playerHeroBackground;
    [SerializeField] private Image playerHeroImg;
    [SerializeField] private Image playerHeroBorderImg;
    [SerializeField] private Image enemyLandzImg;
    [SerializeField] private Image enemyLandzUniqueImg;
    [SerializeField] private GifTexture enemyGifTextureUnique;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI txtPlayerLandzName;
    [SerializeField] private TextMeshProUGUI txtPlayerAttackBonus;
    [SerializeField] private TextMeshProUGUI txtPlayerDice;
    [SerializeField] private TextMeshProUGUI txtPlayerHeroEffect;
    [SerializeField] private TextMeshProUGUI txtEnemyDice;
    [SerializeField] private TextMeshProUGUI txtEnemyLandzName;
    [SerializeField] private TextMeshProUGUI txtEnemyDefenseBonus;
    [Header("Template")]
    [SerializeField] private AllLandzBattleUI allLandzBattlePrefab;
    [SerializeField] private RectTransform allLandzBattleContent;
    [Header("Heroes Cards")]
    [SerializeField] private Image heroCardImg;
    [SerializeField] private Image heroFrameImg;
    [SerializeField] private TextMeshProUGUI heroCardName;
    [SerializeField] private TextMeshProUGUI heroCardSkillName;
    [SerializeField] private TextMeshProUGUI heroCardDescription;
    [Header("Heroes Unique Cards")]
    [SerializeField] private Image heroUniqueCardImg;
    [SerializeField] private Image heroUniqueFrameImg;
    [SerializeField] private TextMeshProUGUI heroUniqueCardName;
    [SerializeField] private TextMeshProUGUI heroUniqueCardSkillName;
    [SerializeField] private TextMeshProUGUI heroUniqueCardDescription;
    [Header("Heroes Center Cards")]
    [SerializeField] private Image heroCenterCardImg;
    [SerializeField] private Image heroCenterFrameImg;
    [SerializeField] private TextMeshProUGUI heroCenterCardName;
    [SerializeField] private TextMeshProUGUI heroCenterCardSkillName;
    [SerializeField] private TextMeshProUGUI heroCenterCardDescription;

    public void OpenSingleBattleInterface()
    {
        singleBattlePanel.Enable();
        battleEffects.PlaySearchingEnemyEffect();
    }
    public void CloseSingleBattleInterface()
    {
        singleBattlePanel.Disable();
        battleEffects.ClearEffects();
        CleanInfos();
    }
    public void OpenAllBattleUI()
    {
        allBattlePanel.Enable();
        battleEffects.ActivateLoadingEffect(true);
    }
    public void CloseAllBattleUI()
    {
        allBattlePanel.Disable();
        battleEffects.ActivateLoadingEffect(false);
    }
    private void CloseAttackAllResultScreen(List<AllLandzBattleUI> _panelList)
    {
        CloseAllBattleUI();
        popupController.DisableAttackWithAllLandzResultPopup();
        DestroyResultPanel(_panelList);
    }
    public void OpenAttackAllResultScreen(AttackAll _attackAllResult, UserSessionController userSessionController)
    {
        battleEffects.ActivateLoadingEffect(false);
        var winCount = 0;
        foreach (var result in _attackAllResult.results)
        {
            if (result.winner == result.attacker.address)
                winCount++;
        }
        ConstructAllResultPanel(_attackAllResult.results, userSessionController, out var panelLandz);
        popupController.EnableAttackWithAllLandzResultPopup(() => CloseAttackAllResultScreen(panelLandz), () => OpenResultPopup(winCount, _attackAllResult.goldz, _isSingularAttack: false, _attackResult: null, _attackAllResult.results));
    }
    public async Task SetBattleResult(CombatLandz _playerLandz, AttackResult _result, UserSessionController userSessionController)
    {
        await SetupImageOrGif(_playerLandz, true, _result, userSessionController);
        await SetupImageOrGif(null, false, _result, userSessionController);
        SetupCards(_playerLandz, _result);
        SetTextInfoBasedOnLandz(_playerLandz, _result);

        if (_playerLandz.hero != null)
        {
            txtPlayerHeroEffect.gameObject.SetActive(true);
            playerHeroImg.gameObject.SetActive(true);
            playerHeroBorderImg.gameObject.SetActive(true);
            playerHeroBackground.gameObject.SetActive(true);
            playerHeroImg.sprite = _playerLandz.hero.heroImage;
            playerHeroBackground.color = _playerLandz.hero.GetRarityBackground();
            playerHeroBorderImg.sprite = _playerLandz.hero.rarityBorderImage;
            if (_result.attacker.hero.bonus)
            {
                txtPlayerHeroEffect.text = HeroInfos(_result.attacker.hero.name, _result);
                txtPlayerHeroEffect.color = Color.yellow;
            }
            else
            {
                txtPlayerHeroEffect.text = "Hero Skill Activated: No";
                txtPlayerHeroEffect.color = Color.red;
            }

        }
        else
        {
            txtPlayerHeroEffect.gameObject.SetActive(false);
            playerHeroImg.gameObject.SetActive(false);
            playerHeroBorderImg.gameObject.SetActive(false);
            playerHeroBackground.gameObject.SetActive(false);
        }
    }

    private void SetupCards(CombatLandz _playerLandz, AttackResult _result)
    {
        if (_playerLandz.LandzUnique && _playerLandz.hero != null && _result.attacker.hero.bonus)
        {
            heroUniqueCardImg.sprite = _playerLandz.uniqueHero.heroCard;
            heroUniqueFrameImg.sprite = _playerLandz.uniqueHero.rarityCardImage;
            heroUniqueCardSkillName.text = _playerLandz.uniqueHero.heroSkillName;
            heroUniqueCardName.text = _playerLandz.uniqueHero.name;
            heroUniqueCardDescription.text = _playerLandz.uniqueHero.heroDescription;

            heroCardImg.sprite = _playerLandz.hero.heroCard;
            heroFrameImg.sprite = _playerLandz.hero.rarityCardImage;
            heroCardSkillName.text = _playerLandz.hero.heroSkillName;
            heroCardName.text = _playerLandz.hero.name;
            heroCardDescription.text = _playerLandz.hero.heroDescription + " " + _playerLandz.hero.heroProbability;
        }
        else if (_playerLandz.hero != null || _playerLandz.LandzUnique)
        {
            if (_playerLandz.hero != null && _result.attacker.hero.bonus)
            {
                heroCenterCardImg.sprite = _playerLandz.hero.heroCard;
                heroCenterFrameImg.sprite = _playerLandz.hero.rarityCardImage;
                heroCenterCardSkillName.text = _playerLandz.hero.heroSkillName;
                heroCenterCardName.text = _playerLandz.hero.name;
                heroCenterCardDescription.text = _playerLandz.hero.heroDescription + " " + _playerLandz.hero.heroProbability;
            }
            else if (_playerLandz.LandzUnique)
            {
                heroCenterCardImg.sprite = _playerLandz.uniqueHero.heroCard;
                heroCenterFrameImg.sprite = _playerLandz.uniqueHero.rarityCardImage;
                heroCenterCardSkillName.text = _playerLandz.uniqueHero.heroSkillName;
                heroCenterCardName.text = _playerLandz.uniqueHero.name;
                heroCenterCardDescription.text = _playerLandz.uniqueHero.heroDescription;
            }
        }
    }

    private void SetTextInfoBasedOnLandz(CombatLandz _playerLandz, AttackResult _result)
    {
        if (_result.attacker.land.unique != null)
        {
            if (_result.attacker.land.unique.Contains("Mead"))
                txtPlayerDice.text = $"Total Attack with Old Berserker: {(_result.attacker.dice + _playerLandz.LandzAttackBonus + _result.attacker.land.diceBonus).ToString()}";
            else if (_result.attacker.land.unique.Contains("Blood"))
                txtPlayerDice.text = $"Total Attack with Ghoul Knight: {(_result.attacker.total).ToString()}";
            else if (_result.attacker.land.unique.Contains("Dragon"))
                txtPlayerDice.text = $"Total Attack with Dragon Knight: {(_result.attacker.total).ToString()}";
            else if (_result.attacker.land.unique.Contains("Thieves"))
                txtPlayerDice.text = $"Total Attack with Thief of Fates: {(_result.attacker.dice + _playerLandz.LandzAttackBonus + _result.attacker.land.stolenDice).ToString()}";
            else if (_result.attacker.land.unique.Contains("Tower") && _result.attacker.land.attackTotal > 0)
            {
                txtPlayerDice.text = "Total Attack (Second Round): " + (_result.attacker.dice + _playerLandz.LandzAttackBonus).ToString();
            }
            else
                txtPlayerDice.text = "Total Attack: " + (_result.attacker.total).ToString();
        }
        else
        {
            //txtPlayerDice.text = "Total attack: " + (_result.attacker.dice + _playerLandz.LandzAttackBonus).ToString();
            txtPlayerDice.text = "Total Attack: " + (_result.attacker.total).ToString();
        }

        txtPlayerAttackBonus.text = $"Attack Bonus: {_playerLandz.LandzAttackBonus.ToString()}";
        this.txtPlayerLandzName.text = _playerLandz.LandzName;
        txtEnemyLandzName.text = _result.defender.land.name;
        txtEnemyDefenseBonus.text = "Defense Bonus: " + (_result.defender.total - _result.defender.dice).ToString();
        txtEnemyDice.text = "Total Defense: " + (_result.defender.total).ToString();
    }

    private async Task SetupImageOrGif(CombatLandz _referenceLandz, bool _isPlayer, AttackResult _result, UserSessionController userSessionController)
    {
        if (_isPlayer)
        {
            if (_referenceLandz.LandzUnique)
            {
                playerLandzImg.gameObject.SetActive(false);
                playerLandzUniqueImg.gameObject.SetActive(true);
                playerGifTextureUnique.m_GifFileNameOrUrl = _referenceLandz.LandzUniqueImgURL;
            }
            else
            {
                playerLandzImg.gameObject.SetActive(true);
                playerLandzUniqueImg.gameObject.SetActive(false);
                playerLandzImg.sprite = _referenceLandz.LandzSprite;
            }
        }
        else
        {
            if (userSessionController.DataController.IsUniqueLandzUnique((_result.defender.land.tokenId)))
            {
                enemyLandzImg.gameObject.SetActive(false);
                enemyLandzUniqueImg.gameObject.SetActive(true);
                enemyGifTextureUnique.m_GifFileNameOrUrl = loadTexture.IPFSHandler(_result.defender.land.image);
            }
            else
            {
                enemyLandzImg.gameObject.SetActive(true);
                enemyLandzUniqueImg.gameObject.SetActive(false);
                var loadedEnemySprite = await loadTexture.GetTexture(_result.defender.land.image);
                if (loadedEnemySprite != null)
                    enemyLandzImg.sprite = loadedEnemySprite;
            }
        }
    }

    public async Task PlayCombatAnimation(bool _isUserWon, bool _heroActivated, CombatLandz _landz)
    {
        await battleEffects.PlayCombatFeedback(_isUserWon, _heroActivated, _landz);
    }
    public void OpenErrorPopup()
    {
        popupController.EnableAttackErrorPopup();
        battleEffects.ResetLoadingEffect();
        CloseSingleBattleInterface();
        CloseAllBattleUI();
    }
    public void OpenResultPopup(int _winCount, float _wonGoldz, bool _isSingularAttack, AttackResult _attackResult, AttackResult[] _attackAllResult)
    {
        popupController.EnablePopup(_wonGoldz, PlayReceiveGoldzAnim, _isSingularAttack, _winCount, _attackResult, _attackAllResult);
        battleEffects.GoldzSpinEffect();
    }
    private void CleanInfos()
    {
        txtPlayerLandzName.text = "";
        txtPlayerAttackBonus.text = "";
        txtPlayerDice.text = "";
        txtEnemyDice.text = "";
        txtEnemyDefenseBonus.text = "";
        txtEnemyLandzName.text = "";
        enemyLandzImg.sprite = baseSprite;
    }

    private string HeroInfos(string _heroType, AttackResult _result)
    {
        string heroDescription = "";

        switch (_heroType)
        {
            case "Guttx":
                heroDescription = "Guttx has increased the morale of your troops giving you one more attack.";
                break;
            case "Urzog":
                heroDescription = $"Urzog gave a critical hit on your enemy, doubling the damage. Total Attack: {_result.attacker.total}";
                break;
            case "Etherman":
                heroDescription = $"Etherman drained {_result.attacker.dice} energy from your enemy, restoring your troops.";
                break;
            case "Godjira":
                heroDescription = $"Godjira has penetrated opponents' defense. You did more damage and may have won more goldz.";
                break;
            case "Yokai":
                heroDescription = $"Yokai predicted the best opportuniy to attack. You rolled {_result.attacker.hero.dices.Length} dices and the highest was {_result.attacker.hero.dices.Max()}";
                break;
            case "Lyz":
                heroDescription = $"Lys dodge skills has prevented any damage you would take in this battle. Damage prevented: {_result.defender.total}";
                break;
            case "BabyDragon":
                heroDescription = $"Baby Dragonz destructive power allowed you to do much more damage.";
                break;
        }

        return heroDescription;
    }

    private void CloseResultPopup()
    {
        popupController.DisableResultPopup();
        battleEffects.ResetGoldzSpinEffect();
    }
    //TODO review that
    private void PlayReceiveGoldzAnim(float _wonGoldz)
    {
        battleEffects.ReceiveCoinsFeedback(_wonGoldz, CloseResultPopup);
    }
    private void ConstructAllResultPanel(AttackResult[] _results, UserSessionController userSessionController, out List<AllLandzBattleUI> panelLandz)
    {
        var allLandz = new List<AllLandzBattleUI>();
        foreach (var item in _results)
        {
            var obj = Instantiate(allLandzBattlePrefab, allLandzBattleContent.transform);
            obj.Setup(item, userSessionController);
            allLandz.Add(obj);
        }
        panelLandz = allLandz;
        LayoutRebuilder.ForceRebuildLayoutImmediate(allLandzBattleContent);
    }
    private void DestroyResultPanel(List<AllLandzBattleUI> _panelLandz)
    {
        foreach (AllLandzBattleUI landz in _panelLandz)
        {
            AllLandzBattleUI landzToDestroy = landz;
            landzToDestroy.DestroyObject();
        }
    }
}
