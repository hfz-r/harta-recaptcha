using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Recaptcha.Design;

namespace Recaptcha
{
    /// <summary>
    /// This class encapsulates reCAPTCHA UI and logic into an ASP.NET server control.
    /// </summary>
    [ToolboxData("<{0}:RecaptchaControl runat=\"server\" />")]
    [Designer(typeof(RecaptchaControlDesigner))]
    public class RecaptchaControl : WebControl, IValidator
    {
        #region Private Fields

        private const string RecaptchaHost = "https://www.google.com/recaptcha/api.js";
        private const string RecaptchaResponseField = "g-recaptcha-response";

        private bool _skipRecaptcha;
        private RecaptchaResponse _recaptchaResponse;

        #endregion

        #region Public Properties

        [Category("Settings")]
        [Description("The site key from https://www.google.com/recaptcha/admin/create. Can also be set using RecaptchaSiteKey in AppSettings.")]
        public string SiteKey { get; set; }

        [Category("Settings")]
        [Description("The secret key from https://www.google.com/recaptcha/admin/create. Can also be set using RecaptchaSecretKey in AppSettings.")]
        public string SecretKey { get; set; }

        [Category("Settings")]
        [DefaultValue(false)]
        [Description("Set this to true to stop reCAPTCHA validation. Can also be set using RecaptchaSkipValidation in AppSettings.")]
        public bool SkipRecaptcha
        {
            get { return _skipRecaptcha; }
            set { _skipRecaptcha = value; }
        }

        #region Associated control 

        [Description("The ID of the HiddenField control to wrapped Browser API."), DefaultValue(""), Category("Behavior")]
        public virtual string HiddenFieldControlId
        {
            get
            {
                var o = ViewState["HiddenFieldControlId"];

                if (o != null)
                    return (string)o;

                return string.Empty;
            }
            set
            {
                ViewState["HiddenFieldControlId"] = value;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaControl"/> class.
        /// </summary>
        public RecaptchaControl()
        {
            SiteKey = ConfigurationManager.AppSettings["RecaptchaSiteKey"];
            SecretKey = ConfigurationManager.AppSettings["RecaptchaSecretKey"];
            if (!bool.TryParse(ConfigurationManager.AppSettings["RecaptchaSkipValidation"], out _skipRecaptcha))
            {
                _skipRecaptcha = false;
            }
        }

        #region Private/Protected members

        private static string GenerateRecaptchaScript()
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(RecaptchaHost);
            return urlBuilder.ToString();
        }

        /// <summary>
        /// Iterates through the Page.Validators property and look for registered instance of <see cref="RecaptchaControl"/>.
        /// </summary>
        /// <returns>True if an instance is found, False otherwise.</returns>
        private bool CheckIfRecaptchaExists()
        {
            return Page.Validators.OfType<RecaptchaControl>().Any();
        }

        protected virtual HiddenField GetHiddenFieldControl()
        {
            if (string.IsNullOrEmpty(this.HiddenFieldControlId))
                throw new HttpException(string.Format("You must provide a value for the HiddenFieldControlId property for the Recaptcha control with ID '{0}'.", this.ID));

            HiddenField hf = this.FindControl(this.HiddenFieldControlId) as HiddenField;
            if (hf == null)
                throw new HttpException(string.Format("The Recaptcha control with ID '{0}' could not find a HiddenField control with the ID '{1}'.", this.ID, this.HiddenFieldControlId));

            return hf;
        }

        #endregion

        #region Overriden Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SecretKey))
            {
                throw new ApplicationException("reCAPTCHA needs to be configured with a site & secret key.");
            }

            if (!CheckIfRecaptchaExists())
            {
                Page.Validators.Add(this);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Page != null && !this.Page.ClientScript.IsClientScriptIncludeRegistered("reCAPTCHA"))
                this.Page.ClientScript.RegisterClientScriptInclude("reCAPTCHA", Page.ClientScript.GetWebResourceUrl(GetType(), "Recaptcha.recaptcha-script.js"));

            var options = "{ method: 'GET', url: 'https://jsonplaceholder.typicode.com/todos/1' }";
            this.Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), string.Format("fetch({0},{1});", options, "function(e) {}"), true);
            this.Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), string.Format("var state = '{0}';", GetHiddenFieldControl().ClientID), true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (_skipRecaptcha)
            {
                //writer.WriteLine("reCAPTCHA validation is skipped. Set SkipRecaptcha property to false to enable validation.");
            }
            else
            {
                RenderContents(writer);
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            // <script> 
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.AddAttribute(HtmlTextWriterAttribute.Src, GenerateRecaptchaScript(), false);
            output.AddAttribute("async", null);
            output.AddAttribute("defer", null);
            output.RenderBeginTag(HtmlTextWriterTag.Script);
            output.RenderEndTag();

            // <g-recaptcha> 
            output.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
            output.AddAttribute("data-sitekey", SiteKey);
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.RenderEndTag();
        }

        #endregion

        #region IValidator Members

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ErrorMessage
        {
            get { return _recaptchaResponse != null ? _recaptchaResponse.ErrorMessage : null; }
            set { throw new NotImplementedException("ErrorMessage property is not settable."); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsValid
        {
            get { return _recaptchaResponse == null || _recaptchaResponse.IsValid; }
            set { throw new NotImplementedException("IsValid property is not settable. This property is populated automatically."); }
        }

        /// <summary>
        /// Perform validation of reCAPTCHA.
        /// </summary>
        public void Validate()
        {
            if (Page.IsPostBack && Visible && Enabled && (GetHiddenFieldControl().Value != "offline") && !_skipRecaptcha)
            {
                if (_recaptchaResponse == null)
                {
                    if (Visible && Enabled)
                    {
                        RecaptchaValidator validator = new RecaptchaValidator
                        {
                            SecretKey = SecretKey,
                            Response = Context.Request.Form[RecaptchaResponseField]
                        };

                        _recaptchaResponse = validator.Response == null
                            ? RecaptchaResponse.InvalidResponse
                            : validator.Validate();
                    }
                }
            }
            else
            {
                _recaptchaResponse = RecaptchaResponse.Valid;
            }
        }

        #endregion
    }
}