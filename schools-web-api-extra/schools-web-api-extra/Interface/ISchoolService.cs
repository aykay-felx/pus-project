using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface;

public interface ISchoolService
{
    /// <summary>
    /// 1) Fetch a list of schools from an external API and return them as List<NewSchool>.
    /// </summary>
    Task<List<NewSchool>> FetchSchoolsFromApiAsync(int page);

    /// <summary>
    /// 2) Compare the NewSchool list with the existing OldSchools 
    ///    (populate SubFields, isDifferentObj, isNewObj for each NewSchool).
    /// </summary>
    Task<List<NewSchool>> CompareWithOldSchoolsAsync(List<NewSchool> newSchools);

    /// <summary>
    /// 3) Save the NewSchool list to the NewSchools table (for subsequent display on the frontend).
    /// </summary>
    Task SaveNewSchoolsAsync(List<NewSchool> newSchools);

    /// <summary>
    /// 4) Apply changes from the NewSchool list to OldSchools upon user request 
    ///    (after manual corrections):
    ///    - If a record with the same RspoNumer doesn't exist, perform an INSERT.
    ///    - If it does exist, perform a "partial" update for only the necessary fields.
    /// </summary>
    Task ApplyChangesFromNewSchoolsAsync(IEnumerable<NewSchool> newSchools);

    /// <summary>
    /// 5) Retrieve all old schools (OldSchools).
    /// </summary>
    Task<IEnumerable<OldSchool>> GetAllOldSchoolsAsync();

    /// <summary>
    /// 5) Retrieve all new schools (NewSchools).
    /// </summary>
    Task<IEnumerable<NewSchool>> GetAllNewSchoolAsync();

    /// <summary>
    /// 6) Delete a single OldSchools record by RspoNumer.
    /// </summary>
    Task DeleteOldSchoolAsync(string rspoNumer);

    /// <summary>
    /// Retrieve the change history for a given RspoNumer.
    /// </summary>
    Task<IEnumerable<SchoolHistory>> GetHistoryByRspoAsync(string rspoNumer);

    /// <summary>
    /// New method: Save changes from the NewSchool list to OldSchools 
    /// (insert/update), and then forcibly set the Nazwa field to '1' 
    /// for each affected school.
    /// </summary>
    Task SetOldSchoolForTestingAsync();

    /// <summary>
    /// Delete all records from the NewSchools table.
    /// </summary>
    Task DeleteAllNewSchoolAsync();
}
