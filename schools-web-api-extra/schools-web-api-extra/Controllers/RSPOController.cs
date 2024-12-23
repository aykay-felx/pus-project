using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace RSPOApiIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSPOController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ISchoolService _service;
        public RSPOController(IHttpClientFactory httpClientFactory, ISchoolService service)
        {
            _httpClient = httpClientFactory.CreateClient();
            _service = service;
        }

        [HttpGet("Get and Compare Schools ")]
        public async Task<IActionResult> GetSchools([FromQuery] int page = 1)
        {
            try
            {
                var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={page}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error while accessing RSPO's Api.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var newSchools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);

                // Исправлено здесь
                var updatedSchools = (await _service.CompareSchoolsAsync(newSchools)).ToList();

                await _service.SaveNewSchoolsAsync(updatedSchools);
                 List<NewSchool> upd = new List<NewSchool>();
                foreach (var school in updatedSchools)
                {
                   
                    if(school.isDiferentObj == true || school.isNewObj == true)
                    {
                        upd.Add(school);
                    }
                }
                return Ok(new
                {
                    Results = updatedSchools
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }


        [HttpGet("Save old-schools")]
        public async Task<IActionResult> SaveSchools([FromQuery] int page = 1)
        {
            try
            {
                var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={page}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error while accessing RSPO's API.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var schools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);
                await _service.SaveOldSchoolsAsync(schools);
                return Ok(schools);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("old-schools")]
        public async Task<IActionResult> DeleteSchools()
        {
            try
            {
                await _service.DeleteAllOldSchoolsAsync();
                return Ok("All schools deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
