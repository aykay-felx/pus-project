using Newtonsoft.Json;
using System.ComponentModel;

namespace schools_web_api.Model
{
    public partial class FullSchool
    {
        [DefaultValue(null)]
        [JsonProperty("id", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int? Id { get; set; }

        [JsonProperty("longtitude", Required = Required.Always)]
        public double Longtitude { get; set; }

        [JsonProperty("latitude", Required = Required.Always)]
        public double Latitude { get; set; }

        [JsonProperty("businessData", Required = Required.Always)]
        public FullSchoolBusinessData BusinessData { get; set; }

        public FullSchool (int? id, double lon, double lat, FullSchoolBusinessData businessData)
        {
            this.Id = id;
            this.Longtitude = lon;
            this.Latitude = lat;
            this.BusinessData = businessData;
        }
    }

    public partial class SimpleSchool
    {
        [DefaultValue(null)]
        [JsonProperty("id", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Id { get; set; }

        [JsonProperty("longtitude", Required = Required.Always)]
        public double Longtitude { get; set; }

        [JsonProperty("latitude", Required = Required.Always)]
        public double Latitude { get; set; }

        [JsonProperty("businessData", Required = Required.Always)]
        public SimpleSchoolBusinessData BusinessData { get; set; }

        public SimpleSchool (int? id, double lon, double lat, SimpleSchoolBusinessData businessData)
        {
            this.Id = id;
            this.Longtitude = lon;
            this.Latitude = lat;
            this.BusinessData = businessData;
        }
    }

    public partial class SimpleSchoolBusinessData
    {
        [JsonProperty("typ", Required = Required.Always)]
        public string Typ { get; set; }

        [JsonProperty("nazwa", Required = Required.Always)]
        public string Nazwa { get; set; }

        [JsonProperty("miejscowosc", Required = Required.Always)]
        public string Miejscowosc { get; set; }

        [JsonProperty("wojewodztwo", Required = Required.Always)]
        public string Wojewodztwo { get; set; }

        [JsonProperty("kodPocztowy", Required = Required.Always)]
        public string KodPocztowy { get; set; }

        [JsonProperty(PropertyName = "numerBudynku", Required = Required.Always)]
        public string NumerBudynku { get; set; }

        #nullable enable

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "email", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Email { get; set; }

        [DefaultValue(null)] 
        [JsonProperty(PropertyName = "ulica", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Ulica { get; set; }

        [DefaultValue(null)]
        [JsonProperty(PropertyName = "poczta", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Poczta { get; set; }

        [DefaultValue(null)]
        [JsonProperty(PropertyName = "telefon", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Telefon { get; set; }

        [DefaultValue(null)] 
        [JsonProperty(PropertyName = "numerLokalu", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? NumerLokalu { get; set; }

        [DefaultValue(null)] 
        [JsonProperty(PropertyName = "statusPublicznosc", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? StatusPublicznosc { get; set; }

        [DefaultValue(null)] 
        [JsonProperty(PropertyName = "stronaInternetowa", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? StronaInternetowa { get; set; }
    }

    public partial class FullSchoolBusinessData : SimpleSchoolBusinessData
    {
        #nullable enable

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "faks", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Faks { get; set; }



        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "gmina", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Gmina { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "powiat", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Powiat { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "dyrektor", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Dyrektor { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "rspoNumer", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? RspoNumer { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "nipPodmiotu", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? NipPodmiotu { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "dataZalozenia", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? DataZalozenia { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "liczbaUczniow", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int? LiczbaUczniow { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "regonPodmiotu", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]

        public string? RegonPodmiotu { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "dataLikwidacji", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? DataLikwidacji { get; set; }

        [DefaultValue(new string[0])]            
        [JsonProperty(PropertyName = "jezykiNauczane", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string[]? JezykiNauczane { get; set; }

        [DefaultValue(new string[0])]            
        [JsonProperty(PropertyName = "terenySportowe", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string[]? TerenySportowe { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "kategoriaUczniow", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? KategoriaUczniow { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "strukturaMiejsce", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? StrukturaMiejsce { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "specyfikaPlacowki", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? SpecyfikaPlacowki { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "rodzajMiejscowosci", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? RodzajMiejscowosci { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyNip", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyNip { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyTyp", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyTyp { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "podmiotNadrzednyTyp", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? PodmiotNadrzednyTyp { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "kodTerytorialnyGmina", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string? KodTerytorialnyGmina { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyGmina", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyGmina { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyNazwa", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyNazwa { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyRegon", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyRegon { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "podmiotNadrzednyRspo", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? PodmiotNadrzednyRspo { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "kodTerytorialnyPowiat", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string? KodTerytorialnyPowiat { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyPowiat", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyPowiat { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "podmiotNadrzednyNazwa", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? PodmiotNadrzednyNazwa { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "kodTerytorialnyMiejscowosc", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? KodTerytorialnyMiejscowosc { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "kodTerytorialnyWojewodztwo", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        //[JsonConverter(typeof(ParseStringConverter))]
        public string? KodTerytorialnyWojewodztwo { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "organProwadzacyWojewodztwo", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? OrganProwadzacyWojewodztwo { get; set; }

        [DefaultValue(null)]            
        [JsonProperty(PropertyName = "dataRozpoczeciaDzialalnosci", Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? DataRozpoczeciaDzialalnosci { get; set; }
    }
}
