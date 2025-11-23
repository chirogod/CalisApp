namespace CalisApi.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int LimitedSpots { get; set; }
        public int Enrolled { get; set; }

        // Propiedad auxiliar para la UI
        public string CuposDisponibles => $"{Enrolled} / {LimitedSpots}";

    }
}
