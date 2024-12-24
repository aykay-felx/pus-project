using Npgsql;
using Newtonsoft.Json;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Repositories;

public class SchoolRepository : ISchoolService
{
    private readonly string _connectionString;
    private readonly IHttpClientFactory _httpClientFactory;

    public SchoolRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
        _httpClientFactory = httpClientFactory;
    }

    #region 1) Fetch from API
    /// <summary>
    /// 1) Извлечь список школ с внешнего API (https://api-rspo.men.gov.pl/api/placowki/?page=...).
    /// Преобразовать их в List<NewSchool> и вернуть.
    /// </summary>
    public async Task<List<NewSchool>> FetchSchoolsFromApiAsync(int page)
    {
        var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={page}";
        var httpClient = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        request.Headers.Add("accept", "application/json");

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error accessing RSPO's API: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();

        // Предположим, что есть некий метод JsonConvertToFullSchols.JsongConvertToFullSchools(...)
        // который десериализует JSON-ответ в List<NewSchool>.
        var newSchools = JsonConvertToFullSchols.JsonConvertToFullSchools(content);

        return newSchools;
    }

    #endregion

    #region 2) CompareWithOldSchoolsAsync

    /// <summary>
    /// 2) Сравнить список NewSchool с уже существующими OldSchools 
    ///    (заполняем SubFields, isDifferentObj, isNewObj).
    /// </summary>
    ///
    ///
    
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

                if (specialProperties.Contains(property.Name)) subField = new SubField(true, string.Join(",", oldValue));
                else subField = new SubField(true, oldValue?.ToString());
                
                subFieldProperty.SetValue(newSchool, subField);
            }
        }

        return newSchools;
    }

    #endregion

    #region 3) SaveNewSchoolsAsync

    /// <summary>
    /// 3) Сохранить список NewSchool в таблицу NewSchools.
    /// </summary>
    public async Task SaveNewSchoolsAsync(List<NewSchool> newSchools)
    {
        if (newSchools == null || newSchools.Count == 0) return;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Пример вставки (INSERT) по одному или батчом
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

                // Заполняем "обычные" поля
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

                // Массив (jezykiNauczane) — можно сериализовать в строку
                if (school.JezykiNauczane != null && school.JezykiNauczane.Any())
                    cmd.Parameters.AddWithValue("JezykiNauczane", string.Join(",", school.JezykiNauczane));
                else
                    cmd.Parameters.AddWithValue("JezykiNauczane", DBNull.Value);

                // Заполняем JSONB-поля (SubFields)
                cmd.Parameters.AddWithValue("SubFieldRspoNumer", (object?)SerializeJson(school.SubFieldRspoNumer) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldLongitude", (object?)SerializeJson(school.SubFieldLongitude) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldLatitude", (object?)SerializeJson(school.SubFieldLatitude) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldTyp", (object?)SerializeJson(school.SubFieldTyp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldNazwa", (object?)SerializeJson(school.SubFieldNazwa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldMiejscowosc", (object?)SerializeJson(school.SubFieldMiejscowosc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldWojewodztwo", (object?)SerializeJson(school.SubFieldWojewodztwo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldKodPocztowy", (object?)SerializeJson(school.SubFieldKodPocztowy) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldNumerBudynku", (object?)SerializeJson(school.SubFieldNumerBudynku) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldEmail", (object?)SerializeJson(school.SubFieldEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldUlica", (object?)SerializeJson(school.SubFieldUlica) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldTelefon", (object?)SerializeJson(school.SubFieldTelefon) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldStatusPublicznosc", (object?)SerializeJson(school.SubFieldStatusPublicznosc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldStronaInternetowa", (object?)SerializeJson(school.SubFieldStronaInternetowa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldDyrektor", (object?)SerializeJson(school.SubFieldDyrektor) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldNipPodmiotu", (object?)SerializeJson(school.SubFieldNipPodmiotu) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldRegonPodmiotu", (object?)SerializeJson(school.SubFieldRegonPodmiotu) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldDataZalozenia", (object?)SerializeJson(school.SubFieldDataZalozenia) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldLiczbaUczniow", (object?)SerializeJson(school.SubFieldLiczbaUczniow) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldKategoriaUczniow", (object?)SerializeJson(school.SubFieldKategoriaUczniow) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldSpecyfikaPlacowki", (object?)SerializeJson(school.SubFieldSpecyfikaPlacowki) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldGmina", (object?)SerializeJson(school.SubFieldGmina) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("SubFieldPowiat", (object?)SerializeJson(school.SubFieldPowiat) ?? DBNull.Value);

                // Если массив subFieldJezykiNauczane -> сериализуем в JSON
                cmd.Parameters.AddWithValue("SubFieldJezykiNauczane", (object?)SerializeJson(school.SubFieldJezykiNauczane) ?? DBNull.Value);

                // Флаги
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

    #endregion

    #region 4) ApplyChangesFromNewSchoolsAsync

    /// <summary>
    /// 4) Применяем изменения из списка NewSchool к OldSchools.
    ///    - Если OldSchool с таким RspoNumer не существует => INSERT
    ///    - Иначе => частичный UPDATE
    /// </summary>
    public async Task ApplyChangesFromNewSchoolsAsync(IEnumerable<NewSchool> newSchools)
    {
        if (newSchools == null) return;

        // Загружаем все OldSchools и делаем словарь для быстрого поиска
        var oldList = (await GetAllOldSchoolsAsync()).ToList();
        var oldDict = oldList.ToDictionary(o => o.RspoNumer, o => o);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            foreach (var newSchool in newSchools)
            {
                if (!oldDict.TryGetValue(newSchool.RspoNumer, out var oldSchool))
                {
                    // Нет такой записи => вставляем
                    await InsertSingleOldSchoolAsync(connection, transaction, newSchool);
                }
                else
                {
                    // Есть такая запись => обновляем частично
                    await UpdateOldSchoolAsync(connection, transaction, oldSchool, newSchool);
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ----------------------------------------------------------------
    // Вставляем одну OldSchool, конвертируя NewSchool → OldSchool
    // ----------------------------------------------------------------
    private async Task InsertSingleOldSchoolAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        NewSchool newSchool)
    {
        var sql = @"
            INSERT INTO OldSchools (
                RspoNumer, Longitude, Latitude, Typ, Nazwa,
                Miejscowosc, Wojewodztwo, KodPocztowy, NumerBudynku, 
                Email, Ulica, Telefon, StatusPublicznosc, StronaInternetowa, 
                Dyrektor, NipPodmiotu, RegonPodmiotu, DataZalozenia, 
                LiczbaUczniow, KategoriaUczniow, SpecyfikaPlacowki, Gmina, Powiat, 
                JezykiNauczane
                -- ManualFlags ? Если нужно
            ) VALUES (
                @RspoNumer, @Longitude, @Latitude, @Typ, @Nazwa,
                @Miejscowosc, @Wojewodztwo, @KodPocztowy, @NumerBudynku,
                @Email, @Ulica, @Telefon, @StatusPublicznosc, @StronaInternetowa,
                @Dyrektor, @NipPodmiotu, @RegonPodmiotu, @DataZalozenia,
                @LiczbaUczniow, @KategoriaUczniow, @SpecyfikaPlacowki, @Gmina, @Powiat,
                @JezykiNauczane
            );
        ";
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("RspoNumer", newSchool.RspoNumer ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Longitude", newSchool.Longitude);
        cmd.Parameters.AddWithValue("Latitude", newSchool.Latitude);
        cmd.Parameters.AddWithValue("Typ", newSchool.Typ ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Nazwa", newSchool.Nazwa ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Miejscowosc", newSchool.Miejscowosc ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Wojewodztwo", newSchool.Wojewodztwo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("KodPocztowy", newSchool.KodPocztowy ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("NumerBudynku", newSchool.NumerBudynku ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Email", newSchool.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Ulica", newSchool.Ulica ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Telefon", newSchool.Telefon ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("StatusPublicznosc", newSchool.StatusPublicznosc ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("StronaInternetowa", newSchool.StronaInternetowa ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Dyrektor", newSchool.Dyrektor ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("NipPodmiotu", newSchool.NipPodmiotu ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("RegonPodmiotu", newSchool.RegonPodmiotu ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("DataZalozenia", newSchool.DataZalozenia ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("LiczbaUczniow", newSchool.LiczbaUczniow ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("KategoriaUczniow", newSchool.KategoriaUczniow ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("SpecyfikaPlacowki", newSchool.SpecyfikaPlacowki ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Gmina", newSchool.Gmina ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("Powiat", newSchool.Powiat ?? (object)DBNull.Value);

        // Массив jezykiNauczane => string.Join(',')
        if (newSchool.JezykiNauczane != null && newSchool.JezykiNauczane.Any())
            cmd.Parameters.AddWithValue("JezykiNauczane", string.Join(",", newSchool.JezykiNauczane));
        else
            cmd.Parameters.AddWithValue("JezykiNauczane", DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    // ----------------------------------------------------------------
    // «Partial Reload OldSchools
    // ----------------------------------------------------------------
    // Нас Гаврила казнит, если увидит 23 сравнения через if, может переделаю завтра этот метод
    private async Task UpdateOldSchoolAsync(
      NpgsqlConnection connection,
      NpgsqlTransaction transaction,
      OldSchool oldSchool,
      NewSchool newSchool)
    {
        var fieldsToUpdate = new Dictionary<string, object?>();

        // 1. double Longitude
        if (newSchool.Longitude != oldSchool.Longitude)
            fieldsToUpdate["Longitude"] = newSchool.Longitude;

        // 2. double Latitude
        if (newSchool.Latitude != oldSchool.Latitude)
            fieldsToUpdate["Latitude"] = newSchool.Latitude;

        // 3. string Typ
        if (newSchool.Typ != oldSchool.Typ)
            fieldsToUpdate["Typ"] = newSchool.Typ ?? (object)DBNull.Value;

        // 4. string Nazwa
        if (newSchool.Nazwa != oldSchool.Nazwa)
            fieldsToUpdate["Nazwa"] = newSchool.Nazwa ?? (object)DBNull.Value;

        // 5. string Miejscowosc
        if (newSchool.Miejscowosc != oldSchool.Miejscowosc)
            fieldsToUpdate["Miejscowosc"] = newSchool.Miejscowosc ?? (object)DBNull.Value;

        // 6. string Wojewodztwo
        if (newSchool.Wojewodztwo != oldSchool.Wojewodztwo)
            fieldsToUpdate["Wojewodztwo"] = newSchool.Wojewodztwo ?? (object)DBNull.Value;

        // 7. string KodPocztowy
        if (newSchool.KodPocztowy != oldSchool.KodPocztowy)
            fieldsToUpdate["KodPocztowy"] = newSchool.KodPocztowy ?? (object)DBNull.Value;

        // 8. string NumerBudynku
        if (newSchool.NumerBudynku != oldSchool.NumerBudynku)
            fieldsToUpdate["NumerBudynku"] = newSchool.NumerBudynku ?? (object)DBNull.Value;

        // 9. string? Email
        if (newSchool.Email != oldSchool.Email)
            fieldsToUpdate["Email"] = newSchool.Email ?? (object)DBNull.Value;

        // 10. string? Ulica
        if (newSchool.Ulica != oldSchool.Ulica)
            fieldsToUpdate["Ulica"] = newSchool.Ulica ?? (object)DBNull.Value;

        // 11. string? Telefon
        if (newSchool.Telefon != oldSchool.Telefon)
            fieldsToUpdate["Telefon"] = newSchool.Telefon ?? (object)DBNull.Value;

        // 12. string? StatusPublicznosc
        if (newSchool.StatusPublicznosc != oldSchool.StatusPublicznosc)
            fieldsToUpdate["StatusPublicznosc"] = newSchool.StatusPublicznosc ?? (object)DBNull.Value;

        // 13. string? StronaInternetowa
        if (newSchool.StronaInternetowa != oldSchool.StronaInternetowa)
            fieldsToUpdate["StronaInternetowa"] = newSchool.StronaInternetowa ?? (object)DBNull.Value;

        // 14. string? Dyrektor
        if (newSchool.Dyrektor != oldSchool.Dyrektor)
            fieldsToUpdate["Dyrektor"] = newSchool.Dyrektor ?? (object)DBNull.Value;

        // 15. string? NipPodmiotu
        if (newSchool.NipPodmiotu != oldSchool.NipPodmiotu)
            fieldsToUpdate["NipPodmiotu"] = newSchool.NipPodmiotu ?? (object)DBNull.Value;

        // 16. string? RegonPodmiotu
        if (newSchool.RegonPodmiotu != oldSchool.RegonPodmiotu)
            fieldsToUpdate["RegonPodmiotu"] = newSchool.RegonPodmiotu ?? (object)DBNull.Value;

        // 17. string? DataZalozenia
        if (newSchool.DataZalozenia != oldSchool.DataZalozenia)
            fieldsToUpdate["DataZalozenia"] = newSchool.DataZalozenia ?? (object)DBNull.Value;

        // 18. int? LiczbaUczniow
        if (newSchool.LiczbaUczniow != oldSchool.LiczbaUczniow)
            fieldsToUpdate["LiczbaUczniow"] = newSchool.LiczbaUczniow ?? (object)DBNull.Value;

        // 19. string? KategoriaUczniow
        if (newSchool.KategoriaUczniow != oldSchool.KategoriaUczniow)
            fieldsToUpdate["KategoriaUczniow"] = newSchool.KategoriaUczniow ?? (object)DBNull.Value;

        // 20. string? SpecyfikaPlacowki
        if (newSchool.SpecyfikaPlacowki != oldSchool.SpecyfikaPlacowki)
            fieldsToUpdate["SpecyfikaPlacowki"] = newSchool.SpecyfikaPlacowki ?? (object)DBNull.Value;

        // 21. string? Gmina
        if (newSchool.Gmina != oldSchool.Gmina)
            fieldsToUpdate["Gmina"] = newSchool.Gmina ?? (object)DBNull.Value;

        // 22. string? Powiat
        if (newSchool.Powiat != oldSchool.Powiat)
            fieldsToUpdate["Powiat"] = newSchool.Powiat ?? (object)DBNull.Value;

        // 23. string[]? JezykiNauczane
        var oldJezyki = oldSchool.JezykiNauczane ?? new string[0];
        var newJezyki = newSchool.JezykiNauczane ?? new string[0];
        if (!Enumerable.SequenceEqual(newJezyki, oldJezyki))
        {
            // Храним в БД как строку, соединённую запятой
            fieldsToUpdate["JezykiNauczane"] = newJezyki.Any()
                ? string.Join(",", newJezyki)
                : (object)DBNull.Value;
        }

        // Если нечего обновлять — выходим
        if (fieldsToUpdate.Count == 0) return;

        // Generate SQL: UPDATE OldSchools SET field1=@p0, field2=@p1... WHERE RspoNumer=@rspo
        var sb = new System.Text.StringBuilder("UPDATE OldSchools SET ");
        var parameters = new List<NpgsqlParameter>();

        int i = 0;
        foreach (var kvp in fieldsToUpdate)
        {
            if (i > 0) sb.Append(", ");
            var colName = kvp.Key;
            var paramName = "@p" + i;
            sb.Append($"{colName}={paramName}");

            parameters.Add(new NpgsqlParameter(paramName, kvp.Value ?? DBNull.Value));
            i++;
        }

        sb.Append(" WHERE RspoNumer=@rspo;");
        parameters.Add(new NpgsqlParameter("@rspo", oldSchool.RspoNumer));

        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddRange(parameters.ToArray());

        await cmd.ExecuteNonQueryAsync();


        var changedFields = string.Join(", ", fieldsToUpdate.Keys);
        var changesDescription = $"Updated fields: {changedFields}";
        // Записываем в историю
        await AddHistoryRecordAsync(connection, transaction, oldSchool.RspoNumer, changesDescription);
    }


    #endregion

    #region 5) GetAllOldSchoolsAsync & 6) DeleteOldSchoolAsync

    /// <summary>
    /// 5) Get All OldSchools
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
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(',')
                // ManualFlags и т.д. по необходимости
            };
            results.Add(oldSchool);
        }

        return results;
    }

    /// <summary>
    /// 6) Delete OldSchool by RspoNumer
    /// </summary>
    public async Task DeleteOldSchoolAsync(string rspoNumer)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "DELETE FROM OldSchools WHERE RspoNumer=@rspo";
        using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("rspo", rspoNumer);

        await cmd.ExecuteNonQueryAsync();
    }

    #endregion
    //Add History


    public async Task<IEnumerable<SchoolHistory>> GetHistoryByRspoAsync(string rspoNumer)
    {
        var historyList = new List<SchoolHistory>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
        SELECT Id, RspoNumer, ChangedAt, Changes
        FROM SchoolHistory
        WHERE RspoNumer = @rspo
        ORDER BY ChangedAt DESC;
    ";

        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("rspo", rspoNumer);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var item = new SchoolHistory
            {
                Id = reader.GetInt32(0),
                RspoNumer = reader.GetString(1),
                ChangedAt = reader.GetDateTime(2),
                Changes = reader.GetString(3)
            };
            historyList.Add(item);
        }

        return historyList;
    }




    private async Task AddHistoryRecordAsync(
      NpgsqlConnection connection,
      NpgsqlTransaction transaction,
      string rspoNumer,
      string changes
  )
    {
        const string sql = @"
        INSERT INTO SchoolHistory (RspoNumer, ChangedAt, Changes)
        VALUES (@rspo, @changedAt, @changes);
    ";

        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("rspo", rspoNumer);
        cmd.Parameters.AddWithValue("changedAt", DateTime.Now); // или DateTime.UtcNow
        cmd.Parameters.AddWithValue("changes", changes);

        await cmd.ExecuteNonQueryAsync();
    }


    private string SerializeJson(object value)
        => value != null ? JsonConvert.SerializeObject(value) : null;

 
}
