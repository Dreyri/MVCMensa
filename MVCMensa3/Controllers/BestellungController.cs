using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCMensa3.Controllers
{
    public class BestellungController : Controller
    {
        public ActionResult Index()
        {
            var session = Models.MensaSession.FromCookie(HttpContext.Request.Cookies);
            var warenkorbCookie = HttpContext.Request.Cookies.Get("warenkorb");
            Dictionary<string, Dictionary<int, int>> warenkorbDict = null;

            if (session == null)
            {
                return RedirectToAction("Login", "Login");
            }

            Models.BestellungViewModel model = null;

            // now that we know the session and cookie exists continue
            if (warenkorbCookie != null)
            {
                warenkorbDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(warenkorbCookie.Value);
                model = Models.BestellungViewModel.FromDict(session.Rolle, warenkorbDict[session.User]);
            }
            else
            {
                model = new Models.BestellungViewModel(null, null);
            }

            return View(model);
        }

        // called with aendern
        [HttpPost]
        public ActionResult Index(Models.BestellungViewModel model)
        {
            return View(model);
            // nothing needs to be commited
        }

        public ActionResult Leeren()
        {
            var session = Models.MensaSession.FromCookie(Request.Cookies);
            var warenkorbCookie = HttpContext.Request.Cookies.Get("warenkorb");

            if (warenkorbCookie != null && session != null)
            {
                Dictionary<string, Dictionary<int, int>> warenkorb = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(warenkorbCookie.Value);
                warenkorb.Remove(session.User);
                warenkorbCookie.Value = JsonConvert.SerializeObject(warenkorb);
            }

            warenkorbCookie.Expires = DateTime.Now.AddHours(6);
            HttpContext.Response.Cookies.Set(warenkorbCookie);

            return RedirectToAction("Index");
        }


    }
}