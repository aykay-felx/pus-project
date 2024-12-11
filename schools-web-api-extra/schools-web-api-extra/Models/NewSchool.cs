using Newtonsoft.Json;
using System.ComponentModel;

namespace schools_web_api_extra.Model
{
    public class NewSchool
    {
        public string RspoNumer { get; set; }
        public SubField? SubFieldRspoNumer { get; set; }

        public double Longitude { get; set; }
        public SubField? SubFieldLongitude { get; set; }

        public double Latitude { get; set; }
        public SubField? SubFieldLatitude { get; set; }

        public string Typ { get; set; }
        public SubField? SubFieldTyp { get; set; }

        public string Nazwa { get; set; }
        public SubField? SubFieldNazwa { get; set; }

        public string Miejscowosc { get; set; }
        public SubField? SubFieldMiejscowosc { get; set; }

        public string Wojewodztwo { get; set; }
        public SubField? SubFieldWojewodztwo { get; set; }

        public string KodPocztowy { get; set; }
        public SubField? SubFieldKodPocztowy { get; set; }

        public string NumerBudynku { get; set; }
        public SubField? SubFieldNumerBudynku { get; set; }

        public string? Email { get; set; }
        public SubField? SubFieldEmail { get; set; }

        public string? Ulica { get; set; }
        public SubField? SubFieldUlica { get; set; }

        public string? Poczta { get; set; }
        public SubField? SubFieldPoczta { get; set; }

        public string? Telefon { get; set; }
        public SubField? SubFieldTelefon { get; set; }

        public string? NumerLokalu { get; set; }
        public SubField? SubFieldNumerLokalu { get; set; }

        public string? StatusPublicznosc { get; set; }
        public SubField? SubFieldStatusPublicznosc { get; set; }

        public string? StronaInternetowa { get; set; }
        public SubField? SubFieldStronaInternetowa { get; set; }

        public string? Faks { get; set; }
        public SubField? SubFieldFaks { get; set; }

        public string? Gmina { get; set; }
        public SubField? SubFieldGmina { get; set; }

        public string? Powiat { get; set; }
        public SubField? SubFieldPowiat { get; set; }

        public string? Dyrektor { get; set; }
        public SubField? SubFieldDyrektor { get; set; }

        public string? NipPodmiotu { get; set; }
        public SubField? SubFieldNipPodmiotu { get; set; }

        public string? DataZalozenia { get; set; }
        public SubField? SubFieldDataZalozenia { get; set; }

        public int? LiczbaUczniow { get; set; }
        public SubField? SubFieldLiczbaUczniow { get; set; }

        public string? RegonPodmiotu { get; set; }
        public SubField? SubFieldRegonPodmiotu { get; set; }

        public string? DataLikwidacji { get; set; }
        public SubField? SubFieldDataLikwidacji { get; set; }

        public string[]? JezykiNauczane { get; set; }
        public SubField? SubFieldJezykiNauczane { get; set; }

        public string[]? TerenySportowe { get; set; }
        public SubField? SubFieldTerenySportowe { get; set; }

        public string? KategoriaUczniow { get; set; }
        public SubField? SubFieldKategoriaUczniow { get; set; }

        public string? StrukturaMiejsce { get; set; }
        public SubField? SubFieldStrukturaMiejsce { get; set; }

        public string? SpecyfikaPlacowki { get; set; }
        public SubField? SubFieldSpecyfikaPlacowki { get; set; }

        public string? RodzajMiejscowosci { get; set; }
        public SubField? SubFieldRodzajMiejscowosci { get; set; }

        public string? OrganProwadzacyNip { get; set; }
        public SubField? SubFieldOrganProwadzacyNip { get; set; }

        public string? OrganProwadzacyTyp { get; set; }
        public SubField? SubFieldOrganProwadzacyTyp { get; set; }

        public string? PodmiotNadrzednyTyp { get; set; }
        public SubField? SubFieldPodmiotNadrzednyTyp { get; set; }

        public string? KodTerytorialnyGmina { get; set; }
        public SubField? SubFieldKodTerytorialnyGmina { get; set; }

        public string? OrganProwadzacyGmina { get; set; }
        public SubField? SubFieldOrganProwadzacyGmina { get; set; }

        public string? OrganProwadzacyNazwa { get; set; }
        public SubField? SubFieldOrganProwadzacyNazwa { get; set; }

        public string? OrganProwadzacyRegon { get; set; }
        public SubField? SubFieldOrganProwadzacyRegon { get; set; }

        public string? PodmiotNadrzednyRspo { get; set; }
        public SubField? SubFieldPodmiotNadrzednyRspo { get; set; }

        public string? KodTerytorialnyPowiat { get; set; }
        public SubField? SubFieldKodTerytorialnyPowiat { get; set; }

        public string? OrganProwadzacyPowiat { get; set; }
        public SubField? SubFieldOrganProwadzacyPowiat { get; set; }

        public string? PodmiotNadrzednyNazwa { get; set; }
        public SubField? SubFieldPodmiotNadrzednyNazwa { get; set; }

        public string? KodTerytorialnyMiejscowosc { get; set; }
        public SubField? SubFieldKodTerytorialnyMiejscowosc { get; set; }

        public string? KodTerytorialnyWojewodztwo { get; set; }
        public SubField? SubFieldKodTerytorialnyWojewodztwo { get; set; }

        public string? OrganProwadzacyWojewodztwo { get; set; }
        public SubField? SubFieldOrganProwadzacyWojewodztwo { get; set; }

        public string? DataRozpoczeciaDzialalnosci { get; set; }
        public SubField? SubFieldDataRozpoczeciaDzialalnosci { get; set; }
    }

    public class SubField
    {
        public SubField(bool IsDifferent, string OldValue) { 
            this.IsDifferent = IsDifferent;
            this.OldValue = OldValue;
        }
        public bool IsDifferent { get; set; } = false;

        public string? OldValue { get; set; }
    }
}
