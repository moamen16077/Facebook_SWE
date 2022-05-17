using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FaceBookApp.Models;

namespace FaceBookApp.ViewModels
{
    public class ProfileViewModel
    {
        public UserPostsViewModel friend { get; set; }
        public UserPostsViewModel User { get; set; }
        public List<Comment> comments { get; set; }
        public Comment comment { get; set; }
    }
}