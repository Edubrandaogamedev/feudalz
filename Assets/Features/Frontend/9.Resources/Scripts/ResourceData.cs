using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Building/ResourceData",fileName = "NewResourceData")]
public class ResourceData : ScriptableObject
{
    [SerializeField] private string resourceName;
    [SerializeField] private Sprite resourceImg;

    public string ResourceName => resourceName;

    public Sprite ResourceImg => resourceImg;
}
