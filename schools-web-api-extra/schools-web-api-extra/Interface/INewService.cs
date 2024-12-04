using schools_web_api.Model;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api_extra.Interface
{
    public interface INewService
    {
        public Task<List<FullSchool>> GetSchoolsAsync(SchoolRequestParameters body);
    }
}
