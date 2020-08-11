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
            //var repair_reason = Repository.Get<Repair_Reason>().Where(rr => rr.Parent_Repair_Reason_ID == problem_id).ToList();
            //foreach (var item in repair_reason)
            //{
            //    result.Add(new Repair_Reason { Repair_Reason_Name = item.Repair_Reason_Name, Repair_Reason_ID = item.Repair_Reason_ID, Requires_More_Info = item.Requires_More_Info });
            //}

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}