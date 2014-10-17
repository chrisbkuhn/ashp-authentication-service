//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
//using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Ashp.AuthenticationService.Helpers
{
    public class LogHelper
    {
        static LogHelper()
        {
            //var configurationSource = ConfigurationSourceFactory.Create();
            //var logWriterFactory = new LogWriterFactory(configurationSource);

            //var logWriterFactory = new LogWriterFactory();
            //Logger.SetLogWriter(logWriterFactory.Create(), false); 
        }

        #region Main Log Method for Logfile
        /// <summary>
        /// Main logging method.  
        /// Logs a string after a datetime stamp
        /// </summary>
        /// <param name="logEntry">The me ssage being logged after the datetime stamp</param>
        /// <returns>Success</returns>
        public static bool Log(string logEntry)
        {
            try
            {
                //Logger.Write(logEntry);
                return true;
            }
            catch (Exception ex)
            {
                EventLog(LogFormatter.FormatException(ex), System.Diagnostics.EventLogEntryType.Error);

                return false;
            }
        }

        #endregion

        #region Main Log Methods for EventLog

        public static bool EventLog(string logEntry)
        {
            return EventLog(logEntry, System.Diagnostics.EventLogEntryType.Information);
        }

        public static bool EventLog(string logEntry, System.Diagnostics.EventLogEntryType eventLogEntryType = System.Diagnostics.EventLogEntryType.Information)
        {
            try
            {
                var appLog = new System.Diagnostics.EventLog();
                appLog.Source = "AshpAuthService";

                if (System.Diagnostics.EventLog.SourceExists(appLog.Source))
                    System.Diagnostics.EventLog.CreateEventSource(appLog.Source, "Application");
    
                appLog.WriteEntry(logEntry, eventLogEntryType, 88865);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool EventLog(Exception ex)
        {
            return EventLog(LogFormatter.FormatException(ex));
        }

        #endregion
    }

    public static class LogFormatter
    {
        private const string Line = "----------------------------------------";
        private const string ClientRequestLabel = "Request From Client";
        private const string ClientResponseLabel = "Response To Client";
        private const string ServerRequestLabel = "Request To Server";
        private const string ServerResponseLabel = "Response From Server";


        //public static string FormatBlack011Exception(Exceptions.Black011Exception exception)
        //{
        //    return FormatBlack011Exception(exception, string.Empty);
        //}

        //public static string FormatBlack011Exception(Exceptions.Black011Exception exception, string customName)
        //{
        //    customName = (customName.Equals(string.Empty)) ? string.Empty : customName.Trim().ToUpper() + " - ";

        //    return customName + "EXCEPTION:\r\n-----------------------------------------------\r\nBLACK011 ERRORCODE:\t"
        //                + exception.ErrorCode
        //                + "\r\nBLACK011 ERRORMESSAGE:\t"
        //                + exception.ErrorMessage
        //                + "\r\nMESSAGE: "
        //                + exception.Message
        //                + "\r\nINNER EXCEPTION:"
        //                + exception.InnerException
        //                + "\r\nSTACK TRACE:\r\n"
        //                + exception.StackTrace
        //                + "\r\n-----------------------------------------------\r\n";
        //}

        public static string FormatException(Exception exception)
        {
            return FormatException(exception, string.Empty);
        }

        public static string FormatException(Exception exception, string customName)
        {
            customName = (customName.Equals(string.Empty)) ? string.Empty : customName.Trim().ToUpper() + " - ";

            return customName + "EXCEPTION:\r\n-----------------------------------------------\r\nMESSAGE: "
                        + exception.Message
                        + "\r\nINNER EXCEPTION:"
                        + exception.InnerException
                        + "\r\nSTACK TRACE:\r\n"
                        + exception.StackTrace
                        + "\r\n-----------------------------------------------\r\n";
        }

        public static string FormatFaultException(System.ServiceModel.FaultException exception)
        {
            return FormatFaultException(exception, string.Empty);
        }

        public static string FormatFaultException(System.ServiceModel.FaultException exception, string customName)
        {
            customName = (customName.Equals(string.Empty)) ? string.Empty : customName.Trim().ToUpper() + " - ";

            System.ServiceModel.Channels.MessageFault messageFault = exception.CreateMessageFault();
            System.Xml.XmlElement exceptionDetail = null;
            if (messageFault.HasDetail) exceptionDetail = messageFault.GetDetail<System.Xml.XmlElement>();

            return customName + "FAULT EXCEPTION:\r\n-----------------------------------------------\r\nMESSAGE: "
                        + exception.Message
                        + "\r\nINNER EXCEPTION:"
                        + exception.InnerException
                        + "\r\nSTACK TRACE:\r\n"
                        + exception.StackTrace
                        + "\r\nFAULT CODE:\r\n"
                        + exception.Code.Name
                        + "\r\nFAULT REASON:\r\n"
                        + ((exceptionDetail != null) ? exceptionDetail.OuterXml : "No Exception Detail Returned")
                        + "\r\n-----------------------------------------------\r\n";
        }



        public static string FormatClientRequest(object request)
        {
            return FormatObject(request, ClientRequestLabel);
        }

        public static string FormatClientRequest(string request)
        {
            return FormatString(request, ClientRequestLabel);
        }

        public static string FormatClientRequest(Message wcfMessageRequest)
        {
            return FormatWcfMessage(wcfMessageRequest, ClientRequestLabel);
        }


        public static string FormatClientResponse(object response)
        {
            return FormatObject(response, ClientResponseLabel);
        }

        public static string FormatClientResponse(string request)
        {
            return FormatString(request, ClientResponseLabel);
        }

        public static string FormatClientResponse(Message wcfMessageResponse)
        {
            return FormatWcfMessage(wcfMessageResponse, ClientResponseLabel);
        }



        public static string FormatServerRequest(string request)
        {
            return FormatString(request, ServerRequestLabel);
        }

        public static string FormatServerRequest(Message request)
        {
            return FormatWcfMessage(request, ServerRequestLabel);
        }


        public static string FormatServerResponse(string response)
        {
            return FormatString(response, ServerResponseLabel);
        }

        public static string FormatServerResponse(Message response)
        {
            return FormatWcfMessage(response, ServerResponseLabel);
        }



        private static string FormatObject(object obj, string customName)
        {
            return FormatString(Serialize(obj), customName);
        }

        private static string FormatWcfMessage(Message message, string customName)
        {
            //return FormatString(SerializeWcfMessage(message), customName);
            return FormatString(SerializeMessage(message), customName);
        }

        private static string FormatString(string message, string customName)
        {
            var dateString = String.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF"));

            customName = (customName.Equals(string.Empty))
                             ? dateString
                             : String.Format("{0}\t-\t{1}", dateString, customName.Trim());

            var logEntry = String.Format("{0}{1}{2}{1}{3}",
                                         customName,
                                         Environment.NewLine,
                                         Line,
                                         message);

            return logEntry;
        }



        #region Helper Methods

        private static string Serialize(object obj)
        {
            var text = new StringBuilder();
            //var writer = new StringWriter(text);
            var writer = XmlWriter.Create(text, new XmlWriterSettings
            {
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                OmitXmlDeclaration = true,
                Indent = true
            });

            if (obj == null)
                return "Object is NULL and cannot be serialized";

            try
            {
                var nsSerializer = new XmlSerializerNamespaces();
                nsSerializer.Add(string.Empty, string.Empty);

                var serial = new XmlSerializer(obj.GetType());

                //serial.Serialize(writer, obj);
                serial.Serialize(writer, obj, nsSerializer);
            }
            catch
            {
                text.Remove(0, text.Length);
                text.Append("Object " + obj + ", of type " + obj.GetType() + " cannot be serialized");
            }
            finally
            {
                writer.Close();
                //writer.Dispose();
            }

            return text.ToString();
        }

        private static string SerializeWcfMessage(Message message)
        {
            var text = new StringBuilder();
            var writer = XmlWriter.Create(text, new XmlWriterSettings
            {
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                OmitXmlDeclaration = true,
                Indent = true
            });

            if (message == null)
                return "Message is NULL and cannot be serialized";

            try
            {
                message.WriteMessage(writer);
            }
            catch (Exception ex)
            {
                text.Remove(0, text.Length);
                text.Append(String.Format("Message cannot be serialized. Error: {0}", ex.Message));
            }
            finally
            {
                writer.Close();
            }

            return text.ToString();
        }

        public static string SerializeMessage(Message message, object correlationState=null)
        {
            var text = new StringBuilder();
            var requestUri = message.Headers.To;

            if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                var httpReq = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];

                text.AppendFormat("{0} {1}", httpReq.Method, requestUri);
                text.AppendLine();
                foreach (var header in httpReq.Headers.AllKeys)
                {
                    text.AppendFormat("{0}: {1}", header, httpReq.Headers[header]);
                    text.AppendLine();
                }
            }

            if (message.Properties.ContainsKey(HttpResponseMessageProperty.Name))
            {
                if (correlationState != null)
                {
                    text.AppendFormat("Response to request to {0}:", (Uri)correlationState);
                    text.AppendLine();
                }
                var httpResp = (HttpResponseMessageProperty)message.Properties[HttpResponseMessageProperty.Name];
                text.AppendFormat("{0} {1}", (int)httpResp.StatusCode, httpResp.StatusCode);
                text.AppendLine();
            }

            if (!message.IsEmpty)
            {
                text.AppendLine();
                text.AppendLine(MessageToString(ref message));
            }

            return text.ToString();
        }



        private static WebContentFormat GetMessageContentFormat(Message message)
        {
            var format = WebContentFormat.Default;
            if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                var bodyFormat = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
                format = bodyFormat.Format;
            }

            return format;
        }

        private static string MessageToString(ref Message message)
        {
            var messageFormat = GetMessageContentFormat(message);
            var ms = new MemoryStream();

            XmlDictionaryWriter writer = null;

            switch (messageFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    writer = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    // special case for raw, easier implemented separately 
                    return ReadRawBody(ref message);
            }

            message.WriteMessage(writer);
            writer.Flush();
            string messageBody = Encoding.UTF8.GetString(ms.ToArray());

            // Here would be a good place to change the message body, if so desired. 

            // now that the message was read, it needs to be recreated. 
            ms.Position = 0;

            // if the message body was modified, needs to reencode it, as show below 
            // ms = new MemoryStream(Encoding.UTF8.GetBytes(messageBody)); 

            XmlDictionaryReader reader;
            if (messageFormat == WebContentFormat.Json)
            {
                reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
            }
            else
            {
                reader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
            }

            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }

        private static string ReadRawBody(ref Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] bodyBytes = bodyReader.ReadContentAsBase64();
            string messageBody = Encoding.UTF8.GetString(bodyBytes);

            // Now to recreate the message 
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
            writer.WriteStartElement("Binary");
            writer.WriteBase64(bodyBytes, 0, bodyBytes.Length);
            writer.WriteEndElement();
            writer.Flush();
            ms.Position = 0;
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }



        #endregion
    }
}