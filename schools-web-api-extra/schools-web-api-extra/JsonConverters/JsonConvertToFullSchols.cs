using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using schools_web_api_extra.Model;
using schools_web_api_extra.HydraCollection;

public static class JsonConvertToFullSchols
{
    public static List<NewSchool> JsongConvertToFullSchools(string data)
    {
        // Исходный JSON (массив объектов)
        string apiData = data;

        // Десериализация массива данных
        var placowki = JsonConvert.DeserializeObject<List<Placowka>>(apiData);

        // Преобразование в целевой формат
        var fullSchools = new List<NewSchool>();

        foreach (var placowka in placowki)
        {
            var oldSchool = new NewSchool(placowka);
            fullSchools.Add(oldSchool);
        }

        return fullSchools;
    }
}

