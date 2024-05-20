using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace isTakipMVC3.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            Session.Abandon(); //BOŞALTCAK

            return RedirectToAction("Index","Login");
        }
    }
}