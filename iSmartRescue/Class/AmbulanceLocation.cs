﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmartRescue.Class
{
    public class AmbulanceLocation
    {
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string AmbulanceCode { get; set; }
        public string AmbulanceContactNo { get; set; }
        public Guid AmbulanceId { get; set; }
        public string Miles { get; set; }
    }
    public class LatLangValues
    {
        public int DistanceValue { get; set; }
        public string AmbulanceCode { get; set; }
        public string AmbulanceContactNo { get; set; }
        public Guid AmbulanceId { get; set; }
        public string MedicalProviderName { get; set; }
        public string MedicalProviderLocation { get; set; }
        public string Miles { get; set; }

    }
}