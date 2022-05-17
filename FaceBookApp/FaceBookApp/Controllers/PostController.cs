using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceBookApp.Models;
using FaceBookApp.ViewModels;

namespace FaceBookApp.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext _context;

        public PostController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ActionResult AddPost() {
            int id = (int)Session["userID"];
            var userPost = new PostandUserViewModel()
            {
                user = _context.Users.SingleOrDefault(c => c.id == id)
        };

            return View(userPost);
        }   
        
        
        [HttpPost]
        public ActionResult PublishPost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Profile","User",new { id = post.userId });
        }

        [HttpPost]
        public void HideOrViewPost(int id, bool hidden)
        {
            var post = _context.Posts.SingleOrDefault(p => p.id == id);

            if (hidden)
                post.hidden = false;
            else
                post.hidden = true;

            _context.SaveChanges();
        }
        [HttpPost]
        public void likeAjax (int id)
        {
            var post = _context.Posts.SingleOrDefault(p => p.id == id);
            post.likes += 1;
            _context.SaveChanges();

        }
        [HttpPost]
        public void dislikeAjax(int id)
        {
            var post = _context.Posts.SingleOrDefault(p => p.id == id);
            post.dislikes += 1;
            _context.SaveChanges();

        }
        [HttpPost]
        public ActionResult addComment (UserFreindsPostsViewModel vm)
        {
            Comment comment = new Comment()
            {
                PostNum = vm.comment.PostNum,
                Text = vm.comment.Text,
                author = vm.comment.author
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("Profile", "User");
        }
        public ActionResult addComment_myprof(UserFreindsPostsViewModel vm)
        {
            Comment comment = new Comment()
            {
                PostNum = vm.comment.PostNum,
                Text = vm.comment.Text,
                author = vm.comment.author
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("MyProfilePage", "User");
        }





    }
}