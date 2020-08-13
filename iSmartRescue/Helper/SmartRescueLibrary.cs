using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using iSmartRescue.Class;
using Newtonsoft.Json;

namespace iSmartRescue.Helper
{
    internal class SmartRescueLibrary
    {
        private static string connString = ConfigurationManager.ConnectionStrings["ismartRescueDB"].ConnectionString;
        public static string CreateServiceRequest(string emergencyCode, string location, string patientName, string phoneNumber, string latitude, string longtitude, string ambulanceId,string medicalProviderId)
        {
            string retVal = "";
            DataTable dtRetValue = new DataTable();
            string strQuery = "INSERT INTO dbo.ServiceRequest(DateOfRequest, EmergencyTypeId, PatientAddressLine1, PatientName, PatientContact, LocationCoordinatesX, LocationCoordinatesY, AssignedAmbulanceId) " +
                "OUTPUT Inserted.ServiceRequestId VALUES (GETDATE(),(SELECT EmergencyTypeId FROM dbo.EmergencyType WHERE EmergencyTypeCode = '"+emergencyCode+"')," +
                "'"+location+"','"+patientName+"','"+phoneNumber+"','"+latitude+"','"+longtitude+"','"+ambulanceId+"')";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter() { SelectCommand = new SqlCommand(strQuery, conn) };
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dataset);
                    try
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                    catch (Exception e)
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            foreach (DataRow row in dtRetValue.Rows)
            {
                retVal = row["ServiceRequestId"].ToString();
            }
            return retVal;
        }
        public static void UpdateServiceRequest(string serviceRequestId, string patientName, string phoneNumber, string healthCard, string healthCardAccountNumber, string medicalProviderName)
        {
            string retVal = "";
            DataTable dtRetValue = new DataTable();
            string strQuery = "UPDATE dbo.ServiceRequest " +
                "SET PatientName = '"+patientName+"'," +
                "PatientContact = '"+phoneNumber+ "'," +
                "HealthCardProviderId = (SELECT HealthCardProviderId FROM dbo.HealthCardProvider " +"WHERE HealthCardProviderName='"+healthCard+"')," +
                "HealthCardNumber='"+healthCardAccountNumber+ "', " +
                "AssignedMedicalProviderId=(SELECT MedicalProviderId FROM dbo.MedicalProvider WHERE MedicalProviderName='"+medicalProviderName+"')" +
                "WHERE ServiceRequestId = '" + serviceRequestId+"'";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter() { SelectCommand = new SqlCommand(strQuery, conn) };
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dataset);
                    try
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                    catch (Exception e)
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            
        }
        public static List<AmbulanceLocation> GetMedicalAmbulanceAvailability(string emergencyCode, string location)
        {
            DataTable dtRetValue = new DataTable();
            List<AmbulanceLocation> ambulanceLocation = new List<AmbulanceLocation>();
            string strQuery = "EXEC [dbo].[GetMedicalAmbulanceAvailability] '" + emergencyCode + "',''";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter() { SelectCommand = new SqlCommand(strQuery, conn) };
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dataset);
                    try
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                    catch (Exception e)
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            foreach (DataRow row in dtRetValue.Rows)
            {
                ambulanceLocation.Add(
                    new AmbulanceLocation
                    {
                        AmbulanceId = (Guid)row["AmbulanceId"],
                        AmbulanceContactNo = row["AmbulanceContactNumber"].ToString(),
                        AmbulanceCode = row["AmbulanceCode"].ToString(),
                        Latitude = row["CoordinatesX"].ToString(),
                        Longtitude = row["CoordinatesY"].ToString(),
                    });
            }


            return ambulanceLocation;
        }
        public static List<MedicalProviders> GetMedicalProviderAvailability(string emergencyCode, string location)
        {
            DataTable dtRetValue = new DataTable();
            List<MedicalProviders> medicalProviders = new List<MedicalProviders>();
            string strQuery = "EXEC [dbo].[GetMedicalProviderAvailability] '" + emergencyCode + "',''";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter() { SelectCommand = new SqlCommand(strQuery, conn) };
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dataset);
                    try
                    {
                        for (int i = 1; i < dataset.Tables.Count; i++)
                        {
                            if (dataset.Tables[i].Rows.Count > 1)
                            {
                                dataset.Tables[0].Merge(dataset.Tables[i]);
                            }
                        }
                        dtRetValue = dataset.Tables[0];
                    }
                    catch (Exception e)
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            int tblRowId = 1;
            foreach (DataRow row in dtRetValue.Rows)
            {
                medicalProviders.Add(
                    new MedicalProviders
                    {
                        TableRowId = tblRowId++,
                        Name = row["MedicalProviderName"].ToString().Replace(" ","-"),
                        Latitude = row["CoordinatesX"].ToString(),
                        Longtitude = row["CoordinatesY"].ToString(),
                        AddressLine1 = row["AddressLine1"].ToString()
                    });
            }

            return medicalProviders;
        }
        public static List<MedicalProviders> GetMedicalProviderHealthCardAvailability(string emergencyCode, string healthCard)
        {
            DataTable dtRetValue = new DataTable();
            List<MedicalProviders> medicalProviders = new List<MedicalProviders>();
            string strQuery = "EXEC dbo.GetMedicalProviderOfHealthCard '" + emergencyCode + "', '"+ healthCard + "'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter() { SelectCommand = new SqlCommand(strQuery, conn) };
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dataset);
                    try
                    {
                        for (int i = 1; i < dataset.Tables.Count; i++)
                        {
                            if (dataset.Tables[i].Rows.Count > 1)
                            {
                                dataset.Tables[0].Merge(dataset.Tables[i]);
                            }
                        }
                        dtRetValue = dataset.Tables[0];
                    }
                    catch (Exception e)
                    {
                        dtRetValue = dataset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
            }
            int tblRowId = 1;
            foreach (DataRow row in dtRetValue.Rows)
            {
                medicalProviders.Add(
                    new MedicalProviders
                    {
                        TableRowId = tblRowId++,
                        Name = row["MedicalProviderName"].ToString().Replace(" ", "-"),
                        Latitude = row["CoordinatesX"].ToString(),
                        Longtitude = row["CoordinatesY"].ToString(),
                        AddressLine1 = row["AddressLine1"].ToString()
                    });
            }

            return medicalProviders;
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
        public static List<LatLangValues> GetLocationDistance(List<AmbulanceLocation> ambulanceLocations,string patientLatitude,string patientLongtitude)
        {
            List<DistanceMatrixApiResponse> apiResults = new List<DistanceMatrixApiResponse>();
            string distanceMatrixUrl = "";


            string html = string.Empty;
            string originsLatitude = patientLatitude;
            string originsLongtitude = patientLongtitude;

            foreach (AmbulanceLocation al in ambulanceLocations)
            {
                distanceMatrixUrl = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=[originsLatitude],[originsLongtitude]&destinations=[ambulanceLocations]&key=AIzaSyAUppA4_tlXLrFh-BpL1HSeYRcRAC9DIBg";
                string ambulanceLocationString = al.Latitude + "," + al.Longtitude;
                distanceMatrixUrl = distanceMatrixUrl.Replace("[originsLatitude]", originsLatitude).Replace("[originsLongtitude]", originsLongtitude).Replace("[ambulanceLocations]", ambulanceLocationString);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(distanceMatrixUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                DistanceMatrixApiResponse apiResponse = JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(html);
                apiResponse.ambulanceid = al.AmbulanceId;
                apiResponse.ambulancecode = al.AmbulanceCode;
                apiResponse.ambulancecontactno = al.AmbulanceContactNo;

                apiResults.Add(apiResponse);
            }

            return CalculateNearestAmbulance(apiResults);
        }
        public static List<LatLangValues> GetLocationDistance2(List<MedicalProviders> medicalProviders, string patientLatitude, string patientLongtitude)
        {
            List<DistanceMatrixApiResponse> apiResults = new List<DistanceMatrixApiResponse>();
            string distanceMatrixUrl = "";


            string html = string.Empty;
            string originsLatitude = patientLatitude;
            string originsLongtitude = patientLongtitude;

            foreach (MedicalProviders al in medicalProviders)
            {
                distanceMatrixUrl = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=[originsLatitude],[originsLongtitude]&destinations=[ambulanceLocations]&key=AIzaSyAUppA4_tlXLrFh-BpL1HSeYRcRAC9DIBg";
                string medicalProvidersLocationString = al.Latitude + "," + al.Longtitude;
                distanceMatrixUrl = distanceMatrixUrl.Replace("[originsLatitude]", originsLatitude).Replace("[originsLongtitude]", originsLongtitude).Replace("[ambulanceLocations]", medicalProvidersLocationString);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(distanceMatrixUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                DistanceMatrixApiResponse apiResponse = JsonConvert.DeserializeObject<DistanceMatrixApiResponse>(html);
                apiResponse.medicalprovidername = al.Name;
                apiResponse.medicalproviderlocation = al.AddressLine1;

                apiResults.Add(apiResponse);
            }

            return CalculateNearestMedicalProvider(apiResults);
        }
        private static List<LatLangValues> CalculateNearestAmbulance(List<DistanceMatrixApiResponse> apiResults)
        {
            List<LatLangValues> result = new List<LatLangValues>();

            //get all values
            foreach (DistanceMatrixApiResponse d in apiResults)
            {

                result.Add(
                    new LatLangValues 
                    { 
                        DistanceValue = Convert.ToInt32(d.rows[0].elements[0].distance.value) , 
                        AmbulanceId = d.ambulanceid,
                        AmbulanceContactNo = d.ambulancecontactno,
                        AmbulanceCode = d.ambulancecode,
                        Miles = d.rows[0].elements[0].distance.text
                    });
                //Console.WriteLine(d.destination_addresses[0]);
                //Console.WriteLine(d.rows[0].elements[0].distance.text);
                //Console.WriteLine(d.rows[0].elements[0].distance.value);
                //Console.WriteLine("");
            }

            return result;
        }
        private static List<LatLangValues> CalculateNearestMedicalProvider(List<DistanceMatrixApiResponse> apiResults)
        {
            List<LatLangValues> result = new List<LatLangValues>();

            //get all values
            foreach (DistanceMatrixApiResponse d in apiResults)
            {

                result.Add(
                    new LatLangValues
                    {
                        DistanceValue = Convert.ToInt32(d.rows[0].elements[0].distance.value),
                        MedicalProviderName = d.medicalprovidername,
                        MedicalProviderLocation = d.medicalproviderlocation,
                        Miles = d.rows[0].elements[0].distance.text
                    });
                //Console.WriteLine(d.destination_addresses[0]);
                //Console.WriteLine(d.rows[0].elements[0].distance.text);
                //Console.WriteLine(d.rows[0].elements[0].distance.value);
                //Console.WriteLine("");
            }

            return result;
        }
        public static void SendText(string strName, bool healthStatus, string strReason, string strInfo, string strRemarks,string emergencyCode,string ambulanceId)
        {
            string CPNumber = "639219833634";
            try
            {
                WebClient client = new WebClient();

                string strhealthStatus = "";

                if (healthStatus == true)
                {

                    strhealthStatus = "YES";
                }
                else
                {
                    strhealthStatus = "NO";
                }
                string message = "EMERGENCY CODE : "+ emergencyCode + "\nAmbulanceID : "+ ambulanceId + "\nWe would like to request an ambulance for: " + strName + "%0a" +
                      "Reason: " + strReason + "%0a" +
                      "With Health Card? " + strhealthStatus + "%0a" +
                      "Health Card Information: " + strInfo + "%0a" +
                      "Additional Notes: " + strRemarks;
                Stream s = client.OpenRead(string.Format("https://platform.clickatell.com/messages/http/send?" +
                    "apiKey=a9fqSjDARTOsQWhHGYztYQ==&" +
                    "to=" + CPNumber + "&" +
                    "content=" + message));
                StreamReader reader = new StreamReader(s);
                string result = reader.ReadToEnd();
                //Console.WriteLine("Send");
            }

            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.ToString());
            }

        }
        public static void SendEmail(string toemail, string providerName, string strName, bool healthStatus, string healthcardname, string healthcardnumber, string emergencycode, string strRemarks, string ambulancecontactno, string ambulanceCode)
        {
            try
            {
                NetworkCredential login;
                SmtpClient client;
                MailMessage msg;
                string strhealthStatus;

                login = new NetworkCredential("anneblance000@gmail.com", "teamcareftw");
                client = new SmtpClient("smtp.gmail.com");
                client.Port = Convert.ToInt32("587");
                client.EnableSsl = true;
                client.Credentials = login;
                msg = new MailMessage { From = new MailAddress("anneblance000@gmail.com", "iSmartRescue", Encoding.UTF8) };
                msg.To.Add(new MailAddress(toemail));
                msg.Subject = "[Urgent] Request for Medical Assistance";

                if (healthStatus == true)
                {
                    strhealthStatus = "YES";
                }
                else
                {
                    strhealthStatus = "NO";
                }

                string message = "Dear " + providerName + ";<br> <br> There is an incoming patient under Emergency Code: <b>" + emergencycode + ".</b> <br>" +
                                   " Please be ready for the availability of medical staff and necessary equipment under this code <br><br>";
                if (strhealthStatus == "YES")
                {
                    msg.Body = message +
                        "<b>Patient Name: </b>" + strName + "<br>" +
                        "<b>Contact Number: </b>" + strName + "<br>" +
                        "<b>Emergency Code: </b>" + emergencycode + "<br>" +
                        "<b>With Health Card?</b>: " + strhealthStatus + "<br>" +
                        "<b>Health Card Provider</b>: " + healthcardname + "<br>" +
                        "<b>Health Card Number</b>: " + healthcardnumber + "<br>" +
                        "<b>Ambulance Code:</b>: " + ambulanceCode + "<br>" +
                        "<b>Ambulance Contact No:</b>: " + ambulancecontactno + "<br>" +
                        "<b>Notes</b>: " + strRemarks + "<br><br>" +
                        "This is auto generate email please do not response." + "<br><br>" +
                        "Thank You! <br><br>" +
                        "Regards, " + "<br><br>" +
                        "iSmartRescue";
                }
                else
                {
                    msg.Body = message +
                        "<b>Patient Name: </b>" + strName + "<br>" +
                        "<b>Emergency Code: </b>" + emergencycode + "<br>" +
                        "<b>With Health Card?</b>: " + strhealthStatus + "<br>" +
                        "<b>Health Card Provider</b>: " + healthcardname + "<br>" +
                        "<b>Notes</b>: " + strRemarks + "<br><br>" +
                        "This is auto generate email please do not response." + "<br><br>" +
                        "Thank You! <br><br>" +
                        "Regards, " + "<br><br>" +
                        "iSmartRescue";
                }
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(msg);
            }
            catch (Exception ex)
            {
                //Console.Write("Error:" + ex.Message);
            }
        }

    }
}