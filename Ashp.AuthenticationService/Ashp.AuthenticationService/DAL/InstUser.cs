//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ashp.AuthenticationService.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class InstUser
    {
        public InstUser()
        {
            this.Referers = new HashSet<Referer>();
            this.IPRanges = new HashSet<IPRange>();
        }
    
        public System.Guid UserUID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    
        public virtual ICollection<Referer> Referers { get; set; }
        public virtual ICollection<IPRange> IPRanges { get; set; }
    }
}