using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace RSPOApiIntegration.Controllers
{
    /// <summary>
    /// ���������� ��� ������ � RSPO API.
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
        /// �������� ������ ���� �� RSPO API.
        /// </summary>
        /// <param name="page">����� �������� (������� � 1).</param>
        /// <param name="count">���������� ������� (�������� 100).</param>
        /// <returns>������ ���� �� RSPO API.</returns>
        [HttpGet("schools")]
        public async Task<IActionResult> GetSchools([FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            if (count > 100) count = 100; // ����������� �� 100 �������, ��� ������� � API RSPO

            try
            {
                var apiUrl = $"https://api-rspo.men.gov.pl/api/placowki/?page={page}";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "������ ��� ��������� � API RSPO.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<object>(content);

                return Ok(new
                {
                    Page = page,
                    Count = count,
                    Results = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"������ �������: {ex.Message}");
            }
        }
    }
}
