using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract]
    public class Session
    {
        [DataMember(Name = "id")]
        public Guid SiteUserUID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}