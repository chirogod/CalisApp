using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Login(string email, string password);
        Task<string> GetTokenAsync();

        void Logout();
    }
}
