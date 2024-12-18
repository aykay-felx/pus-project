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
        
        [HttpGet("schools")]
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
                var updatedSchools = _service.CompareSchoolsAsync(newSchools);

              //  await _service.SaveNewSchoolsAsync(updatedSchools);

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
