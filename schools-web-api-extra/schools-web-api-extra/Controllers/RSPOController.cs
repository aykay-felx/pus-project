using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using schools_web_api_extra.Model;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;

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
        private readonly DatabaseHelper _databaseHelper;
        public RSPOController(IHttpClientFactory httpClientFactory, DatabaseHelper databaseHelper)
        {
            _httpClient = httpClientFactory.CreateClient();
            _databaseHelper = databaseHelper;
        }

        /// <summary>
        /// Получить список школ из RSPO API.
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1).</param>

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
                    return StatusCode((int)response.StatusCode, "Ошибка при обращении к API RSPO.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var newSchools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);

                // Сравнение данных
                var updatedSchools = _databaseHelper.CompareSchools(newSchools);

               // _databaseHelper.SaveNewSchools(updatedSchools);

                return Ok(new
                {
                    Results = updatedSchools
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("CreateTableOldSchoolFromPages")]
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
                    return StatusCode((int)response.StatusCode, "Ошибка при обращении к API RSPO.");
                }

                var content = await response.Content.ReadAsStringAsync();
                // Преобразуем JSON в fullschools
                var schools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);
                _databaseHelper.SaveOldSchools(schools); // Вызов метода сохранения
                return Ok("Schools saved successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Пример метода для удаления всех школ
        [HttpDelete("ClearetableAllOldSchools")]
        public async Task<IActionResult> DeleteSchools()
        {
            try
            {
                _databaseHelper.DeleteAllOldSchools(); // Вызов метода удаления
                return Ok("All schools deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
