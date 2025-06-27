using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace SSLPinningBypass
{
    public partial class Form1 : Form
    {
        HttpClientHandler handler;
        HttpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;

            client = new HttpClient(handler);
        }

        public static bool ValidateServerCertificate(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return !certificate.IssuerName.Name.Contains("PortSwigger");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var response = client.GetAsync("https://blog.davidc96.com");
                MessageBox.Show(response.Result.StatusCode.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }
    }
}
