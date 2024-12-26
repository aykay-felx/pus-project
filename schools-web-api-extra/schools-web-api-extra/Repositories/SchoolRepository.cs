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

                // Обычные поля
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

                // Массив JezykiNauczane → как строку "angielski,niemiecki" (или массив text[])
                if (school.JezykiNauczane != null && school.JezykiNauczane.Any())
                    cmd.Parameters.AddWithValue("JezykiNauczane", string.Join(",", school.JezykiNauczane));
                else
                    cmd.Parameters.AddWithValue("JezykiNauczane", DBNull.Value);

                // Для SubField* полей, в БД — JSONB
                // Указываем NpgsqlDbType.Jsonb
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
    private async Task UpdateOldSchoolAsync(
      NpgsqlConnection connection,
      NpgsqlTransaction transaction,
      OldSchool oldSchool,
      NewSchool newSchool
  )
    {
        var fieldsToUpdate = new Dictionary<string, object?>();

        // Получаем все свойства NewSchool
        var newSchoolProperties = typeof(NewSchool).GetProperties();

        foreach (var newSchoolProperty in newSchoolProperties)
        {
            // 1. Проверяем, есть ли в OldSchool такое же свойство
            var oldSchoolProperty = typeof(OldSchool).GetProperty(newSchoolProperty.Name);
            if (oldSchoolProperty == null)
            {
                // Свойство (например, SubFieldNazwa) отсутствует в OldSchool => пропускаем
                continue;
            }

            // 2. Если это массив JezykiNauczane => делаем отдельную логику
            if (newSchoolProperty.Name == nameof(NewSchool.JezykiNauczane))
            {
                var oldJezyki = oldSchool.JezykiNauczane ?? new string[0];
                var newJezyki = newSchool.JezykiNauczane ?? new string[0];

                if (!Enumerable.SequenceEqual(newJezyki, oldJezyki))
                {
                    fieldsToUpdate["JezykiNauczane"] =
                        newJezyki.Any() ? string.Join(",", newJezyki) : DBNull.Value;
                }
                continue;
            }

            // 3. Получаем значение из newSchool
            var newValue = newSchoolProperty.GetValue(newSchool);
            var oldValue = oldSchoolProperty.GetValue(oldSchool);

            // 4. Если newValue — это SubField (или вообще не тот тип),
            //    а в OldSchool нет аналогичного поля, пропускаем
            //    (в данном случае не нужно переносить SubField в OldSchool).
            if (newValue is SubField)
            {
                // Пропускаем, т.к. OldSchool не умеет хранить SubField
                continue;
            }

            // 5. Сравниваем: если значения отличаются — добавим в fieldsToUpdate
            if (!Equals(newValue, oldValue))
            {
                fieldsToUpdate[newSchoolProperty.Name] = newValue ?? DBNull.Value;
            }
        }

        // 6. Если никаких изменений — выходим
        if (fieldsToUpdate.Count == 0) return;

        // 7. Формируем SQL
        var sb = new System.Text.StringBuilder("UPDATE OldSchools SET ");
        var parameters = new List<NpgsqlParameter>();
        int i = 0;

        foreach (var kvp in fieldsToUpdate)
        {
            if (i > 0) sb.Append(", ");
            var colName = kvp.Key;        // Например, "Nazwa"
            var paramName = "@p" + i;     // "@p0", "@p1", ...
            sb.Append($"{colName}={paramName}");

            parameters.Add(new NpgsqlParameter(paramName, kvp.Value ?? DBNull.Value));
            i++;
        }

        sb.Append(" WHERE RspoNumer=@rspo;");
        parameters.Add(new NpgsqlParameter("@rspo", oldSchool.RspoNumer));

        // 8. Выполняем UPDATE
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddRange(parameters.ToArray());

        await cmd.ExecuteNonQueryAsync();

        // 9. Пишем в историю (если нужно)
        var changedFields = string.Join(", ", fieldsToUpdate.Keys);
        var changesDescription = $"Updated fields: {changedFields}";
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
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(','),
               // ManualFlags = reader["ManualFlags"]?.ToString()?.Split(',')
            };
            results.Add(oldSchool);
        }

        return results;
    }


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
            // Создаём новый объект NewSchool
            var newSchool = new NewSchool
            {
                // 1) «Основные» поля
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

                // 2) Массив JezykiNauczane (если хранится как "angielski,niemiecki" в столбце)
                JezykiNauczane = reader["JezykiNauczane"]?.ToString()?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries),

                // 3) Флаги (bool?) 
                //  - Если столбец isDifferentObj имеет тип BOOLEAN, то читаем как bool? 
                //    (если в БД NULL, вернём null)
                isDifferentObj = reader["isDifferentObj"] == DBNull.Value
                    ? null
                    : (bool?)reader["isDifferentObj"],

                isNewObj = reader["isNewObj"] == DBNull.Value
                    ? null
                    : (bool?)reader["isNewObj"],

                // 4) Подполя (SubField...) десериализуем из JSONB
                //    Используем вспомогательный метод DeserializeJson<SubField>(...)
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
    /// Вспомогательный метод для десериализации столбца JSONB (SubField).
    /// Если значение == DBNull, вернём null.
    /// Иначе десериализуем в тип T.
    /// </summary>
    private T? DeserializeJson<T>(object dbValue)
    {
        if (dbValue == DBNull.Value)
            return default; // null для T-класса

        var jsonString = dbValue.ToString();
        if (string.IsNullOrEmpty(jsonString))
            return default;

        return JsonConvert.DeserializeObject<T>(jsonString);
    }


    /// <summary>
    /// 6) Delete OldSchool by RspoNumer
    /// </summary>
    public async Task DeleteOldSchoolAsync(string rspoNumer)
    {
        // Определите SQL-запросы для удаления и добавления записи в историю
        const string deleteSql = "DELETE FROM public.oldschools WHERE rspoNumer = @rspoNumer;";
        const string insertHistorySql = @"
            INSERT INTO public.schoolhistory (rspoNumer, changedat, changes)
            VALUES (@RspoNumer, @ChangedAt, @Changes);";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Начинаем транзакцию
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 1. Удаление записи из OldSchools
            await using (var deleteCmd = new NpgsqlCommand(deleteSql, connection, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@rspoNumer", rspoNumer);
                int affectedRows = await deleteCmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"Запись с rspoNumer = {rspoNumer} не найдена и не была удалена.");
                }
            }

            // 2. Добавление записи в историю
            await using (var historyCmd = new NpgsqlCommand(insertHistorySql, connection, transaction))
            {
                historyCmd.Parameters.AddWithValue("@RspoNumer", rspoNumer);
                historyCmd.Parameters.AddWithValue("@ChangedAt", DateTime.UtcNow);
                historyCmd.Parameters.AddWithValue("@Changes", "DELETE");

                await historyCmd.ExecuteNonQueryAsync();
            }

            // Фиксируем транзакцию, если все операции прошли успешно
            await transaction.CommitAsync();

            Console.WriteLine($"Запись с rspoNumer = {rspoNumer} успешно удалена и зафиксирована в истории.");
        }
        catch (Exception ex)
        {
            // Откатываем транзакцию в случае ошибки
            await transaction.RollbackAsync();
            Console.Error.WriteLine($"Ошибка при удалении записи с rspoNumer = {rspoNumer}: {ex.Message}");
            throw; // Перебрасываем исключение для дальнейшей обработки
        }
    }



/// 8) Delete All NewSchool
/// </summary>
public async Task DeleteAllNewSchoolAsync()
    {
        const string sql = "DELETE FROM public.newschools;"; // Или используйте TRUNCATE

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, connection);

        // Поскольку удаляются все строки, параметры не нужны
        // Если вы решите использовать TRUNCATE, добавьте опцию CASCADE, если необходимо:
        // const string sql = "TRUNCATE TABLE public.newschools CASCADE;";

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



    #region Новый метод: SaveOldSchoolFromApplyChangesAsync (меняем только его)

    /// <summary>
    /// Берёт данные из NewSchools и сохраняет их в OldSchools (INSERT/UPDATE).
    /// После каждого сохранения ставим Nazwa='1' в OldSchools.
    /// </summary>
    public async Task SaveOldSchoolFromApplyChangesAsync()
    {
        // 1) Берём все записи из таблицы NewSchools
        var newList = (await GetAllNewSchoolAsync()).ToList();
        if (!newList.Any()) return; // если нет данных

        // 2) Считываем OldSchools
        var oldList = (await GetAllOldSchoolsAsync()).ToList();
        var oldDict = oldList.ToDictionary(o => o.RspoNumer, o => o);

        // 3) Открываем соединение
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 4) Для каждой newSchool из newList
            foreach (var newSchool in newList)
            {
                // Есть ли такая запись в OldSchools?
                if (!oldDict.TryGetValue(newSchool.RspoNumer, out var oldSchool))
                {
                    // Нет => INSERT
                    await InsertSingleOldSchoolAsync(connection, transaction, newSchool);
                }
                else
                {
                    // Есть => UPDATE
                    await UpdateOldSchoolAsync(connection, transaction, oldSchool, newSchool);
                }

                // 5) После Insert/Update => Nazwa='1'
                await SetNazwaToOneAsync(connection, transaction, newSchool.RspoNumer);
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
    /// Метод, который ставит Nazwa='1' в OldSchools.
    /// </summary>
    private async Task SetNazwaToOneAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string rspoNumer
    )
    {
        const string sql = "UPDATE OldSchools SET Nazwa='1' WHERE RspoNumer=@rspo";
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sql;

        // Явно укажем, что это текст
        var p = cmd.Parameters.Add("rspo", NpgsqlTypes.NpgsqlDbType.Text);
        p.Value = rspoNumer ?? (object)DBNull.Value;

        await cmd.ExecuteNonQueryAsync();
    }

    #endregion




    private string SerializeJson(object value)
        => value != null ? JsonConvert.SerializeObject(value) : null;

 
}
