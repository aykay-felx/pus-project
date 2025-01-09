namespace schools_web_api_extra.Models;

public class NewSchool
{
    public NewSchool() { }

    /// <summary>
    /// Constructor to initialize the NewSchool object using a Placowka instance.
    /// </summary>
    public NewSchool(Placowka placowka)
    {
        RspoNumer = placowka.NumerRspo.ToString();
        Longitude = placowka.Geolokalizacja.Longitude;
        Latitude = placowka.Geolokalizacja.Latitude;
        Typ = placowka.Typ.Nazwa;
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
    }

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

    public string? Telefon { get; set; }
    public SubField? SubFieldTelefon { get; set; }

    public string? StatusPublicznosc { get; set; }
    public SubField? SubFieldStatusPublicznosc { get; set; }

    public string? StronaInternetowa { get; set; }
    public SubField? SubFieldStronaInternetowa { get; set; }

    public string? Dyrektor { get; set; }
    public SubField? SubFieldDyrektor { get; set; }

    public string? NipPodmiotu { get; set; }
    public SubField? SubFieldNipPodmiotu { get; set; }

    public string? RegonPodmiotu { get; set; }
    public SubField? SubFieldRegonPodmiotu { get; set; }

    public string? DataZalozenia { get; set; }
    public SubField? SubFieldDataZalozenia { get; set; }

    public int? LiczbaUczniow { get; set; }
    public SubField? SubFieldLiczbaUczniow { get; set; }

    public string? KategoriaUczniow { get; set; }
    public SubField? SubFieldKategoriaUczniow { get; set; }

    public string? SpecyfikaPlacowki { get; set; }
    public SubField? SubFieldSpecyfikaPlacowki { get; set; }

    public string? Gmina { get; set; }
    public SubField? SubFieldGmina { get; set; }

    public string? Powiat { get; set; }
    public SubField? SubFieldPowiat { get; set; }
    public string[]? JezykiNauczane { get; set; }
    public SubField? SubFieldJezykiNauczane { get; set; }

    /// <summary>
    /// Fields for comparison:
    /// - isDifferentObj: Indicates whether this school differs in some fields compared to OldSchools.
    /// - isNewObj: Indicates whether this is a new school (not present in OldSchools).
    /// </summary>
    public bool? isDifferentObj { get; set; }
    public bool? isNewObj { get; set; }
}

/// <summary>
/// Class for managing comparisons of properties between NewSchool and OldSchool.
/// Also contains additional metadata (e.g., ShouldApply, IsManual, etc.).
/// </summary>
public class SubField
{
    public SubField() { }
    public SubField(bool isDifferent, string? oldValue)
    {
        IsDifferent = isDifferent;
        OldValue = oldValue;
    }

    /// <summary>
    /// Indicates whether the field differs from OldSchool (used in the Compare method).
    /// </summary>
    public bool IsDifferent { get; set; }

    /// <summary>
    /// The previous value of the field (from OldSchool).
    /// </summary>
    public string? OldValue { get; set; }

    // Fields for determining how the data should be applied:

    /// <summary>
    /// Indicates if the field should be applied based on user action on the front end.
    /// </summary>
    public bool ShouldApply { get; set; }

    /// <summary>
    /// Indicates if the field value was manually corrected by the user.
    /// </summary>
    public bool IsManual { get; set; }
}
