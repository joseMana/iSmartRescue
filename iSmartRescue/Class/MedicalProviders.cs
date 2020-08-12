using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmartRescue.Class
{
    public class MedicalProviders
    {
        public int TableRowId { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string AddressLine1 { get; set; }
    }
}