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
        private string connString = ConfigurationManager.ConnectionStrings["ismartRescueDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RaiseEmergency(string emergencyCode, string location, string patientName, string phoneNumber)
        {
            List<string> result = new List<string>();

            CreateServiceRequest(emergencyCode,location,patientName,phoneNumber);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        private void CreateServiceRequest(string emergencyCode, string location, string patientName, string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("INSERT INTO dbo.ServiceRequest(EmergencyTypeId,PatientAddressLine1,PatientName,PatientContact) " +
                    "VALUES ((SELECT EmergencyTypeId FROM dbo.EmergencyType WHERE EmergencyTypeCode = @EmergencyTypeCode)," +
                    "@Location,@PatientName,@PhoneNumber)"
                    , connection);

                command.Parameters.Add("@EmergencyTypeCode", SqlDbType.VarChar);
                command.Parameters.Add("@Location", SqlDbType.VarChar);
                command.Parameters.Add("@PatientName", SqlDbType.VarChar);
                command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);

                command.Parameters["@EmergencyTypeCode"].Value = emergencyCode;
                command.Parameters["@Location"].Value = location;
                command.Parameters["@PatientName"].Value = patientName;
                command.Parameters["@PhoneNumber"].Value = phoneNumber;

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                }
                catch (DbException dbEx)
                { 
                    //logging
                }
                catch (Exception ex)
                {
                    //logging
                }
            }
        }
    }
}