using Newtonsoft.Json;
using System.Collections.Generic;

namespace schools_web_api_extra.HydraCollection
{
    public class Placowka
    {
        [JsonProperty("numerRspo")]
        public int NumerRspo { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("dataZalozenia")]
        public string DataZalozenia { get; set; }

        [JsonProperty("nip")]
        public string Nip { get; set; }

        [JsonProperty("regon")]
        public string Regon { get; set; }


        [JsonProperty("liczbaUczniow")]
        public int? LiczbaUczniow { get; set; }


        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }


        [JsonProperty("typ")]
        public TypPodmiotu Typ { get; set; }


        [JsonProperty("dyrektorImie")]
        public string DyrektorImie { get; set; }


        [JsonProperty("dyrektorNazwisko")]
        public string DyrektorNazwisko { get; set; }


        [JsonProperty("statusPublicznoPrawny")]
        public StatusPublicznoPrawny StatusPublicznoPrawny { get; set; }


        [JsonProperty("etapyEdukacji")]
        public List<EtapEdukacji> EtapyEdukacji { get; set; }


        [JsonProperty("kategoriaUczniow")]
        public KategoriaUczniow KategoriaUczniow { get; set; }


        [JsonProperty("specyfikaSzkoly")]
        public SpecyfikaSzkoly SpecyfikaSzkoly { get; set; }


        [JsonProperty("gmina")]
        public string Gmina { get; set; }


        [JsonProperty("ulica")]
        public string Ulica { get; set; }


        [JsonProperty("numerBudynku")]
        public string NumerBudynku { get; set; }

        [JsonProperty("powiat")]
        public string Powiat { get; set; }


        [JsonProperty("kodPocztowykodPocztowy")]
        public string KodPocztowykodPocztowy { get; set; }

        [JsonProperty("geolokalizacja")]
        public Geolokalizacja Geolokalizacja { get; set; }

        [JsonProperty("telefon")]
        public string Telefon { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("stronaInternetowa")]
        public string StronaInternetowa { get; set; }
    }

    public class TypPodmiotu
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
    }

    public class StatusPublicznoPrawny
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
    }

    public class SpecyfikaSzkoly
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
    }

    public class KategoriaUczniow
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
    }

    public class EtapEdukacji
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
    }

    public class Geolokalizacja
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}
