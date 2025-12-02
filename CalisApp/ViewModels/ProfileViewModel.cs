using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using CalisApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        private UserDataDto _usuario;
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;
            Task.Run(async () => await CargarDatosUsuario());
            LogoutCommand = new Command(async () => await Logout());
        }
        public UserDataDto Usuario
        {
            get { return _usuario; }
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged(nameof(Usuario));
                }

            }
        }
        private async Task Logout()
        {
            _authService.Logout();
            var loginPage = _serviceProvider.GetRequiredService<LoginView>();

            Application.Current.MainPage = loginPage;
        }

        private async Task CargarDatosUsuario()
        {
            var dataSession = await _authService.ObtenerSesion();
            Usuario = new UserDataDto
            {
                Id = dataSession.Id,
                FullName = dataSession.FullName,
                Email = dataSession.Email,
                Role = dataSession.Role
            };
        }
    }
}
