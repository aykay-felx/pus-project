using Npgsql;
using schools_web_api.Model;
using System.Threading.Tasks;
using schools_web_api.TokenManager.Extensions;
using System.Collections.Generic;
using schools_web_api.TokenManager.Exceptions;
using schools_web_api.TokenManager.ObjectMapping;
using schools_web_api.TokenManager.TransmitModels;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.ServiceHelpers;
using Microsoft.Extensions.Configuration;

namespace schools_web_api.TokenManager.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly string connectionString;
        private readonly UserServiceHelper helper;

        public UserService(IConfiguration configuration) 
        {
            this.connectionString = configuration.GetConnectionString("Postgres");
            this.helper = new UserServiceHelper();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            this.helper.validateRequestUserId(id);
    
            try
            {
                string deleteStatement = $"SELECT delete_user({id})";

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

        public async Task<User> AutenticateUser(AuthenticateRequest ar)
        {
            this.helper.validateAutenticateRequest(ar);

            try 
            {
                string selectStatement = $"SELECT * FROM get_authenticated_user('{ar.Email}', '{ar.Password.Escape()}')";

                using var connection = new NpgsqlConnection(this.connectionString);  

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(selectStatement, connection);
                
                using var reader = await cmd.ExecuteReaderAsync();
                
                await reader.ReadAsync();
                
                var user = ObjectMapper.MapUserObject(reader);

                return user;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            this.helper.validateRequestUserId(id);

            try
            {
                string selectStatement = $"SELECT * FROM get_user({id})";

                using var connection = new NpgsqlConnection(this.connectionString);  

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(selectStatement, connection);
                
                using var reader = await cmd.ExecuteReaderAsync();
                
                await reader.ReadAsync();
                
                return ObjectMapper.MapUserObject(reader);
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }
        
        #nullable enable
        public async Task<List<User>> GetUsersAsync(int[]? exceptIndecies = null)
        #nullable disable
        {
            List<User> users = new();

            try 
            {
                string selectStatement = "SELECT * FROM get_users();";

                using var connection = new NpgsqlConnection(this.connectionString);  

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(selectStatement, connection);
                
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync()) {
                    var user = ObjectMapper.MapUserObject(reader);
                    if (user != null) {
                        users.Add(user);
                    }
                }

                return users;
            }
            catch (NpgsqlException ex) 
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<bool> ChangeUserPasswordAsync(ChangePasswordRequest req)
        {
            this.helper.validateChangePasswordRequest(req);

            try
            {
                string updateStatement = $"SELECT update_password({req.IdUser},'{req.NewPassword}','{req.OldPassword}')";

                using var connection = new NpgsqlConnection(this.connectionString);

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(updateStatement, connection);
                
                using var reader = await cmd.ExecuteReaderAsync();

                await reader.ReadAsync();

                bool result = (int)reader.GetValue(0) == 1 ? true : false;
                
                return result;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<bool> UpdateUserAsync(User oldData, User newData)
        {
            try 
            {
                string updateStatement = this.helper.prepareUsersUpdateStatement(oldData, newData);

                using var connection = new NpgsqlConnection(this.connectionString);

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(updateStatement, connection);
                
                using var reader = await cmd.ExecuteReaderAsync();

                await reader.ReadAsync();

                bool result = (int)reader.GetValue(0) == 1 ? true : false;
                
                return result;
            } 
            catch (NpgsqlException ex) 
            {
                throw ex.SqlState == "23505" ? new DataException("Email is already taken") : new DatabaseException(ex.Message);
            }
        }

        public async Task<bool> AddUserAsync(User user)
        {
            this.helper.validateNewUserData(user);
            
            try
            {
                string insertStatement = string.Format("SELECT add_user('{0}', '{1}', '{2}', '{3}', '{4}')",
                    user.Firstname, user.Lastname, user.Password, user.Email, user.Role);

                using var connection = new NpgsqlConnection(this.connectionString);  

                await connection.OpenAsync();

                using var cmd = new NpgsqlCommand(insertStatement, connection);
                
                var reader = await cmd.ExecuteReaderAsync();

                await reader.ReadAsync();
                
                return (int)reader.GetValue(0) == 1 ? true : false;
            }
            catch (NpgsqlException ex)
            {
                throw ex.SqlState == "23505" ? new DataException("Email is already taken") : new DatabaseException(ex.Message);
            }
        }
    }
}