using System.Collections.Generic;
using System.Threading.Tasks;
using Features.Refactor;
using UnityEngine;
using TMPro;

public class InfoUIUnitsManager : MonoBehaviour
{
    [Header("Template")]
    [SerializeField] UnitsUI feudalzUnitTemplate;
    [Header("Contents")]
    [SerializeField] private RectTransform humanzContent;
    [SerializeField] private RectTransform elvezContent;
    [SerializeField] private RectTransform orczContent;
    [SerializeField] private RectTransform animalzContent;

    [Header("MouseOverReferences")]
    [SerializeField] private GameObject popUpPanel;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private RectTransform canvasPreparation; //Referência do rect transform do canvas para posicionamento da popup.

    private readonly List<UnitsUI> unitsList = new List<UnitsUI>();
    public void SetUnitsInfo(UserSessionController userSessionController)
    {
        AddUnitsToPanel(userSessionController.DataController.GetAllUnitsByType(UnitType.Feudalz), humanzContent, UnitType.Feudalz.ToString().ToLower());
        AddUnitsToPanel(userSessionController.DataController.GetAllUnitsByType(UnitType.Elvez), elvezContent, UnitType.Elvez.ToString().ToLower());
        AddUnitsToPanel(userSessionController.DataController.GetAllUnitsByType(UnitType.Orcz), orczContent, UnitType.Orcz.ToString().ToLower());
        AddUnitsToPanel(userSessionController.DataController.GetAllUnitsByType(UnitType.Animalz), animalzContent, UnitType.Animalz.ToString().ToLower());
    }
    private async void AddUnitsToPanel(List<FeudalzUnit> feudalzs, RectTransform targetContent, string unitType)
    {
        foreach (FeudalzUnit feudalz in feudalzs)
        {
            UnitsUI clone = Instantiate(feudalzUnitTemplate, targetContent.transform);
            if (feudalz.nft_sprite == null)
            {
                feudalz.onLoadImageCompleted.AddListener(() => SetUnitImage(clone,feudalz));
                clone.Setup(feudalz.token_id,unitType);
                //feudalz.nft_sprite = await LoadUnitImage(feudalz.img_url);
            }
            else
                clone.Setup(feudalz.nft_sprite,feudalz.token_id,unitType);
            SetupUnitInfo(feudalz,clone,unitType);
            unitsList.Add(clone);
        }
    }

    private void SetUnitImage(UnitsUI _unitUI,FeudalzUnit _unit)
    {
        var foundUnit = unitsList.Find(unit => (unit.UnitID == _unit.token_id && unit.UnitType == _unitUI.UnitType));
        if (foundUnit != null)
            _unitUI.Setup(_unit.nft_sprite);
        _unit.onLoadImageCompleted.RemoveAllListeners();
    }
    private void SetupUnitInfo(FeudalzUnit _feudalzUnit, UnitsUI _uiUnit, string _unitType)
    {
        var mouseOverPopup = _uiUnit.GetComponent<MovablePopUp>();
        var textToConcatenate = _unitType switch
        {
            "feudalz" => $"<color=yellow>Name:</color>{_feudalzUnit.name}\n<color=yellow>Attack:</color> {_feudalzUnit.bonus.attack} <color=yellow>\nDefense:</color>{_feudalzUnit.bonus.defense}",
            "orcz" => $"<color=yellow>Name:</color> {_feudalzUnit.name}\n<color=yellow>Attack:</color> {_feudalzUnit.bonus.attack} <color=yellow>\nDefense:</color>{_feudalzUnit.bonus.defense}</color>",
            "elvez" => $"<color=yellow>Name:</color> {_feudalzUnit.name}\n<color=yellow>Attack:</color> {_feudalzUnit.bonus.attack} <color=yellow>\nDefense:</color>{_feudalzUnit.bonus.defense}</color>",
            "animalz" => $"<color=yellow>Name:</color> {_feudalzUnit.name}\n<color=yellow>Attack:</color> {_feudalzUnit.bonus.attack} <color=yellow>\nDefense:</color>{_feudalzUnit.bonus.defense}</color>",
            _ => ""
        };
        foreach (Trait trait in _feudalzUnit.traits)
        {
            textToConcatenate += $"\n<color=yellow>{trait.trait_type}</color>: {trait.value}";
        }
        mouseOverPopup.TextToPopUp = textToConcatenate;
        mouseOverPopup.PopUpPanel = popUpPanel;
        mouseOverPopup.PopUpText = popUpText;
        mouseOverPopup.CanvasPreparation = canvasPreparation;
        mouseOverPopup.ObjectToHighlight = _uiUnit.gameObject;
    }
    private async Task<Sprite> LoadUnitImage(string _unitUrl)
    {
        var loadTexture = new LoadTexture();
        Task<Sprite> LoadTextureTask = loadTexture.GetTexture(_unitUrl);
        TaskManager.RegisterTask(LoadTextureTask);
        await LoadTextureTask;
        TaskManager.DisposeTask(LoadTextureTask);
        return LoadTextureTask.Result;
    }
}
