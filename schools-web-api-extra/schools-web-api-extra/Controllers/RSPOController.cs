using Microsoft.AspNetCore.Mvc;

namespace RSPOApiIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSPOController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseHelper _databaseHelper;
        public RSPOController(IHttpClientFactory httpClientFactory, DatabaseHelper databaseHelper)
        {
            _httpClient = httpClientFactory.CreateClient();
            _databaseHelper = databaseHelper;
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
                var updatedSchools = _databaseHelper.CompareSchools(newSchools);

               // _databaseHelper.SaveNewSchools(updatedSchools);

                return Ok(new
                {
                    Results = updatedSchools
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"������ �������: {ex.Message}");
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
                _databaseHelper.SaveOldSchools(schools);
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
                _databaseHelper.DeleteAllOldSchools();
                return Ok("All schools deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
