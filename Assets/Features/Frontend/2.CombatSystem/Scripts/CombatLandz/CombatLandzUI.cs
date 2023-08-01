using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CombatLandzUI : MonoBehaviour
{
    private enum AllocateType { TopBuild, LeftBuild, RightBuild, Hero, Unique }
    [Header("Buttons")]
    [SerializeField] private Button btnAttack;
    [SerializeField] private Button btnFixLandz;
    [SerializeField] private Button btnAllocateBuildTop;
    [SerializeField] private Button btnAllocateBuildLeft;
    [SerializeField] private Button btnAllocateBuildRight;
    [SerializeField] private Button btnAllocateHero;
    [SerializeField] private Button btnInfoBuildTop;
    [SerializeField] private Button btnInfoBuildLeft;
    [SerializeField] private Button btnInfoBuildRight;
    [SerializeField] private Button btnInfoHero;
    [SerializeField] private Button btnInfoUniqueHero;
    [Header("Images")]
    [SerializeField] private Image landzImg;
    [SerializeField] private Image landzUniqueImg;
    [SerializeField] private GifTexture gifTextureUnique;
    [SerializeField] private Image healthBarImg;
    [SerializeField] private Image attackIconImg;
    [SerializeField] private Image heroezIconImg;
    [SerializeField] private Image heroezBackground;
    [SerializeField] private Image borderIconImg;
    [SerializeField] private Image heroezUniqueIconImg;
    [SerializeField] private Image heroezUniqueBackground;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI landzIdInfo;
    [SerializeField] private TextMeshProUGUI remainingAttacksInfo;
    [SerializeField] private TextMeshProUGUI rechargeInfo;
    [SerializeField] private TextMeshProUGUI healthBarInfo;
    [field: Header("Internal")] public bool InitializeTimer { get; private set; }
    public Image LandzImg => landzImg;
    public TextMeshProUGUI LandzIdInfo => landzIdInfo;
    public GifTexture GifTextureUnique { get => gifTextureUnique; set => gifTextureUnique = value; }

    private void OnDisable()
    {
        ClearListeners();
    }
    private void ClearListeners()
    {
        btnAttack.onClick.RemoveAllListeners();
        btnFixLandz.onClick.RemoveAllListeners();
        btnAllocateBuildTop.onClick.RemoveAllListeners();
        btnAllocateBuildLeft.onClick.RemoveAllListeners();
        btnAllocateBuildRight.onClick.RemoveAllListeners();
        btnAllocateHero.onClick.RemoveAllListeners();
        btnInfoBuildTop.onClick.RemoveAllListeners();
        btnInfoBuildLeft.onClick.RemoveAllListeners();
        btnInfoBuildRight.onClick.RemoveAllListeners();
        btnInfoHero.onClick.RemoveAllListeners();
        btnInfoUniqueHero.onClick.RemoveAllListeners();
    }
    public void RegisterListeners(UnityAction _onAttackClickedCallback, UnityAction _onFixClickedCallback, UnityAction<string> _onOpenAllocateCallback, UnityAction<string> _onOpenAllocateInfoCallback)
    {
        ClearListeners();
        btnAttack.onClick.AddListener(_onAttackClickedCallback);
        btnFixLandz.onClick.AddListener(_onFixClickedCallback);
        btnAllocateBuildTop.onClick.AddListener(() => _onOpenAllocateCallback(AllocateType.TopBuild.ToString()));
        btnAllocateBuildLeft.onClick.AddListener(() => _onOpenAllocateCallback(AllocateType.LeftBuild.ToString()));
        btnAllocateBuildRight.onClick.AddListener(() => _onOpenAllocateCallback(AllocateType.RightBuild.ToString()));
        btnAllocateHero.onClick.AddListener(() => _onOpenAllocateCallback(AllocateType.Hero.ToString()));
        btnInfoBuildTop.onClick.AddListener(() => _onOpenAllocateInfoCallback(AllocateType.TopBuild.ToString()));
        btnInfoBuildLeft.onClick.AddListener(() => _onOpenAllocateInfoCallback(AllocateType.LeftBuild.ToString()));
        btnInfoBuildRight.onClick.AddListener(() => _onOpenAllocateInfoCallback(AllocateType.RightBuild.ToString()));
        btnInfoHero.onClick.AddListener(() => _onOpenAllocateInfoCallback(AllocateType.Hero.ToString()));
        btnInfoUniqueHero.onClick.AddListener(() => _onOpenAllocateInfoCallback(AllocateType.Unique.ToString()));
    }
    public void UpdateHealthBar(float _currentHp, float _maxHp)
    {
        healthBarInfo.text = _currentHp + "/" + _maxHp;
        healthBarImg.fillAmount = _currentHp / _maxHp;
    }
    public void UpdateAttackInfo(int _attacks, int _maxAttacks)
    {
        remainingAttacksInfo.text = _attacks + "/" + _maxAttacks;
    }
    public void CheckAttackButton(int _landzAttackQuantity, float _landzHealthValue)
    {
        if (_landzAttackQuantity > 0 && _landzHealthValue > 0)
        {
            btnAttack.interactable = true;
            landzImg.color = new Color(1, 1, 1, 1f);
            landzUniqueImg.color = new Color(1, 1, 1, 1);
        }
        else
        {
            landzImg.color = new Color(1, 1, 1, 0.3f);
            landzUniqueImg.color = new Color(1, 1, 1, 0.3f);
            btnAttack.interactable = false;
        }
    }
    public void CheckRechargeCooldown(DateTimeOffset _nextRechargeTime)
    {
        InitializeTimer = false;
        if ((_nextRechargeTime - DateTimeOffset.UtcNow).Seconds < 0)
            rechargeInfo.text = "Ready To Recharge";
        else
            InitializeTimer = true;

    }
    public void CheckFixButton(float _landzHealthValue)
    {
        if (_landzHealthValue > 0) return;
        btnFixLandz.gameObject.SetActive(true);
        healthBarImg.transform.parent.gameObject.SetActive(false);
        btnAttack.gameObject.SetActive(false);
        remainingAttacksInfo.gameObject.SetActive(false);
        attackIconImg.gameObject.SetActive(false);
        rechargeInfo.gameObject.SetActive(false);
        landzImg.color = new Color(1, 1, 1, 0.3f);
        landzUniqueImg.color = new Color(1, 1, 1, 0.3f);

    }
    public void CheckHeroezButton(FeudalzHeroez _heroez)
    {
        if (_heroez?.name == null || _heroez.heroCharges <= 0)
        {
            btnAllocateHero.gameObject.SetActive(true);
            btnInfoHero.gameObject.SetActive(false);
        }
        else
        {
            btnAllocateHero.gameObject.SetActive(false);
            btnInfoHero.gameObject.SetActive(true);
            borderIconImg.sprite = _heroez.rarityBorderImage;
            heroezIconImg.sprite = _heroez.heroImage;
            heroezBackground.color = _heroez.GetRarityBackground();
        }
    }
    public void CheckUniqueHeroezButton(FeudalzHeroez _heroez, FeudalzCombatLandz _combatLandz)
    {
        if (_combatLandz.uniqueLandz)
        {
            btnInfoUniqueHero.gameObject.SetActive(true);
            heroezUniqueIconImg.sprite = _heroez.heroImage;
            heroezUniqueBackground.color = _heroez.GetRarityBackground();
        }
    }

    public void CheckBuildButtons(List<FeudalzLandzAllocatedBuild> _allocatedBuilds)
    {
        var topBuild = _allocatedBuilds.Find(build => build.position == "top");
        if (topBuild != null)
        {
            btnAllocateBuildTop.gameObject.SetActive(false);
            btnInfoBuildTop.gameObject.SetActive(true);
            btnInfoBuildTop.image.sprite = topBuild.buildImage;
        }
        else
        {
            btnAllocateBuildTop.gameObject.SetActive(true);
            btnInfoBuildTop.gameObject.SetActive(false);
        }
        var leftBuild = _allocatedBuilds.Find(build => build.position == "left");
        if (leftBuild != null)
        {
            btnAllocateBuildLeft.gameObject.SetActive(false);
            btnInfoBuildLeft.gameObject.SetActive(true);
            btnInfoBuildLeft.image.sprite = leftBuild.buildImage;
        }
        else
        {
            btnAllocateBuildLeft.gameObject.SetActive(true);
            btnInfoBuildLeft.gameObject.SetActive(false);
        }
        var rightBuild = _allocatedBuilds.Find(build => build.position == "right");
        if (rightBuild != null)
        {
            btnAllocateBuildRight.gameObject.SetActive(false);
            btnInfoBuildRight.gameObject.SetActive(true);
            btnInfoBuildRight.image.sprite = rightBuild.buildImage;
        }
        else
        {
            btnAllocateBuildRight.gameObject.SetActive(true);
            btnInfoBuildRight.gameObject.SetActive(false);
        }
    }
    public void ReturnInfoAfterFix()
    {
        btnFixLandz.gameObject.SetActive(false);
        healthBarImg.transform.parent.gameObject.SetActive(true);
        btnAttack.gameObject.SetActive(true);
        remainingAttacksInfo.gameObject.SetActive(true);
        attackIconImg.gameObject.SetActive(true);
        rechargeInfo.gameObject.SetActive(true);
        landzImg.color = new Color(1, 1, 1, 1f);
        landzUniqueImg.color = new Color(1, 1, 1, 1f);
    }
    public void UpdateRechargeCooldown(DateTimeOffset _nextRechargeTime)
    {
        TimeSpan duration = _nextRechargeTime - DateTimeOffset.UtcNow;
        rechargeInfo.text = $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        CheckRechargeCooldown(_nextRechargeTime);
    }
    public void SetLandzInfo(string _landzName, Sprite _landzImage, bool _landzUnique, string _landzUniqueUrlGif)
    {
        landzIdInfo.text = _landzName;
        landzImg.sprite = _landzImage;
        if (!_landzUnique) return;
        landzImg.gameObject.SetActive(false);
        landzUniqueImg.gameObject.SetActive(true);
        gifTextureUnique.m_GifFileNameOrUrl = _landzUniqueUrlGif;
    }
    public async Task<Sprite> LoadLandzImage(string _landzUrl)
    {
        LoadTexture loadTexture = new LoadTexture();
        Task<Sprite> LoadTextureTask = loadTexture.GetTexture(_landzUrl);
        TaskManager.RegisterTask(LoadTextureTask);
        await LoadTextureTask;
        TaskManager.DisposeTask(LoadTextureTask);
        return LoadTextureTask.Result;
    }
    public string LoadLandzImageUniqueUrl(string _landzUrl)
    {
        LoadTexture loadTexture = new LoadTexture();
        return loadTexture.IPFSHandler(_landzUrl);
    }
}
