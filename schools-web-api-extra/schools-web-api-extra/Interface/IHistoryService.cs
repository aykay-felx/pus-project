using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface;

public interface IHistoryService
{
    /// <summary>
    /// Retrieve the change history for a given RspoNumer.
    /// </summary>
    Task<IEnumerable<SchoolHistory>> GetHistoryByRspoAsync(string rspoNumer);
}