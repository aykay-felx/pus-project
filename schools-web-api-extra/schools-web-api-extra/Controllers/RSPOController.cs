using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;

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
                var updatedSchools = await _service.CompareSchoolsAsync(newSchools); // ожидаем результат асинхронной операции

                // Преобразуем IEnumerable в List
                var updatedSchoolsList = updatedSchools.ToList();

                await _service.SaveNewSchoolsAsync(updatedSchoolsList); // передаем List<NewSchool>

                return Ok(new
                {
                    Results = updatedSchoolsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred: {ex.Message}");
            }
        }

        [HttpGet("old-schools")]
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
                _service.SaveOldSchoolsAsync(schools);
                return Ok("Schools saved successfully");
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
                _service.DeleteAllOldSchoolsAsync();
                return Ok("All schools deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
