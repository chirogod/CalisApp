using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Models.DTOs
{
    public class SessionFullDetailDto
    {
        public Session Session { get; set; }

        public List<SessionUserDataDto> EnrolledUsers { get; set; }
    }
}
