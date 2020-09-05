using System;
using Newtonsoft.Json;

namespace Recaptcha
{
    /// <summary>
    /// Encapsulates a response from reCAPTCHA upstream.
    /// </summary>
    public class RecaptchaResponse
    {
        public static readonly RecaptchaResponse Valid = new RecaptchaResponse(true, string.Empty);
        public static readonly RecaptchaResponse InvalidResponse = new RecaptchaResponse(false, "Invalid reCAPTCHA request. Missing response value.");
        public static readonly RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "The verification words are incorrect.");
        public static readonly RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(false, "The reCAPTCHA server is unavailable.");

        private readonly bool isValid;
        private readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaResponse"/> class.
        /// </summary>
        /// <param name="isValid">Value indicates whether submitted reCAPTCHA is valid.</param>
        /// <param name="errorCode">Error code returned from reCAPTCHA web service.</param>
        internal RecaptchaResponse(bool isValid, string errorMessage)
        {
            RecaptchaResponse templateResponse = null;

            if (IsValid)
            {
                templateResponse = Valid;
            }
            else
            {
                switch (errorMessage)
                {
                    case "incorrect-captcha-sol":
                        templateResponse = InvalidSolution;
                        break;
                    case null:
                        throw new ArgumentNullException("errorMessage");
                }
            }

            if (templateResponse != null)
            {
                this.isValid = templateResponse.IsValid;
                this.errorMessage = templateResponse.ErrorMessage;
            }
            else
            {
                this.isValid = isValid;
                this.errorMessage = errorMessage;
            }
        }

        public bool IsValid
        {
            get { return isValid; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        public override bool Equals(object obj)
        {
            RecaptchaResponse other = (RecaptchaResponse)obj;
            if (other == null)
            {
                return false;
            }

            return other.IsValid == isValid && other.ErrorMessage == errorMessage;
        }

        public override int GetHashCode()
        {
            return isValid.GetHashCode() ^ errorMessage.GetHashCode();
        }
    }

    public class RecaptchaResponseDto
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}