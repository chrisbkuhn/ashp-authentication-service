using Ashp.AuthenticationService.Contracts.DataContracts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Messages
{
    [DataContract]
    public class RefererRequest
    {
        [DataMember(Name="referer")]
        public Referer Referer { get; set; }
    }
}