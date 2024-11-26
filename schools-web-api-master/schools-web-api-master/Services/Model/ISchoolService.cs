using schools_web_api.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using schools_web_api.TokenManager.TransmitModels;

namespace schools_web_api.TokenManager.Services.Model
{
    public interface ISchoolService
    {
        public Task<List<SimpleSchool>> GetSchoolsAsync(SchoolRequestParameters body);
        public Task<FullSchool> GetSchoolByIdAsync(int id);
        public Task<bool> AddSchoolAsync(FullSchool school);
        public Task<FullSchool> UpdateSchoolAsync(FullSchool oldData, FullSchool newData);
        public Task<bool> DeleteSchoolAsync(int id);
    }
}