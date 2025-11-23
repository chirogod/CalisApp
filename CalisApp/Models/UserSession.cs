namespace CalisApi.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }

        public Session Session { get; set; }
        public User User { get; set; }

    }
}
