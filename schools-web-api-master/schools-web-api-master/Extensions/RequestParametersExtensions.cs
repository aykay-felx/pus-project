using System.ComponentModel.DataAnnotations;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api.TokenManager.Extensions
{
    public static class RequestParametersExtensions
    {

        #region AuthenticateRequestExtensions
        public static bool areParametersValid(this AuthenticateRequest ar)
        {
            return ar.Email.isValidEmail() && ar.Password.isValidPassword();
        }

        #endregion

        #region SchoolRequestParametersExtensions

        public static bool areEmpty(this SchoolRequestParameters psr)
        {
            bool isEmpty = psr.City.isEmpty()
                && string.IsNullOrWhiteSpace(psr.Name) 
                && string.IsNullOrWhiteSpace(psr.Audience) 
                && string.IsNullOrWhiteSpace(psr.Voivodeship);

            return isEmpty;
        }

        #endregion

        #region UserChangePasswordRequestExtensions

        public static bool areValid(this ChangePasswordRequest ucpr)
        {
            string oldPass = ucpr.OldPassword;
            string newPass = ucpr.NewPassword;

            bool passwordsAreValid = oldPass.isValidPassword() && oldPass.Length == 64
                && newPass.isValidPassword() && newPass.Length == 64;

            return passwordsAreValid && ucpr.IdUser >= 0;
        }

        #endregion
    }
}