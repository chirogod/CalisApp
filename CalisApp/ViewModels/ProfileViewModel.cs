using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    public class ProfileViewModel
    {
        private readonly IAuthService _authService;
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(IAuthService authService)
        {
            _authService = authService;

            LogoutCommand = new Command(async () => await Logout());
        }

        private async Task Logout()
        {
            _authService.Logout();
            await Shell.Current.GoToAsync("Login");
        }
    }
}
