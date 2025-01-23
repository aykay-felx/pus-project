using Npgsql;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Repositories;

public class OldSchoolRepository : IOldSchoolService
{
    private readonly string _connectionString;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly INewSchoolService _newSchoolRepo;

    public OldSchoolRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
        _httpClientFactory = httpClientFactory;
        _newSchoolRepo = new NewSchoolRepository(configuration, httpClientFactory);
    }

    /// <summary>
    /// Apply changes from the list of NewSchool to OldSchools.
    ///    - If an OldSchool with the same RspoNumer does not exist => INSERT
    ///    - Otherwise => partial UPDATE
    /// </summary>
    public async Task ApplyChangesFromNewSchoolsAsync(IEnumerable<NewSchool> newSchools)
    {
        if (newSchools == null) return;

        // Load all OldSchools and create a dictionary for quick lookup
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
                    // No such record => insert
                    await InsertSingleOldSchoolAsync(connection, transaction, newSchool);
                }
                else
                {
                    // Record exists => perform partial update
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
    /// Delete OldSchool by RspoNumer
    /// </summary>
    public async Task DeleteOldSchoolAsync(string rspoNumer)
    {
        // Define SQL queries for deleting and adding a record to history
        const string deleteSql = "DELETE FROM public.oldschools WHERE rspoNumer = @rspoNumer;";
        const string insertHistorySql = @"
            INSERT INTO public.schoolhistory (rspoNumer, changedat, changes)
            VALUES (@RspoNumer, @ChangedAt, @Changes);";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Begin transaction
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 1. Delete record from OldSchools
            await using (var deleteCmd = new NpgsqlCommand(deleteSql, connection, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@rspoNumer", rspoNumer);
                int affectedRows = await deleteCmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"Record with rspoNumer = {rspoNumer} not found and was not deleted.");
                }
            }

            // 2. Add record to history
            await using (var historyCmd = new NpgsqlCommand(insertHistorySql, connection, transaction))
            {
                historyCmd.Parameters.AddWithValue("@RspoNumer", rspoNumer);
                historyCmd.Parameters.AddWithValue("@ChangedAt", DateTime.UtcNow);
                historyCmd.Parameters.AddWithValue("@Changes", "DELETE");

                await historyCmd.ExecuteNonQueryAsync();
            }

            // Commit transaction if all operations succeeded
            await transaction.CommitAsync();

            Console.WriteLine($"Record with rspoNumer = {rspoNumer} successfully deleted and recorded in history.");
        }
        catch (Exception ex)
        {
            // Rollback transaction in case of error
            await transaction.RollbackAsync();
            Console.Error.WriteLine($"Error deleting record with rspoNumer = {rspoNumer}: {ex.Message}");
            throw; // Rethrow exception for further handling
        }
    }

    public async Task SetOldSchoolForTestingAsync()
    {
        // 1) Take all records from the NewSchools table
        var newList = (await _newSchoolRepo.GetAllNewSchoolAsync()).ToList();
        if (!newList.Any()) return; // if no data

        // 2) Read OldSchools
        var oldList = (await GetAllOldSchoolsAsync()).ToList();
        var oldDict = oldList.ToDictionary(o => o.RspoNumer, o => o);

        // 3) Open connection
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 4) For each newSchool in newList
            foreach (var newSchool in newList)
            {
                // Check if such a record exists in OldSchools
                if (!oldDict.TryGetValue(newSchool.RspoNumer, out var oldSchool))
                {
                    // No => INSERT
                    await InsertSingleOldSchoolAsync(connection, transaction, newSchool);
                }
                else
                {
                    // Yes => UPDATE
                    await UpdateOldSchoolAsync(connection, transaction, oldSchool, newSchool);
                }

                // 5) After Insert/Update => Nazwa='1'
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
                -- ManualFlags ? If needed
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

        // JezykiNauczane array => string.Join(',')
        if (newSchool.JezykiNauczane != null && newSchool.JezykiNauczane.Any())
            cmd.Parameters.AddWithValue("JezykiNauczane", string.Join(",", newSchool.JezykiNauczane));
        else
            cmd.Parameters.AddWithValue("JezykiNauczane", DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }
    
    private async Task UpdateOldSchoolAsync(
    NpgsqlConnection connection,
    NpgsqlTransaction transaction,
    OldSchool oldSchool,
    NewSchool newSchool
)
    {
        var fieldsToUpdate = new Dictionary<string, (object? OldValue, object? NewValue)>();

        // Get all properties of NewSchool
        var newSchoolProperties = typeof(NewSchool).GetProperties();

        foreach (var newSchoolProperty in newSchoolProperties)
        {
            // 1. Check if OldSchool has the same property
            var oldSchoolProperty = typeof(OldSchool).GetProperty(newSchoolProperty.Name);
            if (oldSchoolProperty == null)
            {
                // Property (e.g., SubFieldNazwa) does not exist in OldSchool => skip
                continue;
            }

            // 2. If it's the JezykiNauczane array => handle separately
            if (newSchoolProperty.Name == nameof(NewSchool.JezykiNauczane))
            {
                var oldJezyki = oldSchool.JezykiNauczane ?? new string[0];
                var newJezyki = newSchool.JezykiNauczane ?? new string[0];

                if (!Enumerable.SequenceEqual(newJezyki, oldJezyki))
                {
                    fieldsToUpdate["JezykiNauczane"] = (
                        oldJezyki.Any() ? string.Join(",", oldJezyki) : DBNull.Value,
                        newJezyki.Any() ? string.Join(",", newJezyki) : DBNull.Value
                    );
                }
                continue;
            }

            // 3. Get the value from newSchool
            var newValue = newSchoolProperty.GetValue(newSchool);
            var oldValue = oldSchoolProperty.GetValue(oldSchool);

            // 4. If newValue is a SubField (or a different type),
            //    and OldSchool does not have a similar field, skip
            //    (in this case, no need to transfer SubField to OldSchool).
            if (newValue is SubField)
            {
                // Skip, as OldSchool cannot store SubField
                continue;
            }

            // 5. Compare: if values differ, add to fieldsToUpdate
            if (!Equals(newValue, oldValue))
            {
                fieldsToUpdate[newSchoolProperty.Name] = (oldValue, newValue ?? DBNull.Value);
            }
        }

        // 6. If there are no changes, exit
        if (fieldsToUpdate.Count == 0) return;

        // 7. Formulate SQL
        var sb = new System.Text.StringBuilder("UPDATE OldSchools SET ");
        var parameters = new List<NpgsqlParameter>();
        int i = 0;

        foreach (var kvp in fieldsToUpdate)
        {
            if (i > 0) sb.Append(", ");
            var colName = kvp.Key;        // For example, "Nazwa"
            var paramName = "@p" + i;     // "@p0", "@p1", ...
            sb.Append($"{colName}={paramName}");

            parameters.Add(new NpgsqlParameter(paramName, kvp.Value.NewValue ?? DBNull.Value));
            i++;
        }

        sb.Append(" WHERE RspoNumer=@rspo;");
        parameters.Add(new NpgsqlParameter("@rspo", oldSchool.RspoNumer));

        // 8. Execute UPDATE
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddRange(parameters.ToArray());

        await cmd.ExecuteNonQueryAsync();

        // 9. Write to history (if needed)
        var changesDescription = string.Join(", ", fieldsToUpdate.Select(kvp =>
        {
            var oldValue = kvp.Value.OldValue ?? "null";
            var newValue = kvp.Value.NewValue ?? "null";
            return $"{kvp.Key}: {oldValue} -> {newValue}";
        }));

        await AddHistoryRecordAsync(connection, transaction, oldSchool.RspoNumer, $"{changesDescription}");
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
        cmd.Parameters.AddWithValue("changedAt", DateTime.Now); // or DateTime.UtcNow
        cmd.Parameters.AddWithValue("changes", changes);

        await cmd.ExecuteNonQueryAsync();
    }
    
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

        // Explicitly specify that this is text
        var p = cmd.Parameters.Add("rspo", NpgsqlTypes.NpgsqlDbType.Text);
        p.Value = rspoNumer ?? (object)DBNull.Value;

        await cmd.ExecuteNonQueryAsync();
    }
}