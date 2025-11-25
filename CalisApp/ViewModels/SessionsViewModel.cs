using CalisApp.Models;
using CalisApp.Services;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using System.Windows.Input; // Necesario para MainThread

namespace CalisApp.ViewModels
{
    public class SessionsViewModel : BaseViewModel
    {
        private ObservableCollection<Session> _sessions;
        private readonly ISessionService _sessionService;

        public ICommand GoToDetailCommand { get; }


        public SessionsViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            _sessions = new ObservableCollection<Session>();

            GoToDetailCommand = new Command<Session>(async (session) => await GoToDetailAsync(session));

            Task.Run(async () => await LoadSessionsAsync());
        }
        private async Task GoToDetailAsync(Session session)
        {
            if (session == null)
                return;

            // Aseguramos la navegación en el hilo principal
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // La misma lógica de navegación, pero ahora ejecutada por el Command
                string route = $"SessionDetail?Id={session.Id}";
                await Shell.Current.GoToAsync(route);
            });
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

        public async Task LoadSessionsAsync()
        {
            try
            {
                Debug.WriteLine("--- INTENTANDO CONEXIÓN CON LA API ---");

                var sessionList = await _sessionService.GetAll();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Debug.WriteLine("--- INTENTANDO CONEXIÓN CON LA API ---");
                    _sessions.Clear(); 

                    if (sessionList != null)
                    {
                        foreach (var session in sessionList)
                        {
                            _sessions.Add(session);
                        }
                    }

                    if (_sessions.Any())
                    {
                        Debug.WriteLine($"✅ ÉXITO: Se cargaron {_sessions.Count} sesiones.");
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ INFO: La API devolvió una lista vacía. Verifica la base de datos.");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ ERROR CRÍTICO DE CONEXIÓN:");
                Debug.WriteLine($"    Mensaje: {ex.Message}");
                Debug.WriteLine($"    Revisa: URL, firewall, o que la API esté encendida.");

                MainThread.BeginInvokeOnMainThread(() => _sessions.Clear());
            }
            finally
            {
                Debug.WriteLine("--- FIN DE LA OPERACIÓN ---");
            }
        }
    }
}