using System;
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
    }
}