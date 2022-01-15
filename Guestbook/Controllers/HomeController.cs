using Guestbook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Guestbook.Controllers
{
    public class HomeController : Controller
    {
        GuestbookDBContext db;
        int? storedId;
        public HomeController(GuestbookDBContext db)
        {
            this
                .db = db;
        }
        //----------------------- Index -----------------------
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                storedId = HttpContext.Session.GetInt32("ID");
                ViewBag.messages = db.Message.ToList();
                return View(db.UserMessages.Include(n => n.Users).Where(n => n.ReceivingUserId == storedId).ToList());
            }
            else return RedirectToAction("Login");
        }
        //----------------------- Create User -----------------------
        public IActionResult CreateUser()
        {
                return View();
        }
        //----------------------- Create User [post] -----------------------
        [HttpPost]
        public IActionResult CreateUser(User uc)
        {
            if (ModelState.IsValid)
            {
                db.Add(uc);
                db.SaveChanges();
                ViewBag.message = "The user " + uc.Username + " is saved successfully";
                return RedirectToAction("Index");
            }
            else return View(); 
        }
        //----------------------- Login -----------------------

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("ID") != null)
                return RedirectToAction("Index");
            else
                return View();
        }
        //----------------------- Login [post] -----------------------

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            User us = db.Users.Where(n=>n.Username==username&&n.Pwd==password).SingleOrDefault();
            
            if (us != null)
            {
                HttpContext.Session.SetInt32("ID", us.UserId);
                HttpContext.Session.SetString("username", us.Username);

                TempData["ID"] = HttpContext.Session.GetInt32("ID");
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid Account";
                return View("Login");
            }
        }
        //----------------------- Logout -----------------------

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("ID");
            return RedirectToAction("Login");
        }
        //----------------------- Create Message -----------------------

        public IActionResult CreateMessage()
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                storedId = HttpContext.Session.GetInt32("ID");
                ViewBag.users = new SelectList(db.Users.Where(n=>n.UserId!=storedId).ToList(), "UserId", "Username");
                return View();
            }
            else return RedirectToAction("Login");
        }
        //----------------------- Create Message [post] -----------------------

        [HttpPost]
        public IActionResult CreateMessage(UserMessage um)
        {
            storedId = HttpContext.Session.GetInt32("ID");
            um.UserId = storedId.Value;
            um.Messages.publishedDate = DateTime.Now;
            ModelState.Remove("Users");
            ModelState.Remove("Messages.UserMessages");
            if (ModelState.IsValid)
            {
                db.Add(um);
                db.SaveChanges();
                return RedirectToAction("Sent");
            }
            else 
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                storedId = HttpContext.Session.GetInt32("ID");
                ViewBag.users = new SelectList(db.Users.Where(n => n.UserId != storedId).ToList(), "UserId", "Username");
                return View();
            }
        }

        //----------------------- Delete Message -----------------------

        public IActionResult DeleteMessage(int id)
        {
            UserMessage um = db.UserMessages.Where(n=>n.MessId==id).SingleOrDefault();
            Message m = db.Message.Find(id);
            db.Remove(um);
            db.Remove(m);
            db.SaveChanges();
            return RedirectToAction("Sent");
        }

        //----------------------- Edit Message -----------------------

        public IActionResult EditMessage(int id)
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                Message m = db.Message.Find(id);
            TempData["modifiedID"] = id;
            return View(m);
            }
            else return RedirectToAction("Login");
        }

        //----------------------- Edit Message [post] -----------------------

        [HttpPost]
        public IActionResult EditMessage(Message m)
        {        
                var modfied_mess = db.Message.FirstOrDefault(n => n.MessId == (int)TempData["modifiedID"]);
                modfied_mess.messBody = m.messBody;
                db.SaveChanges();
                return RedirectToAction("Index");
        }

        //----------------------- Sent Message -----------------------

        public IActionResult Sent()
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                storedId = HttpContext.Session.GetInt32("ID");
                ViewBag.messages = db.Message.ToList();
                ViewBag.users = db.Users.ToList();
                return View(db.UserMessages.Include(n => n.Users).Where(n => n.UserId == storedId).ToList());
            }
            else return RedirectToAction("Login");
            
        }

        //----------------------- Reply to Message -----------------------

        public IActionResult Reply(int id)
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                Message m = db.Message.Find(id);
                TempData["modifiedID"] = id;
                return View(m);
            }
            else return RedirectToAction("Login");
        }

        //----------------------- Reply to Message [post] -----------------------

        [HttpPost]
        public IActionResult Reply(Message m)
        {
            if (HttpContext.Session.GetInt32("ID") != null)
            {
                var modfied_mess = db.Message.FirstOrDefault(n => n.MessId == (int)TempData["modifiedID"]);
                modfied_mess.respnose = m.respnose;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            else return View();
        }
    }
}
