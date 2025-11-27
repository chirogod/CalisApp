using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private UserDataDto _usuario;
        public HomeViewModel(IAuthService authService)
        {
            _authService = authService;
            Task.Run(async () => await CargarDatosUsuario());

        }

        public UserDataDto Usuario
        {
            get { return _usuario; }
            set
            {
                if(_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged(nameof(Usuario));
                }
            }
        }

        public async Task CargarDatosUsuario()
        {
            var datosSesion = await _authService.ObtenerSesion();

            if (datosSesion != null)
            {
                Usuario = new UserDataDto
                {
                    Id = datosSesion.Id,
                    FullName = datosSesion.FullName,
                    Email = datosSesion.Email,
                    Role = datosSesion.Role
                };
            }
        }

    }
}
