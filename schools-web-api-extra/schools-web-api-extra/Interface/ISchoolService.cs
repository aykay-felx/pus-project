using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface
{
    public interface ISchoolService
    {
        Task SaveOldSchoolsAsync(List<NewSchool> oldSchools);
        Task DeleteAllOldSchoolsAsync();
        Task<IEnumerable<OldSchool>> GetOldSchoolsAsync();

        Task<IEnumerable<NewSchool>> CompareSchoolsAsync(List<NewSchool> newSchools);
        Task <IEnumerable<NewSchool>> SaveNewSchoolsAsync(List<NewSchool> newSchools);
    }
}
