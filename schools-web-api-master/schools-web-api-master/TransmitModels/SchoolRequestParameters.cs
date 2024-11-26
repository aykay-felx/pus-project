using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace schools_web_api.TokenManager.TransmitModels
{
    public class SchoolRequestParameters
    {
        #nullable enable

        [JsonProperty(PropertyName = "audience", Required = Required.AllowNull)]
        public string? Audience { get; set; }

        [JsonProperty(PropertyName = "voivodeship", Required = Required.AllowNull)]
        public string? Voivodeship { get; set; }

        [JsonProperty(PropertyName = "city", Required = Required.AllowNull)]
        public string[]? City { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.AllowNull)]
        public string? Name { get; set; }

        #nullable disable
    }
}