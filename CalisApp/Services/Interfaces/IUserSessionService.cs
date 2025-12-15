using CalisApp.Models;

namespace CalisApp.Services.Interfaces
{
    public interface IUserSessionService
    {
        Task<Session> Enroll(string userId, int sessionId);
        Task UnEnroll(int sessionId);
    }
}
