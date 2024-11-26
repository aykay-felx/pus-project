using Newtonsoft.Json;

namespace schools_web_api.TokenManager.TransmitModels
{
    public class RefreshTokenRequest
    {
        [JsonProperty(PropertyName = "refreshToken", Required = Required.Always)]
        public string RefreshToken { get; set; }
    }
}