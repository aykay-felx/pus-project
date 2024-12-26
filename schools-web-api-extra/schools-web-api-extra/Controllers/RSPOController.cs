using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Controllers
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
        /// GET: api/RSPO/old-schools
        /// </summary>
        [HttpGet("old-schools")]
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
        /// Получить все новые школы (NewSchools).
        /// GET: api/RSPO/new-schools
        /// </summary>
        [HttpGet("new-schools")]
        public async Task<IActionResult> GetAllNewSchools()
        {
            try
            {
                var newSchools = await _service.GetAllNewSchoolAsync();
                return Ok(newSchools);
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
        /// 3) Сходить во внешний API RSPO (page=?), 
        ///    сравнить с OldSchools, сохранить результат в NewSchools,
        ///    и вернуть список NewSchools на фронт.
        /// GET: api/RSPO/new-schools/fetch-and-compare?page=1
        /// </summary>
        [HttpGet("new-schools/fetch-and-compare")]
        public async Task<IActionResult> FetchAndCompare(int page = 1)
        {
            try
            {
                await _service.DeleteAllNewSchoolAsync();
                // 3.1. Сходить в внешний API, получить newSchools
                var newSchools = await _service.FetchSchoolsFromApiAsync(page);

                // 3.2. Сравнить с OldSchools (заполнение SubField, isDifferentObj, isNewObj)
                var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);
               
                // 3.3. Сохранить в таблицу NewSchools
                await _service.SaveNewSchoolsAsync(comparedList);

                // 3.4. Возвращаем результат
                return Ok(comparedList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 4) Применить изменения из списка NewSchool в OldSchools:
        ///    POST: api/RSPO/old-schools/apply-changes
        /// </summary>
        [HttpPost("old-schools/apply-changes")]
        public async Task<IActionResult> ApplyChanges([FromBody] List<NewSchool>? newSchools)
        {
            try
            {
                if (newSchools is null || newSchools.Count == 0)
                {
                    return BadRequest("No data received.");
                }

                // Применяем изменения к OldSchools (insert/update)
                await _service.ApplyChangesFromNewSchoolsAsync(newSchools);

                return Ok("Changes applied successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// НОВЫЙ метод: Сохранить данные из списка NewSchool в OldSchools,
        /// после чего установить поле Nazwa = '1' (для каждой школы).
        /// POST: api/RSPO/save-oldschool-nazwa
        /// </summary>
        [HttpPost("save-oldschool-nazwa")]
        public async Task<IActionResult> SaveOldSchoolNazwa([FromBody] List<NewSchool>? newSchools)
        {
            try
            {
                if (newSchools is null || newSchools.Count == 0)
                {
                    return BadRequest("No data received.");
                }

                // Вызываем новый метод в интерфейсе/репозитории,
                // который сохранит (insert/update) и установит Nazwa='1'
                await _service.SaveOldSchoolFromApplyChangesAsync();

                return Ok("OldSchools saved (Nazwa set to '1').");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occured: {ex.Message}");
            }
        }

        /// <summary>
        /// 5) Получить историю изменений для заданного RspoNumer.
        /// GET: api/RSPO/history/{rspoNumer}
        /// </summary>
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
