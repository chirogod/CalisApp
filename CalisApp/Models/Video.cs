using System;
using System.Collections.Generic;
using System.Text;

namespace CalisApp.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
