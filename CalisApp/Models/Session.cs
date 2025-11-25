namespace CalisApp.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int LimitedSpots { get; set; }
        public int Enrolled { get; set; }
        public int FreeSpots
        {
            get
            {
                return LimitedSpots - Enrolled;
            }
        }

        public string Spots
        {
            get
            {
                return $"{Enrolled}/{LimitedSpots}";
            }
        }

        public decimal EnrollPercent
        {
            get
            {
                if (LimitedSpots > 0)
                {
                    return ((decimal)Enrolled / LimitedSpots);
                }
                return 0;
            }
        }


    }
}
