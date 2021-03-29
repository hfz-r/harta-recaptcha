using System;
using System.Net;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace RecaptchaTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RecaptchaButton_Click(object sender, EventArgs e)
        {
            this.RecaptchaResult.Text = this.Page.IsValid ? "Success" : this.RecaptchaControl.ErrorMessage;
        }

        [WebMethod]
        public static string HelloWorld()
        {
            string url = "https://global-reporting.hartalega.com.my/favicon.ico";
            return (new WebClient()).DownloadString(url);
        }
    }
}