using Ashp.AuthenticationService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;

namespace Ashp.AuthenticationService.WcfExtension
{
    public class CustomMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        #region IClientMessageInspector Members

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            var originalMessage = buffer.CreateMessage();

            //LogHelper.Log(LogFormatter.FormatServerRequest(originalMessage));

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            reply = buffer.CreateMessage();
            var originalMessage = buffer.CreateMessage();

            //LogHelper.Log(LogFormatter.FormatServerResponse(originalMessage));
        }

        #endregion

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            var originalMessage = buffer.CreateMessage();


            var httpReq = (HttpRequestMessageProperty)originalMessage.Properties[HttpRequestMessageProperty.Name];

            //LogHelper.Log(LogFormatter.FormatClientRequest(originalMessage));
            var txt = LogFormatter.SerializeMessage(originalMessage);

            var destinationPath = originalMessage.Headers.To.AbsolutePath;

            // Uncomment to turn off Server-Side check for Session Token
            //return null;

            if (destinationPath.EndsWith(@"/session") ||
                destinationPath.EndsWith(@"/session/"))
                return null;

            string token, accountId;
            if (GetAuthTokenAndAccountId(httpReq, out token, out accountId) &&
                DAL.AshpDbEntities.CheckSession(token))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
            }
            else
            {
                throw new WebFaultException<string>("Session has expired or has not been set. Please log in.", 
                                                    System.Net.HttpStatusCode.Unauthorized);
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            reply = buffer.CreateMessage();
            var originalMessage = buffer.CreateMessage();

            //LogHelper.Log(LogFormatter.FormatClientResponse(originalMessage));
            var txt = LogFormatter.SerializeMessage(originalMessage, correlationState);
        }


        private bool GetAuthTokenAndAccountId(HttpRequestMessageProperty httpRequest, out string token, out string accountId)
        {
            token = httpRequest.Headers["X-AUTHENTICATION-TOKEN"];
            accountId = null;

            var sessionCookies = httpRequest.Headers["Cookie"];
            string cookieToken;
            if (sessionCookies != null)
            {
                var cookies = sessionCookies.Split(';');

                cookieToken = cookies.Where(cp => string.Compare(GetCookieName(cp), "authToken", StringComparison.InvariantCultureIgnoreCase) == 0)
                                     .FirstOrDefault();
                accountId = cookies.Where(cp => string.Compare(GetCookieName(cp), "authToken", StringComparison.InvariantCultureIgnoreCase) == 0)
                                   .FirstOrDefault();

                if (token == null)
                    token = cookieToken;
            }

            return token != null;
        }

        private string GetCookieName(string cookie)
        {
            var end = cookie.IndexOf('=');
            if (end == -1)
                return null;

            return cookie.Substring(0, end + 1);
        }
        #endregion
    }
}