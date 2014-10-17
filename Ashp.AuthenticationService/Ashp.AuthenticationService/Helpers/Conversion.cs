using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Ashp.AuthenticationService.Helpers
{
    public static class Conversion
    {
        #region Conversion Methods

        public static IPAddress ConvertStringToIP(string pIP)
        {
            if (string.IsNullOrEmpty(pIP)) return null;

            IPAddress addr = null;
            if (IPAddress.TryParse(pIP, out addr))
            {
                return addr;
            }
            else
                return null;
        }

        #endregion
    }
}