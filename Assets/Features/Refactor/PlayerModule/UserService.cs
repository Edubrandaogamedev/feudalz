using System;
using UnityEngine;

namespace Features.Refactor
{
    public class UserService : MonoBehaviour, IUserService
    {
        [SerializeField] 
        private UserSessionController _userSessionController;
        private UserInformationLoaderService _userInformationLoaderService;
        
        public event Action OnLoginSuccessful;
        public event Action OnLoginFailed;

        private void OnEnable()
        {
            _userInformationLoaderService.OnLoadFailed += OnLoadUserInformationFailed;
        }

        private void OnDisable()
        {
            _userInformationLoaderService.OnLoadFailed -= OnLoadUserInformationFailed;
        }
        
        public void Login(string userMaticAddress, string token)
        {
            _userSessionController.SetLoginCredentials(userMaticAddress,token);
            if (!_userInformationLoaderService.IsLoaded)
            {
                _userSessionController.InitializeSession(LoadUserInformation<UserInfo>(token));
            }
        }
        
        public bool IsUserSessionInitialized()
        {
            return _userSessionController.Initialized;
        }
        
        public T LoadUserInformation<T>(string token) where T : class
        {
            var userInformation = new UserInformationLoaderService().LoadFromAPI<T>("userInfo", token).Result;//path here is not necessary, just used to cache retries
            if (userInformation == null)
            {
                new Exception("The user information was not possible to be loaded");
            }
            return userInformation;
        }

        public void ReloadUserInformation()
        {
            LoadUserInformation<UserInfo>(_userSessionController.Token);
        }

        private void OnLoadUserInformationFailed()
        {
            
        }
    }
}