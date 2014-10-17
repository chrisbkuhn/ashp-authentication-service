using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract(Name = "user")]
    public class InstUser
    {
        [DataMember(Name = "id")]
        public Guid UserUID { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "fullname")]
        public string Fullname { get; set; }

        [DataMember(Name = "ipranges")]
        public List<Guid> IpRanges { get; set; }

        [DataMember(Name = "referers")]
        public List<Guid> Referers { get; set; }
    }
}