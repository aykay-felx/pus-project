using System;
using System.Text;
using Newtonsoft.Json;
using schools_web_api.Model;
using schools_web_api.TokenManager.Exceptions;
using schools_web_api.TokenManager.Extensions;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api.TokenManager.ServiceHelpers
{
    public class SchoolServiceHelper
    {
        public void ValidateNewSchoolData(FullSchool school)
        {
            if (!ValidatePolandCoordinates(school.Longtitude, school.Latitude))
            {
                throw new ValidationException("School coordinates are not within Poland coordinates");
            }

            if (!SchoolBusinessDataIsValid(school.BusinessData))
            {
                throw new ValidationException("School business data is invalid");
            }
        }

        private bool SchoolBusinessDataIsValid(FullSchoolBusinessData businessData)
        {
            try 
            {
                JsonConvert.SerializeObject(businessData, Formatting.None);
                return true;
            } 
            catch
            {
                return false;
            }
        }

        public void ValidateRequestSchoolId(int id)
        {
            if (id < 0)
            {
                throw new ValidationException("Id is not supported");
            }
        }
        public string prepareUpdateSchoolCommand(FullSchool oldData, FullSchool newData)
        {
            bool coordsHaveChanged = oldData.Latitude != newData.Latitude || oldData.Longtitude != newData.Longtitude;
            var newDataComparision = newData.isDifferentThan(oldData);

            if (!coordsHaveChanged && !newDataComparision.isDifferent) 
            { 
                throw new DataException("New data has to be different than the old one");
            }

            StringBuilder sb = new StringBuilder($"SELECT update_school({oldData.Id},");

            if (coordsHaveChanged)  {
                string lon = newData.Longtitude.ToString().Replace(',','.');
                string lat = newData.Latitude.ToString().Replace(',', '.');

                sb.Append($"{lon},{lat},");
            }
            else {
                sb.Append("null, null,");
            }

            string businessData = newDataComparision.isDifferent ? 
                $"'{JsonConvert.SerializeObject(newData.BusinessData, Formatting.None)}'" : "null";

            sb.Append($"{businessData});");

            return sb.ToString();
        }

        public string prepareSchoolsSelectStatements(SchoolRequestParameters srp)
        {
            var selectBuilder = new StringBuilder("SELECT * FROM get_schools");

            if (srp.areEmpty()) 
            { 
                return selectBuilder.Append("()").ToString();
            }

            var voivodeship = !string.IsNullOrEmpty(srp.Voivodeship) && srp.Voivodeship.isValid() ? $"'{srp.Voivodeship}'" : "null";
            var cities = !srp.City.isEmpty() && srp.City.isValid() ? $"'{String.Join('|', srp.City)}'" : "null";
            var audience = !string.IsNullOrEmpty(srp.Audience) && srp.Audience.isValid() ? $"'{srp.Audience}'" : "null";
            var name = !string.IsNullOrEmpty(srp.Name) && srp.Name.isValid() ? $"'{srp.Name}'" : "null";

            selectBuilder.Append($"({audience},{name},{voivodeship},{cities})");

            return selectBuilder.ToString();
        }

        public string prepareInsertSchoolCommand(FullSchool fs)
        {
            string lon = fs.Longtitude.ToString().Replace(',', '.');
            string lat = fs.Latitude.ToString().Replace(',', '.');
            var businessDataJson = JsonConvert.SerializeObject(fs.BusinessData, Formatting.Indented);

            string insertCommand = $"SELECT add_school({lon}, {lat}, '{businessDataJson}');";

            return insertCommand;
        }

        private bool ValidatePolandCoordinates(double lon, double lat)
        {
            double minPolandLatitude = 49.29899;
            double maxPolandLatitude = 54.79086;
            double minPolandLongtitude = 14.24712;
            double maxPolandLongtitude = 23.89251;

            return lat >= minPolandLatitude && lat <= maxPolandLatitude
                && lon >= minPolandLongtitude && lon <= maxPolandLongtitude;
        }
    }
}