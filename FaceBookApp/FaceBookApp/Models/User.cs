using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FaceBookApp.Models
{
    public class User
    {
        public int id { get; set; }
        public string firstName{ get; set; }
        public string lastName { get; set; }
        [FileExtensions(Extensions ="jpg,jpeg,png" )]
        [DataType(DataType.ImageUrl)]
        public byte[] profileImage { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public int? mobileNo { get; set; }
         public string email { get; set; } 
        public string password { get; set; }
        public List<Post> posts { get; set; } 
    }
}