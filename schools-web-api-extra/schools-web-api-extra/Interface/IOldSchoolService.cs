using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface;

public interface IOldSchoolService
{
    /// <summary>
    /// Apply changes from the NewSchool list to OldSchools upon user request 
    ///    (after manual corrections):
    ///    - If a record with the same RspoNumer doesn't exist, perform an INSERT.
    ///    - If it does exist, perform a "partial" update for only the necessary fields.
    /// </summary>
    Task ApplyChangesFromNewSchoolsAsync(IEnumerable<NewSchool> newSchools);
    
    /// <summary>
    /// Retrieve all old schools (OldSchools).
    /// </summary>
    Task<IEnumerable<OldSchool>> GetAllOldSchoolsAsync();
    
    /// <summary>
    /// Delete a single OldSchools record by RspoNumer.
    /// </summary>
    Task DeleteOldSchoolAsync(string rspoNumer);
    
    /// <summary>
    /// Save changes from the NewSchool list to OldSchools 
    /// (insert/update), and then forcibly set the Nazwa field to '1' 
    /// for each affected school.
    /// </summary>
    Task SetOldSchoolForTestingAsync();

}