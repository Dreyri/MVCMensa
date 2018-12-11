using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCMensa3.Models;

namespace MVCMensa3.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View(new LoginFeedback(null, null));
        }

        public ActionResult Success()
        {
            var msession = MensaSession.FromCookie(Request.Cookies);

            if (msession == null)
            {
                return RedirectToAction("Login");
            }

            return View(msession);
        }

        public ActionResult Register()
        {
            return View(new Registration());
        }

        public ActionResult RegisterGast()
        {
            return View(new GastRegistration());
        }

        [HttpPost]
        public ActionResult RegisterGast(GastRegistration gr)
        {
            if (!ModelState.IsValid)
            {
                return View(gr);
            }

            if (gr.Register())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(gr);
        }

        public ActionResult RegisterStudent()
        {
            return View(new StudentRegistration());
        }

        [HttpPost]
        public ActionResult RegisterStudent(StudentRegistration sr)
        {
            if (!ModelState.IsValid)
            {
                return View(sr);
            }
            if (sr.Register())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(sr);
        }

        public ActionResult RegisterMA()
        {
            return View(new MARegistration());
        }

        [HttpPost]
        public ActionResult RegisterMA(MARegistration mr)
        {
            if (!ModelState.IsValid)
            {
                return View(mr);
            }
            if (mr.Register())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(mr);
        }

        public ActionResult Logout()
        {
            MensaSession.Delete(Response.Cookies);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            var benutzer = Benutzer.GetBenutzerFromName(user);
            var loginFeedback = LoginFeedback.TryLogin(user, password);
            if (loginFeedback.Success)
            {
                var rolle = Benutzer.GetRole(benutzer.Nummer);
                MensaSession.Create(Response.Cookies, user, rolle);
                return RedirectToAction("Success");
            }

            return View(loginFeedback);
        }
    }
}