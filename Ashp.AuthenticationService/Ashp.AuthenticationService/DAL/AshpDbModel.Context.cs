﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AshpDbEntities : DbContext
    {
        public AshpDbEntities()
            : base("name=AshpDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<App> Apps { get; set; }
        public DbSet<ServiceStatu> ServiceStatus { get; set; }
        public DbSet<Referer> Referers { get; set; }
        public DbSet<InstUser> InstUsers { get; set; }
        public DbSet<IPRange> IPRanges { get; set; }
        public DbSet<AuthenticationState> AuthenticationStates { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SiteUser> SiteUsers { get; set; }
    }
}