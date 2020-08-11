using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace iSmartRescue.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GeoLocate(string location)
        {
            List<string> result = new List<string>();

            return Json(new { result = HelperClass.GetGeoLocationApiResponse(location) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RaiseEmergency(string emergencyCode, string location, string patientName, string phoneNumber)
        {
            List<string> result = new List<string>();

            HelperClass.CreateServiceRequest(emergencyCode,location,patientName,phoneNumber);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}