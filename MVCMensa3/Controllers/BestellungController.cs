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
            string user = null;
            if (Session["user"] != null)
            {
                user = Session["user"].ToString();
            }
            var model = Models.BestellungViewModel.FromCookie(null, user);

            return View(model);
        }

        // called with aendern
        [HttpPost]
        public ActionResult Index(Models.BestellungViewModel model)
        {
            return View(model);
            // nothing needs to be commited
        }


    }
}