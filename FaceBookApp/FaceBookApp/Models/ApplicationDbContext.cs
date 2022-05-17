using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace FaceBookApp.Models
{   
    public class ApplicationDbContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFriend>().HasKey(u => new { u.userId, u.friendId });
            modelBuilder.Entity<UserRequest>().HasKey(u => new { u.userId, u.requestId });
          }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }
        public DbSet<Comment> Comments { get; set; }



    }


}