using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceBookApp.Models;
using FaceBookApp.ViewModels;
using System.Text.RegularExpressions;

namespace FaceBookApp.Controllers
{
    [System.Runtime.InteropServices.Guid("3E19AFC9-CEC9-4191-B0BF-F8F62E1901B5")]
    public class UserController : Controller
    {

        private ApplicationDbContext _context;


        public UserController()
        {
            _context = new ApplicationDbContext();
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ActionResult Log_in()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(User user)
        {
            var dbuser = _context.Users.SingleOrDefault(du => du.email == user.email && du.password == user.password);
            if (Object.ReferenceEquals(dbuser, null))
            {
                return RedirectToAction("FailedLogin", "User");
            }
            else
            {
                Session["userID"] = dbuser.id;
                return RedirectToAction("Profile", "User");
            }
        }


        public ActionResult FailedLogin()
        {
            return View();
        }
        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveAccount(User user)
        {
        _context.Users.Add(user);
            _context.SaveChanges();
            Session["userID"] = user.id;

            return RedirectToAction("Profile", "User", new { id = Session["userID"] });
        }


        [HttpPost]
        public ActionResult EditAcc (User newinfo)
        {
            User oldinfo = _context.Users.Find(newinfo.id);
            oldinfo.email = newinfo.email;
            oldinfo.password = newinfo.password;
            oldinfo.country = newinfo.country;
            oldinfo.mobileNo = newinfo.mobileNo;
            oldinfo.city = newinfo.city;
            oldinfo.firstName = newinfo.firstName;
            oldinfo.lastName = newinfo.lastName;
            oldinfo.profileImage = newinfo.profileImage; 
            _context.SaveChanges();
            return RedirectToAction("Settings", "User", new { id = newinfo.id });

        }
        public ActionResult Settings ()
        {
            int id = (int)Session["userID"];
            User user = fetchUserFromId(id);
            return View(user);
        }

        public ActionResult Profile()
        {

            int id = (int)Session["userID"];
            var freindsAccounts = fetchUsersFriendsAccounts();
            var freindsAccountsAndPostsList = new List<UserPostsViewModel>();

            foreach (var freind in freindsAccounts)
            {
                var element = new UserPostsViewModel()
                {
                    user = freind,
                    posts = _context.Posts.Where(c => c.userId == freind.id && c.hidden==false ).ToList()
                };
                freindsAccountsAndPostsList.Add(element);
            }



            var userFreindsPostsVm = new UserFreindsPostsViewModel()
            {
                user = fetchUserFromId((int)Session["userID"]),
                FreindsAndPosts = freindsAccountsAndPostsList,
                comments = _context.Comments.ToList()
            };

            return View(userFreindsPostsVm);
        }


        public ActionResult MyProfilePage()
        {
            int id = (int)Session["userID"];
            var usermodel = new UserPostsViewModel()
            {
                user = fetchUserFromId(id),
                posts = fetchPosts(id).ToList()
            };
            var userPostsVm = new ProfileViewModel()
            {
                User = usermodel,
                comments = _context.Comments.ToList()
            };

          

            return View(userPostsVm);
        }
        public ActionResult FriendProfile (int friend_id)
        {
            int user_id = (int)Session["userID"];
            var friendmodel = new UserPostsViewModel()
            {
                user = fetchUserFromId(friend_id),
                posts = fetchPosts(friend_id).ToList()
            };
            var usermodel = new UserPostsViewModel()
            {
                user = fetchUserFromId(user_id),
                posts = fetchPosts(user_id).ToList()
            };
            var friendprofile = new ProfileViewModel()
            {
                
                friend = friendmodel,
                User = usermodel,
                comments = _context.Comments.ToList(),

            };
            return View(friendprofile);
            


        }
      


        public ActionResult FreindsPage()
        {
            int id = (int)Session["userID"];
           var userFriends = new UserFreindsViewModel()
            {
                user = fetchUserFromId(id),
                friends = fetchUsersFriendsAccounts().ToList()

            };   
            return View(userFriends);
        }


        
       
        public ActionResult FindFriends(string searchqry = "")
        {
            int id = (int)Session["userID"];
            var vm = new findFriendsViewModel()
            {
                user = fetchUserFromId(id),
                friends = _context.Users.ToList(),
                search = searchqry

            };
            return View(vm);

        }

        public ActionResult ShowRequests()
        {
            int id = (int)Session["userID"];
            var requestfriendsids = _context.UserRequests.Where(c => c.requestId == id);
            var requestedAccounts = _context.Users.Where(p => requestfriendsids.Any(y => y.userId == p.id)).ToList();

            var vm = new UserFreindsViewModel()
            {

                user = fetchUserFromId(id),
                friends = requestedAccounts

            };

            return View(vm);

        }

        [HttpPost]
        public void SendRequestAjax(int r_user)
        {
            int id = (int)Session["userID"];
            if (_context.UserRequests.SingleOrDefault(c => c.userId == id && c.requestId == r_user) != null)
                return;

            if (_context.UserRequests.SingleOrDefault(c => c.userId == r_user && c.requestId == id) != null)
                return;

            var userRequest = new UserRequest()
            {
                userId = id,
                user = _context.Users.SingleOrDefault(u => u.id == id),
                requestId = r_user,
               // request = _context.Users.SingleOrDefault(u => u.id == r_user),
            };

            _context.UserRequests.Add(userRequest);
            _context.SaveChanges();
        }

        [HttpPost]
        public void RespondToRequestAjax(int r_user, bool accept)
        {
            int id = (int)Session["userID"];
            var req = _context.UserRequests.Single(c => c.userId == r_user && c.requestId == id);

            _context.UserRequests.Attach(req);
            _context.UserRequests.Remove(req);
            _context.SaveChanges();

            if (accept)
            {
                var user1FriUser2 = new UserFriend()
                {
                    userId = r_user,
                    user = _context.Users.SingleOrDefault(u => u.id == r_user),
                    friendId = id,
                   // friend = _context.Users.SingleOrDefault(u => u.id == id),
                };

                var user2FriUser1 = new UserFriend()
                {
                    userId = id,
                    user = _context.Users.SingleOrDefault(u => u.id == id),
                    friendId = r_user,
                  //  friend = _context.Users.SingleOrDefault(u => u.id == r_user),
                };

                _context.UserFriends.Add(user1FriUser2);
                _context.UserFriends.Add(user2FriUser1);
                _context.SaveChanges();
            }
          


        }

    


        /************************************ Helping Methods ****************************************************/
        private IEnumerable<Post> fetchPosts(int id)
        {
            var posts = _context.Posts.Where(c => c.userId == id);
            return posts;
        }

        private IEnumerable<UserFriend> fetchFriendsIds()
        {
            int id = (int)Session["userID"];
            var friendsIds = _context.UserFriends.Where(c => c.userId == id);
            return friendsIds;
        }

        private IEnumerable<Post> fetchFriendsPosts()
        {

            var friendsIds = fetchFriendsIds();
            var friendsPosts = _context.Posts.Where(p => friendsIds.Any(y => y.friendId == p.userId)).ToList();

            return friendsPosts;
        }

        private IEnumerable<User> fetchUsersFriendsAccounts()
        {
            var friendsIds = fetchFriendsIds();
            var friendsAccounts = _context.Users.Where(p => friendsIds.Any(y => y.friendId == p.id)).ToList();

            return friendsAccounts;
        }

        private IEnumerable<User> fetchNonFriendsUsersAccounts()
        {
            int id = (int)Session["userID"];
            var friendsIds = fetchFriendsIds();
            var NonfriendsAccounts = _context.Users.Where(p => !friendsIds.Any(y => y.friendId == p.id) && p.id != id).ToList();

            var requests = _context.UserRequests.Where(c => c.userId == id);
            var requestedIds = _context.UserRequests.Where(c => c.requestId == id);

            var NonFreinds_list = _context.Users.Where(p => 
            !friendsIds.Any(y => y.friendId == p.id)
            && p.id != id
            && !requests.Any(y => y.requestId == p.id)
            && !requestedIds.Any(y => y.userId == p.id)
            ).ToList();

            return NonFreinds_list;

        }

        private User fetchUserFromId(int id)
        {
            var user = _context.Users.SingleOrDefault(c => c.id == id);
            return user;
        }

    }
}