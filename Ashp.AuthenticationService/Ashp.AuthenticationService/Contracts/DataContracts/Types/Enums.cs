using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract]
    public enum AuthenticationState
    {
        [EnumMember]
        Error = -1,         // The authentication service did not respond or returned an error
        [EnumMember]
        Unknown = 0,        // Default state
        [EnumMember]
        LoginSucceeded = 1, // The authentication service recognized the user
        [EnumMember]
        LoginFailed = 2,    // The authentication service did not recognize this user
        [EnumMember]
        IpUnmapped = 3,     // The IP address was not mapped to a known user
        //[EnumMember]
        //AppOffline = 9,       // User's App is currently offline
        [EnumMember]
        ServiceDown = 10,       // The Ashp.AuthService site/service is down
        [EnumMember]
        ExpiredSubscription = 11 // Subscription's Expiry Date has been exeeded.
    }


    [DataContract]
    public enum AuthenticationType
    {
        [EnumMember]
        None = -1,
        [EnumMember]
        SingleUser = 1,
        [EnumMember]
        UrlReferer = 2,
        [EnumMember]
        IpAddress = 3,
        [EnumMember]
        AppOffline = 4 // User's App is currently offline, Automatic Passthru
    }

    [DataContract]
    public enum IpType
    {
        [EnumMember]
        Other = -1,
        [EnumMember]
        IpV4 = 4,
        [EnumMember]
        IpV6 = 6
    }
}
