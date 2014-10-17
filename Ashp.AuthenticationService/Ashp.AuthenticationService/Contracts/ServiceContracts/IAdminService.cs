using Ashp.AuthenticationService.Contracts.DataContracts.Messages;
using Ashp.AuthenticationService.Contracts.DataContracts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ashp.AuthenticationService.Contracts.ServiceContracts
{
    [ServiceContract]
    public interface IAdminService
    {
        #region Authentication
        
        [OperationContract]
        [WebInvoke(UriTemplate = "/session",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        SessionResponse CreateSession(SessionRequest session);

        [OperationContract]
        [WebInvoke(UriTemplate = "/session",
                   Method = "DELETE",
                   ResponseFormat = WebMessageFormat.Json)]
        bool DestroySession();


        #endregion


        #region Users

        [OperationContract]
        [WebGet(UriTemplate = "users",
                ResponseFormat = WebMessageFormat.Json)]
        UsersResponse GetInstUsers();

        [OperationContract]
        [WebGet(UriTemplate = "users/{id}",
                ResponseFormat = WebMessageFormat.Json)]
        UserResponse GetInstUser(string id);


        [OperationContract]
        [WebInvoke(UriTemplate = "/users", 
                   Method = "POST",
                   RequestFormat=WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool AddInstUser(UserRequest user);

        [OperationContract]
        [WebInvoke(UriTemplate = "/users/{id}",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool EditInstUserWithId(UserRequest user, string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/users/{id}", 
            Method = "DELETE")]
        bool DeleteInstUser(string id);

        #endregion

        #region Applications

        [OperationContract]
        [WebGet(UriTemplate = "apps",
                ResponseFormat = WebMessageFormat.Json)]
        AppsResponse GetApplications();

        [OperationContract]
        [WebGet(UriTemplate = "apps/{id}",
                ResponseFormat = WebMessageFormat.Json)]
        AppResponse GetApplication(string id);
        
        [OperationContract]
        [WebInvoke(UriTemplate = "/apps",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool AddApplication(AppRequest user);

        [OperationContract]
        [WebInvoke(UriTemplate = "/apps/{id}",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool EditApplicationWithId(AppRequest user, string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/apps/{id}",
            Method = "DELETE")]
        bool DeleteApplication(string id);

        #endregion

        #region IpRanges

        [OperationContract]
        [WebGet(UriTemplate = "ipRanges",
                ResponseFormat = WebMessageFormat.Json)]
        IpRangesResponse GetIpRanges();

        [OperationContract]
        [WebGet(UriTemplate = "ipRanges/{id}",
                ResponseFormat = WebMessageFormat.Json)]
        IpRangeResponse GetIpRange(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/ipRanges",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool AddIpRange(IpRangeRequest ipRange);

        [OperationContract]
        [WebInvoke(UriTemplate = "/ipRanges/{id}",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool EditIpRangeWithId(IpRangeRequest ipRange, string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/ipRanges/{id}",
            Method = "DELETE")]
        bool DeleteIpRange(string id);
        
        #endregion

        #region Referers

        [OperationContract]
        [WebGet(UriTemplate = "referers",
                ResponseFormat = WebMessageFormat.Json)]
        ReferersResponse GetReferers();

        [OperationContract]
        [WebGet(UriTemplate = "referers/{id}",
                ResponseFormat = WebMessageFormat.Json)]
        RefererResponse GetReferer(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/referers",
                   Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool AddReferer(RefererRequest referer);

        [OperationContract]
        [WebInvoke(UriTemplate = "/referers/{id}",
                   Method = "PUT",
                   RequestFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare)]
        bool EditRefererWithId(RefererRequest referer, string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/referers/{id}",
            Method = "DELETE")]
        bool DeleteReferer(string id);
        
        #endregion
    }
}
