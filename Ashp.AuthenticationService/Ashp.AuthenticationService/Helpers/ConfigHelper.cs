using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Ashp.AuthenticationService.Helpers
{
    public class ConfigHelper
    {
        public static bool IsServiceDisabled
        {
            get
            {
                bool isServiceDisabled = false;
                bool.TryParse(ConfigurationManager.AppSettings["IsServiceDisabled"], out isServiceDisabled);

                return isServiceDisabled;
            }
        }

        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        public static int SessionMaxLifetime 
        {
            get
            {
                var sessionMaxLifetime = 1440; // default
                int.TryParse(ConfigurationManager.AppSettings["SessionMaxLifetime"], out sessionMaxLifetime);

                return sessionMaxLifetime;
            }
        }
    }
}