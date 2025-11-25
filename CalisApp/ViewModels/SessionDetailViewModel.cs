using CalisApp.Models;
using CalisApp.Services.Interfaces;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace CalisApp.ViewModels
{
    [QueryProperty(nameof(SessionId), "Id")]
    public partial class SessionDetailViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;
        private Session _session = new Session();
        private int _sessionId;

        public SessionDetailViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // 1. Propiedad que almacena el objeto de sesión completo
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

        private async void LoadSessionDetails(int id)
        {
            if (id == 0) return;

            try
            {
                var fetchedSession = await _sessionService.GetSession(id);

                if (fetchedSession != null)
                {
                    // 🚨 CORRECCIÓN CRÍTICA: Envolver la asignación en el Main Thread.
                    // Esto garantiza que la notificación de OnPropertyChanged sea procesada por la UI.
                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Asignamos el objeto cargado. El setter llama a OnPropertyChanged.
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
    }
}