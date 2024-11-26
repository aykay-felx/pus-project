using schools_web_api.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api.TokenManager.Services.Model
{
    public interface IUserService {
        public Task<User> AutenticateUser(AuthenticateRequest ar);
        public Task<User> GetUserByIdAsync(int id);
        public Task<bool> ChangeUserPasswordAsync(ChangePasswordRequest req);
        public Task<bool> DeleteUserAsync(int id);
        public Task<bool> AddUserAsync(User user);
        public Task<bool> UpdateUserAsync(User oldData, User newData);

        #nullable enable
        public Task<List<User>> GetUsersAsync(int[]? exceptIndecies);
        #nullable disable
    }
}