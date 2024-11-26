using schools_web_api.Model;

namespace schools_web_api.TokenManager
{
    public interface ITokenManager {
        public Authentification GenerateAccessTokens(int idUser, string userRole, string ipAddress);
        public Authentification RefreshToken(string refreshToken, string ip);
        public bool RevokeRefreshToken(string refreshToken);
        public bool VerifyHasHighestPrivilleges(string token);
        public bool VerifyCanDeleteUser(int userId, string token);
        public bool VerifyCanChangePassword(string token, int userId);
    }
}