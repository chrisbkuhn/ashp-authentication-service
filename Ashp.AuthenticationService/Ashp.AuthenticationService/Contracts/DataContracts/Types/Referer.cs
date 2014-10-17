using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract]
    public class Referer
    {
        [DataMember(Name = "id")]
        public Guid RefererUID { get; set; }
        [DataMember(Name = "url")]
        public string RefererUrl { get; set; }

        [DataMember(Name = "app")]
        //public string AppCode { get; set; }
        public Guid AppUID { get; set; }
        public App App { get; set; }

        [DataMember(Name = "user")]
        public Guid UserUID { get; set; }
        public InstUser User { get; set; }
    }
}