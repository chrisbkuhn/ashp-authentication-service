using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract]
    public class SessionCredentials
    {
        [DataMember(Name="login_or_email")]
        public string LoginOrEmail { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}