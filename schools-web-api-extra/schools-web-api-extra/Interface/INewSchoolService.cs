using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface;

public interface INewSchoolService
{
    /// <summary>
    /// Fetch a list of schools from an external API and return them as List<NewSchool>.
    /// </summary>
    Task<List<NewSchool>> FetchSchoolsFromApiAsync(Func<int, double, Task> reportProgress, CancellationToken cancellationToken);
    
    /// <summary>
    /// Compare the NewSchool list with the existing OldSchools 
    ///    (populate SubFields, isDifferentObj, isNewObj for each NewSchool).
    /// </summary>
    Task<List<NewSchool>> CompareWithOldSchoolsAsync(List<NewSchool> newSchools);
    
    /// <summary>
    /// Save the NewSchool list to the NewSchools table (for subsequent display on the frontend).
    /// </summary>
    Task SaveNewSchoolsAsync(List<NewSchool> newSchools);
    
    /// <summary>
    /// Retrieve all new schools (NewSchools).
    /// </summary>
    Task<IEnumerable<NewSchool>> GetAllNewSchoolAsync();
    
    /// <summary>
    /// Delete all records from the NewSchools table.
    /// </summary>
    Task DeleteAllNewSchoolAsync();
    
    /// <summary>
    /// Delete record from the NewSchools table by rsponumer.
    /// </summary>
    Task DeleteNewSchoolAsync(string rsponumer);
}