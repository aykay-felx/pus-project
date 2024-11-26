using Newtonsoft.Json;
using System.ComponentModel;

namespace schools_web_api.Model
{
    public interface IUser
    {
        #nullable enable
        int? Id { get; }
        string? Password { get; set; }
        #nullable disable
        string Firstname { get; set; }
        string Lastname { get; set; }
        string Email { get; set; }
        string Role { get; set; }
    }

    public class User : IUser
    {
        #nullable enable

        [DefaultValue(null)]
        [JsonProperty("id", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; }

        [DefaultValue(null)]
        [JsonProperty("password", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }

        #nullable disable

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
        public User(int? id, string fName, string lName, string email, string role)
        {
            this.Id = id;
            this.Firstname = fName;
            this.Lastname = lName;
            this.Email = email;
            this.Role = role;
        }
    }
}