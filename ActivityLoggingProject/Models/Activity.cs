using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActivityLoggingProject.Models
{
    //Activity Model
    public class Activity
    {
        public int ID { get; set; }
        public string activityname { get; set; }
        public string usrname { get; set; }
        public string priority { get; set; }
        public string labels { get; set; }
    }
}