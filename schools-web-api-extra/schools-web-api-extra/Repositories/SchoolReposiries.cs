using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

public class SchoolRepository : ISchoolService
{
    private readonly string? _connectionString;

    public SchoolRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }


    public async Task SaveOldSchoolsAsync(List<NewSchool> oldSchools)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        foreach (var school in oldSchools)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"
                INSERT INTO OldSchools (
                    RspoNumer, Longitude, Latitude, Typ, Nazwa, Miejscowosc, Wojewodztwo, KodPocztowy, 
                    NumerBudynku, Email, Ulica, Poczta, Telefon, NumerLokalu, StatusPublicznosc, 
                    StronaInternetowa, Faks, Gmina, Powiat, Dyrektor, NipPodmiotu, DataZalozenia, 
                    LiczbaUczniow, RegonPodmiotu, DataLikwidacji, JezykiNauczane, TerenySportowe,
                    KategoriaUczniow, StrukturaMiejsce, SpecyfikaPlacowki, RodzajMiejscowosci,
                    OrganProwadzacyNip, OrganProwadzacyTyp, PodmiotNadrzednyTyp, KodTerytorialnyGmina, 
                    OrganProwadzacyGmina, OrganProwadzacyNazwa, OrganProwadzacyRegon, PodmiotNadrzednyRspo, 
                    KodTerytorialnyPowiat, OrganProwadzacyPowiat, PodmiotNadrzednyNazwa, 
                    KodTerytorialnyMiejscowosc, KodTerytorialnyWojewodztwo, OrganProwadzacyWojewodztwo
                ) VALUES (
                    @RspoNumer, @Longitude, @Latitude, @Typ, @Nazwa, @Miejscowosc, @Wojewodztwo, @KodPocztowy, 
                    @NumerBudynku, @Email, @Ulica, @Poczta, @Telefon, @NumerLokalu, @StatusPublicznosc, 
                    @StronaInternetowa, @Faks, @Gmina, @Powiat, @Dyrektor, @NipPodmiotu, @DataZalozenia, 
                    @LiczbaUczniow, @RegonPodmiotu, @DataLikwidacji, @JezykiNauczane, @TerenySportowe,
                    @KategoriaUczniow, @StrukturaMiejsce, @SpecyfikaPlacowki, @RodzajMiejscowosci,
                    @OrganProwadzacyNip, @OrganProwadzacyTyp, @PodmiotNadrzednyTyp, @KodTerytorialnyGmina, 
                    @OrganProwadzacyGmina, @OrganProwadzacyNazwa, @OrganProwadzacyRegon, @PodmiotNadrzednyRspo, 
                    @KodTerytorialnyPowiat, @OrganProwadzacyPowiat, @PodmiotNadrzednyNazwa, 
                    @KodTerytorialnyMiejscowosc, @KodTerytorialnyWojewodztwo, @OrganProwadzacyWojewodztwo
                );
            ";

            // TODO: maybe it is possible to automate this?
            command.Parameters.AddWithValue("RspoNumer", (object?)school.RspoNumer ?? DBNull.Value);
            command.Parameters.AddWithValue("Longitude", school.Longitude);
            command.Parameters.AddWithValue("Latitude", school.Latitude);
            command.Parameters.AddWithValue("Typ", (object?)school.Typ ?? DBNull.Value);
            command.Parameters.AddWithValue("Nazwa", (object?)school.Nazwa ?? DBNull.Value);
            command.Parameters.AddWithValue("Miejscowosc", (object?)school.Miejscowosc ?? DBNull.Value);
            command.Parameters.AddWithValue("Wojewodztwo", (object?)school.Wojewodztwo ?? DBNull.Value);
            command.Parameters.AddWithValue("KodPocztowy", (object?)school.KodPocztowy ?? DBNull.Value);
            command.Parameters.AddWithValue("NumerBudynku", (object?)school.NumerBudynku ?? DBNull.Value);
            command.Parameters.AddWithValue("Email", (object?)school.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("Ulica", (object?)school.Ulica ?? DBNull.Value);
            command.Parameters.AddWithValue("Poczta", (object?)school.Poczta ?? DBNull.Value);
            command.Parameters.AddWithValue("Telefon", (object?)school.Telefon ?? DBNull.Value);
            command.Parameters.AddWithValue("NumerLokalu", (object?)school.NumerLokalu ?? DBNull.Value);
            command.Parameters.AddWithValue("StatusPublicznosc", (object?)school.StatusPublicznosc ?? DBNull.Value);
            command.Parameters.AddWithValue("StronaInternetowa", (object?)school.StronaInternetowa ?? DBNull.Value);
            command.Parameters.AddWithValue("Faks", (object?)school.Faks ?? DBNull.Value);
            command.Parameters.AddWithValue("Gmina", (object?)school.Gmina ?? DBNull.Value);
            command.Parameters.AddWithValue("Powiat", (object?)school.Powiat ?? DBNull.Value);
            command.Parameters.AddWithValue("Dyrektor", (object?)school.Dyrektor ?? DBNull.Value);
            command.Parameters.AddWithValue("NipPodmiotu", (object?)school.NipPodmiotu ?? DBNull.Value);
            command.Parameters.AddWithValue("DataZalozenia", (object?)school.DataZalozenia ?? DBNull.Value);
            command.Parameters.AddWithValue("LiczbaUczniow", (object?)school.LiczbaUczniow ?? DBNull.Value);
            command.Parameters.AddWithValue("RegonPodmiotu", (object?)school.RegonPodmiotu ?? DBNull.Value);
            command.Parameters.AddWithValue("DataLikwidacji", (object?)school.DataLikwidacji ?? DBNull.Value);
            command.Parameters.AddWithValue("JezykiNauczane", school.JezykiNauczane?.Length > 0 ? (object)school.JezykiNauczane : DBNull.Value);
            command.Parameters.AddWithValue("TerenySportowe", school.TerenySportowe?.Length > 0 ? (object)school.TerenySportowe : DBNull.Value);
            command.Parameters.AddWithValue("KategoriaUczniow", (object?)school.KategoriaUczniow ?? DBNull.Value);
            command.Parameters.AddWithValue("StrukturaMiejsce", (object?)school.StrukturaMiejsce ?? DBNull.Value);
            command.Parameters.AddWithValue("SpecyfikaPlacowki", (object?)school.SpecyfikaPlacowki ?? DBNull.Value);
            command.Parameters.AddWithValue("RodzajMiejscowosci", (object?)school.RodzajMiejscowosci ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyNip", (object?)school.OrganProwadzacyNip ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyTyp", (object?)school.OrganProwadzacyTyp ?? DBNull.Value);
            command.Parameters.AddWithValue("PodmiotNadrzednyNazwa", (object?)school.PodmiotNadrzednyNazwa ?? DBNull.Value);
            command.Parameters.AddWithValue("KodTerytorialnyGmina", (object?)school.KodTerytorialnyGmina ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyGmina", (object?)school.OrganProwadzacyGmina ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyNazwa", (object?)school.OrganProwadzacyNazwa ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyRegon", (object?)school.OrganProwadzacyRegon ?? DBNull.Value);
            command.Parameters.AddWithValue("PodmiotNadrzednyRspo", (object?)school.PodmiotNadrzednyRspo ?? DBNull.Value);
            command.Parameters.AddWithValue("KodTerytorialnyPowiat", (object?)school.KodTerytorialnyPowiat ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyPowiat", (object?)school.OrganProwadzacyPowiat ?? DBNull.Value);
            command.Parameters.AddWithValue("PodmiotNadrzednyNazwa", (object?)school.PodmiotNadrzednyNazwa ?? DBNull.Value);
            command.Parameters.AddWithValue("KodTerytorialnyMiejscowosc", (object?)school.KodTerytorialnyMiejscowosc ?? DBNull.Value);
            command.Parameters.AddWithValue("KodTerytorialnyWojewodztwo", (object?)school.KodTerytorialnyWojewodztwo ?? DBNull.Value);
            command.Parameters.AddWithValue("OrganProwadzacyWojewodztwo", (object?)school.OrganProwadzacyWojewodztwo ?? DBNull.Value);
            command.Parameters.AddWithValue("PodmiotNadrzednyTyp", (object?)school.PodmiotNadrzednyTyp ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }

    public async Task DeleteAllOldSchoolsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText = @"TRUNCATE TABLE OldSchools RESTART IDENTITY;";

        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    public async Task<IEnumerable<OldSchool>> GetOldSchoolsAsync()
    {
        var oldSchools = new List<OldSchool>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand("SELECT * FROM oldSchools", connection);
        await using var reader = await command.ExecuteReaderAsync();

        // TODO: maybe make a constructor, that will be accepting OldSchool?
        while (await reader.ReadAsync())
        {
            var school = new OldSchool
            {
                RspoNumer = reader["RspoNumer"].ToString(),
                Longitude = Double.Parse(reader["Longitude"].ToString()),
                Latitude = Double.Parse(reader["Latitude"].ToString()),
                Typ = reader["Typ"]?.ToString(),
                Nazwa = reader["Nazwa"]?.ToString(),
                Miejscowosc = reader["Miejscowosc"]?.ToString(),
                Wojewodztwo = reader["Wojewodztwo"]?.ToString(),
                KodPocztowy = reader["KodPocztowy"]?.ToString(),
                NumerBudynku = reader["NumerBudynku"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Ulica = reader["Ulica"]?.ToString(),
                Poczta = reader["Poczta"]?.ToString(),
                Telefon = reader["Telefon"]?.ToString(),
                NumerLokalu = reader["NumerLokalu"]?.ToString(),
                StatusPublicznosc = reader["StatusPublicznosc"]?.ToString(),
                StronaInternetowa = reader["StronaInternetowa"]?.ToString(),
                Faks = reader["Faks"]?.ToString(),
                Gmina = reader["Gmina"]?.ToString(),
                Powiat = reader["Powiat"]?.ToString(),
                Dyrektor = reader["Dyrektor"]?.ToString(),
                NipPodmiotu = reader["NipPodmiotu"]?.ToString(),
                DataZalozenia = reader["DataZalozenia"].ToString(),
                LiczbaUczniow = reader["LiczbaUczniow"] as int?,
                RegonPodmiotu = reader["RegonPodmiotu"]?.ToString(),
                DataLikwidacji = reader["DataLikwidacji"].ToString(),
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(','),
                TerenySportowe = reader["TerenySportowe"]?.ToString()?.Split(','),
                KategoriaUczniow = reader["KategoriaUczniow"]?.ToString(),
                StrukturaMiejsce = reader["StrukturaMiejsce"]?.ToString(),
                SpecyfikaPlacowki = reader["SpecyfikaPlacowki"]?.ToString(),
                RodzajMiejscowosci = reader["RodzajMiejscowosci"]?.ToString(),
                OrganProwadzacyNip = reader["OrganProwadzacyNip"]?.ToString(),
                OrganProwadzacyTyp = reader["OrganProwadzacyTyp"]?.ToString(),
                PodmiotNadrzednyTyp = reader["PodmiotNadrzednyTyp"]?.ToString(),
                KodTerytorialnyGmina = reader["KodTerytorialnyGmina"]?.ToString(),
                OrganProwadzacyGmina = reader["OrganProwadzacyGmina"]?.ToString(),
                OrganProwadzacyNazwa = reader["OrganProwadzacyNazwa"]?.ToString(),
                OrganProwadzacyRegon = reader["OrganProwadzacyRegon"]?.ToString(),
                PodmiotNadrzednyRspo = reader["PodmiotNadrzednyRspo"]?.ToString(),
                KodTerytorialnyPowiat = reader["KodTerytorialnyPowiat"]?.ToString(),
                OrganProwadzacyPowiat = reader["OrganProwadzacyPowiat"]?.ToString(),
                PodmiotNadrzednyNazwa = reader["PodmiotNadrzednyNazwa"]?.ToString(),
                KodTerytorialnyMiejscowosc = reader["KodTerytorialnyMiejscowosc"]?.ToString(),
                KodTerytorialnyWojewodztwo = reader["KodTerytorialnyWojewodztwo"]?.ToString(),
                OrganProwadzacyWojewodztwo = reader["OrganProwadzacyWojewodztwo"]?.ToString()
            };

            oldSchools.Add(school);
        }

        return oldSchools;
    }

    // TODO: refactor this
    public async Task<IEnumerable<NewSchool>> CompareSchoolsAsync(List<NewSchool> newSchools)
    {
        var oldSchools = (await GetOldSchoolsAsync()).ToList();

        foreach (var newSchool in newSchools)
        {
            var oldSchool = oldSchools.FirstOrDefault(o => o.RspoNumer == newSchool.RspoNumer);

            if (oldSchool != null)
            {
                newSchool.isDiferentObj = false;

                if (newSchool.Nazwa != oldSchool.Nazwa)
                {
                    newSchool.SubFieldNazwa = new SubField(true, oldSchool.Nazwa);
                }
                if (newSchool.Miejscowosc != oldSchool.Miejscowosc)
                {
                    newSchool.SubFieldMiejscowosc = new SubField(true, oldSchool.Miejscowosc);
                }
                if (newSchool.Wojewodztwo != oldSchool.Wojewodztwo)
                {
                    newSchool.SubFieldWojewodztwo = new SubField(true, oldSchool.Wojewodztwo);
                }
                if (newSchool.KodPocztowy != oldSchool.KodPocztowy)
                {
                    newSchool.SubFieldKodPocztowy = new SubField(true, oldSchool.KodPocztowy);
                }
                if (newSchool.Email != oldSchool.Email)
                {
                    newSchool.SubFieldEmail = new SubField(true, oldSchool.Email);
                }
                if (newSchool.Telefon != oldSchool.Telefon)
                {
                    newSchool.SubFieldTelefon = new SubField(true, oldSchool.Telefon);
                }
                if (newSchool.StatusPublicznosc != oldSchool.StatusPublicznosc)
                {
                    newSchool.SubFieldStatusPublicznosc = new SubField(true, oldSchool.StatusPublicznosc);
                }
                if (newSchool.Dyrektor != oldSchool.Dyrektor)
                {
                    newSchool.SubFieldDyrektor = new SubField(true, oldSchool.Dyrektor);
                }
                if (newSchool.NipPodmiotu != oldSchool.NipPodmiotu)
                {
                    newSchool.SubFieldNipPodmiotu = new SubField(true, oldSchool.NipPodmiotu);
                }
                if (newSchool.DataZalozenia != oldSchool.DataZalozenia)
                {
                    newSchool.SubFieldDataZalozenia = new SubField(true, oldSchool.DataZalozenia?.ToString());
                }
                if (newSchool.LiczbaUczniow != oldSchool.LiczbaUczniow)
                {
                    newSchool.SubFieldLiczbaUczniow = new SubField(true, oldSchool.LiczbaUczniow.ToString());
                }
                if (newSchool.RegonPodmiotu != oldSchool.RegonPodmiotu)
                {
                    newSchool.SubFieldRegonPodmiotu = new SubField(true, oldSchool.RegonPodmiotu);
                }
                if (newSchool.DataLikwidacji != oldSchool.DataLikwidacji)
                {
                    newSchool.SubFieldDataLikwidacji = new SubField(true, oldSchool.DataLikwidacji?.ToString());
                }
                if (newSchool.JezykiNauczane != oldSchool.JezykiNauczane)
                {
                    newSchool.SubFieldJezykiNauczane = new SubField(true, string.Join(",", oldSchool.JezykiNauczane));
                }
                if (newSchool.TerenySportowe != oldSchool.TerenySportowe)
                {
                    newSchool.SubFieldTerenySportowe = new SubField(true, string.Join(",", oldSchool.TerenySportowe));
                }
                if (newSchool.KategoriaUczniow != oldSchool.KategoriaUczniow)
                {
                    newSchool.SubFieldKategoriaUczniow = new SubField(true, oldSchool.KategoriaUczniow);
                }
                if (newSchool.StrukturaMiejsce != oldSchool.StrukturaMiejsce)
                {
                    newSchool.SubFieldStrukturaMiejsce = new SubField(true, oldSchool.StrukturaMiejsce);
                }
                if (newSchool.SpecyfikaPlacowki != oldSchool.SpecyfikaPlacowki)
                {
                    newSchool.SubFieldSpecyfikaPlacowki = new SubField(true, oldSchool.SpecyfikaPlacowki);
                }
                if (newSchool.RodzajMiejscowosci != oldSchool.RodzajMiejscowosci)
                {
                    newSchool.SubFieldRodzajMiejscowosci = new SubField(true, oldSchool.RodzajMiejscowosci);
                }
            }
            else
            {
                newSchool.isDiferentObj = true;
                newSchool.isNewObj = true;
            }
        }

        return newSchools;
    }







    public async Task<IEnumerable<NewSchool>> SaveNewSchoolsAsync(List<NewSchool> newSchools)
    {
        if (newSchools == null || !newSchools.Any())
            return Enumerable.Empty<NewSchool>();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var insertQuery = @"
        INSERT INTO NewSchools (
            RspoNumer, SubFieldRspoNumer, Longitude, SubFieldLongitude, Latitude, SubFieldLatitude, Typ, SubFieldTyp,
            Nazwa, SubFieldNazwa, Miejscowosc, SubFieldMiejscowosc, Wojewodztwo, SubFieldWojewodztwo, KodPocztowy,
            SubFieldKodPocztowy, NumerBudynku, SubFieldNumerBudynku, Email, SubFieldEmail, Ulica, SubFieldUlica,
            Poczta, SubFieldPoczta, Telefon, SubFieldTelefon, NumerLokalu, SubFieldNumerLokalu, StatusPublicznosc,
            SubFieldStatusPublicznosc, StronaInternetowa, SubFieldStronaInternetowa, Faks, SubFieldFaks, Gmina,
            SubFieldGmina, Powiat, SubFieldPowiat, Dyrektor, SubFieldDyrektor, NipPodmiotu, SubFieldNipPodmiotu,
            DataZalozenia, SubFieldDataZalozenia, LiczbaUczniow, SubFieldLiczbaUczniow, RegonPodmiotu,
            SubFieldRegonPodmiotu, DataLikwidacji, SubFieldDataLikwidacji, JezykiNauczane, SubFieldJezykiNauczane,
            TerenySportowe, SubFieldTerenySportowe, KategoriaUczniow, SubFieldKategoriaUczniow, StrukturaMiejsce,
            SubFieldStrukturaMiejsce, SpecyfikaPlacowki, SubFieldSpecyfikaPlacowki, RodzajMiejscowosci,
            SubFieldRodzajMiejscowosci, OrganProwadzacyNip, SubFieldOrganProwadzacyNip, OrganProwadzacyTyp,
            SubFieldOrganProwadzacyTyp, KodTerytorialnyGmina, SubFieldKodTerytorialnyGmina, OrganProwadzacyGmina,
            SubFieldOrganProwadzacyGmina, OrganProwadzacyNazwa, SubFieldOrganProwadzacyNazwa, OrganProwadzacyRegon,
            SubFieldOrganProwadzacyRegon, PodmiotNadrzednyRspo, SubFieldPodmiotNadrzednyRspo, KodTerytorialnyPowiat,
            SubFieldKodTerytorialnyPowiat, OrganProwadzacyPowiat, SubFieldOrganProwadzacyPowiat, PodmiotNadrzednyNazwa,
            SubFieldPodmiotNadrzednyNazwa, KodTerytorialnyMiejscowosc, SubFieldKodTerytorialnyMiejscowosc,
            KodTerytorialnyWojewodztwo, SubFieldKodTerytorialnyWojewodztwo, OrganProwadzacyWojewodztwo,
            SubFieldOrganProwadzacyWojewodztwo, DataRozpoczeciaDzialalnosci, SubFieldDataRozpoczeciaDzialalnosci,
            IsDiferentObj, IsNewObj
        ) VALUES (
            @RspoNumer, @SubFieldRspoNumer, @Longitude, @SubFieldLongitude, @Latitude, @SubFieldLatitude, @Typ,
            @SubFieldTyp, @Nazwa, @SubFieldNazwa, @Miejscowosc, @SubFieldMiejscowosc, @Wojewodztwo, @SubFieldWojewodztwo,
            @KodPocztowy, @SubFieldKodPocztowy, @NumerBudynku, @SubFieldNumerBudynku, @Email, @SubFieldEmail, @Ulica,
            @SubFieldUlica, @Poczta, @SubFieldPoczta, @Telefon, @SubFieldTelefon, @NumerLokalu, @SubFieldNumerLokalu,
            @StatusPublicznosc, @SubFieldStatusPublicznosc, @StronaInternetowa, @SubFieldStronaInternetowa, @Faks,
            @SubFieldFaks, @Gmina, @SubFieldGmina, @Powiat, @SubFieldPowiat, @Dyrektor, @SubFieldDyrektor, @NipPodmiotu,
            @SubFieldNipPodmiotu, @DataZalozenia, @SubFieldDataZalozenia, @LiczbaUczniow, @SubFieldLiczbaUczniow,
            @RegonPodmiotu, @SubFieldRegonPodmiotu, @DataLikwidacji, @SubFieldDataLikwidacji, @JezykiNauczane,
            @SubFieldJezykiNauczane, @TerenySportowe, @SubFieldTerenySportowe, @KategoriaUczniow, @SubFieldKategoriaUczniow,
            @StrukturaMiejsce, @SubFieldStrukturaMiejsce, @SpecyfikaPlacowki, @SubFieldSpecyfikaPlacowki,
            @RodzajMiejscowosci, @SubFieldRodzajMiejscowosci, @OrganProwadzacyNip, @SubFieldOrganProwadzacyNip,
            @OrganProwadzacyTyp, @SubFieldOrganProwadzacyTyp, @KodTerytorialnyGmina, @SubFieldKodTerytorialnyGmina,
            @OrganProwadzacyGmina, @SubFieldOrganProwadzacyGmina, @OrganProwadzacyNazwa, @SubFieldOrganProwadzacyNazwa,
            @OrganProwadzacyRegon, @SubFieldOrganProwadzacyRegon, @PodmiotNadrzednyRspo, @SubFieldPodmiotNadrzednyRspo,
            @KodTerytorialnyPowiat, @SubFieldKodTerytorialnyPowiat, @OrganProwadzacyPowiat, @SubFieldOrganProwadzacyPowiat,
            @PodmiotNadrzednyNazwa, @SubFieldPodmiotNadrzednyNazwa, @KodTerytorialnyMiejscowosc, @SubFieldKodTerytorialnyMiejscowosc,
            @KodTerytorialnyWojewodztwo, @SubFieldKodTerytorialnyWojewodztwo, @OrganProwadzacyWojewodztwo,
            @SubFieldOrganProwadzacyWojewodztwo, @DataRozpoczeciaDzialalnosci, @SubFieldDataRozpoczeciaDzialalnosci,
            @IsDiferentObj, @IsNewObj
        );";

            await using var command = new NpgsqlCommand(insertQuery, connection, transaction);

            foreach (var school in newSchools)
            {
                command.Parameters.Clear();

                AddParameter(command, "RspoNumer", school.RspoNumer);
                AddParameter(command, "SubFieldRspoNumer", SerializeJson(school.SubFieldRspoNumer), NpgsqlDbType.Jsonb);
                AddParameter(command, "Longitude", school.Longitude, NpgsqlDbType.Double);
                AddParameter(command, "SubFieldLongitude", SerializeJson(school.SubFieldLongitude), NpgsqlDbType.Jsonb);
                AddParameter(command, "Latitude", school.Latitude, NpgsqlDbType.Double);
                AddParameter(command, "SubFieldLatitude", SerializeJson(school.SubFieldLatitude), NpgsqlDbType.Jsonb);
                AddParameter(command, "Typ", school.Typ);
                AddParameter(command, "SubFieldTyp", SerializeJson(school.SubFieldTyp), NpgsqlDbType.Jsonb);
                AddParameter(command, "Nazwa", school.Nazwa);
                AddParameter(command, "SubFieldNazwa", SerializeJson(school.SubFieldNazwa), NpgsqlDbType.Jsonb);
                AddParameter(command, "Miejscowosc", school.Miejscowosc);
                AddParameter(command, "SubFieldMiejscowosc", SerializeJson(school.SubFieldMiejscowosc), NpgsqlDbType.Jsonb);
                AddParameter(command, "Wojewodztwo", school.Wojewodztwo);
                AddParameter(command, "SubFieldWojewodztwo", SerializeJson(school.SubFieldWojewodztwo), NpgsqlDbType.Jsonb);
                AddParameter(command, "KodPocztowy", school.KodPocztowy);
                AddParameter(command, "SubFieldKodPocztowy", SerializeJson(school.SubFieldKodPocztowy), NpgsqlDbType.Jsonb);
                AddParameter(command, "NumerBudynku", school.NumerBudynku);
                AddParameter(command, "SubFieldNumerBudynku", SerializeJson(school.SubFieldNumerBudynku), NpgsqlDbType.Jsonb);
                AddParameter(command, "Email", school.Email);
                AddParameter(command, "SubFieldEmail", SerializeJson(school.SubFieldEmail), NpgsqlDbType.Jsonb);
                AddParameter(command, "Ulica", school.Ulica);
                AddParameter(command, "SubFieldUlica", SerializeJson(school.SubFieldUlica), NpgsqlDbType.Jsonb);
                AddParameter(command, "Poczta", school.Poczta);
                AddParameter(command, "SubFieldPoczta", SerializeJson(school.SubFieldPoczta), NpgsqlDbType.Jsonb);
                AddParameter(command, "Telefon", school.Telefon);
                AddParameter(command, "SubFieldTelefon", SerializeJson(school.SubFieldTelefon), NpgsqlDbType.Jsonb);
                AddParameter(command, "NumerLokalu", school.NumerLokalu);
                AddParameter(command, "SubFieldNumerLokalu", SerializeJson(school.SubFieldNumerLokalu), NpgsqlDbType.Jsonb);
                AddParameter(command, "StatusPublicznosc", school.StatusPublicznosc);
                AddParameter(command, "SubFieldStatusPublicznosc", SerializeJson(school.SubFieldStatusPublicznosc), NpgsqlDbType.Jsonb);
                AddParameter(command, "StronaInternetowa", school.StronaInternetowa);
                AddParameter(command, "SubFieldStronaInternetowa", SerializeJson(school.SubFieldStronaInternetowa), NpgsqlDbType.Jsonb);
                AddParameter(command, "Faks", school.Faks);
                AddParameter(command, "SubFieldFaks", SerializeJson(school.SubFieldFaks), NpgsqlDbType.Jsonb);
                AddParameter(command, "Gmina", school.Gmina);
                AddParameter(command, "SubFieldGmina", SerializeJson(school.SubFieldGmina), NpgsqlDbType.Jsonb);
                AddParameter(command, "Powiat", school.Powiat);
                AddParameter(command, "SubFieldPowiat", SerializeJson(school.SubFieldPowiat), NpgsqlDbType.Jsonb);
                AddParameter(command, "Dyrektor", school.Dyrektor);
                AddParameter(command, "SubFieldDyrektor", SerializeJson(school.SubFieldDyrektor), NpgsqlDbType.Jsonb);
                AddParameter(command, "NipPodmiotu", school.NipPodmiotu);
                AddParameter(command, "SubFieldNipPodmiotu", SerializeJson(school.SubFieldNipPodmiotu), NpgsqlDbType.Jsonb);
                // Важно: DataZalozenia теперь строка (VARCHAR) => передаём как Text
                AddParameter(command, "DataZalozenia", school.DataZalozenia);
                AddParameter(command, "SubFieldDataZalozenia", SerializeJson(school.SubFieldDataZalozenia), NpgsqlDbType.Jsonb);
                AddParameter(command, "LiczbaUczniow", school.LiczbaUczniow, NpgsqlDbType.Integer);
                AddParameter(command, "SubFieldLiczbaUczniow", SerializeJson(school.SubFieldLiczbaUczniow), NpgsqlDbType.Jsonb);
                AddParameter(command, "RegonPodmiotu", school.RegonPodmiotu);
                AddParameter(command, "SubFieldRegonPodmiotu", SerializeJson(school.SubFieldRegonPodmiotu), NpgsqlDbType.Jsonb);
                // DataLikwidacji тоже строка
                AddParameter(command, "DataLikwidacji", school.DataLikwidacji);
                AddParameter(command, "SubFieldDataLikwidacji", SerializeJson(school.SubFieldDataLikwidacji), NpgsqlDbType.Jsonb);
                AddParameter(command, "JezykiNauczane", SerializeJson(school.JezykiNauczane), NpgsqlDbType.Jsonb);
                AddParameter(command, "SubFieldJezykiNauczane", SerializeJson(school.SubFieldJezykiNauczane), NpgsqlDbType.Jsonb);
                AddParameter(command, "TerenySportowe", SerializeJson(school.TerenySportowe), NpgsqlDbType.Jsonb);
                AddParameter(command, "SubFieldTerenySportowe", SerializeJson(school.SubFieldTerenySportowe), NpgsqlDbType.Jsonb);
                AddParameter(command, "KategoriaUczniow", school.KategoriaUczniow);
                AddParameter(command, "SubFieldKategoriaUczniow", SerializeJson(school.SubFieldKategoriaUczniow), NpgsqlDbType.Jsonb);
                AddParameter(command, "StrukturaMiejsce", school.StrukturaMiejsce);
                AddParameter(command, "SubFieldStrukturaMiejsce", SerializeJson(school.SubFieldStrukturaMiejsce), NpgsqlDbType.Jsonb);
                AddParameter(command, "SpecyfikaPlacowki", school.SpecyfikaPlacowki);
                AddParameter(command, "SubFieldSpecyfikaPlacowki", SerializeJson(school.SubFieldSpecyfikaPlacowki), NpgsqlDbType.Jsonb);
                AddParameter(command, "RodzajMiejscowosci", school.RodzajMiejscowosci);
                AddParameter(command, "SubFieldRodzajMiejscowosci", SerializeJson(school.SubFieldRodzajMiejscowosci), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyNip", school.OrganProwadzacyNip);
                AddParameter(command, "SubFieldOrganProwadzacyNip", SerializeJson(school.SubFieldOrganProwadzacyNip), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyTyp", school.OrganProwadzacyTyp);
                AddParameter(command, "SubFieldOrganProwadzacyTyp", SerializeJson(school.SubFieldOrganProwadzacyTyp), NpgsqlDbType.Jsonb);
                AddParameter(command, "KodTerytorialnyGmina", school.KodTerytorialnyGmina);
                AddParameter(command, "SubFieldKodTerytorialnyGmina", SerializeJson(school.SubFieldKodTerytorialnyGmina), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyGmina", school.OrganProwadzacyGmina);
                AddParameter(command, "SubFieldOrganProwadzacyGmina", SerializeJson(school.SubFieldOrganProwadzacyGmina), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyNazwa", school.OrganProwadzacyNazwa);
                AddParameter(command, "SubFieldOrganProwadzacyNazwa", SerializeJson(school.SubFieldOrganProwadzacyNazwa), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyRegon", school.OrganProwadzacyRegon);
                AddParameter(command, "SubFieldOrganProwadzacyRegon", SerializeJson(school.SubFieldOrganProwadzacyRegon), NpgsqlDbType.Jsonb);
                AddParameter(command, "PodmiotNadrzednyRspo", school.PodmiotNadrzednyRspo);
                AddParameter(command, "SubFieldPodmiotNadrzednyRspo", SerializeJson(school.SubFieldPodmiotNadrzednyRspo), NpgsqlDbType.Jsonb);
                AddParameter(command, "KodTerytorialnyPowiat", school.KodTerytorialnyPowiat);
                AddParameter(command, "SubFieldKodTerytorialnyPowiat", SerializeJson(school.SubFieldKodTerytorialnyPowiat), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyPowiat", school.OrganProwadzacyPowiat);
                AddParameter(command, "SubFieldOrganProwadzacyPowiat", SerializeJson(school.SubFieldOrganProwadzacyPowiat), NpgsqlDbType.Jsonb);
                AddParameter(command, "PodmiotNadrzednyNazwa", school.PodmiotNadrzednyNazwa);
                AddParameter(command, "SubFieldPodmiotNadrzednyNazwa", SerializeJson(school.SubFieldPodmiotNadrzednyNazwa), NpgsqlDbType.Jsonb);
                AddParameter(command, "KodTerytorialnyMiejscowosc", school.KodTerytorialnyMiejscowosc);
                AddParameter(command, "SubFieldKodTerytorialnyMiejscowosc", SerializeJson(school.SubFieldKodTerytorialnyMiejscowosc), NpgsqlDbType.Jsonb);
                AddParameter(command, "KodTerytorialnyWojewodztwo", school.KodTerytorialnyWojewodztwo);
                AddParameter(command, "SubFieldKodTerytorialnyWojewodztwo", SerializeJson(school.SubFieldKodTerytorialnyWojewodztwo), NpgsqlDbType.Jsonb);
                AddParameter(command, "OrganProwadzacyWojewodztwo", school.OrganProwadzacyWojewodztwo);
                AddParameter(command, "SubFieldOrganProwadzacyWojewodztwo", SerializeJson(school.SubFieldOrganProwadzacyWojewodztwo), NpgsqlDbType.Jsonb);
                // DataRozpoczeciaDzialalnosci тоже строка
                AddParameter(command, "DataRozpoczeciaDzialalnosci", school.DataRozpoczeciaDzialalnosci);
                AddParameter(command, "SubFieldDataRozpoczeciaDzialalnosci", SerializeJson(school.SubFieldDataRozpoczeciaDzialalnosci), NpgsqlDbType.Jsonb);
                AddParameter(command, "IsDiferentObj", school.isDiferentObj, NpgsqlDbType.Boolean);
                AddParameter(command, "IsNewObj", school.isNewObj, NpgsqlDbType.Boolean);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return newSchools;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private void AddParameter(NpgsqlCommand command, string parameterName, object value, NpgsqlDbType? type = null)
    {
        var parameter = command.Parameters.Add(parameterName, type ?? InferDbType(value));
        parameter.Value = value ?? DBNull.Value;
    }

    /// <summary>
    /// Определяет NpgsqlDbType (тип параметра) по типу value, 
    /// если тип явно не задан в AddParameter.
    /// </summary>
    private NpgsqlDbType InferDbType(object value)
    {
        return value switch
        {
            double => NpgsqlDbType.Double,
            int => NpgsqlDbType.Integer,
            string => NpgsqlDbType.Text,
            bool => NpgsqlDbType.Boolean,
            // Если бы у нас были DateTime или DateOnly, можно добавить соответствующий вариант
            _ => NpgsqlDbType.Jsonb,
        };
    }

    /// <summary>
    /// Сериализуем объект в JSON-строку. Если value == null — вернём null.
    /// </summary>
    private string SerializeJson(object value)
        => value != null ? JsonConvert.SerializeObject(value) : null;


}

