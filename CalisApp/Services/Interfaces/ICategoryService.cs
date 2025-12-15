using CalisApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
    }
}
