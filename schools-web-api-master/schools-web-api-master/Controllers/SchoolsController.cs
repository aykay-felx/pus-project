using schools_web_api.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.TransmitModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace schools_web_api.TokenManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchoolsController : ControllerBase
    {
        private readonly ILogger<SchoolsController> logger;
        private readonly ISchoolService schoolService;

        public SchoolsController(ILogger<SchoolsController> logger, ISchoolService context)
        {
            this.schoolService = context;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchoolsAsync([FromQuery] SchoolRequestParameters body)
        {
            var schools = await this.schoolService.GetSchoolsAsync(body);

            if (schools == null) 
            { 
                return StatusCode(500);
            }

            if (schools.Count == 0) 
            { 
                return NotFound(); 
            }

            return Ok(schools);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchoolAsync(int id)
        {
            var school = await this.schoolService.GetSchoolByIdAsync(id);

            if (school == null) 
            { 
                return NotFound(); 
            }

            return Ok(school);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteSchoolAsync(int id)
        {
            var school = await this.schoolService.GetSchoolByIdAsync(id);

            if (school == null) 
            { 
                return NotFound();
            }

            var result = await this.schoolService.DeleteSchoolAsync(id);
            
            return result ? Ok() : BadRequest();
        }

        [HttpPost("Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateSchoolAsync([FromBody] FullSchool newData)
        {
            var oldData = await this.schoolService.GetSchoolByIdAsync((int)newData.Id);

            if (oldData == null) 
            {
                return NotFound(); 
            }

            var editedSchool = await this.schoolService.UpdateSchoolAsync(oldData, newData);

            return editedSchool != null ? Ok(editedSchool) : BadRequest();
        }

        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddSchoolAsync([FromBody] FullSchool schoolJson)
        {
            var result = await this.schoolService.AddSchoolAsync(schoolJson);

            return result ? Ok() : BadRequest();
        }
    }
}
