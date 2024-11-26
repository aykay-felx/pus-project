using Newtonsoft.Json;

namespace schools_web_api.TokenManager.TransmitModels
{
    public class ChangePasswordRequest
    {
        [JsonProperty(PropertyName = "idUser", Required = Required.Always)]
        public int IdUser { get; set; }

        [JsonProperty(PropertyName = "oldPassword", Required = Required.Always)]
        public string OldPassword { get; set; }

        [JsonProperty(PropertyName = "newPassword", Required = Required.Always)]
        public string NewPassword { get; set; }
    }
}