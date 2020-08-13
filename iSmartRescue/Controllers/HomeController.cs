using iSmartRescue.Class;
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
using iSmartRescue.Helper;

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

            return Json(new { result = SmartRescueLibrary.GetGeoLocationApiResponse(location) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchAmbulance(string emergencyCode, string location, string patientName, string phoneNumber,string latitude, string longtitude)
        {
            List<LatLangValues> result = new List<LatLangValues>();

            //var medicalProviders = SmartRescueLibrary.GetMedicalProviderAvailability(emergencyCode, location);

            var ambilanceProviders = SmartRescueLibrary.GetMedicalAmbulanceAvailability(emergencyCode, location);
            //if (medicalProviders.Count() != 0)
            //{
                result = SmartRescueLibrary.GetLocationDistance(ambilanceProviders, latitude, longtitude);
            //}

            AmbulanceAndProviderObject ambAndProv = new AmbulanceAndProviderObject();
           ambAndProv.LatLangValues = result.OrderBy(x=> x.DistanceValue).ToList();
            //ambAndProv.MedicalProviders = medicalProviders;

            return Json(new { result = ambAndProv }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DispatchAmbulance(string emergencyCode, string location, string patientName, string phoneNumber, string latitude, string longtitude,string ambulanceId,string medicalProviderName)
        {
            List<string> result = new List<string>();

            result.Add(SmartRescueLibrary.CreateServiceRequest(emergencyCode, location, patientName, phoneNumber, latitude, longtitude, ambulanceId, medicalProviderName));

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchMedicalProvider(string emergencyCode, string location, string patientName, string phoneNumber, string latitude, string longtitude,string healthCard,string healthCardNumber)
        {
            List<MedicalProviders> medicalProviders;
            List<LatLangValues> result = new List<LatLangValues>();
            AmbulanceAndProviderObject ambAndProv = new AmbulanceAndProviderObject();


            if (string.IsNullOrWhiteSpace(healthCardNumber) && string.IsNullOrWhiteSpace(healthCard))
            {
                medicalProviders = SmartRescueLibrary.GetMedicalProviderAvailability(emergencyCode, location);

                result = SmartRescueLibrary.GetLocationDistance2(medicalProviders, latitude, longtitude);

                ambAndProv = new AmbulanceAndProviderObject();
                ambAndProv.LatLangValues = result.OrderBy(x => x.DistanceValue).ToList();
                ambAndProv.MedicalProviders = medicalProviders;
            }
            else
            {
                medicalProviders = SmartRescueLibrary.GetMedicalProviderHealthCardAvailability(emergencyCode, healthCard);

                result = SmartRescueLibrary.GetLocationDistance2(medicalProviders, latitude, longtitude);

                ambAndProv = new AmbulanceAndProviderObject();
                ambAndProv.LatLangValues = result.OrderBy(x => x.DistanceValue).ToList();
                ambAndProv.MedicalProviders = medicalProviders;
            }
            

            return Json(new { result = ambAndProv }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RaiseEmergency(string serviceRequestId, string patientName, string phoneNumber, string healthCard, string healthCardAccountNumber, string medicalProviderName,string emergencyCode,string ambulanceId)
        {
            List<string> result = new List<string>();

            SmartRescueLibrary.UpdateServiceRequest(serviceRequestId, patientName, phoneNumber, healthCard, healthCardAccountNumber, medicalProviderName);

            SmartRescueLibrary.SendText(patientName, ((healthCard != "") ? true : false),"EMERGENCY!", healthCardAccountNumber,"EMERGENCY", emergencyCode, ambulanceId) ;

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}