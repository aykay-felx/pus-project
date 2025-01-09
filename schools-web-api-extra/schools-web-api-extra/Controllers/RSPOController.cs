using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;
using schools_web_api_extra.DTOs;

namespace schools_web_api_extra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSPOController : ControllerBase
    {
        private readonly ISchoolService _service;

        public RSPOController(ISchoolService service)
        {
            _service = service;
        }

        /// <summary>
        /// 1) Retrieve all old schools (OldSchools).
        /// GET: api/RSPO/old-schools
        /// </summary>
        [HttpGet("old-schools")]
        public async Task<IActionResult> GetAllOldSchools()
        {
            try
            {
                var oldSchools = await _service.GetAllOldSchoolsAsync();
                return Ok(oldSchools);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieve all new schools (NewSchools).
        /// GET: api/RSPO/new-schools
        /// </summary>
        [HttpGet("new-schools")]
        public async Task<IActionResult> GetAllNewSchools()
        {
            try
            {
                var newSchools = await _service.GetAllNewSchoolAsync();
                return Ok(newSchools);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 2) Delete a single school by RspoNumer.
        /// DELETE: api/RSPO/oldschools/{rspoNumer}
        /// </summary>
        [HttpDelete("oldschools/{rspoNumer}")]
        public async Task<IActionResult> DeleteOldSchool(string rspoNumer)
        {
            try
            {
                await _service.DeleteOldSchoolAsync(rspoNumer);
                return Ok($"School with RspoNumer={rspoNumer} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 3) Fetch data from an external RSPO API (page=?),
        ///    compare it with OldSchools, save the result in NewSchools,
        ///    and return the list of NewSchools to the frontend.
        /// GET: api/RSPO/new-schools/fetch-and-compare?page=1
        /// </summary>
        [HttpGet("new-schools/fetch-and-compare")]
        public async Task<IActionResult> FetchAndCompare(int page = 1)
        {
            try
            {
                await _service.DeleteAllNewSchoolAsync();
                // 3.1. Fetch data from the external API, get newSchools
                var newSchools = await _service.FetchSchoolsFromApiAsync(page);

                // 3.2. Compare with OldSchools (populate SubField, isDifferentObj, isNewObj)
                var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);

                // 3.3. Save to the NewSchools table
                await _service.SaveNewSchoolsAsync(comparedList);

                // 3.4. Return the result
                return Ok(comparedList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 4) Apply changes from the NewSchool list to OldSchools:
        ///    POST: api/RSPO/old-schools/apply-changes
        /// </summary>
        [HttpPost("old-schools/apply-changes")]
        public async Task<IActionResult> ApplyChanges([FromBody] List<NewSchool>? newSchools)
        {
            try
            {
                if (newSchools is null || newSchools.Count == 0)
                {
                    return BadRequest("No data received.");
                }
                if (newSchools[0].RspoNumer == "string")
                {
                    return BadRequest("No data received.");
                }
                // Apply changes to OldSchools (insert/update)
                await _service.ApplyChangesFromNewSchoolsAsync(newSchools);

                return Ok("Changes applied successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// NEW method: Save data from the NewSchool list to OldSchools,
        /// then set the field Nazwa = '1' (for each school).
        /// POST: api/RSPO/set-oldschool-nazwa-for-testing
        /// </summary>
        [HttpPost("set-oldschool-nazwa-for-testing")]
        public async Task<IActionResult> SetOldSchoolNazwaForTesting([FromBody] List<NewSchool>? newSchools)
        {
            try
            {
                if (newSchools is null || newSchools.Count == 0)
                {
                    return BadRequest("No data received.");
                }

                // Call a new method in the interface/repository
                // that saves (insert/update) and sets Nazwa='1'
                await _service.SetOldSchoolForTestingAsync();

                return Ok("OldSchools saved (Nazwa set to '1').");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 5) Retrieve the change history for a given RspoNumer.
        /// GET: api/RSPO/history/{rspoNumer}
        /// </summary>
        [HttpGet("history/{rspoNumer}")]
        public async Task<IActionResult> GetHistory(string rspoNumer)
        {
            try
            {
                var history = await _service.GetHistoryByRspoAsync(rspoNumer);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 6) Get filtered records from oldschools.
        /// GET: api/RSPO/old-school/filters
        /// </summary>
        [HttpGet("old-school/filters")]
        public async Task<IActionResult> GetOldSchoolsByFilters([FromQuery] FiltersDTO filters)
        {
            try
            {
                var oldSchools = (await _service.GetAllOldSchoolsAsync()).ToList();
                var filterProperties = typeof(FiltersDTO).GetProperties();
                var filteredSchools = new List<OldSchool>();

                foreach (var property in filterProperties)
                {
                    if (property.GetValue(filters) is null)
                        continue;

                    var desiredValue = property.GetValue(filters);

                    filteredSchools = oldSchools.Where(o => Equals(o.GetType().GetProperty(property.Name)?.GetValue(o), desiredValue)).ToList();
                }

                return Ok(filteredSchools);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }
    }
}