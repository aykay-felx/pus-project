using System.Text;
using schools_web_api.Model;
using schools_web_api.TokenManager.Exceptions;
using schools_web_api.TokenManager.Extensions;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api.TokenManager.ServiceHelpers
{
    public class UserServiceHelper
    {
        public string prepareUsersUpdateStatement(User oldData, User newData)
        {
            string[] ignoredCompareProperties = new[] { "Id", "Password" };

            var comparision = newData.isDifferentThan(oldData);

            if (!comparision.isDifferent)
            {
                throw new DataException("Modified data cannot be the same as the old one");
            }

            var firstname = newData.Firstname.isValid() && newData.Firstname != oldData.Firstname ? $"'{newData.Firstname}'" : "null";
            var lastname = newData.Lastname.isValid() && newData.Lastname != oldData.Lastname ? $"'{newData.Lastname}'" : "null";
            var email = newData.Email.isValidEmail() && newData.Email != oldData.Email ? $"'{newData.Email}'" : "null";
            var user_role = newData.Role.isValid() && newData.Role != oldData.Role ? $"'{newData.Role}'" : "null";

            return $"SELECT update_user({oldData.Id},{firstname},{lastname},{email},{user_role})";
        }

        public string preprareUsersSelectStatement(int[] exceptIndecies)
	    {
            var selectBuilder = new StringBuilder("SELECT * FROM users_view");

            if (exceptIndecies == null || exceptIndecies.Length == 0) { return selectBuilder.ToString(); }

            selectBuilder.Append(" WHERE ID NOT IN(");

            for (int i = 0; i < exceptIndecies.Length; i++)
            {
                bool isLastValue = i == exceptIndecies.Length - 1;
                selectBuilder.Append(isLastValue ? $"{exceptIndecies[i]})" : $"{exceptIndecies[i]},");
            }

            return selectBuilder.ToString();
	    }

        public void validateNewUserData(User user)
        {
            bool isValid = user.Email.isValidEmail()
                && user.Firstname.isValid()
                && user.Lastname.isValid()
                && user.Password.isValid()
                && user.Email.isValidEmail()
                && user.Role.isValid() &&  (user.Role is "admin" or "super-admin");

            if (!isValid)
            {
                throw new ValidationException("New user data is invalid");
            }
        }

        public void validateAutenticateRequest(AuthenticateRequest ar)
        {
            if (!ar.areParametersValid())
            {
                throw new ValidationException("Authenticate body was not valid");
            }
        }

        public void validateRequestUserId(int id)
        {
            if (id < 0)
            {
                throw new ValidationException("Invalid id was given");
            }
        }

        public void validateChangePasswordRequest(ChangePasswordRequest req)
        {
            if (!req.areValid()) 
            {
                throw new ValidationException("Change password request data was invalid");
            }
        }
    }
}