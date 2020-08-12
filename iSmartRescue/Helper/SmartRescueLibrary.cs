﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using iSmartRescue.Class;
using Newtonsoft.Json;

namespace iSmartRescue.Helper
{
    
    internal class SmartRescueLibrary
    {
        private static string connString = ConfigurationManager.ConnectionStrings["ismartRescueDB"].ConnectionString;
        public static void CreateServiceRequest(string emergencyCode, string location, string patientName, string phoneNumber, string latitude, string longtitude, string ambulanceId,string medicalProviderId)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand("INSERT INTO dbo.ServiceRequest(EmergencyTypeId,PatientAddressLine1,PatientName,PatientContact,LocationCoordinatesX,LocationCoordinatesY,AssignedAmbulanceId,AssignedMedicalProviderId) " +
                    "VALUES ((SELECT EmergencyTypeId FROM dbo.EmergencyType WHERE EmergencyTypeCode = @EmergencyTypeCode)," +
                    "@Location,@PatientName,@PhoneNumber,@Latitude,@Longtitude,@AssignedAmbulanceId,(SELECT MedicalProviderId FROM dbo.MedicalProvider WHERE MedicalProviderName=@AssignedMedicalProviderId))"
                    , connection);

                command.Parameters.Add("@EmergencyTypeCode", SqlDbType.VarChar);
                command.Parameters.Add("@Location", SqlDbType.VarChar);
                command.Parameters.Add("@PatientName", SqlDbType.VarChar);
                command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                command.Parameters.Add("@Latitude", SqlDbType.VarChar);
                command.Parameters.Add("@Longtitude", SqlDbType.VarChar);
                command.Parameters.Add("@AssignedAmbulanceId", SqlDbType.VarChar);
                command.Parameters.Add("@AssignedMedicalProviderId", SqlDbType.VarChar);

                command.Parameters["@EmergencyTypeCode"].Value = emergencyCode;
                command.Parameters["@Location"].Value = location;
                command.Parameters["@PatientName"].Value = patientName;
                command.Parameters["@PhoneNumber"].Value = phoneNumber;
                command.Parameters["@Latitude"].Value = latitude;
                command.Parameters["@Longtitude"].Value = longtitude;
                command.Parameters["@AssignedAmbulanceId"].Value = ambulanceId;
                command.Parameters["@AssignedMedicalProviderId"].Value = medicalProviderId;

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
                        Longtitude = row["CoordinatesY"].ToString()
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
            foreach (DataRow row in dtRetValue.Rows)
            {
                medicalProviders.Add(
                    new MedicalProviders
                    {
                        Name = row["MedicalProviderName"].ToString(),
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
        private static List<LatLangValues> CalculateNearestAmbulance(List<DistanceMatrixApiResponse> apiResults)
        {
            List<LatLangValues> result = new List<LatLangValues>();

            //get all values
            foreach (DistanceMatrixApiResponse d in apiResults)
            {

                result.Add(
                    new LatLangValues 
                    { 
                        DistanceValue = d.rows[0].elements[0].distance.value , 
                        AmbulanceId = d.ambulanceid,
                        AmbulanceContactNo = d.ambulancecontactno,
                        AmbulanceCode = d.ambulancecode
                    });
                //Console.WriteLine(d.destination_addresses[0]);
                //Console.WriteLine(d.rows[0].elements[0].distance.text);
                //Console.WriteLine(d.rows[0].elements[0].distance.value);
                //Console.WriteLine("");
            }

            return result;
        }
    }
}