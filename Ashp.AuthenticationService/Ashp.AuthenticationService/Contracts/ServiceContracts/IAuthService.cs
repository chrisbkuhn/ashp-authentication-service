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
    public interface IAuthService
    {
        #region Authentication
        [OperationContract]
        [WebGet(UriTemplate = "authenticate?authorization={authorization}&referer={referer}",
                ResponseFormat = WebMessageFormat.Json)]
        AshpLogin AuthenticateJson(string authorization, string referer);

        [OperationContract]
        [WebGet(UriTemplate = "authenticate/xml?authorization={authorization}&referer={referer}",
                ResponseFormat = WebMessageFormat.Xml)]
        AshpLogin AuthenticateXml(string authorization, string referer);
        #endregion
    }
}
