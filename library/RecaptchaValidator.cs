using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Recaptcha
{
    /// <summary>
    /// Calls the reCAPTCHA server to validate the answer to a reCAPTCHA challenge. 
    /// </summary>
    public class RecaptchaValidator
    {
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        public string SecretKey { get; set; }
        public string Response { get; set; }

        public RecaptchaResponse Validate()
        {
            if (Response == string.Empty)
            {
                return RecaptchaResponse.InvalidSolution;
            }

            var request = (HttpWebRequest) WebRequest.Create(VerifyUrl);
            request.Timeout = 30 * 1000;
            request.Method = "POST";
            request.UserAgent = "Harta/reCAPTCHA";
            request.ContentType = "application/x-www-form-urlencoded";

            string formdata = String.Format("secret={0}&response={1}", HttpUtility.UrlEncode(this.SecretKey), HttpUtility.UrlEncode(this.Response));
            byte[] formbytes = Encoding.ASCII.GetBytes(formdata);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formbytes, 0, formbytes.Length);
            }

            RecaptchaResponseDto response;

            try
            {
                using (WebResponse httpResponse = request.GetResponse())
                {
                    using (TextReader readStream = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        var streamResult = readStream.ReadToEnd();
                        response = JsonConvert.DeserializeObject<RecaptchaResponseDto>(streamResult);
                    }
                }
            }
            catch (WebException ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error);
                return RecaptchaResponse.RecaptchaNotReachable;
            }

            switch (response.Success)
            {
                case true:
                    return RecaptchaResponse.Valid;
                case false:
                    return new RecaptchaResponse(false, string.Join(", ", response.ErrorCodes));
                default:
                    throw new InvalidProgramException("Unknown status response.");
            }
        }
    }
}