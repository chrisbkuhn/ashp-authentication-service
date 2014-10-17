using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ashp.AuthenticationService.DAL
{
    public static class ConversionExtensions
    {
        #region Conversion Extension Methods
        
        #region LogRecord

        public static Contracts.DataContracts.Types.LogRecord Convert(this Log dbLog)
        {
            return new Contracts.DataContracts.Types.LogRecord
            {
                AppCode = dbLog.AppCode,
                AshpLoginAttempted = dbLog.AshpLoginAttempted,
                AuthenticationState = dbLog.AuthenticationState,
                AuthorizationQueryString = dbLog.AuthorizationQueryString,
                AuthorizationRequestHeader = dbLog.AuthorizationRequestHeader,
                DecodedAuthorizationString = dbLog.DecodedAuthorizationString,
                ExpiryDate = dbLog.ExpiryDate,
                IPAddress = dbLog.IPAddress,
                LogDate = dbLog.LogDate,
                LoginOK = dbLog.LoginOK,
                LogUID = dbLog.LogUID,
                Password = dbLog.Password,
                Referer = dbLog.Referer,
                ServerOK = dbLog.ServerOK,
                Username = dbLog.Username
            };
        }

        public static Log Convert(this Contracts.DataContracts.Types.LogRecord log)
        {
            return new Log
            {
                AppCode = log.AppCode,
                AshpLoginAttempted = log.AshpLoginAttempted,
                AuthenticationState = log.AuthenticationState,
                AuthorizationQueryString = log.AuthorizationQueryString,
                AuthorizationRequestHeader = log.AuthorizationRequestHeader,
                DecodedAuthorizationString = log.DecodedAuthorizationString,
                ExpiryDate = log.ExpiryDate,
                IPAddress = log.IPAddress,
                LogDate = log.LogDate,
                LoginOK = log.LoginOK,
                LogUID = log.LogUID,
                Password = log.Password,
                Referer = log.Referer,
                ServerOK = log.ServerOK,
                Username = log.Username
            };
        }


        #endregion

        #region App

        public static Contracts.DataContracts.Types.App Convert(this App dbApp, List<Referer> referers = null, List<IPRange> ipRanges = null)
        {
            return new Contracts.DataContracts.Types.App
            {
                AppUID = dbApp.AppUID,
                AppCode = dbApp.AppCode,
                AppName = dbApp.AppName,
                IsOffline = dbApp.IsOffline,
                IpRanges = ipRanges == null
                         ? null 
                         : ipRanges.Where(ipr => ipr.AppUID == dbApp.AppUID).Select(ipr => ipr.IPRangeUID).ToList(),
                Referers = referers == null
                         ? null
                         : referers.Where(r => r.AppUID == dbApp.AppUID).Select(r => r.RefererUID).ToList(),
            };
        }
        
        public static App Convert(this Contracts.DataContracts.Types.App app)
        {
            return new App
            {
                AppUID = app.AppUID,
                AppCode = app.AppCode,
                AppName = app.AppName,
                IsOffline = app.IsOffline
            };
        }

        #endregion

        #region IpRange

        public static Contracts.DataContracts.Types.IpRange Convert(this IPRange dbIpRange)
        {
            return new Contracts.DataContracts.Types.IpRange
            {
                App = dbIpRange.App.Convert(),
                AppUID = dbIpRange.AppUID,
                User = dbIpRange.InstUser.Convert(),
                UserUID = dbIpRange.UserUID,
                IpMax = dbIpRange.IPMax.Trim(),
                IpMin = dbIpRange.IPMin.Trim(),
                IpRangeUID = dbIpRange.IPRangeUID,
                IpType = dbIpRange.IPType,
                IpTypeEnum = dbIpRange.IPType == 4
                            ? Contracts.DataContracts.Types.IpType.IpV4
                            : dbIpRange.IPType == 6
                                ? Contracts.DataContracts.Types.IpType.IpV6
                                : Contracts.DataContracts.Types.IpType.Other,

                Slug = CreateSlug(dbIpRange.IPType, dbIpRange.IPMin, dbIpRange.IPMax)
            };
        }

        public static IPRange Convert(this Contracts.DataContracts.Types.IpRange ipRange)
        {
            return new IPRange
            {
                App = ipRange.App != null
                    ? ipRange.App.Convert()
                    : null,
                AppUID = ipRange.AppUID,
                InstUser = ipRange.User != null
                    ? ipRange.User.Convert()
                    : null,
                UserUID = ipRange.UserUID,
                IPMax = ipRange.IpMax,
                IPMin = ipRange.IpMin,
                IPRangeUID = ipRange.IpRangeUID,
                IPType = ipRange.IpType
                //IPType = ipRange.IpType == Contracts.DataContracts.Types.IpType.IpV4
                //        ? 4
                //        : 6
            };
        }

        private static string CreateSlug(int ipType, string ipMin, string ipMax)
        {
            return string.Format("{0}|{1}-{2}", ipType,
                                                ipMin,
                                                ipMax);
        }

        #endregion

        #region User

        public static Contracts.DataContracts.Types.InstUser Convert(this InstUser dbUser, List<Referer> referers = null, List<IPRange> ipRanges = null)
        {
            return new Contracts.DataContracts.Types.InstUser
            {
                UserUID = dbUser.UserUID,
                Username = dbUser.Username.Trim(),
                Password = dbUser.Password.Trim(),
                Fullname = dbUser.FullName.Trim(),
                IpRanges = ipRanges == null
                         ? null
                         : ipRanges.Where(ipr => ipr.UserUID == dbUser.UserUID).Select(ipr => ipr.IPRangeUID).ToList(),
                Referers = referers == null
                         ? null
                         : referers.Where(r => r.UserUID == dbUser.UserUID).Select(r => r.RefererUID).ToList(),
            };
        }

        public static InstUser Convert(this Contracts.DataContracts.Types.InstUser user)
        {
            return new InstUser
            {
                UserUID = user.UserUID,
                Username = user.Username,
                Password = user.Password,
                FullName = user.Fullname
            };
        }

        #endregion

        #region Referer

        public static Contracts.DataContracts.Types.Referer Convert(this Referer dbReferer)
        {
            return new Contracts.DataContracts.Types.Referer
            {
                RefererUID = dbReferer.RefererUID,
                RefererUrl = dbReferer.RefererURL,
                App = dbReferer.App.Convert(),
                //AppCode = dbReferer.App.AppCode,
                AppUID = dbReferer.AppUID,
                User = dbReferer.InstUser.Convert(),
                UserUID = dbReferer.UserUID,
            };
        }

        public static Referer Convert(this Contracts.DataContracts.Types.Referer referer)
        {
            return new Referer
            {
                RefererUID = referer.RefererUID,
                RefererURL = referer.RefererUrl,
                App = referer.App != null
                    ? referer.App.Convert()
                    : null,
                AppUID = referer.AppUID,
                InstUser = referer.User != null
                    ? referer.User.Convert()
                    : null,
                UserUID = referer.UserUID
            };
        }

        #endregion

        #endregion
    }
}