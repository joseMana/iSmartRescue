using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmartRescue.Class
{
    public class GeoLocationApiResponse
    {
        public List<candidates> candidates { get; set; }
        public string status { get; set; }
    }
    public class candidates
    {
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string name { get; set; }
        public List<photos> photos { get; set; }
    }

    public class geometry
    {
        public location location { get; set; }
    }
    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class viewport
    {
        public northeast northeast { get; set; }
        public southwest southwest { get; set; }
    }

    public class southwest
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
    public class photos
    {
        public string height { get; set; }
        public List<string> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public string width { get; set; }
    }
}