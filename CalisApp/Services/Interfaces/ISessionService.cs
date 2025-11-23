using CalisApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<Session>> GetAll();
    }
}
