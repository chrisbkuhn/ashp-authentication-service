using Ashp.AuthenticationService.Contracts.DataContracts.Messages;
using Ashp.AuthenticationService.Contracts.DataContracts.Types;
using Ashp.AuthenticationService.Contracts.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ashp.AuthenticationService
{
    [WcfExtension.LogMessageBehavior]
    public class AdminService : IAdminService
    {
        #region Authentication
        
        public SessionResponse CreateSession(SessionRequest session)
        {
            var createdSession = new Session();

            Guid siteUserUID;
            if (DAL.AshpDbEntities.Login(session.Session.LoginOrEmail, session.Session.Password, out siteUserUID))
            {
                var token = DAL.AshpDbEntities.CreateSession(siteUserUID);

                createdSession.SiteUserUID = siteUserUID;
                createdSession.Token = token;
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            }

            return new SessionResponse { Session = createdSession };
        }

        public bool DestroySession()
        {
            var token = WebOperationContext.Current.IncomingRequest.Headers["HTTP_X_AUTHENTICATION_TOKEN"];
            return DAL.AshpDbEntities.DestroySession(token);
        }

        #endregion

        #region Users

        public UsersResponse GetInstUsers()
        {
            var instUsers = DAL.AshpDbEntities.GetAllInstUsers();

            return new UsersResponse { Users = instUsers };
        }

        public UserResponse GetInstUser(string id)
        {
            var instUsers = DAL.AshpDbEntities.GetAllInstUsers();

            InstUser instUser;
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                instUser = instUsers.Where(iu => iu.UserUID == guid)
                                    .FirstOrDefault();
            }
            else
            {
                // Assume "id" is the Username
                instUser = instUsers.Where(iu => string.Compare(iu.Username, id, StringComparison.InvariantCultureIgnoreCase) == 0)
                                  .FirstOrDefault();
            }

            if (instUser == null)
            {
                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
            }

            return new UserResponse { User = instUser };
        }

        public bool AddInstUser(UserRequest user)
        {
            try
            {
                DAL.AshpDbEntities.AddInstUser(user.User);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                return false;
            }
        }


        public bool EditInstUserWithId(UserRequest user, string id)
        {
            try
            {
                var guid = new Guid(id);
                if (guid == Guid.Empty)
                    return AddInstUser(user);

                user.User.UserUID = guid;

                DAL.AshpDbEntities.EditInstUser(user.User);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        public bool DeleteInstUser(string id)
        {
            try
            {
                var guid = new Guid(id);
                DAL.AshpDbEntities.DeleteInstUser(guid);

                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        #endregion

        #region Applications

        public AppsResponse GetApplications()
        {
            try
            {
                var applications = DAL.AshpDbEntities.GetAllApplications();

                return new AppsResponse { Apps = applications };
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                throw;
            }
        }

        public AppResponse GetApplication(string id)
        {
            var applications = DAL.AshpDbEntities.GetAllApplications();

            App application = null;

            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                application = applications.Where(app => app.AppUID == guid)
                                          .FirstOrDefault();
            }
            else
            {
                // Assume "id" is the AppCode
                application = applications.Where(app => string.Compare(app.AppCode, id, StringComparison.InvariantCultureIgnoreCase) == 0)
                                          .FirstOrDefault();
            }

            return new AppResponse { App = application };
        }

        public bool AddApplication(AppRequest app)
        {
            try
            {
                DAL.AshpDbEntities.AddApplication(app.App);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditApplicationWithId(AppRequest app, string id)
        {
            try
            {
                var guid = new Guid(id);

                if (guid != Guid.Empty)
                    app.App.AppUID = guid;

                if (app.App.AppUID == Guid.Empty) 
                    return AddApplication(app);

                DAL.AshpDbEntities.EditApplication(app.App);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteApplication(string id)
        {
            try
            {
                var guid = new Guid(id);
                DAL.AshpDbEntities.DeleteApplication(guid);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
        
        #region IpRanges

        public IpRangesResponse GetIpRanges()
        {
            try
            {
                var ipRanges = DAL.AshpDbEntities.GetAllIpRanges();
                return new IpRangesResponse { IpRange = ipRanges };
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                throw;
            }
        }

        public IpRangeResponse GetIpRange(string id)
        {
            try
            {
                var ipRanges = DAL.AshpDbEntities.GetAllIpRanges();

                IpRange ipRange = null;
                Guid guid;
                if (Guid.TryParse(id, out guid))
                {
                    ipRange = ipRanges.Where(ipr => ipr.IpRangeUID == guid)
                                      .FirstOrDefault();
                }
                else
                {
                    // Assume "id" is the a Slug in this format: "IPType|IPMin-IPMax"
                    int ipType = 0;
                    string ipMin, ipMax;
                    if (ExtractValuesFromIpRangeSlug(id, out ipType, out ipMin, out ipMax))
                    {
                        ipRange = ipRanges.Where(ipr => ipr.IpType == ipType &&
                                                        string.Compare(ipr.IpMin, ipMin, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                                                        string.Compare(ipr.IpMax, ipMax, StringComparison.InvariantCultureIgnoreCase) == 0)
                                          .FirstOrDefault();
                    }
                }

                return new IpRangeResponse { IpRange = ipRange };
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                throw;
            }
        }

        public bool AddIpRange(IpRangeRequest ipRange)
        {
            try
            {
                DAL.AshpDbEntities.AddIpRange(ipRange.IpRange);

                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                
                return false;
            }
        }

        public bool EditIpRangeWithId(IpRangeRequest ipRange, string id)
        {
            try
            {
                var guid = new Guid(id);

                if (guid != Guid.Empty)
                    ipRange.IpRange.IpRangeUID = guid;

                if (ipRange.IpRange.IpRangeUID == Guid.Empty)
                    return AddIpRange(ipRange);

                DAL.AshpDbEntities.EditIpRange(ipRange.IpRange);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        public bool DeleteIpRange(string id)
        {
            try
            {
                var guid = new Guid(id);
                DAL.AshpDbEntities.DeleteIpRange(guid);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        #region Private Helper
        
        public static bool ExtractValuesFromIpRangeSlug(string slug, out int ipType, out string ipMin, out string ipMax)
        {
            ipType = 4;
            ipMin = System.Net.IPAddress.None.ToString();
            ipMax = System.Net.IPAddress.None.ToString();

            try
            {
                var ipTypeAndAddresses = slug.Split('|');

                ipType = int.Parse(ipTypeAndAddresses[0]);

                var ipAddresses = ipTypeAndAddresses[1].Split('-');

                ipMin = ipAddresses[0];
                ipMax = ipAddresses[1];

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region Referers

        public ReferersResponse GetReferers()
        {
            var referers = DAL.AshpDbEntities.GetAllReferers();
            return new ReferersResponse { Referers = referers };
        }

        public RefererResponse GetReferer(string id)
        {
            var referers = DAL.AshpDbEntities.GetAllReferers();
            
            Referer referer;
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                referer = referers.Where(r => r.RefererUID == guid)
                                  .FirstOrDefault();
            }
            else
            {
                // Assume "id" is the Url
                referer = referers.Where(r => string.Compare(r.RefererUrl, id, StringComparison.InvariantCultureIgnoreCase) == 0)
                                  .FirstOrDefault();
            }

            return new RefererResponse { Referer = referer };
        }


        public bool AddReferer(RefererRequest referer)
        {
            try
            {
                DAL.AshpDbEntities.AddReferer(referer.Referer);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        public bool EditRefererWithId(RefererRequest referer, string id)
        {
            try
            {
                var guid = new Guid(id);

                if (guid != Guid.Empty)
                    referer.Referer.RefererUID = guid;

                if (referer.Referer.RefererUID == Guid.Empty)
                    return AddReferer(referer);

                DAL.AshpDbEntities.EditReferer(referer.Referer);

                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        public bool DeleteReferer(string id)
        {
            try
            {
                var guid = new Guid(id);
                DAL.AshpDbEntities.DeleteReferer(guid);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.EventLog(Helpers.LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                var ctx = WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return false;
            }
        }

        #endregion
    }
}
