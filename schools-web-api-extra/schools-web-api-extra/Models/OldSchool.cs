namespace schools_web_api_extra.Models;

public class OldSchool
{
    public string RspoNumer { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Typ { get; set; }
    public string Nazwa { get; set; }
    public string Miejscowosc { get; set; }
    public string Wojewodztwo { get; set; }
    public string KodPocztowy { get; set; }
    public string NumerBudynku { get; set; }

    public string? Email { get; set; }
    public string? Ulica { get; set; }
    public string? Telefon { get; set; }
    public string? StatusPublicznosc { get; set; }
    public string? StronaInternetowa { get; set; }
    public string? Dyrektor { get; set; }
    public string? NipPodmiotu { get; set; }
    public string? RegonPodmiotu { get; set; }
    public string? DataZalozenia { get; set; }
    public int? LiczbaUczniow { get; set; }
    public string? KategoriaUczniow { get; set; }
    public string? SpecyfikaPlacowki { get; set; }
    public string? Gmina { get; set; }
    public string? Powiat { get; set; }
    public string[]? JezykiNauczane { get; set; }

    /// <summary>
    /// Flags for manual adjustments. 
    /// Key = field name, Value = true/false (true => adjustment applied).
    /// </summary>
    public Dictionary<string, bool>? ManualFlags { get; set; }
}
