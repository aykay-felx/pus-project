using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Repositories;

public class NewSchoolRepository : INewSchoolService
{
    private readonly string _connectionString;
    private readonly IHttpClientFactory _httpClientFactory;

    public NewSchoolRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
        _httpClientFactory = httpClientFactory;
    }
    
    /// <summary>
    /// Extract a list of schools from an external API (https://api-rspo.men.gov.pl/api/placowki).
    /// Convert them to List<NewSchool> and return.
    /// </summary>
    public async Task<List<NewSchool>> FetchSchoolsFromApiAsync()
    {
        var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page=1";
        var httpClient = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        request.Headers.Add("accept", "application/ld+json");

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error accessing RSPO's API: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(content);

        var totalPages = int.Parse(jsonResponse["hydra:view"]["hydra:last"].ToString().Split('=')[1]);

        var newSchools = new List<NewSchool>();

        for (int i = 1; i <= totalPages; i++)
        {
            apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={i}";
            request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("accept", "application/json");

            response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error accessing RSPO's API: {response.StatusCode}");
            }

            content = await response.Content.ReadAsStringAsync();
            var schools = JsonConvertToFullSchols.JsonConvertToFullSchools(content);
            newSchools.AddRange(schools);
        }

        return newSchools;
    }

    /// <summary>
    /// Compare the list of NewSchool with existing OldSchools
    ///    (populate SubFields, isDifferentObj, isNewObj).
    /// </summary>
    public async Task<List<NewSchool>> CompareWithOldSchoolsAsync(List<NewSchool> newSchools)
    {
        var oldSchools = (await GetAllOldSchoolsAsync()).ToList();
        var specialProperties = new HashSet<string> { nameof(NewSchool.JezykiNauczane) };

        foreach (var newSchool in newSchools)
        {
            var oldSchool = oldSchools.FirstOrDefault(o => o.RspoNumer == newSchool.RspoNumer);

            if (oldSchool is null)
            {
                newSchool.isDifferentObj = true;
                newSchool.isNewObj = true;
                continue;
            }

            foreach (var property in typeof(NewSchool).GetProperties())
            {
                if (property.Name.StartsWith("SubField")) continue;

                var newValue = property.GetValue(newSchool);
                var oldProperty = oldSchool.GetType().GetProperty(property.Name);
                var oldValue = oldProperty?.GetValue(oldSchool);

                if (Equals(newValue, oldValue)) continue;

                var subFieldProperty = typeof(NewSchool).GetProperty($"SubField{property.Name}");

                if (subFieldProperty == null) continue;

                SubField subField;

                if (specialProperties.Contains(property.Name))
                    subField = new SubField(true, string.Join(",", oldValue));
                else
                    subField = new SubField(true, oldValue?.ToString());

                subFieldProperty.SetValue(newSchool, subField);
            }
        }

        return newSchools;
    }

    /// <summary>
    /// Save the list of NewSchool to the NewSchools table.
    /// </summary>
    public async Task SaveNewSchoolsAsync(List<NewSchool> newSchools)
    {
        if (newSchools == null || newSchools.Count == 0) return;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var insertSql = @"
            INSERT INTO NewSchools (
                RspoNumer,
                Longitude,
                Latitude,
                Typ,
                Nazwa,
                Miejscowosc,
                Wojewodztwo,
                KodPocztowy,
                NumerBudynku,
                Email,
                Ulica,
                Telefon,
                StatusPublicznosc,
                StronaInternetowa,
                Dyrektor,
                NipPodmiotu,
                RegonPodmiotu,
                DataZalozenia,
                LiczbaUczniow,
                KategoriaUczniow,
                SpecyfikaPlacowki,
                Gmina,
                Powiat,
                JezykiNauczane,
                SubFieldRspoNumer,
                SubFieldLongitude,
                SubFieldLatitude,
                SubFieldTyp,
                SubFieldNazwa,
                SubFieldMiejscowosc,
                SubFieldWojewodztwo,
                SubFieldKodPocztowy,
                SubFieldNumerBudynku,
                SubFieldEmail,
                SubFieldUlica,
                SubFieldTelefon,
                SubFieldStatusPublicznosc,
                SubFieldStronaInternetowa,
                SubFieldDyrektor,
                SubFieldNipPodmiotu,
                SubFieldRegonPodmiotu,
                SubFieldDataZalozenia,
                SubFieldLiczbaUczniow,
                SubFieldKategoriaUczniow,
                SubFieldSpecyfikaPlacowki,
                SubFieldGmina,
                SubFieldPowiat,
                SubFieldJezykiNauczane,
                isDifferentObj,
                isNewObj
            ) VALUES (
                @RspoNumer,
                @Longitude,
                @Latitude,
                @Typ,
                @Nazwa,
                @Miejscowosc,
                @Wojewodztwo,
                @KodPocztowy,
                @NumerBudynku,
                @Email,
                @Ulica,
                @Telefon,
                @StatusPublicznosc,
                @StronaInternetowa,
                @Dyrektor,
                @NipPodmiotu,
                @RegonPodmiotu,
                @DataZalozenia,
                @LiczbaUczniow,
                @KategoriaUczniow,
                @SpecyfikaPlacowki,
                @Gmina,
                @Powiat,
                @JezykiNauczane,
                @SubFieldRspoNumer,
                @SubFieldLongitude,
                @SubFieldLatitude,
                @SubFieldTyp,
                @SubFieldNazwa,
                @SubFieldMiejscowosc,
                @SubFieldWojewodztwo,
                @SubFieldKodPocztowy,
                @SubFieldNumerBudynku,
                @SubFieldEmail,
                @SubFieldUlica,
                @SubFieldTelefon,
                @SubFieldStatusPublicznosc,
                @SubFieldStronaInternetowa,
                @SubFieldDyrektor,
                @SubFieldNipPodmiotu,
                @SubFieldRegonPodmiotu,
                @SubFieldDataZalozenia,
                @SubFieldLiczbaUczniow,
                @SubFieldKategoriaUczniow,
                @SubFieldSpecyfikaPlacowki,
                @SubFieldGmina,
                @SubFieldPowiat,
                @SubFieldJezykiNauczane,
                @isDifferentObj,
                @isNewObj
            );";

            using var cmd = new NpgsqlCommand(insertSql, connection, transaction);

            foreach (var school in newSchools)
            {
                cmd.Parameters.Clear();

                // Regular fields
                cmd.Parameters.AddWithValue("RspoNumer", school.RspoNumer ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Longitude", school.Longitude);
                cmd.Parameters.AddWithValue("Latitude", school.Latitude);
                cmd.Parameters.AddWithValue("Typ", school.Typ ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Nazwa", school.Nazwa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Miejscowosc", school.Miejscowosc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Wojewodztwo", school.Wojewodztwo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("KodPocztowy", school.KodPocztowy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("NumerBudynku", school.NumerBudynku ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Email", school.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Ulica", school.Ulica ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Telefon", school.Telefon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("StatusPublicznosc", school.StatusPublicznosc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("StronaInternetowa", school.StronaInternetowa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Dyrektor", school.Dyrektor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("NipPodmiotu", school.NipPodmiotu ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("RegonPodmiotu", school.RegonPodmiotu ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("DataZalozenia", school.DataZalozenia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("LiczbaUczniow", school.LiczbaUczniow ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("KategoriaUczniow", school.KategoriaUczniow ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("SpecyfikaPlacowki", school.SpecyfikaPlacowki ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Gmina", school.Gmina ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Powiat", school.Powiat ?? (object)DBNull.Value);

                // JezykiNauczane array → as a string "english,german" (or text[] array)
                if (school.JezykiNauczane != null && school.JezykiNauczane.Any())
                    cmd.Parameters.AddWithValue("JezykiNauczane", string.Join(",", school.JezykiNauczane));
                else
                    cmd.Parameters.AddWithValue("JezykiNauczane", DBNull.Value);

                // For SubField* fields in the DB — JSONB
                // Specify NpgsqlDbType.Jsonb
                cmd.Parameters.AddWithValue(
                    "SubFieldRspoNumer",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldRspoNumer) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldLongitude",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldLongitude) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldLatitude",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldLatitude) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldTyp",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldTyp) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldNazwa",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldNazwa) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldMiejscowosc",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldMiejscowosc) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldWojewodztwo",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldWojewodztwo) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldKodPocztowy",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldKodPocztowy) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldNumerBudynku",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldNumerBudynku) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldEmail",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldEmail) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldUlica",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldUlica) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldTelefon",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldTelefon) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldStatusPublicznosc",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldStatusPublicznosc) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldStronaInternetowa",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldStronaInternetowa) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldDyrektor",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldDyrektor) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldNipPodmiotu",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldNipPodmiotu) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldRegonPodmiotu",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldRegonPodmiotu) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldDataZalozenia",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldDataZalozenia) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldLiczbaUczniow",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldLiczbaUczniow) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldKategoriaUczniow",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldKategoriaUczniow) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldSpecyfikaPlacowki",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldSpecyfikaPlacowki) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldGmina",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldGmina) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldPowiat",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldPowiat) ?? DBNull.Value
                );
                cmd.Parameters.AddWithValue(
                    "SubFieldJezykiNauczane",
                    NpgsqlTypes.NpgsqlDbType.Jsonb,
                    (object?)SerializeJson(school.SubFieldJezykiNauczane) ?? DBNull.Value
                );

                // bool?
                cmd.Parameters.AddWithValue("isDifferentObj", (object?)school.isDifferentObj ?? DBNull.Value);
                cmd.Parameters.AddWithValue("isNewObj", (object?)school.isNewObj ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    /// <summary>
    /// Get All OldSchools
    /// </summary>
    public async Task<IEnumerable<OldSchool>> GetAllOldSchoolsAsync()
    {
        var results = new List<OldSchool>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT * FROM OldSchools";
        using var cmd = new NpgsqlCommand(sql, connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var oldSchool = new OldSchool
            {
                RspoNumer = reader["RspoNumer"]?.ToString() ?? "",
                Longitude = reader["Longitude"] as double? ?? 0,
                Latitude = reader["Latitude"] as double? ?? 0,
                Typ = reader["Typ"]?.ToString() ?? "",
                Nazwa = reader["Nazwa"]?.ToString() ?? "",
                Miejscowosc = reader["Miejscowosc"]?.ToString(),
                Wojewodztwo = reader["Wojewodztwo"]?.ToString(),
                KodPocztowy = reader["KodPocztowy"]?.ToString(),
                NumerBudynku = reader["NumerBudynku"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Ulica = reader["Ulica"]?.ToString(),
                Telefon = reader["Telefon"]?.ToString(),
                StatusPublicznosc = reader["StatusPublicznosc"]?.ToString(),
                StronaInternetowa = reader["StronaInternetowa"]?.ToString(),
                Dyrektor = reader["Dyrektor"]?.ToString(),
                NipPodmiotu = reader["NipPodmiotu"]?.ToString(),
                RegonPodmiotu = reader["RegonPodmiotu"]?.ToString(),
                DataZalozenia = reader["DataZalozenia"]?.ToString(),
                LiczbaUczniow = reader["LiczbaUczniow"] as int?,
                KategoriaUczniow = reader["KategoriaUczniow"]?.ToString(),
                SpecyfikaPlacowki = reader["SpecyfikaPlacowki"]?.ToString(),
                Gmina = reader["Gmina"]?.ToString(),
                Powiat = reader["Powiat"]?.ToString(),
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(','),
                // ManualFlags = reader["ManualFlags"]?.ToString()?.Split(',')
            };
            results.Add(oldSchool);
        }

        return results;
    }

    /// <summary>
    /// Get All NewSchools
    /// </summary>
    public async Task<IEnumerable<NewSchool>> GetAllNewSchoolAsync()
    {
        var results = new List<NewSchool>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT * FROM NewSchools";
        using var cmd = new NpgsqlCommand(sql, connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            // Create a new NewSchool object
            var newSchool = new NewSchool
            {
                // 1) "Main" fields
                RspoNumer = reader["RspoNumer"]?.ToString() ?? "",
                Longitude = reader["Longitude"] as double? ?? 0,
                Latitude = reader["Latitude"] as double? ?? 0,
                Typ = reader["Typ"]?.ToString() ?? "",
                Nazwa = reader["Nazwa"]?.ToString() ?? "",
                Miejscowosc = reader["Miejscowosc"]?.ToString(),
                Wojewodztwo = reader["Wojewodztwo"]?.ToString(),
                KodPocztowy = reader["KodPocztowy"]?.ToString(),
                NumerBudynku = reader["NumerBudynku"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Ulica = reader["Ulica"]?.ToString(),
                Telefon = reader["Telefon"]?.ToString(),
                StatusPublicznosc = reader["StatusPublicznosc"]?.ToString(),
                StronaInternetowa = reader["StronaInternetowa"]?.ToString(),
                Dyrektor = reader["Dyrektor"]?.ToString(),
                NipPodmiotu = reader["NipPodmiotu"]?.ToString(),
                RegonPodmiotu = reader["RegonPodmiotu"]?.ToString(),
                DataZalozenia = reader["DataZalozenia"]?.ToString(),
                LiczbaUczniow = reader["LiczbaUczniow"] as int?,
                KategoriaUczniow = reader["KategoriaUczniow"]?.ToString(),
                SpecyfikaPlacowki = reader["SpecyfikaPlacowki"]?.ToString(),
                Gmina = reader["Gmina"]?.ToString(),
                Powiat = reader["Powiat"]?.ToString(),

                // 2) JezykiNauczane array (if stored as "english,german" in the column)
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries),

                // 3) Flags (bool?) 
                //  - If the isDifferentObj column is of BOOLEAN type, read as bool? 
                //    (if NULL in DB, return null)
                isDifferentObj = reader["isDifferentObj"] == DBNull.Value
                    ? null
                    : (bool?)reader["isDifferentObj"],

                isNewObj = reader["isNewObj"] == DBNull.Value
                    ? null
                    : (bool?)reader["isNewObj"],

                // 4) Subfields (SubField...) deserialize from JSONB
                //    Use helper method DeserializeJson<SubField>(...)
                SubFieldRspoNumer = DeserializeJson<SubField>(reader["SubFieldRspoNumer"]),
                SubFieldLongitude = DeserializeJson<SubField>(reader["SubFieldLongitude"]),
                SubFieldLatitude = DeserializeJson<SubField>(reader["SubFieldLatitude"]),
                SubFieldTyp = DeserializeJson<SubField>(reader["SubFieldTyp"]),
                SubFieldNazwa = DeserializeJson<SubField>(reader["SubFieldNazwa"]),
                SubFieldMiejscowosc = DeserializeJson<SubField>(reader["SubFieldMiejscowosc"]),
                SubFieldWojewodztwo = DeserializeJson<SubField>(reader["SubFieldWojewodztwo"]),
                SubFieldKodPocztowy = DeserializeJson<SubField>(reader["SubFieldKodPocztowy"]),
                SubFieldNumerBudynku = DeserializeJson<SubField>(reader["SubFieldNumerBudynku"]),
                SubFieldEmail = DeserializeJson<SubField>(reader["SubFieldEmail"]),
                SubFieldUlica = DeserializeJson<SubField>(reader["SubFieldUlica"]),
                SubFieldTelefon = DeserializeJson<SubField>(reader["SubFieldTelefon"]),
                SubFieldStatusPublicznosc = DeserializeJson<SubField>(reader["SubFieldStatusPublicznosc"]),
                SubFieldStronaInternetowa = DeserializeJson<SubField>(reader["SubFieldStronaInternetowa"]),
                SubFieldDyrektor = DeserializeJson<SubField>(reader["SubFieldDyrektor"]),
                SubFieldNipPodmiotu = DeserializeJson<SubField>(reader["SubFieldNipPodmiotu"]),
                SubFieldRegonPodmiotu = DeserializeJson<SubField>(reader["SubFieldRegonPodmiotu"]),
                SubFieldDataZalozenia = DeserializeJson<SubField>(reader["SubFieldDataZalozenia"]),
                SubFieldLiczbaUczniow = DeserializeJson<SubField>(reader["SubFieldLiczbaUczniow"]),
                SubFieldKategoriaUczniow = DeserializeJson<SubField>(reader["SubFieldKategoriaUczniow"]),
                SubFieldSpecyfikaPlacowki = DeserializeJson<SubField>(reader["SubFieldSpecyfikaPlacowki"]),
                SubFieldGmina = DeserializeJson<SubField>(reader["SubFieldGmina"]),
                SubFieldPowiat = DeserializeJson<SubField>(reader["SubFieldPowiat"]),
                SubFieldJezykiNauczane = DeserializeJson<SubField>(reader["SubFieldJezykiNauczane"])
            };

            results.Add(newSchool);
        }

        return results;
    }

    /// <summary>
    /// Delete All NewSchool
    /// </summary>
    public async Task DeleteAllNewSchoolAsync()
    {
        const string sql = "DELETE FROM public.newschools;"; // Or use TRUNCATE

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, connection);

        // Since all rows are being deleted, no parameters are needed
        // If you decide to use TRUNCATE, add CASCADE option if necessary:
        // const string sql = "TRUNCATE TABLE public.newschools CASCADE;";

        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Delete a NewSchool by rsponumer
    /// </summary>
    /// <param name="rsponumer"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DeleteNewSchoolAsync(string rsponumer)
    {
        const string deleteSql = "DELETE FROM public.newschools as ns WHERE ns.rspoNumer = @rspoNumer;";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Begin transaction
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 1. Delete record from OldSchools
            await using (var deleteCmd = new NpgsqlCommand(deleteSql, connection, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@rspoNumer", rsponumer);
                var affectedRows = await deleteCmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"Record with rsponumer = {rsponumer} not found and was not deleted.");
                }
            }

            // Commit transaction if all operations succeeded
            await transaction.CommitAsync();

            Console.WriteLine($"Record with rsponumer = {rsponumer} successfully deleted and recorded in history.");
        }
        catch (Exception ex)
        {
            // Rollback transaction in case of error
            await transaction.RollbackAsync();
            await Console.Error.WriteLineAsync($"Error deleting record with rsponumer = {rsponumer}: {ex.Message}");
            throw; // Rethrow exception for further handling
        }
    }
    
    private string SerializeJson(object value)
        => value != null ? JsonConvert.SerializeObject(value) : null;
    
    private T? DeserializeJson<T>(object dbValue)
    {
        if (dbValue == DBNull.Value)
            return default; // null for T class

        var jsonString = dbValue.ToString();
        if (string.IsNullOrEmpty(jsonString))
            return default;

        return JsonConvert.DeserializeObject<T>(jsonString);
    }
}