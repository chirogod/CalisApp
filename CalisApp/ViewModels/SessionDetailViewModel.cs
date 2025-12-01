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
        private bool _enrollButtonVisible = true;
        private bool _unEnrollButtonVisible = false;
        public ICommand EnrollCommand { get; set; }
        public ICommand UnEnrollCommand { get; set; }

        public SessionDetailViewModel(ISessionService sessionService, IAuthService authService)
        {
            _sessionService = sessionService;
            _authService = authService;

            _sessionUsers = new ObservableCollection<SessionUserDataDto>();
            _session = new Session();


            EnrollCommand = new Command(async () => await UserEnroll());
            UnEnrollCommand = new Command(async () => await UserUnEnroll());
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
        public bool EnrollButtonVisible
        {
            get => _enrollButtonVisible;
            set
            {
                if (value != _enrollButtonVisible)
                {
                    _enrollButtonVisible = value;
                    OnPropertyChanged(nameof(EnrollButtonVisible));
                }
            }
        }
        public bool UnEnrollButtonVisible
        {
            get => _unEnrollButtonVisible;
            set
            {
                if (value != _unEnrollButtonVisible)
                {
                    _unEnrollButtonVisible = value;
                    OnPropertyChanged(nameof(UnEnrollButtonVisible));
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
        }


        private async Task LoadSessionDetails(int id)
        {
            if (id == 0) return;

            try
            {
                var fetchedSession = await _sessionService.GetSessionFullDetails(id);
                var SessionUser = await _authService.ObtenerSesion();
                int UserId = int.Parse(SessionUser.Id);

                if (fetchedSession != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Session = fetchedSession.Session;
                        SessionUsers.Clear();
                        foreach (var user in fetchedSession.EnrolledUsers)
                        {
                            SessionUsers.Add(user);
                        }

                        EnrollButtonVisible = !fetchedSession.EnrolledUsers.Any(p => p.Id == UserId);
                        UnEnrollButtonVisible = fetchedSession.EnrolledUsers.Any(p => p.Id == UserId);

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
                var me = new SessionUserDataDto
                {
                    Id = int.Parse(sesionData.Id),
                    FullName = sesionData.FullName 
                };
                SessionUsers.Add(me);

                EnrollButtonVisible = false;
                UnEnrollButtonVisible = true;
                Session.Enrolled++;
                OnPropertyChanged(nameof(Session));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR EN EL ENROLL: {ex.Message}");
            }
        }

        public async Task UserUnEnroll()
        {
            var sesionData = await _authService.ObtenerSesion();
            Debug.WriteLine($"INTENTANDO UNROLL de user {sesionData.Id} de la sesion {SessionId} ");
            try
            {
                await _sessionService.UnEnroll(SessionId);
                Debug.WriteLine($"UNENRROL SATISFACTORIO");
                var userToRemove = SessionUsers.FirstOrDefault(u => u.Id == int.Parse(sesionData.Id));

                if (userToRemove != null)
                {
                    SessionUsers.Remove(userToRemove);
                }
                ;
                UnEnrollButtonVisible = false;
                EnrollButtonVisible = true;
                Session.Enrolled--;
                OnPropertyChanged(nameof(Session));
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"ERROR EN EL UNENROLL: {ex.Message}");
            }
            
        }


    }
}