using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FaceBookApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int PostNum { get; set; }
        public string author { get; set; }
    }
}