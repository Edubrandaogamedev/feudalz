using UnityEngine;
[CreateAssetMenu(menuName = "Building/Data", fileName = "NewBuildingData")]
public class BuildingData : ScriptableObject
{
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite buildingImg;
    [Multiline]
    [SerializeField] private string buildingDescription;
    public Sprite BuildingImg => buildingImg;
    public string BuildingDescription => buildingDescription;
    public string BuildingName => buildingName;
}
