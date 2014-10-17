using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Ashp.AuthenticationService.Extensions;
using Ashp.AuthenticationService.Helpers;

namespace Ashp.AuthenticationService.DAL
{
    public partial class AshpDbEntities
    {
        #region Authentication

        internal static bool IsServiceDown(string serviceName)
        {
            bool serviceIsDisabled = true;

            using (var context = new AshpDbEntities())
            {
                var serviceStatus = context.ServiceStatus
                                           .Where(ss => string.Compare(ss.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase) == 0)
                                           .FirstOrDefault();

                serviceIsDisabled = serviceStatus.MasterIsDisabled;
            }

            return serviceIsDisabled;
        }

        internal static bool IsIpAddressMapped(IPAddress ipaddress, out Contracts.DataContracts.Types.InstUser user, out Contracts.DataContracts.Types.App app)
        {
            user = null;
            app = null;

            var ipRangeList = GetAllIpRanges();

            if (ipRangeList != null && ipRangeList.Count > 0)
            {
                foreach (var ipRange in ipRangeList)
                {
                    // Get the IP type
                    Contracts.DataContracts.Types.IpType callerIPType;
                    switch (ipaddress.AddressFamily)
                    { 
                        case System.Net.Sockets.AddressFamily.InterNetwork:
                            callerIPType = Contracts.DataContracts.Types.IpType.IpV4;
                            break;

                        case System.Net.Sockets.AddressFamily.InterNetworkV6:
                            callerIPType = Contracts.DataContracts.Types.IpType.IpV6;
                            break;

                        default:
                            callerIPType = Contracts.DataContracts.Types.IpType.Other;
                            break;
                    }
                    //var callerIPType = Contracts.DataContracts.Types.IpType.IpV4;
                    //if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    //    callerIPType = Contracts.DataContracts.Types.IpType.IpV6;
                    //else
                    //    callerIPType = Contracts.DataContracts.Types.IpType.Other;

                    // If this range is a different type than the caller (ipv4 vs ipv6) then skip 
                    // this one - no comparison is possible
                    if (ipRange.IpTypeEnum != callerIPType) continue;

                    // Get the start address
                    var start = Conversion.ConvertStringToIP(ipRange.IpMin);
                    if (start == null) continue;

                    // Get the end address (may be null)
                    var end = Conversion.ConvertStringToIP(ipRange.IpMax);

                    if (end == null)
                    {
                        // This is a single ip
                        if (ipaddress.CompareTo(start) == 0)
                        {
                            user = ipRange.User;
                            app = ipRange.App;

                            return true;
                        }
                    }
                    else
                    {
                        // This is a range
                        if (ipaddress.IsInRange(start, end))
                        {
                            user = ipRange.User;
                            app = ipRange.App;

                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        internal static bool IsUrlMapped(string urlHost, out Contracts.DataContracts.Types.InstUser urlUser, out Contracts.DataContracts.Types.App urlApp)
        {
            urlUser = null;
            urlApp = null;

            Referer referer = null;
            using (var context = new AshpDbEntities())
            {
                var wwwUrlHost = string.Concat("www.", urlHost);

                if (context.Referers.Any(r => r.RefererURL.ToLower() == urlHost ||
                                              r.RefererURL.ToLower() == wwwUrlHost))
                {
                    referer = context.Referers.Where(r => r.RefererURL.ToLower() == urlHost || r.RefererURL.ToLower() == wwwUrlHost).FirstOrDefault();
                }

                if (referer != null)
                {
                    urlUser = referer.InstUser.Convert();
                    urlApp = referer.App.Convert();

                    return true;
                }
            }

            return false;
        }
        
        #region Log

        internal static bool Log(Contracts.DataContracts.Types.LogRecord logRecord)
        {
            try
            {
                using (var context = new AshpDbEntities())
                {
                    context.Logs.Add(logRecord.Convert());
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {

                return false;
            }
        }

        #endregion

        #endregion


        #region Admin

        #region Session

        internal static bool Login(string username, string password, out Guid siteUserUID)
        {
            siteUserUID = Guid.Empty;

            try
            {
                using (var context = new AshpDbEntities())
                {
                    var siteUser = context.SiteUsers.Where(su => string.Compare(su.Username, username, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                    .FirstOrDefault();

                    if (siteUser != null &&
                        string.Compare(siteUser.Password, password, StringComparison.InvariantCulture) == 0)
                    {
                        siteUserUID = siteUser.SiteUserUID;
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        internal static string CreateSession(Guid siteUserUID)
        { 
            using (var context = new AshpDbEntities())
            {
                var now = DateTime.Now;

                var session = new Session
                {
                    SiteUserSessionUID = Guid.NewGuid(),
                    SiteUserUID = siteUserUID,
                    SessionStart = now,
                    SessionLastActivity = now,
                    Expired = false
                };

                context.Sessions.Add(session);
                context.SaveChanges();

                return session.SiteUserSessionUID.ToString();
            }
        }

        internal static bool CheckSession(string token)
        {
            using (var context = new AshpDbEntities())
            {
                Guid guid;
                if (!Guid.TryParse(token, out guid))
                    return false;

                var session = context.Sessions.Where(s => s.SiteUserSessionUID == guid)
                                              .FirstOrDefault();

                var maxLifetime = ConfigHelper.SessionMaxLifetime;

                if (session.SessionLastActivity.AddSeconds(maxLifetime) < DateTime.Now)
                {
                    session.Expired = true;
                    context.SaveChanges();
                    return false;
                }

                if (session.Expired) return false;

                session.SessionLastActivity = DateTime.Now;
                context.SaveChanges();

                return true;
            }        
        }
        
        internal static bool DestroySession(string token)
        {
            using (var context = new AshpDbEntities())
            {
                Guid guid;
                if (!Guid.TryParse(token, out guid))
                    return false;

                var session = context.Sessions.Where(s => s.SiteUserSessionUID == guid)
                                              .FirstOrDefault();
                session.Expired = true;

                context.SaveChanges();

                return true;
            }
        }

        #endregion

        #region InstUser

        public static List<Contracts.DataContracts.Types.InstUser> GetAllInstUsers()
        {
            List<InstUser> instUsers = null;
            List<Referer> referers = null;
            List<IPRange> ipRanges = null;
            using (var context = new AshpDbEntities())
            {
                instUsers = context.InstUsers.Include("IPRanges").Include("Referers").ToList();
                referers = context.Referers.ToList();
                ipRanges = context.IPRanges.ToList();
            }

            var returnIpRanges = instUsers.Select(iu => iu.Convert(referers, ipRanges)).ToList();
            return returnIpRanges;
        }

        internal static void AddInstUser(Contracts.DataContracts.Types.InstUser user)
        {
            var newUser = user.Convert();
            newUser.UserUID = Guid.NewGuid();

            using (var context = new AshpDbEntities())
            {
                if (context.InstUsers.Any(iu => string.Compare(iu.Username, user.Username, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("Username already exists");

                context.InstUsers.Add(newUser);
                context.SaveChanges();
            }
        }

        internal static void EditInstUser(Contracts.DataContracts.Types.InstUser user)
        {
            using (var context = new AshpDbEntities())
            {
                if (!context.InstUsers.Any(iu => string.Compare(iu.Username, user.Username, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("Username does not exist");

                var userToEdit = context.InstUsers.Where(iu => string.Compare(iu.Username, user.Username, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                  .FirstOrDefault();

                userToEdit.FullName = user.Fullname;
                userToEdit.Password = user.Password;

                context.SaveChanges();
            }
        }

        internal static void DeleteInstUser(Guid guid)
        {
            using (var context = new AshpDbEntities())
            {
                var userToDelete = context.InstUsers.Where(iu => iu.UserUID == guid)
                                                    .FirstOrDefault();

                context.InstUsers.Remove(userToDelete);

                context.SaveChanges();
            }
        }
        #endregion

        #region App

        internal static List<Contracts.DataContracts.Types.App> GetAllApplications()
        {
            List<Contracts.DataContracts.Types.App> returnApplications = null;
            using (var context = new AshpDbEntities())
            {
                //var applications = context.Apps.Include("IPRanges").Include("Referers").ToList();
                var applications = context.Apps.ToList();
                var ipRanges = context.IPRanges.ToList();
                var referers = context.Referers.ToList();

                returnApplications = applications.Select(a => a.Convert(referers, ipRanges)).ToList();
            }

            return returnApplications;
        }
        
        internal static void AddApplication(Contracts.DataContracts.Types.App app)
        {
            var newApp = app.Convert();
            newApp.AppUID = Guid.NewGuid();

            using (var context = new AshpDbEntities())
            {
                if (context.Apps.Any(a => string.Compare(a.AppCode, app.AppCode, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("AppCode already exists");

                context.Apps.Add(newApp);
                context.SaveChanges();
            }
        }

        internal static void EditApplication(Contracts.DataContracts.Types.App app)
        {
            using (var context = new AshpDbEntities())
            {
                if (!context.Apps.Any(a => string.Compare(a.AppCode, app.AppCode, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("AppCode does not exist");

                var appToEdit = context.Apps.Where(a => string.Compare(a.AppCode, app.AppCode, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                  .FirstOrDefault();

                appToEdit.AppCode = app.AppCode;
                appToEdit.AppName = app.AppName;
                appToEdit.IsOffline = app.IsOffline;

                context.SaveChanges();
            }
        }

        internal static void DeleteApplication(Guid guid)
        {
            using (var context = new AshpDbEntities())
            {
                var appToDelete = context.Apps.Where(a => a.AppUID == guid)
                                                    .FirstOrDefault();

                context.Apps.Remove(appToDelete);

                context.SaveChanges();
            }
        }

        #endregion

        #region IpRange

        public static List<Contracts.DataContracts.Types.IpRange> GetAllIpRanges()
        {
            List<Contracts.DataContracts.Types.IpRange> returnIpRanges = null;
            using (var context = new AshpDbEntities())
            {
                var ipRanges = context.IPRanges.ToList();
                returnIpRanges = ipRanges.Select(ipr => ipr.Convert()).ToList();
            }

            return returnIpRanges;
        }



        internal static void AddIpRange(Contracts.DataContracts.Types.IpRange ipRange)
        {
            var newIpRange = ipRange.Convert();
            newIpRange.IPRangeUID = Guid.NewGuid();

            using (var context = new AshpDbEntities())
            {
                context.IPRanges.Add(newIpRange);
                context.SaveChanges();
            }
        }

        internal static void EditIpRange(Contracts.DataContracts.Types.IpRange ipRange)
        {
            using (var context = new AshpDbEntities())
            {
                var ipRangeToEdit = context.IPRanges.Where(ipr => ipr.IPRangeUID == ipRange.IpRangeUID)
                                                    .FirstOrDefault();

                ipRangeToEdit.IPType = ipRange.IpType;
                ipRangeToEdit.IPMin = ipRange.IpMin;
                ipRangeToEdit.IPMax = ipRange.IpMax;

                ipRangeToEdit.AppUID = ipRange.AppUID;

                context.SaveChanges();
            }
        }

        internal static void DeleteIpRange(Guid guid)
        {
            using (var context = new AshpDbEntities())
            {
                var ipRangeToDelete = context.IPRanges.Where(ipr => ipr.IPRangeUID == guid)
                                                      .FirstOrDefault();

                context.IPRanges.Remove(ipRangeToDelete);

                context.SaveChanges();
            }
        }

        #endregion

        #region Referer

        internal static List<Contracts.DataContracts.Types.Referer> GetAllReferers()
        {
            List<Referer> referers = null;
            using (var context = new AshpDbEntities())
            {
                referers = context.Referers.Include("App").Include("InstUser").ToList();
            }

            var returnReferers = referers.Select(r => r.Convert()).ToList();
            return returnReferers;
        }


        internal static void AddReferer(Contracts.DataContracts.Types.Referer referer)
        {
            var newReferer = referer.Convert();
            newReferer.RefererUID = Guid.NewGuid();

            using (var context = new AshpDbEntities())
            {
                if (context.Referers.Any(r => string.Compare(r.RefererURL, referer.RefererUrl, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("Referer Url already exists");

                context.Referers.Add(newReferer);
                context.SaveChanges();
            }
        }

        internal static void EditReferer(Contracts.DataContracts.Types.Referer referer)
        {
            using (var context = new AshpDbEntities())
            {
                if (!context.Referers.Any(r => string.Compare(r.RefererURL, referer.RefererUrl, StringComparison.InvariantCultureIgnoreCase) == 0))
                    throw new Exception("Referer Url does not exist");

                var refererToEdit = context.Referers.Where(r => string.Compare(r.RefererURL, referer.RefererUrl, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                    .FirstOrDefault();

                var refererApp = context.Apps.Where(a => a.AppUID == referer.AppUID)
                                             .FirstOrDefault();

                refererToEdit.AppUID = refererApp.AppUID;
                refererToEdit.UserUID = referer.UserUID;

                context.SaveChanges();
            }
        }

        internal static void DeleteReferer(Guid guid)
        {
            using (var context = new AshpDbEntities())
            {
                var refererToDelete = context.Referers.Where(r => r.RefererUID == guid)
                                                      .FirstOrDefault();

                context.Referers.Remove(refererToDelete);

                context.SaveChanges();
            }
        }

        #endregion


        #endregion


    }
}