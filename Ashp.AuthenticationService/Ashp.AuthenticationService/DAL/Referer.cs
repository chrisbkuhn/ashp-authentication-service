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
    
    public partial class Referer
    {
        public System.Guid RefererUID { get; set; }
        public string RefererURL { get; set; }
        public System.Guid AppUID { get; set; }
        public System.Guid UserUID { get; set; }
    
        public virtual App App { get; set; }
        public virtual InstUser InstUser { get; set; }
    }
}
