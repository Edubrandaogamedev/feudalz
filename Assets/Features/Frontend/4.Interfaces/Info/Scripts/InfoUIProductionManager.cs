using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoUIProductionManager : MonoBehaviour
{
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;
    [Header("References")]
    [SerializeField] private InfoPopupController popupController;
    [Header("Template")]
    [SerializeField] private BuildingsUI productTemplate;
    [SerializeField] private CraftingUI craftingTemplate;
    [SerializeField] private ProductionUI productionTemplate;

    [Header("Contents")]
    [SerializeField] private RectTransform buildingContent;
    [SerializeField] private RectTransform craftingContent;
    [SerializeField] private RectTransform deployContent;
    [SerializeField] private RectTransform productionContent;

    [Header("GameObjects")]
    [SerializeField] private GameObject craftingHeader;
    [SerializeField] private GameObject craftingExplanationText;
    [SerializeField] private GameObject deployHeader;
    [SerializeField] private GameObject productionHeader;
    [SerializeField] private GameObject mainCanvas;

    [Header("Canvas Group Controller")]
    [SerializeField] private CanvasGroupController productionPanel;
    [SerializeField] private CanvasGroupController craftingPanel;
    [SerializeField] private CanvasGroupController lockInputPanel;

    [Header("Button")]
    [SerializeField] private Button deployButton;
    [SerializeField] private Button claimAllButton;

    [Header("Slider")][SerializeField] private Slider autoCraftSlider;

    [Header("Internal")]
    private readonly List<BuildingsUI> resourcePoolList = new List<BuildingsUI>();
    private readonly List<CraftingUI> craftingPoolList = new List<CraftingUI>();
    private readonly List<ProductionUI> productionPoolList = new List<ProductionUI>();
    [Header("CachedValues")]
    private BuildingsUI cachedResource;
    private CraftingUI cachedCraftingOption;
    private int cachedResourceMultiplier;
    private int resourceOriginalHierarquyIndex;
    private int craftOriginalHierarquyIndex;

    [field: Header("Events")]
    public static event UnityAction<string, int> OnResourceClaimed;
    public static readonly UnityEvent UpdateLandz = new UnityEvent();
    public static readonly UnityEvent UpdateUserInfo = new UnityEvent();
    private void OnEnable()
    {
        BuildingsUI.OnBuildAdded += OnBuildingAddedHandler;
        BuildingsUI.OnBuildRemoved += OnBuildingRemovedHandler;
        CraftingUI.OnCraftOptionChosen += OnCraftingOptionSelected;
        InfoUIManager.OnPanelChange += OnPanelSelect;
        AllocationInfo.onUpdateProductionPanel.AddListener(UpdateBuildingPanel);
        AllocationInfo.onUpdateProductionPanel.AddListener(UpdateProductionPanel);
        deployButton.onClick.AddListener(OnDeploy);
        claimAllButton.onClick.AddListener(OnClaimAll);
    }
    private void OnDisable()
    {
        BuildingsUI.OnBuildAdded -= OnBuildingAddedHandler;
        BuildingsUI.OnBuildRemoved -= OnBuildingRemovedHandler;
        CraftingUI.OnCraftOptionChosen -= OnCraftingOptionSelected;
        InfoUIManager.OnPanelChange -= OnPanelSelect;
        AllocationInfo.onUpdateProductionPanel.RemoveAllListeners();
        deployButton.onClick.RemoveListener(OnDeploy);
        claimAllButton.onClick.RemoveListener(OnClaimAll);
    }
    private void Update()
    {
        CheckClaimAllButton();
    }
    public void UpdateBuildingPanel()
    {
        ClearCraftingPanelInfo();
        OpenProductionPanel();
        CloseDeployPanel();
        CloseCraftingPanel();
        ResetBuildQuantityValues();
        var avaiableProductionBuilds = GetAllAvaiableProductionBuilds(userSessionController.DataController.GetAllLandz());
        SetResourceManagerPanel(avaiableProductionBuilds);
    }
    public void UpdateProductionPanel()
    {
        ClearCraftingPanelInfo();
        OpenProductionPanel();
        CloseDeployPanel();
        CloseCraftingPanel();
        SetProductionPanel(userSessionController.DataController.ProductionItems);
        CheckClaimAllButton();
    }
    private void ResetBuildQuantityValues()
    {
        foreach (var resource in resourcePoolList)
        {
            resource.ResetStatus();
        }
    }
    #region EventsHandler
    private void OnBuildingAddedHandler(BuildingsUI _resource, int _resourceMultiplier)
    {
        cachedResourceMultiplier = _resourceMultiplier;
        if (craftingPanel.Enabled)
            UpdateCraftingInfo(_resource, _resourceMultiplier);
        else
        {
            CloseProductionPanel();
            cachedResource = _resource;
            ResourceSelectionHandler();
            SetCraftingOptions(cachedResource);
            OpenCraftingPanel();
            LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
        }
    }
    private void OnBuildingRemovedHandler(BuildingsUI _resource, int _resourceMultiplier)
    {
        cachedResourceMultiplier = _resourceMultiplier;
        if (_resourceMultiplier > 0)
            UpdateCraftingInfo(_resource, _resourceMultiplier);
        else
        {
            ReturnObjectToOriginalHierarquyPosition(cachedResource.transform, ref resourceOriginalHierarquyIndex);
            ClearCraftingPanelInfo();
            ResourceSelectionHandler();
            CloseCraftingPanel();
            OpenProductionPanel();
        }
    }
    private async void ChangeAutoCraftSettings(ProductionUI _productionBatch, float _value)
    {
        lockInputPanel.Enable();
        await APIServices.DatabaseServer.SwitchAutoCraft(userSessionController.Token, _productionBatch.BatchId, _value != 0);
        lockInputPanel.Disable();
    }
    private void OnPanelSelect(CanvasGroupController _selectedPanel)
    {
        if (_selectedPanel != productionPanel)
        {
            CloseCraftingPanel();
            OpenProductionPanel();
        }
        else
        {
            CheckClaimAllButton();
            if (cachedResource == null) return;
            OpenCraftingPanel();
            CloseProductionPanel();
        }
    }
    private void OnCraftingOptionSelected(CraftingUI _option, bool _isSelected)
    {
        if (_isSelected)
        {
            cachedCraftingOption = _option;
            SetObjectOnTopHierarquy(cachedCraftingOption.transform, ref craftOriginalHierarquyIndex);
            OpenDeployPanel();
            cachedResource.SetLockButtonState(false);
        }
        else
        {
            ReturnObjectToOriginalHierarquyPosition(cachedCraftingOption.transform, ref craftOriginalHierarquyIndex);
            cachedCraftingOption = null;
            CloseDeployPanel();
            cachedResource.SetLockButtonState(_unlock: true);
        }
        CraftSelectionHandler(_option, _isSelected);
        LayoutRebuilder.ForceRebuildLayoutImmediate(craftingContent);
    }
    private async void OnDeploy()
    {
        try
        {
            lockInputPanel.Enable();
            var response = await APIServices.DatabaseServer.StartProduction(userSessionController.Token, cachedResource.BuildingType, cachedResourceMultiplier, cachedCraftingOption.Index, autoCraftSlider.value != 0);
            userSessionController.UpdateLandzBuildsInfo(response.affectedLandz);
            foreach (var craftOption in cachedCraftingOption.ResourcesNeeded)
            {
                userSessionController.UpdateInventoryItem(craftOption.Type, -cachedResourceMultiplier * craftOption.BaseQuantity);
                OnResourceClaimed?.Invoke(craftOption.Type, -craftOption.BaseQuantity * cachedResourceMultiplier);
            }
            userSessionController.DataController.UpdateProductionItems(response.batches.ToList());
            UpdateLandz?.Invoke();
            ReturnObjectToOriginalHierarquyPosition(cachedResource.transform, ref resourceOriginalHierarquyIndex);
            ClearCraftingPanelInfo();
            OpenProductionPanel();
            CloseDeployPanel();
            CloseCraftingPanel();
            UpdateBuildingPanel();
            UpdateProductionPanel();
        }
        catch (WebRequestException e)
        {
            Console.WriteLine(e);
            popupController.ErrorHandler(e.Message);
        }
        finally
        {
            lockInputPanel.Disable();
        }
    }
    private async void OnClaim(ProductionUI _productionItem)
    {
        lockInputPanel.Enable();
        _productionItem.LockAllButtons();
        claimAllButton.interactable = false;
        try
        {
            var response = await APIServices.DatabaseServer.Harvest(userSessionController.Token, _productionItem.BatchId);
            AddHeroSpecialResource("babyDragon", response.harvestResults);
            AddHeroSpecialResource("etherman", response.harvestResults);
            AddHeroSpecialResource("godjira", response.harvestResults);
            AddHeroSpecialResource("lys", response.harvestResults);
            AddHeroSpecialResource("urzog", response.harvestResults);
            AddHeroSpecialResource("yokai", response.harvestResults);
            AddHeroSpecialResource("guttx", response.harvestResults);
            if (response.harvestResults.Count > 0)
            {
                _productionItem.SetupFeedbackQuantity(response);
                _productionItem.SetupClaimFeedback(_productionItem.ClaimButton.transform);
                userSessionController.UpdateInventoryItem(_productionItem.BatchItemName, _productionItem.BatchQuantity);
                userSessionController.DataController.UpdateFeudalzBonus(response.feudalzBonus);
                OnResourceClaimed?.Invoke(_productionItem.BatchItemName, _productionItem.BatchQuantity);
            }
            UpdateUserInfo?.Invoke();
            userSessionController.UpdateLandzBuildsInfo(response.landz);
            userSessionController.DataController.UpdateProductionItems(response.batches.ToList());
            UpdateLandz?.Invoke();
            UpdateBuildingPanel();
            UpdateProductionPanel();
            LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(productionContent);
        }
        catch (WebRequestException e)
        {
            Console.WriteLine(e);
            popupController.ErrorHandler(e.Message);
        }
        finally
        {
            lockInputPanel.Disable();
            _productionItem.UnlockAllButtons();
            claimAllButton.interactable = true;
        }
    }

    private void AddHeroSpecialResource(string _resource, Dictionary<string, int> harvestResponse)
    {
        var results = harvestResponse.Where(result => result.Key.Contains(_resource)).Select(result => result.Key);
        foreach (var result in results)
        {
            var indexOfResourceName = result.IndexOf(_resource);
            var rarity = result.Substring(0, indexOfResourceName);
            userSessionController.DataController.AddHeroez(_resource, rarity);
        }
    }
    private void OnTryCancel(ProductionUI _productionItem)
    {
        popupController.EnableCancelProductionPopup(() => OnCancel(_productionItem));
    }
    private async void OnCancel(ProductionUI _productionItem)
    {
        try
        {
            lockInputPanel.Enable();
            var response = await APIServices.DatabaseServer.CancelProduction(userSessionController.Token, _productionItem.BatchId);
            userSessionController.UpdateLandzBuildsInfo(response.landz);
            userSessionController.DataController.UpdateProductionItems(response.batches.ToList());
            UpdateLandz?.Invoke();
            UpdateBuildingPanel();
            UpdateProductionPanel();
            popupController.DisablePopup();
            LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
        }
        catch (WebRequestException e)
        {
            popupController.ErrorHandler(e.Message);
            Console.WriteLine(e);
        }
        finally
        {
            lockInputPanel.Disable();
        }
    }
    private async void OnClaimAll()
    {
        lockInputPanel.Enable();
        claimAllButton.interactable = false;
        try
        {
            var response = await APIServices.DatabaseServer.HarvestAll(userSessionController.Token);
            userSessionController.DataController.UpdateFeudalzBonus(response.feudalzBonus);
            UpdateUserInfo?.Invoke();
            AddHeroSpecialResource("babyDragon", response.harvestResults);
            AddHeroSpecialResource("etherman", response.harvestResults);
            AddHeroSpecialResource("godjira", response.harvestResults);
            AddHeroSpecialResource("lys", response.harvestResults);
            AddHeroSpecialResource("urzog", response.harvestResults);
            AddHeroSpecialResource("yokai", response.harvestResults);
            AddHeroSpecialResource("guttx", response.harvestResults);
            foreach (var harvestItem in response.harvestResults)
            {
                userSessionController.UpdateInventoryItem(harvestItem.Key, harvestItem.Value);
                OnResourceClaimed?.Invoke(harvestItem.Key, harvestItem.Value);
                ProductionUI productionUI;
                if (harvestItem.Key.Contains("Dragon"))
                {
                    productionUI = productionPoolList.Find(x => x.BatchItemName.Contains("Dragon"));
                    if (productionUI == null)
                        productionUI = productionPoolList.Find(x => x.BatchItemName.Contains("Hero"));
                    
                }
                else if (harvestItem.Key.Contains("babyDragon") || harvestItem.Key.Contains("etherman") || harvestItem.Key.Contains("godjira") || harvestItem.Key.Contains("lys")
                         || harvestItem.Key.Contains("urzog") || harvestItem.Key.Contains("yokai") || harvestItem.Key.Contains("guttx"))
                    productionUI = productionPoolList.Find(x => x.BatchItemName.Contains("Hero"));
                else
                    productionUI = productionPoolList.Find(x => x.BatchItemName == harvestItem.Key);
                ResourceData data;
                if (harvestItem.Key.Contains("Dragon"))
                {
                    data = productionUI.BatchItemName.Contains("Dragon") ? userSessionController.DataController.BuildingResourceData.Find(resource => resource.ResourceName.Contains("Dragon")) : userSessionController.DataController.BuildingResourceData.Find(resource => resource.ResourceName.Contains("Hero"));
                }
                else if (harvestItem.Key.Contains("babyDragon") || harvestItem.Key.Contains("etherman") || harvestItem.Key.Contains("godjira") || harvestItem.Key.Contains("lys")
                         || harvestItem.Key.Contains("urzog") || harvestItem.Key.Contains("yokai") || harvestItem.Key.Contains("guttx"))
                    data = userSessionController.DataController.BuildingResourceData.Find(resource => resource.ResourceName.Contains("Hero"));
                else
                    data = userSessionController.DataController.BuildingResourceData.Find(resource => resource.ResourceName == harvestItem.Key);
                productionUI.FeedbackQuantity = DefineQuantityText(harvestItem);
                productionUI.ItemImage.sprite = data.ResourceImg;
                productionUI.SetupClaimFeedback(claimAllButton.transform);
                await new WaitForSeconds(0.5f);
            }
            userSessionController.UpdateLandzBuildsInfo(response.landz);
            userSessionController.DataController.UpdateProductionItems(response.batches.ToList());
            UpdateLandz?.Invoke();
            UpdateBuildingPanel();
            UpdateProductionPanel();
            LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(productionContent);
        }
        catch (WebRequestException e)
        {
            popupController.ErrorHandler(e.Message);
            Console.WriteLine(e);
        }
        finally
        {
            lockInputPanel.Disable();
            claimAllButton.interactable = true;
        }
    }

    private string DefineQuantityText(KeyValuePair<string, int> _harvestItem)
    {
        if (_harvestItem.Key.Contains("babyDragon"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "babyDragon");
        else if (_harvestItem.Key.Contains("etherman"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "etherman");
        else if (_harvestItem.Key.Contains("godjira"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "godjira");
        else if (_harvestItem.Key.Contains("lys"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "lys");
        else if (_harvestItem.Key.Contains("urzog"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "urzog");
        else if (_harvestItem.Key.Contains("yokai"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "yokai");
        else if (_harvestItem.Key.Contains("guttx"))
            return DefineHeroTextInQuantity(_harvestItem.Key, "guttx");
        else
            return "x" + _harvestItem.Value.ToString();
    }

    private string DefineHeroTextInQuantity(string _response, string _hero)
    {
        string _heroRarity = _response.Replace(_hero, "");

        switch (_heroRarity)
        {
            case "common":
                return $"Common {_hero}";
            case "uncommon":
                return $"<color=#1eff00>Uncommon</color> {_hero}";
            case "rare":
                return $"<color=#0070dd>Rare</color> {_hero}";
            case "epic":
                return $"<color=#a335ee>Epic</color> {_hero}";
            default:
                return $"";
        }
    }

    #endregion

    #region SelectionHandler

    private void ResourceSelectionHandler()
    {
        if (cachedResource != null)
        {
            SetObjectOnTopHierarquy(cachedResource.transform, ref resourceOriginalHierarquyIndex);
            foreach (var resource in resourcePoolList.Where(resource => resource != cachedResource))
                resource.gameObject.SetActive(false);
        }
        else
        {
            foreach (var resource in resourcePoolList)
                resource.gameObject.SetActive(true);
            var avaiableProductionResources = GetAllAvaiableProductionResources(userSessionController.DataController.GetAllLandz());
            CheckUnusedResourcePoolObject(avaiableProductionResources);
        }
    }
    private void CraftSelectionHandler(CraftingUI _craftingOption, bool _isOptionSelected)
    {
        if (_isOptionSelected)
        {
            foreach (var craftOption in craftingPoolList.Where(option => option != _craftingOption))
                craftOption.gameObject.SetActive(false);
        }
        else
        {
            foreach (var craftOption in craftingPoolList)
                craftOption.gameObject.SetActive(true);
        }
    }
    private void SetObjectOnTopHierarquy(Transform _targetObject, ref int cachedObjectType)
    {
        if (_targetObject.GetSiblingIndex() == 0) return;
        cachedObjectType = _targetObject.GetSiblingIndex();
        _targetObject.SetSiblingIndex(0);
    }
    private void ReturnObjectToOriginalHierarquyPosition(Transform _targetObject, ref int cachedObjectType)
    {
        _targetObject.SetSiblingIndex(cachedObjectType);
        cachedObjectType = 0;
    }
    #endregion
    #region Panels Methods
    private void OpenCraftingPanel()
    {
        craftingPanel.Enable();
        craftingHeader.SetActive(true);
        craftingExplanationText.SetActive(true);
        craftingContent.gameObject.SetActive(true);
    }
    private void CloseCraftingPanel()
    {
        craftingPanel.Disable();
        craftingHeader.SetActive(false);
        craftingExplanationText.SetActive(false);
        craftingContent.gameObject.SetActive(false);
    }
    private void UpdateCraftingInfo(BuildingsUI _resource, int _resourceMultiplier)
    {
        foreach (var craftingUI in craftingPoolList)
        {
            var foundCraftOption = _resource.CraftingOptions.Find(craftOption => craftOption.productType == craftingUI.Type);

            craftingUI.UpdateResourceQuantity(foundCraftOption, _resourceMultiplier, userSessionController);
        }
    }
    private void ClearCraftingPanelInfo()
    {
        if (cachedResource != null)
            cachedResource.SetLockButtonState(_unlock: true);
        cachedResource = null;
        cachedCraftingOption = null;
        cachedResourceMultiplier = 0;
        foreach (var craftingOption in craftingPoolList)
        {
            Destroy(craftingOption.gameObject);
        }
        craftingPoolList.Clear();
    }
    private void OpenProductionPanel()
    {
        productionHeader.gameObject.SetActive(true);
        productionContent.gameObject.SetActive(true);
        CheckClaimAllButton();
    }
    private void CloseProductionPanel()
    {
        productionHeader.gameObject.SetActive(false);
        productionContent.gameObject.SetActive(false);
    }

    private void OpenDeployPanel()
    {
        deployContent.gameObject.SetActive(true);
        deployHeader.gameObject.SetActive(true);
    }
    private void CloseDeployPanel()
    {
        deployContent.gameObject.SetActive(false);
        deployHeader.gameObject.SetActive(false);
        autoCraftSlider.value = 0;
    }
    private void CheckClaimAllButton()
    {
        if (productionPoolList.Count > 0)
        {
            foreach (var productionItem in productionPoolList)
            {
                if (productionItem.gameObject.activeInHierarchy == false || productionItem.canRecharge == false) continue;
                claimAllButton.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(productionContent);
                return;
            }
        }
        claimAllButton.gameObject.SetActive(false);
    }
    #endregion
    #region CreationMethods/HelperCreationsMethods
    private void SetResourceManagerPanel(List<FeudalzLandzAllocatedBuild> allocatedBuilds)
    {
        foreach (var build in allocatedBuilds)
        {
            if (build.currentProduction != null) continue;
            var existingOption = new List<string>();
            foreach (var resource in build.allowedCraftings)
            {
                var resourceOnPool = resourcePoolList.Find(option => option.ProductType == resource.productType);
                if (resourceOnPool != null)
                {
                    if (existingOption.Contains(resource.productType)) continue;
                    if (resourceOnPool.gameObject.activeInHierarchy == false)
                        resourceOnPool.gameObject.SetActive(true);
                    existingOption.Add(resourceOnPool.ProductType);
                    resourceOnPool.IncreaseMaxQuantity();
                }
                else
                {
                    var productCraftingOptions = build.allowedCraftings.ToList().FindAll(option => option.productType == resource.productType);
                    var clone = Instantiate(productTemplate, buildingContent);
                    clone.Setup(resource, userSessionController.DataController.BuildingResourceData, build, build.allowedCraftings.ToList());
                    resourcePoolList.Add(clone);
                    existingOption.Add(clone.ProductType);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(buildingContent);
            }
        }
        CheckUnusedResourcePoolObject(GetAllAvaiableProductionResources(userSessionController.DataController.GetAllLandz()));
    }
    private void SetProductionPanel(List<Batch> batches)
    {
        foreach (var batch in batches)
        {
            var existingBatch = productionPoolList.Find(productionItem => productionItem.BatchId.Equals(batch.batchId));
            if (existingBatch != null)
            {
                if (existingBatch.gameObject.activeInHierarchy == false)
                {
                    existingBatch.gameObject.SetActive(true);
                }
                existingBatch.Setup(batch, userSessionController.DataController.BuildingResourceData);
            }
            else
            {
                var clone = Instantiate(productionTemplate, productionContent);
                clone.Setup(batch, userSessionController.DataController.BuildingResourceData);
                clone.RegisterListeners(OnClaim, OnTryCancel);
                clone.RegisterAutoCraftListener(ChangeAutoCraftSettings);
                clone.CanvasScene = mainCanvas;
                productionPoolList.Add(clone);
            }
        }
        CheckUnusedProductionPoolObject(batches);
        LayoutRebuilder.ForceRebuildLayoutImmediate(productionContent);
    }

    private void CheckUnusedProductionPoolObject(List<Batch> productionList)
    {

        foreach (var item in productionPoolList)
        {
            var foundItem = productionList.Find(searchingItem => searchingItem.batchId.Equals(item.BatchId));
            if (foundItem == null)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
    private void CheckUnusedResourcePoolObject(List<AllowedCrafting> buildList)
    {
        foreach (var resource in resourcePoolList)
        {
            var foundResource = buildList.Find(searchingResource => searchingResource.productType == resource.ProductType);
            if (foundResource == null)
            {
                resource.gameObject.SetActive(false);
            }
        }
    }
    private List<FeudalzLandzAllocatedBuild> GetAllAvaiableProductionBuilds(List<FeudalzCombatLandz> landzList)
    {
        var builds = new List<FeudalzLandzAllocatedBuild>();
        foreach (var landz in landzList)
        {
            foreach (var allocatedBuild in landz.allocatedBuilds)
            {
                if (allocatedBuild.currentProduction != null) continue;
                var foundBuild = landz.allowedBuilds.Find(build => build.type == allocatedBuild.type);
                if (foundBuild.allowedCraftings.Length <= 0) continue;
                allocatedBuild.allowedCraftings = foundBuild.allowedCraftings;
                builds.Add(allocatedBuild);
            }
        }
        return builds;
    }
    private List<AllowedCrafting> GetAllAvaiableProductionResources(List<FeudalzCombatLandz> landzList)
    {
        var resources = new List<AllowedCrafting>();
        foreach (var landz in landzList)
        {
            foreach (var allocatedBuild in landz.allocatedBuilds)
            {
                var existingOption = new List<string>();
                if (allocatedBuild.currentProduction != null) continue;
                var foundBuild = landz.allowedBuilds.Find(build => build.type == allocatedBuild.type);
                foreach (var craftingOption in foundBuild.allowedCraftings)
                {
                    if (existingOption.Contains(craftingOption.productType)) continue;
                    existingOption.Add(craftingOption.productType);
                    resources.Add(craftingOption);
                }
            }
        }
        return resources;
    }
    private void SetCraftingOptions(BuildingsUI _resource)
    {
        int repeatResourceCount = 0;
        for (var index = 0; index < _resource.CraftingOptions.Count; index++)
        {
            if (index > 0 && _resource.CraftingOptions[index].productType == _resource.CraftingOptions[index - 1].productType)
                repeatResourceCount++;
            else
                repeatResourceCount = 0;
            var option = _resource.CraftingOptions[index];
            var foundData = userSessionController.DataController.BuildingResourceData.Find(resource => resource.ResourceName == option.productType);
            if (foundData == null) continue;
            var clone = Instantiate(craftingTemplate, craftingContent);
            clone.Setup(option, userSessionController.DataController.BuildingResourceData, GetCorretIndex(_resource.OriginalCraftingOptions, _resource.CraftingOptions[index].productType) + repeatResourceCount, userSessionController);
            craftingPoolList.Add(clone);
        }
        GreyOutResources();
    }
    private int GetCorretIndex(List<AllowedCrafting> craftingOptions, string _productType)
    {
        for (int i = 0; i < craftingOptions.Count; i++)
        {
            if (craftingOptions[i].productType == _productType)
            {
                return i;
            }
        }
        return 0;
    }
    private void GreyOutResources()
    {
        foreach (var crafting in craftingPoolList)
        {
            foreach (var resource in crafting.ResourcesNeeded)
            {
                var inventoryItem = userSessionController.DataController.Consumables.Find(item => item.name == resource.Type);
                if (inventoryItem != null && inventoryItem.quantity >= resource.BaseQuantity) continue;
                crafting.BlockResource();
                crafting.transform.SetAsLastSibling();
            }
        }
    }
    #endregion
}
