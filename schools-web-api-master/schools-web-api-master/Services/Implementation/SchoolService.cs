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

namespace schools_web_api.TokenManager.Services.Implementation
{
    public class SchoolService : ISchoolService
    {
        private readonly string connectionString;
        private readonly SchoolServiceHelper helper;
        public SchoolService(IConfiguration configuration) 
        {
            this.connectionString = configuration.GetConnectionString("Postgres");
            this.helper = new SchoolServiceHelper();
        }

        public async Task<bool> AddSchoolAsync(FullSchool school)
        {
            this.helper.ValidateNewSchoolData(school);

            try 
            {
                string insertStatement = this.helper.prepareInsertSchoolCommand(school);
                
                using var connection = new NpgsqlConnection(this.connectionString);  
                
                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(insertStatement, connection);
                    
                var reader = await cmd.ExecuteReaderAsync();

                await reader.ReadAsync();

                bool result = (int)reader.GetValue(0) == 1 ? true : false;

                return result;
            }
            catch (NpgsqlException ex)
            { 
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<bool> DeleteSchoolAsync(int id)
        {
            this.helper.ValidateRequestSchoolId(id);

            try
            {
                string deleteStatement = $"SELECT delete_school({id})";

                using var connection = new NpgsqlConnection(this.connectionString);

                await connection.OpenAsync();

                using var command = new NpgsqlCommand(deleteStatement, connection);

                command.Prepare();

                var reader = await command.ExecuteReaderAsync();

                await reader.ReadAsync();

                bool result = (int)reader.GetValue(0) == 1 ? true : false;

                return result;
            }
            catch (NpgsqlException ex)
            { 
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<FullSchool> UpdateSchoolAsync(FullSchool oldData, FullSchool newData)
        {
            this.helper.ValidateNewSchoolData(newData);

            try
            {
                string updateStatement = this.helper.prepareUpdateSchoolCommand(oldData, newData);

                if (updateStatement == null) { return null; }
        
                using var connection = new NpgsqlConnection(this.connectionString);

                await connection.OpenAsync();

                using var command = new NpgsqlCommand(updateStatement, connection);

                var reader = await command.ExecuteReaderAsync();

                await reader.ReadAsync();

                var result = (int)reader.GetValue(0) == 1 ? newData : null;

                return result;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<FullSchool> GetSchoolByIdAsync(int id)
        {
            this.helper.ValidateRequestSchoolId(id);

            try
            {
                using var connection = new NpgsqlConnection(this.connectionString);

                await connection.OpenAsync();

                string selectStatement = $"SELECT * FROM get_school({id})";

                using var cmd = new NpgsqlCommand(selectStatement, connection);
            
                using var reader = await cmd.ExecuteReaderAsync();
                
                await reader.ReadAsync();

                var school = ObjectMapper.MapFullSchoolObject(reader);

                return school;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<List<SimpleSchool>> GetSchoolsAsync(SchoolRequestParameters body)
        {
            List<SimpleSchool> schools = new List<SimpleSchool>();

            try
            {
                string selectStatement = this.helper.prepareSchoolsSelectStatements(body);

                using var connection = new NpgsqlConnection(this.connectionString);  

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(selectStatement, connection);

                cmd.Prepare();
                
                using var reader = await cmd.ExecuteReaderAsync();
                    
                while (reader.Read()) 
                {
                    var school = ObjectMapper.MapSimpleSchoolObject(reader);
                    if (school != null)
                    {
                        schools.Add(ObjectMapper.MapSimpleSchoolObject(reader));
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