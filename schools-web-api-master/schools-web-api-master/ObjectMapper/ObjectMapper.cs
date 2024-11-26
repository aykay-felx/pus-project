using Npgsql;
using System;
using Newtonsoft.Json;
using schools_web_api.Model;

namespace schools_web_api.TokenManager.ObjectMapping {
    public static class ObjectMapper {
        public static SimpleSchool MapSimpleSchoolObject(NpgsqlDataReader reader) 
        {
            try {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                double lon = reader.GetDouble(reader.GetOrdinal("longtitude"));
                double lat = reader.GetDouble(reader.GetOrdinal("latitude"));

                string json = reader.GetString(reader.GetOrdinal("business_data"));

                var businessData = JsonConvert.DeserializeObject<SimpleSchoolBusinessData>(json);

                return new SimpleSchool(id, lon, lat, businessData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static FullSchool MapFullSchoolObject(NpgsqlDataReader reader) 
        {
            try 
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                double lon = reader.GetDouble(reader.GetOrdinal("longtitude"));
                double lat = reader.GetDouble(reader.GetOrdinal("latitude"));

                string json = reader.GetString(reader.GetOrdinal("business_data"));

                var businessData = JsonConvert.DeserializeObject<FullSchoolBusinessData>(json);

                return new FullSchool(id, lon, lat, businessData);
            }
            catch
            {
                return null;
            }
        }
        
        #nullable enable
        public static User? MapUserObject(NpgsqlDataReader reader)
        {
            try 
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string name = reader.GetString(reader.GetOrdinal("firstname"));
                string lastname = reader.GetString(reader.GetOrdinal("lastname"));
                string email = reader.GetString(reader.GetOrdinal("email"));
                string role = reader.GetString(reader.GetOrdinal("role"));

                return new User(id, name, lastname, email, role);
            } 
            catch { return null; }
        }

        #nullable disable
    }
}