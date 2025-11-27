using CalisApp.Models;
using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    [QueryProperty(nameof(SessionId), "Id")]
    public partial class SessionDetailViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;
        private ObservableCollection<SessionUserDataDto> _sessionUsers = new();

        private Session _session = new Session();
        private int _sessionId;
        private UserDataDto _user;
        public ICommand EnrollCommand { get; set; }

        public SessionDetailViewModel(ISessionService sessionService, IAuthService authService)
        {
            _sessionService = sessionService;
            _authService = authService;

            _sessionUsers = new ObservableCollection<SessionUserDataDto>();
            _session = new Session();

            EnrollCommand = new Command(async () => await UserEnroll());
        }
        public ObservableCollection<SessionUserDataDto> SessionUsers
        {
            get { return _sessionUsers; }
            set 
            { 
                if(_sessionUsers != value)
                {
                    _sessionUsers = value;
                    OnPropertyChanged(nameof(SessionUsers));
                }
                
            }
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
                    Task.Run(async () => await CargarDatos(_sessionId));

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

        private async Task CargarDatos(int id)
        {
            await LoadSessionDetails(id);
            await LoadUsers(id);
        }


        private async Task LoadSessionDetails(int id)
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

        private async Task LoadUsers(int id)
        {
            try
            {
                var users = await _sessionService.GetUsers(id);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SessionUsers.Clear();
                    foreach (var user in users)
                    {
                        SessionUsers.Add(user);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"❌ Error cargando usuarios: {e.Message}");
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
                await CargarDatos(SessionId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR EN EL ENROLL: {ex.Message}");
            }


        }

    }
}