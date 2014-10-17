using Ashp.AuthenticationService.Contracts.DataContracts.Types;
using Ashp.AuthenticationService.Extensions;
using Ashp.AuthenticationService.Contracts.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Ashp.AuthenticationService.Helpers;

namespace Ashp.AuthenticationService
{
    //[WcfExtension.LogMessageBehavior]
    public class AuthService : IAuthService
    {
        public AshpLogin AuthenticateXml(string authorization, string referer)
        {
            return DoAuthentication(authorization, referer);
        }

        public AshpLogin AuthenticateJson(string authorization, string referer)
        {
            return DoAuthentication(authorization, referer);
        }


        private AshpLogin DoAuthentication(string authorization, string referer)
        {
            var ashpLogin = new AshpLogin 
            {
                AuthenticationState = AuthenticationState.LoginFailed,
                AuthenticationType = AuthenticationType.None,

                LoginOk = false,
                ServerOk = false
            };

            var log = new LogRecord(ashpLogin);

            try
            {
                if (DAL.AshpDbEntities.IsServiceDown(ConfigHelper.ServiceName))
                {
                    ashpLogin.AuthenticationState = AuthenticationState.ServiceDown;
                }
                else
                {
                    IPAddress ipaddress;
                    if (GetIpAddress(out ipaddress))
                    {
                        ashpLogin.AuthenticationType = AuthenticationType.IpAddress;
                        ashpLogin.IpAddress = ipaddress.ToString();
                        ashpLogin.IpType = ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                            ? IpType.IpV4
                                            : ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                                                ? IpType.IpV6
                                                : IpType.Other;

                        log.IPAddress = ipaddress.ToString();

                        #region IP Authenticate

                        InstUser ipUser = null;
                        App ipApp = null;

                        if (DAL.AshpDbEntities.IsIpAddressMapped(ipaddress, out ipUser, out ipApp))
                        {
                            ashpLogin.UserFullName = ipUser.Fullname;

                            if (ipApp.IsOffline)
                            {
                                ashpLogin.AuthenticationState = AuthenticationState.LoginSucceeded;
                                ashpLogin.AuthenticationType = AuthenticationType.AppOffline;                                
                            }
                            else
                            {
                                Services.AsphAuthenticationService.LoginSingleUser(ipUser.Username,
                                                                                   ipUser.Password,
                                                                                   ipApp.AppCode,
                                                                                   AuthenticationType.IpAddress,
                                                                                   ref ashpLogin,
                                                                                   ref log);
                            }
                        }
                        else
                            ashpLogin.AuthenticationState = AuthenticationState.IpUnmapped;

                        #endregion
                    }

                    string urlReferer;
                    if (ashpLogin.AuthenticationState != AuthenticationState.LoginSucceeded &&
                        GetUrlReferer(referer, out urlReferer))
                    {
                        ashpLogin.AuthenticationType = AuthenticationType.UrlReferer;
                        ashpLogin.RefererUrl = urlReferer;

                        log.Referer = urlReferer;

                        #region URL Authenticate

                        InstUser urlUser = null;
                        App urlApp = null;

                        if (DAL.AshpDbEntities.IsUrlMapped(urlReferer, out urlUser, out urlApp))
                        {
                            ashpLogin.UserFullName = urlUser.Fullname;

                            if (urlApp.IsOffline)
                            {
                                ashpLogin.AuthenticationState = AuthenticationState.LoginSucceeded;
                                ashpLogin.AuthenticationType = AuthenticationType.AppOffline;
                            }
                            else
                            {
                                Services.AsphAuthenticationService.LoginSingleUser(urlUser.Username,
                                                                                   urlUser.Password,
                                                                                   urlApp.AppCode,
                                                                                   AuthenticationType.UrlReferer,
                                                                                   ref ashpLogin,
                                                                                   ref log);
                            }
                        }

                        #endregion
                    }

                    string username, password, appcode;

                    #region Single User Authentication

                    if (string.IsNullOrWhiteSpace(authorization))
                    {
                        if (ashpLogin.AuthenticationState != AuthenticationState.LoginSucceeded &&
                            GetCredentialsFromHeader(out username, out password, out appcode, ref log))
                        {
                            var applications = DAL.AshpDbEntities.GetAllApplications();
                            var application = applications != null
                                            ? applications.Where(app => string.Compare(app.AppCode, appcode, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                          .FirstOrDefault()
                                            : null;

                            if (application != null && application.IsOffline)
                            {
                                ashpLogin.AuthenticationState = AuthenticationState.LoginSucceeded;
                                ashpLogin.AuthenticationType = AuthenticationType.AppOffline;
                            }
                            else
                            {
                                Services.AsphAuthenticationService.LoginSingleUser(username,
                                                                                   password,
                                                                                   appcode,
                                                                                   AuthenticationType.SingleUser,
                                                                                   ref ashpLogin,
                                                                                   ref log);
                            }
                        }
                    }
                    else if (ashpLogin.AuthenticationState != AuthenticationState.LoginSucceeded &&
                             GetCredentialsFromAuthorizationString(authorization, out username, out password, out appcode, ref log))
                    {
                        log.AuthorizationRequestHeader = authorization;

                        var applications = DAL.AshpDbEntities.GetAllApplications();
                        var application = applications != null
                                        ? applications.Where(app => string.Compare(app.AppCode, appcode, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                        .FirstOrDefault()
                                        : null;

                        if (application != null && application.IsOffline)
                        {
                            ashpLogin.AuthenticationState = AuthenticationState.LoginSucceeded;
                            ashpLogin.AuthenticationType = AuthenticationType.AppOffline;
                        }
                        else
                        {
                            Services.AsphAuthenticationService.LoginSingleUser(username,
                                                                               password,
                                                                               appcode,
                                                                               AuthenticationType.SingleUser,
                                                                               ref ashpLogin,
                                                                               ref log);
                        }
                    }
                    #endregion
                }
            }
            catch (FaultException ex)
            {
                var messageFault = ex.CreateMessageFault();
                System.Xml.XmlElement exceptionDetail = null;
                if (messageFault.HasDetail) 
                    exceptionDetail = messageFault.GetDetail<System.Xml.XmlElement>();

                ashpLogin.AuthenticationState = AuthenticationState.Error;
                ashpLogin.ErrorText = ex.Message;
            }
            catch (Exception ex)
            {
                LogHelper.EventLog(ex);

                ashpLogin.AuthenticationState = AuthenticationState.Error;
                ashpLogin.ErrorText = ex.Message;
            }

            ashpLogin.AuthenticationTypeName = ashpLogin.AuthenticationType.ToString();
            
            if (ashpLogin.AuthenticationState != AuthenticationState.Error)
                ashpLogin.ErrorText = ashpLogin.AuthenticationState.ToString();

            log.AuthenticationState = (int)ashpLogin.AuthenticationState;
            DAL.AshpDbEntities.Log(log);

            return ashpLogin;
        }



        #region Private Helper Methods
        
        /// <summary>
        /// Assuming the credentials are passed in the HTTP Request Header under the key "Authorization"
        /// The format of the credentials should be: "Basic [username]:[password]:[appcode]"
        /// The credentials need to be encoded to Base 64
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="appcode"></param>
        private bool GetCredentialsFromHeader(out string username, out string password, out string appcode, ref LogRecord log)
        {
            username = null;
            password = null;
            appcode = null;

            string credentials = WebOperationContext.Current.IncomingRequest.Headers["Authorization"];

            log.AuthorizationRequestHeader = credentials;

            if (credentials != null) credentials = credentials.Trim();

            if (!string.IsNullOrEmpty(credentials))
            {
                try
                {
                    string[] credentialParts = credentials.Split(new char[] { ' ' });
                    if (credentialParts.Length == 2 &&
                        credentialParts[0].Equals("basic", StringComparison.OrdinalIgnoreCase))
                    {
                        var authorizationString = credentialParts[1];

                        return GetCredentialsFromAuthorizationString(authorizationString, out username, out password, out appcode, ref log);
                    }
                }
                catch { }
            }

            return false;
        }

        private bool GetCredentialsFromAuthorizationString(string authorizationString, out string username, out string password, out string appcode, ref LogRecord log)
        {
            username = null;
            password = null;
            appcode = null;

            if (authorizationString != null)
                authorizationString = authorizationString.Trim();

            if (!string.IsNullOrEmpty(authorizationString))
            {
                try
                {
                    authorizationString = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(authorizationString));
                    log.DecodedAuthorizationString = authorizationString;

                    var credentialParts = authorizationString.Split(new char[] { ':' });
                    
                    if (credentialParts.Length == 3)
                    {
                        username = credentialParts[0];
                        password = credentialParts[1];
                        appcode = credentialParts[2];

                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private bool GetUrlReferer(string referer, out string urlReferer)
        {
            urlReferer = string.Empty;

            if (!string.IsNullOrWhiteSpace(referer))
            {
                urlReferer = referer;
                urlReferer = urlReferer.ToLower().Replace("www.", string.Empty);
                return true;
            }

            var refererUrl = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Referer];

            if (refererUrl == null)
                return false;

            Uri uri = null;
            try
            {
                uri = new Uri(refererUrl);
            }
            catch
            {
                return false;
            }

            if (uri == null)
                return false;

            urlReferer = uri.Host.ToLower().Replace("www.", string.Empty);
            return true;
        }

        private bool GetIpAddress(out IPAddress ipAddress)
        {
            ipAddress = null;

            var props = OperationContext.Current.IncomingMessageProperties;
            var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            string ip;
            if (endpointProperty != null)
            {
                ip = endpointProperty.Address;
                
                return IPAddress.TryParse(ip, out ipAddress);                
            }

            return false;
        }

        #endregion
    }
}
