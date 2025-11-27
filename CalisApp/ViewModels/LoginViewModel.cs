using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        public ICommand LoginCommand { get; private set; }
        private LoginDto _LoginDto;
        private string _ErrorMessage;
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await Login());
            LoginDto = new LoginDto();
        }

        public LoginDto LoginDto
        {
            get { return _LoginDto; }
            set
            {
                if(_LoginDto != value)
                {
                    _LoginDto = value;
                    OnPropertyChanged(nameof(LoginDto));
                }
            }
        }
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(LoginDto.Email) || string.IsNullOrWhiteSpace(LoginDto.Password))
            {
                return;
            }
            Debug.WriteLine($"✅ INICIANDO AUTENTICACION: {LoginDto.Email}-{LoginDto.Password}");
            var success = await _authService.Login(LoginDto.Email, LoginDto.Password);
            if (success)
            {
                Debug.WriteLine($"✅ login exitoso");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            }
            else
            {
                Debug.WriteLine($"Login fallido");
                ErrorMessage = "Email o contrasena incorrectos.";
            }
        }
    }
}
