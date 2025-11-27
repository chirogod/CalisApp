using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Models.DTOs
{
    public class UserDataDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
