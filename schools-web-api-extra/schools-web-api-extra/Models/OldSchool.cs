using System.Collections.Generic;

namespace schools_web_api_extra.Models
{
    public class OldSchool
    {
        public string RspoNumer { get; set; }           // Уникальный идентификатор
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
        /// Храним флаги «поле отредактировано вручную». 
        /// Ключ словаря = имя поля, значение = true/false (true => редактировано вручную).
        /// </summary>
        public Dictionary<string, bool>? ManualFlags { get; set; }
    }
}




/*public OldSchool(Placowka placowka)
{
    RspoNumer = placowka.NumerRspo.ToString();
    Longitude = placowka.Geolokalizacja?.Longitude ?? 0;
    Latitude = placowka.Geolokalizacja?.Latitude ?? 0;
    Typ = placowka.Typ?.Nazwa;
    Nazwa = placowka.Nazwa;
    Miejscowosc = placowka.Gmina;
    Wojewodztwo = placowka.Powiat;
    Telefon = placowka.Telefon;
    Email = placowka.Email;
    StronaInternetowa = placowka.StronaInternetowa;
    NipPodmiotu = placowka.Nip;
    RegonPodmiotu = placowka.Regon;
    DataZalozenia = placowka.DataZalozenia;
    LiczbaUczniow = placowka.LiczbaUczniow;
    Dyrektor = placowka.DyrektorImie + " " + placowka.DyrektorNazwisko;
    StatusPublicznosc = placowka.StatusPublicznoPrawny?.Nazwa;
    KategoriaUczniow = placowka.KategoriaUczniow?.Nazwa;
    SpecyfikaPlacowki = placowka.SpecyfikaSzkoly?.Nazwa;
    Gmina = placowka.Gmina;
    Ulica = placowka.Ulica;
    KodPocztowy = placowka.KodPocztowykodPocztowy;
    NumerBudynku = placowka.NumerBudynku;
    Powiat = placowka.Powiat;
}*/