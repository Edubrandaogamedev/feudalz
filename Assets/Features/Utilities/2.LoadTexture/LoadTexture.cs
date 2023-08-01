using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LoadTexture
{
    public async Task<Sprite> GetTexture(string img_url,int retry = 0)
    {
        var maxRetries = 1;
        img_url = IPFSHandler(img_url);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(img_url.Trim()))
        {
            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                //Debug.LogError("ERRO EM DOWNLOADS: " + "URL: " + img_url + ".png");
                if (retry <= maxRetries)
                    await GetTexture(img_url,retry +1);
            }
            else if (request.isDone)
            {
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture.LoadImage(request.downloadHandler.data);
                texture.filterMode = FilterMode.Point;
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }
        }
        
        return null;
    }
    //TODO create a cancelation token on GetTexture
    public string IPFSHandler(string _uri)
    {
        if (_uri.Contains("ipfs://"))
        {
            var foundedIndex = _uri.IndexOf("/");
            var subString = _uri.Substring(foundedIndex + 1);

            //return "https://ipfs.io/ipfs" + subString;
            return "https://ipfs.feudalz.io/ipfs" + subString;
        }
        else
        {
            // _uri = _uri.Substring(0,_uri.IndexOf(".png"));
            // Debug.Log(_uri);
            return _uri;
        }
    }
}
