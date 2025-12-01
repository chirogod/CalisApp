using CalisApp.Models;
using CalisApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<Session>> GetAll();

        Task<Session> GetSession(int sessionId);
        Task<SessionFullDetailDto> GetSessionFullDetails(int sessionId);
        Task<Session> Enroll(string userId, int sessionId);
        Task UnEnroll(int sessionId);
        Task<List<SessionUserDataDto>> GetUsers(int sessionId);
    }
}
