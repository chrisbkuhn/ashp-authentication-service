using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    public class LogRecord
    {
        public LogRecord() { }

        public LogRecord(AshpLogin ashpLogin)
        {
            LogUID = Guid.NewGuid();

            AppCode = string.Empty;
            AshpLoginAttempted = false;
            AuthenticationState = (int) Contracts.DataContracts.Types.AuthenticationState.Unknown;
            AuthorizationQueryString = null;
            AuthorizationRequestHeader = null;
            DecodedAuthorizationString = null;
            ExpiryDate = null;
            IPAddress = ashpLogin.IpAddress;
            LogDate = DateTime.Now;
            LoginOK = ashpLogin.LoginOk;
            Referer = ashpLogin.RefererUrl;
            ServerOK = ashpLogin.ServerOk;
            Username = ashpLogin.Username;
        }

        public string AppCode { get; set; }

        public bool AshpLoginAttempted { get; set; }

        public int AuthenticationState { get; set; }

        public string AuthorizationQueryString { get; set; }

        public string AuthorizationRequestHeader { get; set; }

        public string DecodedAuthorizationString { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string IPAddress { get; set; }

        public DateTime LogDate { get; set; }

        public bool? LoginOK { get; set; }

        public Guid LogUID { get; set; }

        public string Password { get; set; }

        public string Referer { get; set; }

        public bool? ServerOK { get; set; }

        public string Username { get; set; }
    }
}