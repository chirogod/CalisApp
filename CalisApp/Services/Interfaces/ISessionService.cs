using CalisApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<Session>> GetAll();

        Task<Session> GetSession(int sessionId);

        Task<Session> Enroll(string userId, int sessionId);
    }
}
