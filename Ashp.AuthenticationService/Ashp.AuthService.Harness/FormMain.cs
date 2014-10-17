
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Ashp.AuthService.Harness
{
    public partial class FormMain : Form
    {
        #region Fields

        Uri _BaseAddress = null;
        string _Username = null;
        string _Password = null;
        string _Appcode = null;
        //AshpLogin _AL;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string uri = System.Configuration.ConfigurationManager.AppSettings["BaseURI"];
            _BaseAddress = new Uri(uri);
        }

        private void cmdAuthenticate_Click(object sender, EventArgs e)
        {
            string response = null;
            WebRequest request = null;
            WebResponse ws = null;

            string login = txtUsername.Text;
            string password = txtPassword.Text;
            string appcode = txtPassword.Text;
            txtAuthResult.Text = "";
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password)) return;



            string[] credentials = {
                                       "erin.lanham@mallinckrodt.com:Pharmainf0:HID-ONLINE",
                                       "062701@test.com:aw3se4:HID-ONLINE",
                                       "tlacosta@hotmail.com.test:aw3se4:HID-ONLINE",
                                       "champ3@gmail.com.test:aw3se4:PPO1",
                                       "mcoleman@pechs.net.test:aw3se4:PPO12"
                                   };

            foreach (var c in credentials)
            {
                var bytes = Encoding.ASCII.GetBytes(c);
                var base64 = Convert.ToBase64String(bytes);
            }

            // Make the request
            string url = _BaseAddress + "authenticate";
            request = WebRequest.Create(url);

            // Populate the security credentials
            SetCredentials(request, login, password, appcode);

            ws = request.GetResponse();

            // Get a string response
            var enc = System.Text.Encoding.GetEncoding(1252);
            var loResponseStream = new StreamReader(ws.GetResponseStream(), enc);
            response = loResponseStream.ReadToEnd();
            response = HttpUtility.HtmlDecode(response);

            // Remove the namespaces
            response = response.Replace(" xmlns=\"http://tempuri.org/\"", "");

            // Load the result into an xml document
            var doc = new XmlDocument();
            //doc.LoadXml(response);

            // If possible, drill down past the REST wrapper
            string xmlStr = null;
            var listNode = doc.SelectSingleNode("/AuthenticateResponse/AuthenticateResult/AshpLogin");
            if (listNode != null)
            {
                xmlStr = listNode.OuterXml;
                _Username = login;
                _Password = password;
                //_AL = new AshpLogin(listNode);
            }
            else
            {
                xmlStr = doc.OuterXml;
            }
            txtAuthResult.Text = xmlStr;
            txtXML.Text = xmlStr;

        }

        private void SetCredentials(WebRequest request, string username, string password, string appcode)
        {
            if (string.IsNullOrEmpty(username)) username = _Username;
            if (string.IsNullOrEmpty(password)) password = _Password;
            if (string.IsNullOrEmpty(appcode)) appcode = _Appcode;

            var credentials = String.Format("{0}:{1}:{2}", username, password, appcode);

            var bytes = Encoding.ASCII.GetBytes(credentials);
            var base64 = Convert.ToBase64String(bytes);
            var authorization = String.Concat("basic ", base64);

            request.Headers.Add("Authorization", authorization);
        }

    }
}
