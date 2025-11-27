using CalisApp.Models;
using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    [QueryProperty(nameof(SessionId), "Id")]
    public partial class SessionDetailViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;
        private Session _session = new Session();
        private int _sessionId;
        private UserDataDto _user;
        public ICommand EnrollCommand { get; set; }

        public SessionDetailViewModel(ISessionService sessionService, IAuthService authService)
        {
            _sessionService = sessionService;
            _authService = authService;
            EnrollCommand = new Command(async () => await UserEnroll());
        }

       public Session Session
        {
            get => _session;
            set
            {
                if (value != _session)
                {
                    _session = value;
                    OnPropertyChanged(nameof(Session));
                }
            }
        }
        public int SessionId
        {
            get => _sessionId;
            set
            {
                if (_sessionId != value)
                {
                    _sessionId = value;
                    Debug.WriteLine($"✅ ID recibido para detalle: {value}");
                    LoadSessionDetails(_sessionId);
                }
            }
        }
        public UserDataDto User
        {
            get => _user;
            set
            {
                if (value != _user)
                {
                    _user = value;
                    OnPropertyChanged(nameof(User));
                }
            }

        }

        private async void LoadSessionDetails(int id)
        {
            if (id == 0) return;

            try
            {
                var fetchedSession = await _sessionService.GetSession(id);

                if (fetchedSession != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Session = fetchedSession;
                        Debug.WriteLine($"✅ Detalles de sesión {id} cargados con éxito. spots: {Session.Spots} - percent: {Session.EnrollPercent}");
                    });
                }
                else
                {
                    Debug.WriteLine($"⚠️ ERROR: Sesión {id} no encontrada en el endpoint.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR al cargar detalles de sesión: {ex.Message}");
            }
        }

        public async Task UserEnroll()
        {
            
            if (Session == null) return;
            var sesionData = await _authService.ObtenerSesion();
            UserDataDto User = new UserDataDto
            {
                Id = sesionData.Id
            };

            Debug.WriteLine($"INICIANDO ENRROLL - USER: {User.Id} - SESSION: {SessionId}");
            try
            {
                await _sessionService.Enroll(User.Id, SessionId);
                Debug.WriteLine($"ENRROL SATISFACTORIO");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR EN EL ENROLL: {ex.Message}");
            }


        }
    }
}