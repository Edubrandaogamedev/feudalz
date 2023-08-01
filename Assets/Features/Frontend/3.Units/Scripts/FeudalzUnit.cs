using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class FeudalzUnit
{
    [JsonProperty]
    public int token_id {get;set;}
    
    [JsonProperty("image")]
    public string img_url {get;set;}
    [JsonProperty]
    public string name {get;set;}
    
    [JsonProperty]
    public List<Trait> traits {get;set;}
    
    [JsonProperty]
    public FeudalzBonusStats bonus { get; set; }
    public Sprite nft_sprite {get; private set;}
    
    private readonly LoadTexture LoadTexture = new LoadTexture();
    
    public UnityEvent onLoadImageCompleted = new UnityEvent();
    public FeudalzUnit(int token_id, string img_url, List<Trait> traits, string name, FeudalzBonusStats _bonus)
    {
        this.token_id = token_id;
        this.img_url = img_url;
        this.traits = traits;
        this.name = name;
        this.bonus = _bonus;
    }
    public async void LoadNFTImage()
    {
        nft_sprite = await LoadTexture.GetTexture(img_url);
        onLoadImageCompleted?.Invoke();
    }
}
