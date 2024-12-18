using Newtonsoft.Json;
using Npgsql;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

public class SchoolReposiries : ISchoolService
{
    private readonly string _connectionString;

    public SchoolReposiries(IConfiguration configuration)
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
            command.Parameters.AddWithValue("PodmiotNadrzednyTyp", (object?)school.PodmiotNadrzednyTyp ?? DBNull.Value);
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

        var command = new NpgsqlCommand("SELECT * FROM OldSchools", connection);
        await using var reader = await command.ExecuteReaderAsync();

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
                SubFieldOrganProwadzacyTyp, PodmiotNadrzednyTyp, SubFieldPodmiotNadrzednyTyp, KodTerytorialnyGmina,
                SubFieldKodTerytorialnyGmina, OrganProwadzacyGmina, SubFieldOrganProwadzacyGmina, OrganProwadzacyNazwa,
                SubFieldOrganProwadzacyNazwa, OrganProwadzacyRegon, SubFieldOrganProwadzacyRegon, PodmiotNadrzednyRspo,
                SubFieldPodmiotNadrzednyRspo, KodTerytorialnyPowiat, SubFieldKodTerytorialnyPowiat, OrganProwadzacyPowiat,
                SubFieldOrganProwadzacyPowiat, PodmiotNadrzednyNazwa, SubFieldPodmiotNadrzednyNazwa, KodTerytorialnyMiejscowosc,
                KodTerytorialnyWojewodztwo, SubFieldKodTerytorialnyWojewodztwo, OrganProwadzacyWojewodztwo,
                SubFieldOrganProwadzacyWojewodztwo, DataRozpoczeciaDzialalnosci, SubFieldDataRozpoczeciaDzialalnosci,
                isDiferentObj, isNewObj
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
                @OrganProwadzacyTyp, @SubFieldOrganProwadzacyTyp, @PodmiotNadrzednyTyp, @SubFieldPodmiotNadrzednyTyp,
                @KodTerytorialnyGmina, @SubFieldKodTerytorialnyGmina, @OrganProwadzacyGmina, @SubFieldOrganProwadzacyGmina,
                @OrganProwadzacyNazwa, @SubFieldOrganProwadzacyNazwa, @OrganProwadzacyRegon, @SubFieldOrganProwadzacyRegon,
                @PodmiotNadrzednyRspo, @SubFieldPodmiotNadrzednyRspo, @KodTerytorialnyPowiat, @SubFieldKodTerytorialnyPowiat,
                @OrganProwadzacyPowiat, @SubFieldOrganProwadzacyPowiat, @PodmiotNadrzednyNazwa, @SubFieldPodmiotNadrzednyNazwa,
                @KodTerytorialnyMiejscowosc, @KodTerytorialnyWojewodztwo, @SubFieldKodTerytorialnyWojewodztwo,
                @OrganProwadzacyWojewodztwo, @SubFieldOrganProwadzacyWojewodztwo, @DataRozpoczeciaDzialalnosci,
                @SubFieldDataRozpoczeciaDzialalnosci, @isDiferentObj, @isNewObj
            );
        ";

            using var command = new NpgsqlCommand(insertQuery, connection, transaction);

            foreach (var school in newSchools)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("RspoNumer", school.RspoNumer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldRspoNumer", school.SubFieldRspoNumer != null ? JsonConvert.SerializeObject(school.SubFieldRspoNumer) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Longitude", (object)school.Longitude ?? DBNull.Value);
                command.Parameters.AddWithValue("SubFieldLongitude", school.SubFieldLongitude != null ? JsonConvert.SerializeObject(school.SubFieldLongitude) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Latitude", (object)school.Latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("SubFieldLatitude", school.SubFieldLatitude != null ? JsonConvert.SerializeObject(school.SubFieldLatitude) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Typ", school.Typ ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldTyp", school.SubFieldTyp != null ? JsonConvert.SerializeObject(school.SubFieldTyp) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Nazwa", school.Nazwa ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldNazwa", school.SubFieldNazwa != null ? JsonConvert.SerializeObject(school.SubFieldNazwa) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Miejscowosc", school.Miejscowosc ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldMiejscowosc", school.SubFieldMiejscowosc != null ? JsonConvert.SerializeObject(school.SubFieldMiejscowosc) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Wojewodztwo", school.Wojewodztwo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldWojewodztwo", school.SubFieldWojewodztwo != null ? JsonConvert.SerializeObject(school.SubFieldWojewodztwo) : (object)DBNull.Value);

                command.Parameters.AddWithValue("KodPocztowy", school.KodPocztowy ?? (object)DBNull.Value);
                command.Parameters.Add(new NpgsqlParameter("SubFieldKodPocztowy", NpgsqlTypes.NpgsqlDbType.Jsonb)
                {
                    Value = school.SubFieldKodPocztowy != null ? JsonConvert.SerializeObject(school.SubFieldKodPocztowy) : (object)DBNull.Value
                });

                command.Parameters.AddWithValue("NumerBudynku", school.NumerBudynku ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldNumerBudynku", school.SubFieldNumerBudynku != null ? JsonConvert.SerializeObject(school.SubFieldNumerBudynku) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Email", school.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldEmail", school.SubFieldEmail != null ? JsonConvert.SerializeObject(school.SubFieldEmail) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Ulica", school.Ulica ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldUlica", school.SubFieldUlica != null ? JsonConvert.SerializeObject(school.SubFieldUlica) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Poczta", school.Poczta ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldPoczta", school.SubFieldPoczta != null ? JsonConvert.SerializeObject(school.SubFieldPoczta) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Telefon", school.Telefon ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldTelefon", school.SubFieldTelefon != null ? JsonConvert.SerializeObject(school.SubFieldTelefon) : (object)DBNull.Value);

                command.Parameters.AddWithValue("NumerLokalu", school.NumerLokalu ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldNumerLokalu", school.SubFieldNumerLokalu != null ? JsonConvert.SerializeObject(school.SubFieldNumerLokalu) : (object)DBNull.Value);

                command.Parameters.AddWithValue("StatusPublicznosc", school.StatusPublicznosc ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldStatusPublicznosc", school.SubFieldStatusPublicznosc != null ? JsonConvert.SerializeObject(school.SubFieldStatusPublicznosc) : (object)DBNull.Value);

                command.Parameters.AddWithValue("StronaInternetowa", school.StronaInternetowa ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldStronaInternetowa", school.SubFieldStronaInternetowa != null ? JsonConvert.SerializeObject(school.SubFieldStronaInternetowa) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Faks", school.Faks ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldFaks", school.SubFieldFaks != null ? JsonConvert.SerializeObject(school.SubFieldFaks) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Gmina", school.Gmina ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldGmina", school.SubFieldGmina != null ? JsonConvert.SerializeObject(school.SubFieldGmina) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Powiat", school.Powiat ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldPowiat", school.SubFieldPowiat != null ? JsonConvert.SerializeObject(school.SubFieldPowiat) : (object)DBNull.Value);

                command.Parameters.AddWithValue("Dyrektor", school.Dyrektor ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldDyrektor", school.SubFieldDyrektor != null ? JsonConvert.SerializeObject(school.SubFieldDyrektor) : (object)DBNull.Value);

                command.Parameters.AddWithValue("NipPodmiotu", school.NipPodmiotu ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldNipPodmiotu", school.SubFieldNipPodmiotu != null ? JsonConvert.SerializeObject(school.SubFieldNipPodmiotu) : (object)DBNull.Value);

                command.Parameters.AddWithValue("DataZalozenia", school.DataZalozenia ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldDataZalozenia", school.SubFieldDataZalozenia != null ? JsonConvert.SerializeObject(school.SubFieldDataZalozenia) : (object)DBNull.Value);

                command.Parameters.AddWithValue("LiczbaUczniow", school.LiczbaUczniow ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldLiczbaUczniow", school.SubFieldLiczbaUczniow != null ? JsonConvert.SerializeObject(school.SubFieldLiczbaUczniow) : (object)DBNull.Value);

                command.Parameters.AddWithValue("RegonPodmiotu", school.RegonPodmiotu ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldRegonPodmiotu", school.SubFieldRegonPodmiotu != null ? JsonConvert.SerializeObject(school.SubFieldRegonPodmiotu) : (object)DBNull.Value);

                command.Parameters.AddWithValue("DataLikwidacji", school.DataLikwidacji ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldDataLikwidacji", school.SubFieldDataLikwidacji != null ? JsonConvert.SerializeObject(school.SubFieldDataLikwidacji) : (object)DBNull.Value);
                command.Parameters.AddWithValue("JezykiNauczane", school.JezykiNauczane != null ? JsonConvert.SerializeObject(school.JezykiNauczane) : (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldJezykiNauczane", school.SubFieldJezykiNauczane != null ? JsonConvert.SerializeObject(school.SubFieldJezykiNauczane) : (object)DBNull.Value);

                command.Parameters.AddWithValue("TerenySportowe", school.TerenySportowe != null ? JsonConvert.SerializeObject(school.TerenySportowe) : (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldTerenySportowe", school.SubFieldTerenySportowe != null ? JsonConvert.SerializeObject(school.SubFieldTerenySportowe) : (object)DBNull.Value);

                command.Parameters.AddWithValue("KategoriaUczniow", school.KategoriaUczniow ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldKategoriaUczniow", school.SubFieldKategoriaUczniow != null ? JsonConvert.SerializeObject(school.SubFieldKategoriaUczniow) : (object)DBNull.Value);

                command.Parameters.AddWithValue("StrukturaMiejsce", school.StrukturaMiejsce ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldStrukturaMiejsce", school.SubFieldStrukturaMiejsce != null ? JsonConvert.SerializeObject(school.SubFieldStrukturaMiejsce) : (object)DBNull.Value);

                command.Parameters.AddWithValue("SpecyfikaPlacowki", school.SpecyfikaPlacowki ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldSpecyfikaPlacowki", school.SubFieldSpecyfikaPlacowki != null ? JsonConvert.SerializeObject(school.SubFieldSpecyfikaPlacowki) : (object)DBNull.Value);

                command.Parameters.AddWithValue("RodzajMiejscowosci", school.RodzajMiejscowosci ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldRodzajMiejscowosci", school.SubFieldRodzajMiejscowosci != null ? JsonConvert.SerializeObject(school.SubFieldRodzajMiejscowosci) : (object)DBNull.Value);

                command.Parameters.AddWithValue("OrganProwadzacyNip", school.OrganProwadzacyNip ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldOrganProwadzacyNip", school.SubFieldOrganProwadzacyNip != null ? JsonConvert.SerializeObject(school.SubFieldOrganProwadzacyNip) : (object)DBNull.Value);

                command.Parameters.AddWithValue("OrganProwadzacyTyp", school.OrganProwadzacyTyp ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("SubFieldOrganProwadzacyTyp", school.SubFieldOrganProwadzacyTyp != null ? JsonConvert.SerializeObject(school.SubFieldOrganProwadzacyTyp) : (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            return newSchools; 
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
