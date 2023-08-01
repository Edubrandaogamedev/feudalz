using UnityEngine;
using UnityEngine.UI;
public class HyperlinkManager : MonoBehaviour
{
	[SerializeField] private string linkAdress = "https://opensea.io/collection/feudalz";
    [SerializeField] private Button button;
	public void OpenLink() 
	{
		Application.OpenURL(linkAdress);
	}
}