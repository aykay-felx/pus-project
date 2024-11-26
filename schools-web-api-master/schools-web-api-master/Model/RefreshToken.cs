using System;

namespace schools_web_api.Model
{
    public class RefreshToken
    {
        public string Token { get; }
        public DateTime ExpirationDate { get; }
        public string RelatedAuthToken { get; }
        public bool Revoked { get; set; }

        public RefreshToken(string token, string relatedAuthToken, DateTime exp) {
            this.Token = token;
            this.ExpirationDate = exp;
            this.RelatedAuthToken = relatedAuthToken;
            this.Revoked = false;
        }
    }
}