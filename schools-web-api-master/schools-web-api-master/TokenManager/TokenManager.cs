using System;
using System.Text;
using JWT.Builder;
using System.Timers;
using JWT.Algorithms;
using schools_web_api.Model;
using System.Security.Claims;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using schools_web_api.TokenManager.Extensions;

namespace schools_web_api.TokenManager {
    public class TokenManager : ITokenManager
    {
        private readonly string secret;
        private readonly string subject;
        private readonly string issuer;
        private readonly string audience;
        private readonly Timer cleanUpTimer;
        private List<RefreshToken> refreshTokens;

        public TokenManager(IConfiguration configuration)
        {
            this.secret = configuration["Jwt:Key"];
            this.subject = configuration["Jwt:Subject"];
            this.issuer = configuration["Jwt:Issuer"];
            this.audience = configuration["Jwt:Audience"];
            this.refreshTokens = new List<RefreshToken>();

            this.cleanUpTimer = new Timer(600000);  //10 minutes
            this.cleanUpTimer.Elapsed += this.HandleCleanUpTimerElapsed;
            this.cleanUpTimer.Start();
        }

        public void HandleCleanUpTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.RemoveExpiredRefreshTokens();
        }

        private void RemoveExpiredRefreshTokens()
        {
            var disallowedTokens = this.refreshTokens.FindAll(t => {
                if (IsTokenExpired(t.ExpirationDate) || t.Revoked)
                {
                    return true;
                }

                return false;
            });

            disallowedTokens.ForEach(token => {
                refreshTokens.Remove(token);
            });
        }

        public bool VerifyHasHighestPrivilleges(string token)
        {
            var tokenData = DecodeAuthToken(token);

            bool isSuperAdmin = bool.Parse(tokenData["isSuperAdmin"].ToString());

            return isSuperAdmin;
        }

        public bool VerifyCanDeleteUser(int userId, string token)
        {
            var tokenData = DecodeAuthToken(token);

            bool isSuperAdmin = bool.Parse(tokenData["isSuperAdmin"].ToString());
            bool isNotDeletingHimself = int.Parse(tokenData["id"].ToString()) != userId;

            return isSuperAdmin && isNotDeletingHimself;
        }

        public bool VerifyCanChangePassword(string token, int userId)
        {
            var tokenData = DecodeAuthToken(token);
            
            string idToken = tokenData["id"].ToString();

            bool modifiesOwnAccount = int.Parse(idToken) == userId;

            return modifiesOwnAccount;
        }

        public Authentification GenerateAccessTokens(int idUser, string userRole, string ipAddress)
        {
            bool isSuperAdmin = userRole == "super-admin" ? true : false;

            Claim[] claims = CreateClaims(idUser, ipAddress, isSuperAdmin);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signIn);

            var authToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken(GenerateRefreshToken(), authToken, DateTime.UtcNow.ToLocalTime().AddMinutes(45));

            refreshTokens.Add(refreshToken);

            return new Authentification(authToken, refreshToken.Token, idUser, isSuperAdmin);
        }

        public bool RevokeRefreshToken(string refreshToken)
        {
            var token = refreshTokens.Find(x => x.Token == refreshToken);

            if (token == null) { return false; }

            token.Revoked = true;

            return true;
        }

        private IDictionary<string, object> DecodeAuthToken(string token)
        {
            try 
            {
                return new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(Encoding.UTF8.GetBytes(secret))
                    .Decode<IDictionary<string, object>>(token);
            }
            catch { return null; }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[60];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber).Escape();
        }

        private Claim[] CreateClaims(int idUser, string ipAddress, bool isSuperAdmin)
        {
            return new[] {
                new Claim(JwtRegisteredClaimNames.Sub, this.subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", idUser.ToString()),
                new Claim("isSuperAdmin", isSuperAdmin.ToString()),
                new Claim("ip", ipAddress)
            };
        }

        public Authentification RefreshToken(string refreshToken, string ip)
        {
            var refreshTokenObject = refreshTokens.Find(x => x.Token == refreshToken);

            if (refreshTokenObject == null) { return null; }

            var currentDate = DateTime.UtcNow.ToLocalTime();

            if (IsTokenExpired(refreshTokenObject.ExpirationDate)) { return null; }

            var authTokenData = DecodeAuthToken(refreshTokenObject.RelatedAuthToken);

            if (authTokenData == null) { return null; }

            DateTime authTokenExp = DateTimeOffset.FromUnixTimeSeconds((long)authTokenData["exp"]).LocalDateTime;

            if (!IsTokenExpired(authTokenExp)) { return null; }

            string userRole = authTokenData["isSuperAdmin"].ToString();

            int id = int.Parse(authTokenData["id"].ToString());

            refreshTokenObject.Revoked = true;

            return GenerateAccessTokens(id, userRole, ip);
        }

        private bool IsTokenExpired(DateTime tokenExpirationDate)
        {
            return DateTime.Compare(DateTime.UtcNow.ToLocalTime(), tokenExpirationDate) > -1;
        }
    }
}