using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FaceBookApp.Models;

namespace FaceBookApp.ViewModels
{
    public class findFriendsViewModel
    {
        public User user { get; set; }
        public List<User> friends { get; set; }
        public string search { get; set; } = "";
    }
}