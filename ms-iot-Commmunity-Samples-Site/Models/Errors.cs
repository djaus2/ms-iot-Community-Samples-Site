using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace msiotCommunitySamples.Models
{
    public class Errors
    {
        public string Message { get; set; } = "";
        public string Source { get; set; } = "";
        public static bool LoggedInStatus { get; set; } = false;

        public bool loggedInStatus
        {
            get { return LoggedInStatus; }
        }

        public string LogLink
        {
            get {
                if (!LoggedInStatus)
                    return "/ms_iot_Community_Samples/Login";
                else
                    return "/ms_iot_Community_Samples/Logout";
            }
        }
        public string LogLabel
        {
            get
            {
                if (!LoggedInStatus)
                    return "Login";
                else
                    return "Logout";
            }
        }
    }
}