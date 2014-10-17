using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract(Name="app")]
    public class App
    {
        [DataMember(Name="id")]
        public Guid AppUID { get; set; }
        [DataMember(Name = "appcode")]
        public string AppCode { get; set; }
        [DataMember(Name = "appname")]
        public string AppName { get; set; }
        [DataMember(Name = "isoffline")]
        public bool IsOffline { get; set; }

        [DataMember(Name = "ipranges")]
        public List<Guid> IpRanges { get; set; }

        [DataMember(Name = "referers")]
        public List<Guid> Referers { get; set; }
    }
}