using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class AllocationSystem : MonoBehaviour
{
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [SerializeField] private AllocationData data;
    [Header("Popup")]
    [SerializeField] private AllocationPopupController popupController;
    [Header("Canvas References")]
    [SerializeField] private CanvasGroupController allocationPanel;

    [SerializeField] private CanvasGroupController lockInputPanel;
    [Header("Content")]
    [SerializeField] private RectTransform buildingContent;
    [SerializeField] private RectTransform heroContent;
    [Header("Template")]
    [SerializeField] private BuildingAllocationTemplate buildingsTemplate;
    [SerializeField] private HeroezAllocationTemplate heroezTemplate;
    [Header("Button")]
    [SerializeField] private Button closeBtn;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _header;
    [Header("List")]
    private readonly List<HeroezAllocationTemplate> panelHeroez = new List<HeroezAllocationTemplate>();
    private readonly List<BuildingAllocationTemplate> panelBuildings = new List<BuildingAllocationTemplate>();
    [Header("Cached")]
    private CombatLandz cachedLandz;
    private FeudalzHeroez cachedHeroez;
    private FeudalzLandzBuild cachedBuild;
    [field: Header(("Events"))]
    public event UnityAction onHeroAllocationCompleted;

    public event UnityAction onBuildAllocationCompleted;
    private void OnEnable() {
        closeBtn.onClick.AddListener(CloseAllocationPanel);
    }
    private void OnDisable() {
        closeBtn.onClick.RemoveListener(CloseAllocationPanel);
    }
    public void OpenAllocationPanel(CombatLandz _landz, string _allocationType)
    {
        cachedLandz = _landz;
        SetHeaderText(_allocationType);
        if (_allocationType.Contains("Build"))
        {
            heroContent.gameObject.SetActive(false);
            buildingContent.gameObject.SetActive(true);
            ConstructBuildingsPanel(_landz,_allocationType);
        }
        else
        {
            buildingContent.gameObject.SetActive(false);
            heroContent.gameObject.SetActive(true);
            ConstructHeroezPanel();
        }
        allocationPanel.Enable();
    }
    public void CloseAllocationPanel()
    {
        allocationPanel.Disable();
        popupController.DisableAllocationPopup();
    }
    private void OnTryAllocateHero(FeudalzHeroez _heroez)
    {
        cachedHeroez = _heroez;
        if (SkipPopup.GetSettings())
            OnHeroAllocationConfirmed();
        else
            popupController.EnableHeroAllocationPopup(OnHeroAllocationConfirmed);
    }
    private async void OnHeroAllocationConfirmed()
    {
        if (cachedLandz == null) return;
        lockInputPanel.Enable();
        try
        {
            await APIServices.DatabaseServer.InstallHero(userSessionController.Token, cachedLandz.TokenId, cachedHeroez.name, cachedHeroez.heroRarity);
            var feudalzHeroez = userSessionController.DataController.GetAllHeroez().Find(hero => (hero.name == cachedHeroez.name && hero.heroRarity == cachedHeroez.heroRarity));
            if (feudalzHeroez != null)
            {
                feudalzHeroez.heroQuantity--;
                if (feudalzHeroez.heroQuantity == 0)
                    userSessionController.DataController.GetAllHeroez().Remove(cachedHeroez);
            }
            userSessionController.CombatLandzs.Find(land => land.tokenId == cachedLandz.TokenId).heroez = cachedHeroez;
            cachedLandz.OnHeroAllocationSuccessful(cachedHeroez);
            onHeroAllocationCompleted?.Invoke();
            CloseAllocationPanel();
            ClearCache();
        }
        catch (WebRequestException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            popupController.DisableAllocationPopup();
            lockInputPanel.Disable();
        }
    }
    private void OnTryAllocateBuild(BuildingAllocationTemplate _targetBuild)
    {
        cachedBuild = _targetBuild.BuildData;
        if (SkipPopup.GetSettings())
            OnBuildAllocationConfirmed(_targetBuild);
        else
            popupController.EnableBuildingAllocationPopup(() => OnBuildAllocationConfirmed(_targetBuild));
    }
    private async void OnBuildAllocationConfirmed(BuildingAllocationTemplate _targetBuild)
    {
        if (cachedBuild == null) return;
        try
        {
            lockInputPanel.Enable();
            await APIServices.DatabaseServer.AllocateBuild(userSessionController.Token, cachedLandz.TokenId, cachedBuild.position, cachedBuild.type, _targetBuild.GetAutoRebuilderInfo());
            cachedLandz.OnBuildAllocationSuccessful(cachedBuild,_targetBuild.GetAutoRebuilderInfo());
            userSessionController.UpdateLandzBuildsInfo(cachedLandz);
            if (cachedBuild.costType == "goldz")
                userSessionController.DataController.UpdateTokenBalance(-cachedBuild.cost);
            else
                userSessionController.UpdateInventoryItem(cachedBuild.costType,-(int)cachedBuild.cost);
            onBuildAllocationCompleted?.Invoke();
            CloseAllocationPanel();
            ClearCache();
        }
        catch (WebRequestException ex)
        {
            if (ex.Message.Contains("have building"))
                popupController.ErrorHandler("Build");
            else if (ex.Message.Contains("goldz"))
                popupController.ErrorHandler("Goldz");
            else if (ex.Message.Contains("You don't have enough"))
                popupController.ErrorHandler("Resources");
        }
        finally
        {
            
            popupController.DisableAllocationPopup();
            lockInputPanel.Disable();
        }
    }
    private void ConstructHeroezPanel()
    {
        var heroezByRarity = userSessionController.DataController.GetAllHeroezByRarity("epic");
        heroezByRarity.AddRange(userSessionController.DataController.GetAllHeroezByRarity("rare"));
        heroezByRarity.AddRange(userSessionController.DataController.GetAllHeroezByRarity("uncommon"));
        heroezByRarity.AddRange(userSessionController.DataController.GetAllHeroezByRarity("common"));
        var index = 0;
        foreach (var heroez in heroezByRarity)
        {
            if (index < panelHeroez.Count)
            {
                panelHeroez[index].gameObject.SetActive(true);
                panelHeroez[index].Setup(heroez);
                panelHeroez[index].RegisterSelectListener(OnTryAllocateHero);
            }
            else
            {
                var clone = Instantiate(heroezTemplate,heroContent);
                clone.Setup(heroez);
                clone.RegisterSelectListener(OnTryAllocateHero);
                panelHeroez.Add(clone);
            }
            index ++;
        }
        if (panelHeroez.Count > heroezByRarity.Count)
        {
            TurnOffUnusedContentObjects(heroezByRarity.Count,"heroez");
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(heroContent);
    }
    private void ConstructBuildingsPanel(CombatLandz _landz,string _position)
    {
        var index = 0;
        foreach (var build in _landz.AllowedBuilds)
        {
            if (index < panelBuildings.Count)
            {
                panelBuildings[index].gameObject.SetActive(true);
                panelBuildings[index].Setup(build,_position,userSessionController.BuildingResourceData);
                panelBuildings[index].RegisterBuyListener(OnTryAllocateBuild);
            }
            else
            {
                BuildingAllocationTemplate clone = Instantiate(buildingsTemplate,buildingContent);
                clone.Setup(build,_position,userSessionController.BuildingResourceData);
                clone.RegisterBuyListener(OnTryAllocateBuild);
                panelBuildings.Add(clone);            
            }
            index++;
        }
        if (panelBuildings.Count > _landz.AllowedBuilds.Count)
        {
            TurnOffUnusedContentObjects(_landz.AllowedBuilds.Count,"building");
        }
        GreyOutBuilds();
        LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
    }

    private void GreyOutBuilds()
    {
        foreach (var build in panelBuildings)
        {
            if (build.ResourceType == "goldz")
            {
                if (userSessionController.DataController.TokenBalance >= build.ResourceCost) continue;
                build.BlockBuild();
                build.transform.SetAsLastSibling();
            }
            else
            {
                var inventoryItem = userSessionController.InventoryItens.Find(item => item.name == build.ResourceType);
                if (inventoryItem != null && inventoryItem.quantity >= build.ResourceCost) continue;
                build.BlockBuild();
                build.transform.SetAsLastSibling();
            }
        }
    }
    private void ClearCache()
    {
        cachedLandz = null;
        cachedHeroez = null;
        cachedBuild = null;
    }
    private void TurnOffUnusedContentObjects(int _startIndex, string _contentType)
    {
        switch (_contentType)
        {
            case "heroez":
            {
                for (var i = _startIndex; i< panelHeroez.Count;i++)
                {
                    panelHeroez[i].gameObject.SetActive(false);
                }
                break;
            }
            case "building":
            {
                for (var i = _startIndex; i < panelBuildings.Count;i++)
                {
                    panelBuildings[i].gameObject.SetActive(false);
                }
                break;
            }
        }
    }
    private void SetHeaderText(string _allocationType)
    {
        if (_allocationType.Contains("TopBuild"))
            _header.text = "Northern Building Allocation";
        else if (_allocationType.Contains("LeftBuild"))
            _header.text = "Western Building Allocation";
        else if (_allocationType.Contains("RightBuild"))
            _header.text = "Eastern  Building Allocation";
        else if (_allocationType.Contains("Hero"))
            _header.text = "Hero Allocation";
    }
}
