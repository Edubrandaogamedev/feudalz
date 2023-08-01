using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading.Tasks;
using Features.Refactor;
using UnityEngine.Serialization;

#if UNITY_WEBGL
public class MetamaskLogin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();
    private string account = "";
    [FormerlySerializedAs("userSessionManager")]
    [FormerlySerializedAs("userSessionDataManager")]
    [Header("Data")]
    [SerializeField] private UserSessionController userSessionController;

    [Header("Debug")]
    [SerializeField] private string debugAccount;

    private IUserService _userService;
    public async void TryLogin()
    {
#if UNITY_EDITOR
        
        Nonce nonce = await APIServices.DatabaseServer.GetNonce(debugAccount);
        // Debug.Log("Nonce: " + nonce.nonce);
        string signature = "";
        AuthenticationToken login = await APIServices.DatabaseServer.LoginAuthentication(debugAccount, signature);
        // Debug.Log("TOKEN: " + login.token);
        //userSessionDataManager.OnLoginSuccessful(debugAccount, login.token);
        _userService.Login(debugAccount,login.token);
#else
        // Web3Connect();  
        // await OnConnected();
        // Nonce nonce = await APIServices.DatabaseServer.GetNonce(account);
        // // Debug.Log("Nonce: " + nonce.nonce);
        // string signature = await WebGLSignIn(nonce.nonce.ToString());
        // // Debug.Log("Signature: " + signature);
        // AuthenticationToken login = await APIServices.DatabaseServer.LoginAuthentication(account, signature);
        // // Debug.Log("Token: " + login.token);
        // userSessionDataManager.OnLoginSuccessful(account, login.token);
#endif
    }
    private async Task OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };
    }
    private async Task<string> WebGLSignIn(string _nonce)
    {
        try
        {
            string response = await Web3GL.Sign($"I am signing my one-time nonce: {_nonce}");
            return response;
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return null;
        }
    }
}
#endif
