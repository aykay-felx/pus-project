using Microsoft.AspNetCore.Mvc;
using schools_web_api.Model;
using schools_web_api.TokenManager;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.TransmitModels;
using schools_web_api_extra.Interface;
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
        private readonly ISchoolService _schoolService;
        private readonly INewService _service;

        public RSPOController(IHttpClientFactory httpClientFactory,  ISchoolService schoolService, INewService service)
        {
            _httpClient = httpClientFactory.CreateClient();
            _schoolService = schoolService;
            _service = service;
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
                // Преобразуем JSON в fullschools
                var schools = JsonConvertToFullSchols.JsongConvertToFullSchools(content);



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
                    Results = schools
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("schoolsBaaza")]
        public async Task<IActionResult> GetSchoolsBaza([FromQuery] SchoolRequestParameters bod)
        {
            var school = await _service.GetSchoolsAsync(bod);

            if (school == null)
            {
                return NotFound();
            }

            return Ok(school);
        }
    }
}
