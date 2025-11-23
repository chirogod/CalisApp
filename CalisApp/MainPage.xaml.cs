// MainPage.xaml.cs
using CalisApp.Services;
using CalisApp.Services.Interfaces;
using System.Diagnostics; // Necesario para Debug.WriteLine
using System.Linq;

namespace CalisApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ISessionService _sessionService;

        public MainPage(ISessionService sessionService)
        {
            InitializeComponent();
            _sessionService = sessionService;
        }

        // Firma CORREGIDA: debe ser 'async void'
        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            // 1. Iniciar estado de carga
            CounterBtn.IsEnabled = false;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            try
            {
                Debug.WriteLine("--- INTENTANDO CONEXIÓN CON LA API ---");

                // 2. Llamada a la API
                var sessionList = await _sessionService.GetAll();

                // 3. Asignar los resultados a la CollectionView
                sesiones.ItemsSource = sessionList;

                if (sessionList != null && sessionList.Any())
                {
                    // ⭐️ ÉXITO: Mensaje solo en la Salida de Depuración
                    Debug.WriteLine($"✅ ÉXITO: Se cargaron {sessionList.Count()} sesiones.");
                }
                else
                {
                    // ⭐️ ÉXITO (Lista vacía): Mensaje solo en la Salida de Depuración
                    Debug.WriteLine("⚠️ INFO: La API devolvió una lista vacía. Verifica la base de datos.");
                }
            }
            catch (Exception ex)
            {
                // ⭐️ ERROR: Mensaje solo en la Salida de Depuración
                Debug.WriteLine("❌ ERROR CRÍTICO DE CONEXIÓN:");
                Debug.WriteLine($"   Mensaje: {ex.Message}");
                Debug.WriteLine($"   Revisa: URL, firewall, o que la API esté encendida.");
                // Opcional: Limpiar la lista para no mostrar datos antiguos
                sesiones.ItemsSource = Enumerable.Empty<object>();
            }
            finally
            {
                // 4. Finalizar estado de carga
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                CounterBtn.IsEnabled = true;
                Debug.WriteLine("--- FIN DE LA OPERACIÓN ---");
            }
        }
    }
}