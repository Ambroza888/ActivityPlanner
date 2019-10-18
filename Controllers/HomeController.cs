using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using c_sharp_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace c_sharp_proj.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        // ---------------------------------------------------------------------
        // Index
        // ---------------------------------------------------------------------
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("index");
        }
        // ---------------------------------------------------------------------
        // Registrating to db
        // ---------------------------------------------------------------------
        [HttpPost("/Register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email","Email already in use!");
                    return View("Index");
                }
                else if(dbContext.Users.Any(u => u.FirstName == user.FirstName))
                {
                    ModelState.AddModelError("FirstName","First Name is already in use!");
                    return View("Index");
                }
                else if(dbContext.Users.Any(u => u.LastName == user.LastName))
                {
                    ModelState.AddModelError("LastName","Last Name is already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user,user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                int user_id = user.UserId;
                HttpContext.Session.SetInt32("user_id",user_id);
                return RedirectToAction("Dashbord");
            }
            return View("Index");
        }
        // ---------------------------------------------------------------------
        // Login Page
        // ---------------------------------------------------------------------
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View("Login");
        }
        // ---------------------------------------------------------------------
        // Log in TO success or BELT EXAM would be this page !
        // ---------------------------------------------------------------------
        [HttpPost("logToSuccess")]
        public IActionResult LoginSuccess(LoginUser userSubmision)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmision.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email","Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmision,userInDb.Password,userSubmision.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password","Invalid Email/Password");
                    return View("Login");
                }
                //--------------------- Creating session after all those checks -------------------
                int user_id = userInDb.UserId;
                HttpContext.Session.SetInt32("user_id",user_id);
                return RedirectToAction("Dashbord");
            }
            return View("Login");
        }
        // ---------------------------------------------------------------------
        // Clear Session.
        // ---------------------------------------------------------------------
        [HttpGet("clear")]
        public IActionResult clearSession()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        // ---------------------------------------------------------------------
        // Success PAGE potential belt-exam page
        // ---------------------------------------------------------------------

        [HttpGet("Dashbord")]
        public IActionResult Dashbord()
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Login");
            }
            List <_Activity> all_activities = dbContext.Activities
            .Include(q =>q.User).Include(w =>w.Associations)
                .ThenInclude(q =>q.User).ToList();


            ViewBag.user_id = HttpContext.Session.GetInt32("user_id");
            ViewBag.all_activities = all_activities;
                return View("Dashbord");
        }

        // ---------------------------------------------------------------------
        // View for form to add a Wedding
        // ---------------------------------------------------------------------
        [HttpGet("/createActivity")]
        public IActionResult createActivity()
        {
            if(HttpContext.Session.GetInt32("user_id")==null)
            {
                return RedirectToAction("Login");
            }
            @ViewBag.user_id = (int)HttpContext.Session.GetInt32("user_id");
            return View("createActivity");
        }


        // ---------------------------------------------------------------------
        // CREATE THE ACTIVITY
        // ---------------------------------------------------------------------

        [HttpPost("/newActivity")]
        public IActionResult newActivity(_Activity newact)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Login");
            }
            if(newact.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date","Activity must be in the future");

                //*** i need to assign this session again because i need it when i View it back
                @ViewBag.user_id = (int)HttpContext.Session.GetInt32("user_id");
                return View("createActivity");
            }
            if(ModelState.IsValid)
            {
            @ViewBag.user_id = (int)HttpContext.Session.GetInt32("user_id");
            dbContext.Add(newact);
            dbContext.SaveChanges();
            int num = newact._ActivityId;
            return Redirect($"INFOActivity/{num}");
            }
            else
            {
                @ViewBag.user_id = (int)HttpContext.Session.GetInt32("user_id");
                ViewBag.errors = ModelState.Values;
                return View("createActivity");
            }
        }

        // ---------------------------------------------------------------------
        // INFO ABOUT THE ACTIVITY
        // ---------------------------------------------------------------------
        [HttpGet("/INFOActivity/{_ActivityId}")]
        public IActionResult INFOActivity(int _ActivityId)
        {
            if(HttpContext.Session.GetInt32("user_id")== null)
            {
                return RedirectToAction("Login");
            }
            _Activity activity = dbContext.Activities.Include(a =>a.User).Include(a => a.Associations)
                .ThenInclude(a => a.User)
                    .FirstOrDefault(i =>i._ActivityId == _ActivityId);


            List<_Activity> all_activity = dbContext.Activities.Include(w =>w.User).Include(q =>q.Associations).ThenInclude(w => w.User).ToList();

            ViewBag.all_activities = all_activity;
            ViewBag.activity = activity;
            ViewBag.user_id = (int)HttpContext.Session.GetInt32("user_id");

            return View("INFOActivity");
        }

        // ---------------------------------------------------------------------
        // Deleting Activity
        // ---------------------------------------------------------------------

        [HttpGet("/deleteActivity/{_ActivityId}")]
        public IActionResult deleteActivity(int _ActivityId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Login");
            }
            _Activity one = dbContext.Activities.FirstOrDefault(a=> a._ActivityId == _ActivityId);
            dbContext.Remove(one);
            dbContext.SaveChanges();
            return RedirectToAction("Dashbord");
        }


        // ---------------------------------------------------------------------
        // Cancel Activity jOIN
        // ---------------------------------------------------------------------
        [HttpGet("/Cancel/{_ActivityId}")]
        public IActionResult Cancel(int _ActivityId)
        {
            if(HttpContext.Session.GetInt32("user_id")== null)
            {
                return RedirectToAction("Login");
            }

            int user_id = (int)HttpContext.Session.GetInt32("user_id");
            
            Association ass = dbContext.Associations
                    .FirstOrDefault(u=>u.UserId == user_id && u._ActivityId == _ActivityId);

            dbContext.Remove(ass);
            dbContext.SaveChanges();
            

            return RedirectToAction("Dashbord");
        }

        // ---------------------------------------------------------------------
        // Joint activity
        // ---------------------------------------------------------------------
        [HttpGet("/Join/{_ActivityId}")]
        public IActionResult Join(int _ActivityId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)
            {
                return RedirectToAction("Login");
            }
            User user = dbContext.Users.Include(q =>q.Associations)
                .ThenInclude(w=>w._Activity)
                    .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("user_id"));


            _Activity _Activity = dbContext.Activities.Include(q =>q.Associations).ThenInclude(u => u.User).FirstOrDefault(w => w._ActivityId == _ActivityId);

            Association ass = new Association()
            {
                UserId = user.UserId,
                User = user,
                _ActivityId = _ActivityId,
                _Activity = _Activity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            dbContext.Add(ass);
            dbContext.SaveChanges();
                return RedirectToAction("Dashbord");
            }















        // ---------------------------------------------------------------------
        // ERRORRR
        // ---------------------------------------------------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
