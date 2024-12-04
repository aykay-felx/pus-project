using Npgsql;
using schools_web_api.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using schools_web_api.TokenManager.Exceptions;
using schools_web_api.TokenManager.ObjectMapping;
using schools_web_api.TokenManager.TransmitModels;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.ServiceHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;

namespace schools_web_api.TokenManager.Services.Implementation
{
    public class NewSchoolService : INewService
    {
        private readonly string _connectionString;
        private readonly SchoolServiceHelper _helper;
        public NewSchoolService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Postgres");
            _helper = new SchoolServiceHelper();
        }


        public async Task<List<FullSchool>> GetSchoolsAsync(SchoolRequestParameters body)
        {
            List<FullSchool> schools = new List<FullSchool>();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // SQL-запрос для выборки всех школ
                string selectStatement = @"
            SELECT s.id, s.longtitude, s.latitude, s.business_data
            FROM private_schools_view s";

                using var cmd = new NpgsqlCommand(selectStatement, connection);

                using var reader = await cmd.ExecuteReaderAsync();

                // Чтение всех строк из результата
                while (await reader.ReadAsync())
                {
                    var school = ObjectMapper.MapFullSchoolObject(reader);
                    if (school != null)
                    {
                        schools.Add(school);
                    }
                }

                return schools;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }
    }
}