using Ashp.AuthenticationService.AshpService;
using Ashp.AuthenticationService.Contracts.DataContracts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Ashp.AuthenticationService.Services
{
    public static class AsphAuthenticationService
    {
        internal static void LoginSingleUser(string username, string password, string appcode, AuthenticationType authenticationType, ref AshpLogin ashpLogin, ref LogRecord log)
        {
            ashpLogin.Username = username;
            ashpLogin.AuthenticationType = authenticationType;

            log.AppCode = appcode;
            log.Username = username;
            log.Password = password;
            log.AshpLoginAttempted = true;

            AshpService.AuthenticationResponse auth = null;
            AshpService.ASHPServiceClient client = null;
            try
            {
                client = new AshpService.ASHPServiceClient();

                auth = client.ASHPProductAuthentication(username, password, appcode);

                client.Close();
            }
            catch (CommunicationException)
            {
                if (client != null) client.Abort();
            }
            catch (TimeoutException)
            {
                if (client != null) client.Abort();
            }
            catch (Exception)
            {
                if (client != null) client.Abort();
                throw;
            }


            if (auth != null)
            {
                ashpLogin.ExpirationDate = auth.ExpirationDate;
                ashpLogin.ExpirationDateString = auth.ExpirationDate.ToLongDateString();
                ashpLogin.DaysUntilExpiration = (DateTime.Now - auth.ExpirationDate).Days;
                ashpLogin.ProductPurchased = auth.ProductPurchased;
                ashpLogin.ServerOk = auth.ServerOK;
                ashpLogin.LoginOk = auth.LoginOK;
                ashpLogin.AuthenticationState = auth.ServerOK && auth.LoginOK
                                              ? AuthenticationState.LoginSucceeded
                                              : AuthenticationState.LoginFailed;
                
                log.ExpiryDate = auth.ExpirationDate;
                log.ServerOK = auth.ServerOK;
                log.LoginOK = auth.LoginOK;
            }

            #region If IpAddress Authentication, "LoginOK = true" not necessary from AshpService

            if (ashpLogin.AuthenticationType == AuthenticationType.IpAddress)
                ashpLogin.AuthenticationState = auth.ServerOK && auth.ProductPurchased
                                              ? AuthenticationState.LoginSucceeded
                                              : AuthenticationState.LoginFailed;
            #endregion

            if (ashpLogin.ExpirationDate <= DateTime.Now)
                ashpLogin.AuthenticationState = AuthenticationState.ExpiredSubscription;

        }
    }
}