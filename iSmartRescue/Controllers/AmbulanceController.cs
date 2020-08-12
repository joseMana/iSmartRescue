using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSmartRescue.Controllers
{
    public class AmbulanceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult getSolutionFromProblem(string emergencyCode,string location)
        {
            List<string> result = new List<string>();
           
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}