using System;

namespace Features.Refactor
{
    public interface IUserService
    {
        public event Action OnLoginSuccessful;
        public event Action OnLoginFailed;
        
        public void Login(string userMaticAddress, string token);
        public bool IsUserSessionInitialized();
        public T LoadUserInformation<T>(string token) where T : class;
        public void ReloadUserInformation();
    }
}