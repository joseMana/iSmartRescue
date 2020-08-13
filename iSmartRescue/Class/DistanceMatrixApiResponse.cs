using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmartRescue.Class
{
    public class DistanceMatrixApiResponse
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<rows> rows { get; set; }
        public string status { get; set; }
        public Guid ambulanceid { get; set; }
        public string ambulancecontactno { get; set; }
        public string ambulancecode { get; set; }
        public Guid medicalproviderid { get; set; }
        public string medicalprovidername { get; set; }
        public string medicalproviderlocation { get; set; }
        public string miles { get; set; }


    }
    public class rows
    {
        public List<elements> elements { get; set; }
    }
    public class elements
    {
        public distance distance { get; set; }
        public duration duration { get; set; }
        public string status { get; set; }
    }
    public class distance
    {
        public string text { get; set; }
        public string value { get; set; }
    }
    public class duration
    {
        public string text { get; set; }
        public string value { get; set; }
    }
}