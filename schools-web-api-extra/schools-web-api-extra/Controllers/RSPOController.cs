using Microsoft.AspNetCore.Mvc;
using schools_web_api.Model;
using schools_web_api.TokenManager;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.TransmitModels;
using System.Diagnostics.CodeAnalysis;
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
        private readonly ISchoolService _schoolService;

        public RSPOController(IHttpClientFactory httpClientFactory,  ISchoolService schoolService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _schoolService = schoolService;
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
                var schools = JsonSerializer.Deserialize<List<object>>(content);

               
               
                SchoolRequestParameters body = new SchoolRequestParameters();
                var school = await _schoolService.GetSchoolsAsync(body);
                foreach(var oldschool in school)
                {
                   /* if (FullSchoolExtensions.isDifferentThan(oldschool, schools))
                    {

                    }*/
                }



                return Ok(new
                {
                    Page = page,
                    Count = count,
                    Results = schools.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"������ �������: {ex.Message}");
            }
        }
    }
}
