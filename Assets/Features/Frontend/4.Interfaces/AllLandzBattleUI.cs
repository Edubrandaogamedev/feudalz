using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using System.Threading;

public class AllLandzBattleUI : MonoBehaviour
{
    [SerializeField] private Image playerLandzImg;
    [SerializeField] private Image playerUniqueLandzImg;
    [SerializeField] private GifTexture playerGifTextureUnique;
    [SerializeField] private TextMeshProUGUI playerLandzId;
    [SerializeField] private TextMeshProUGUI playerLandzCombatInfo;
    [SerializeField] private TextMeshProUGUI playerLandzDice;
    [SerializeField] private TextMeshProUGUI playerLandzTotalAttack;
    [SerializeField] private Image enemyLandzImg;
    [SerializeField] private Image enemyUniqueLandzImg;
    [SerializeField] private GifTexture enemyGifTextureUnique;
    [SerializeField] private TextMeshProUGUI enemyLandzId;
    [SerializeField] private TextMeshProUGUI enemyLandzCombatInfo;
    [SerializeField] private TextMeshProUGUI enemyLandzDice;
    [SerializeField] private TextMeshProUGUI enemyLandzTotalAttack;
    [SerializeField] private TextMeshProUGUI headerResult;
    [SerializeField] private Image panelFrame;
    [SerializeField] private Sprite[] spritesPanel;
    private string enemyImgUrl;
    private LoadTexture loadTexture = new LoadTexture();
    private Task loadingTexture;
    public void Setup(AttackResult _result, UserSessionController userSessionController)
    {
        var foundLandz = userSessionController.CombatLandzs.Find(landz => landz.tokenId == _result.attacker.land.id);
        playerLandzId.text = "Landz\n" + _result.attacker.land.name;
        playerLandzCombatInfo.text = "Attack Bonus\n" + foundLandz.attackBonus.ToString();
        playerLandzDice.text = "You rolled\n" + _result.attacker.dice.ToString();
        playerLandzTotalAttack.text = "Total Attack\n" + _result.attacker.total.ToString();
        enemyLandzId.text = "Landz\n" + _result.defender.land.name;
        enemyLandzCombatInfo.text = "Defense Bonus\n" + (_result.defender.total - _result.defender.dice).ToString();
        enemyLandzDice.text = "Enemy rolled\n" + _result.defender.dice.ToString();
        enemyLandzTotalAttack.text = "Total Defense\n" + _result.defender.total.ToString();
        enemyImgUrl = _result.defender.land.image;
        if (foundLandz.uniqueLandz)
        {
            playerLandzImg.gameObject.SetActive(false);
            playerUniqueLandzImg.gameObject.SetActive(true);
            playerGifTextureUnique.m_GifFileNameOrUrl = loadTexture.IPFSHandler(foundLandz.sprite_url);

        }
        else
            playerLandzImg.sprite = foundLandz.nft_sprite;

        if (_result.winner == _result.attacker.address)
        {
            panelFrame.sprite = spritesPanel[0];
            headerResult.text = "VICTORY!";
        }
        else
        {
            panelFrame.sprite = spritesPanel[1];
            headerResult.text = "DEFEAT!";
        }
        loadingTexture = LoadEnemyLandzTexture(_result, userSessionController);
    }
    public async void DestroyObject()
    {
        if (loadingTexture.IsCompleted == false)
            await loadingTexture;
        loadingTexture.Dispose();
        Destroy(this.gameObject);
    }
    private async Task LoadEnemyLandzTexture(AttackResult _result, UserSessionController userSessionController)
    {
        try
        {
            if (userSessionController.DataController.IsUniqueLandzUnique(_result.defender.land.tokenId))
            {
                enemyLandzImg.gameObject.SetActive(false);
                enemyUniqueLandzImg.gameObject.SetActive(true);
                enemyGifTextureUnique.m_GifFileNameOrUrl = loadTexture.IPFSHandler(enemyImgUrl);
            }
            else
            {
                var loadedEnemySprite = await loadTexture.GetTexture(enemyImgUrl);
                if (loadedEnemySprite != null)
                    enemyLandzImg.sprite = loadedEnemySprite;
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}
