using Microsoft.AspNetCore.Mvc;
using Npgsql;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace RSPOApiIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSPOController : ControllerBase
    {
        private readonly ISchoolService _service;
        

        public RSPOController(ISchoolService service)
        {
            _service = service;
        }

        /// <summary>
        /// 1) Получить все старые школы (OldSchools).
        /// GET: api/RSPO/oldschools
        /// </summary>
        [HttpGet("oldschools")]
        public async Task<IActionResult> GetAllOldSchools()
        {
            try
            {
                var oldSchools = await _service.GetAllOldSchoolsAsync();
                return Ok(oldSchools);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 2) Удалить одну школу по RspoNumer.
        /// DELETE: api/RSPO/oldschools/{rspoNumer}
        /// </summary>
        [HttpDelete("oldschools/{rspoNumer}")]
        public async Task<IActionResult> DeleteOldSchool(string rspoNumer)
        {
            try
            {
                await _service.DeleteOldSchoolAsync(rspoNumer);
                return Ok($"School with RspoNumer={rspoNumer} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 3) Сходить во внешний API RSPO, взять список школ (page=?), 
        ///    сравнить с OldSchools, сохранить результат в NewSchools, 
        ///    и вернуть список NewSchools пользователю (фронту).
        /// GET: api/RSPO/fetch-and-compare?page=1
        /// </summary>
        [HttpGet("fetch-and-compare")]
        public async Task<IActionResult> FetchAndCompare(int page = 1)
        {
            try
            {
                // 3.1. Сходить в внешний API, получить newSchools
                var newSchools = await _service.FetchSchoolsFromApiAsync(page);

                // 3.2. Сравнить с OldSchools (заполняем SubField, isDifferentObj, isNewObj)
                var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);

                // 3.3. Сохранить в таблицу NewSchools
                await _service.SaveNewSchoolsAsync(comparedList);

                // 3.4. Возвращаем на фронт
                return Ok(comparedList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 4) Применить изменения из списка NewSchool:
        ///    Обычно вызывается, когда пользователь на фронте 
        ///    выбрал, какие поля обновлять/менять.
        ///    POST: api/RSPO/apply-changes
        /// </summary>
        [HttpPost("apply-changes")]
        public async Task<IActionResult> ApplyChanges([FromBody] List<NewSchool> newSchools)
        {
            try
            {
                if (newSchools == null || newSchools.Count == 0)
                {
                    return BadRequest("No data received.");
                }

                // 4.1. Применяем изменения к OldSchools (insert/update)
                await _service.ApplyChangesFromNewSchoolsAsync(newSchools);

                return Ok("Changes applied successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        [HttpGet("history/{rspoNumer}")]
        public async Task<IActionResult> GetHistory(string rspoNumer)
        {
            try
            {
                var history = await _service.GetHistoryByRspoAsync(rspoNumer);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }


    }
}
