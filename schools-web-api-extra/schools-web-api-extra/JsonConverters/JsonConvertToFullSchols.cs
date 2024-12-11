using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using schools_web_api;
using schools_web_api_extra.Model;

public static class JsonConvertToFullSchols
{
    public static List<OldSchool> JsongConvertToFullSchools(string data)
    {
        // Исходный JSON (массив объектов)
        string apiData = data;

        // Десериализация массива данных
        var placowki = JsonConvert.DeserializeObject<List<Placowka>>(apiData);

        // Преобразование в целевой формат
        var fullSchools = new List<OldSchool>();
        foreach (var placowka in placowki)
        {
            var businessData = new OldSchool
            {
                Faks = null, // Не указано в данных
                Gmina = placowka.Gmina,
                Powiat = placowka.Powiat,
                Dyrektor = $"{placowka.DyrektorImie} {placowka.DyrektorNazwisko}",
                RspoNumer = placowka.NumerRspo.ToString(),
                NipPodmiotu = placowka.Nip,
                DataZalozenia = placowka.DataZalozenia,
                LiczbaUczniow = placowka.LiczbaUczniow,
                RegonPodmiotu = placowka.Regon,
                DataLikwidacji = placowka.DataLikwidacji,
                KategoriaUczniow = placowka.KategoriaUczniow?.Nazwa,
                SpecyfikaPlacowki = placowka.SpecyfikaSzkoly?.Nazwa,
                KodTerytorialnyGmina = placowka.GminaKodTERYT,
                OrganProwadzacyGmina = placowka.PodmiotProwadzacy?[0]?.Gmina,
                OrganProwadzacyNazwa = placowka.PodmiotProwadzacy?[0]?.Nazwa,
                OrganProwadzacyRegon = placowka.PodmiotProwadzacy?[0]?.Regon
            };
        }

        return fullSchools;
    }

    public class Placowka
    {
        public int NumerRspo { get; set; }
        public string DataZalozenia { get; set; }
        public string Nip { get; set; }
        public string Regon { get; set; }
        public int? LiczbaUczniow { get; set; }
        public string DyrektorImie { get; set; }
        public string DyrektorNazwisko { get; set; }
        public string Gmina { get; set; }
        public string Powiat { get; set; }

        public string DataLikwidacji { get; set; }
        public Geolokalizacja Geolokalizacja { get; set; }
        public KategoriaUczniow KategoriaUczniow { get; set; }
        public SpecyfikaSzkoly SpecyfikaSzkoly { get; set; }
        public List<PodmiotProwadzacy> PodmiotProwadzacy { get; set; }
        public string GminaKodTERYT { get; set; }
    }

    public class Geolokalizacja
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class KategoriaUczniow
    {
        public string Nazwa { get; set; }
    }

    public class SpecyfikaSzkoly
    {
        public string Nazwa { get; set; }
    }

    public class PodmiotProwadzacy
    {
        public string Nazwa { get; set; }
        public string Regon { get; set; }
        public string Gmina { get; set; }
    }
}

