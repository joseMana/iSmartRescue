using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using iSmartRescue.Class;
using Newtonsoft.Json;

namespace iSmartRescue.Controllers
{
    internal class HelperClass
    {
        private static string connString = ConfigurationManager.ConnectionStrings["ismartRescueDB"].ConnectionString;

        public static void CreateServiceRequest(string emergencyCode, string location, string patientName, string phoneNumber)
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

        public static GeoLocationApiResponse GetGeoLocationApiResponse(string location)
        {
            string html = string.Empty;
            string url = @"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input="+ location + "&inputtype=textquery&fields=photos,formatted_address,name,rating,opening_hours,geometry&key=AIzaSyAUppA4_tlXLrFh-BpL1HSeYRcRAC9DIBg";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            GeoLocationApiResponse geoLocationApiResponse = JsonConvert.DeserializeObject<GeoLocationApiResponse>(html);

            return geoLocationApiResponse;
        }
    }
}