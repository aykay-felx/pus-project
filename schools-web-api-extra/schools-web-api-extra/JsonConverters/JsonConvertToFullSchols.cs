using Newtonsoft.Json;
using schools_web_api_extra.Models;

public static class JsonConvertToFullSchols
{
    public static List<NewSchool> JsongConvertToFullSchools(string data)
    {
        var placowki = JsonConvert.DeserializeObject<List<Placowka>>(data);

        return placowki.Select(placowka => new NewSchool(placowka)).ToList();
    }
}

