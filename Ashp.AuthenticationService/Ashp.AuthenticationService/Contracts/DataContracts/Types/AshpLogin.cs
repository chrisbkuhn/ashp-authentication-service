using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ashp.AuthenticationService.Contracts.DataContracts.Types
{
    [DataContract]
    public class AshpLogin 
    {
        #region Constructors

        public AshpLogin() { }

        #endregion

        [DataMember]
        public AuthenticationState AuthenticationState { get; set; }

        [DataMember]
        public AuthenticationType AuthenticationType { get; set; }
        [DataMember]
        public string AuthenticationTypeName{ get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public string ExpirationDateString { get; set; }

        [DataMember]
        public int DaysUntilExpiration { get; set; }

        [DataMember]
        public bool LoginOk { get; set; }

        [DataMember]
        public bool ProductPurchased { get; set; }

        [DataMember]
        public bool ServerOk { get; set; }

        [DataMember]
        public string ErrorText { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string UserFullName { get; set; }

        [DataMember]
        public string IpAddress { get; set; }

        [DataMember]
        public IpType IpType { get; set; }

        [DataMember]
        public string RefererUrl { get; set; }

        
        public string Serialize()
        {
            var sb = new StringBuilder();

            var xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add(string.Empty, string.Empty);

            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                new XmlSerializer(GetType()).Serialize(writer, this, xmlnsEmpty);
            }

            return sb.ToString();
        }

    }
}
