using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Ashp.AuthenticationService.Extensions
{
    public static class IPAddressExtensions
    {
        #region Comparison
        public static int CompareTo(this IPAddress thisIpAddress, IPAddress value)
        {
            int returnVal = 0;
            if (thisIpAddress.AddressFamily == value.AddressFamily)
            {
                byte[] b1 = thisIpAddress.GetAddressBytes();
                byte[] b2 = value.GetAddressBytes();

                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] < b2[i])
                    {
                        returnVal = -1;
                        break;
                    }
                    else if (b1[i] > b2[i])
                    {
                        returnVal = 1;
                        break;
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("value",
                      "Cannot compare two addresses from different address families.");
            }

            return returnVal;
        }
        #endregion

        #region Range
        public static bool IsInRange(this IPAddress thisIpAddress, IPAddress rangeStartAddress, IPAddress rangeEndAddress)
        {
            bool returnVal = false;
            // ensure that all addresses are of the same type otherwise reject //
            if (rangeStartAddress.AddressFamily != rangeEndAddress.AddressFamily)
            {
                throw new ArgumentOutOfRangeException("rangeStartAddress",
                String.Format("The Start Range type {0} and End Range type {1}" +
                              " are not compatible ip address families.",
                              rangeStartAddress.AddressFamily.ToString(),
                              rangeEndAddress.AddressFamily.ToString()));
            }

            if (rangeStartAddress.AddressFamily == thisIpAddress.AddressFamily)
            {
                returnVal = (thisIpAddress.CompareTo(rangeStartAddress) >= 0
                               && thisIpAddress.CompareTo(rangeEndAddress) <= 0);
                // no need to check for -2 value as this
                // check has already been undertaken to get into this block //
            }
            else
            {
                throw new ArgumentOutOfRangeException("rangeStartAddress",
                    String.Format("The range type {0} and current value type {1}" +
                                  " are not compatible ip address families",
                                  rangeStartAddress.AddressFamily.ToString(),
                                  thisIpAddress.AddressFamily.ToString()));
            }
            return returnVal;
        }
        #endregion

    }
}