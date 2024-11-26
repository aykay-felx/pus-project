using Newtonsoft.Json;

namespace schools_web_api.TokenManager.TransmitModels
{
    public class AuthenticateRequest
    {
        #nullable disable

        [JsonProperty(PropertyName = "email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password", Required = Required.Always)]
        public string Password { get; set; }
    }
}