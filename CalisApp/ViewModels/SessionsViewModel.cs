using CalisApp.Models;
using CalisApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CalisApp.ViewModels
{
    // Clase auxiliar para el calendario (Si ya la tienes en otro archivo, puedes borrarla de aquí)
    public class FechaFilter : BaseViewModel
    {
        public DateTime Fecha { get; set; }

        // Formateamos para que la Vista solo muestre lo necesario
        public string DiaSemana => Fecha.ToString("ddd"); // Ej: Lun, Mar
        public string DiaNumero => Fecha.Day.ToString();  // Ej: 27, 28

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
    }

    public class SessionsViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;

        // 1. Colección visible en pantalla (se filtra según la fecha)
        private ObservableCollection<Session> _sessions;

        // 2. Colección de respaldo (contiene TODAS las sesiones descargadas de la API)
        private List<Session> _allSessions = new();

        // 3. Colección para los botones de fechas de arriba
        public ObservableCollection<FechaFilter> DiasCalendario { get; set; } = new();

        // COMANDOS
        public ICommand GoToDetailCommand { get; }
        public ICommand SelectDateCommand { get; }

        public SessionsViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            _sessions = new ObservableCollection<Session>();

            GoToDetailCommand = new Command<Session>(async (session) => await GoToDetailAsync(session));
            SelectDateCommand = new Command<FechaFilter>(FiltrarPorFecha);

            GenerarDias();

            Task.Run(async () => await LoadSessionsAsync());
        }

        public ObservableCollection<Session> Sessions
        {
            get { return _sessions; }
            set
            {
                if (_sessions != value)
                {
                    _sessions = value;
                    OnPropertyChanged(nameof(Sessions));
                }
            }
        }

        private void GenerarDias()
        {
            var hoy = DateTime.Now;
            DiasCalendario.Clear();
            for (int i = 0; i < 7; i++)
            {
                DiasCalendario.Add(new FechaFilter
                {
                    Fecha = hoy.AddDays(i),
                    IsSelected = i == 0
                });
            }
        }

        private void FiltrarPorFecha(FechaFilter fechaTocada)
        {
            if (fechaTocada == null) return;

            foreach (var dia in DiasCalendario)
            {
                dia.IsSelected = false;
            }
            fechaTocada.IsSelected = true;

            FiltrarLista(fechaTocada.Fecha);
        }

        private void FiltrarLista(DateTime fecha)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Sessions.Clear();

                var filtradas = _allSessions
                    .Where(s => s.Date.Date == fecha.Date)
                    .OrderBy(s => s.Date)
                    .ToList();

                foreach (var s in filtradas)
                {
                    Sessions.Add(s);
                }

                if (!Sessions.Any())
                {
                    Debug.WriteLine($"ℹ️ No hay clases para el día {fecha.ToShortDateString()}");
                }
            });
        }

        public async Task LoadSessionsAsync()
        {
            try
            {
                Debug.WriteLine("--- INTENTANDO CONEXIÓN CON LA API ---");

                var sessionList = await _sessionService.GetAll();

                if (sessionList != null)
                {
                    _allSessions = sessionList.ToList();

                    var fechaSeleccionada = DiasCalendario.FirstOrDefault(d => d.IsSelected)?.Fecha ?? DateTime.Now;

                    FiltrarLista(fechaSeleccionada);

                    Debug.WriteLine($"✅ ÉXITO: Se descargaron {_allSessions.Count} sesiones en total.");
                }
                else
                {
                    Debug.WriteLine("⚠️ INFO: La API devolvió una lista vacía.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ ERROR CRÍTICO DE CONEXIÓN:");
                Debug.WriteLine($"    Mensaje: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => _sessions.Clear());
            }
            finally
            {
                Debug.WriteLine("--- FIN DE LA OPERACIÓN ---");
            }
        }

        private async Task GoToDetailAsync(Session session)
        {
            if (session == null) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                 string route = $"SessionDetail?Id={session.Id}";
                await Shell.Current.GoToAsync(route);
            });
        }
    }
}