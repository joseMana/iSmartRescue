using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmartRescue.Class
{
    public class AmbulanceAndProviderObject
    {
        public List<LatLangValues> LatLangValues { get; set; }
        public List<MedicalProviders> MedicalProviders { get; set; }
    }
}