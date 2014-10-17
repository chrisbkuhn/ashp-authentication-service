using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract(Name = "iprange")]
    public class IpRange
    {
        [DataMember(Name = "id")]
        public Guid IpRangeUID { get; set; }

        [DataMember(Name = "user")]
        public Guid UserUID { get; set; }
        public InstUser User { get; set; }

        [DataMember(Name = "app")]
        public Guid AppUID { get; set; }
        public App App { get; set; }

        [DataMember(Name = "iptype")]
        public int IpType { get; set; }
        public IpType IpTypeEnum { get; set; }

        [DataMember(Name = "ipmin")]
        public string IpMin { get; set; }

        [DataMember(Name = "ipmax")]
        public string IpMax { get; set; }
        
        [DataMember(Name = "slug")]
        public string Slug { get; set; } 
    }


}