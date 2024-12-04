using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using schools_web_api.Model;

namespace RSPOApiIntegration.Controllers
{
    /// <summary>
    /// Контроллер для работы с RSPO API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RSPOController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public RSPOController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Получить список школ из RSPO API.
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1).</param>
        /// <param name="count">Количество записей (максимум 100).</param>
        /// <returns>Список школ из RSPO API.</returns>
        [HttpGet("schools")]
        public async Task<IActionResult> GetSchools([FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            if (count > 100) count = 100;

            try
            {
                var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={page}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Ошибка при обращении к API RSPO.");
                }

                var content = await response.Content.ReadAsStringAsync();

                // Преобразуем JSON: заменяем "longitude" на "longtitude"
                var schools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);

                return Ok(new
                {
                    Page = page,
                    Count = count,
                    Results = schools
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
    }
}
