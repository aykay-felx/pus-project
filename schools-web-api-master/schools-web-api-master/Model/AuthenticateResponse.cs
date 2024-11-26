using Newtonsoft.Json;

namespace schools_web_api.Model
{
    public class Authentification
    {
        public string AuthentificationToken { get; }
        public string RefreshToken { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? SuperAdmin { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? UserId { get; }

        public Authentification(string authToken, string refreshToken, int? idUser, bool? isSuperAdmin)
        {
            this.AuthentificationToken = authToken;
            this.RefreshToken = refreshToken;
            this.UserId = idUser;
            this.SuperAdmin = isSuperAdmin;
        }
    }
}