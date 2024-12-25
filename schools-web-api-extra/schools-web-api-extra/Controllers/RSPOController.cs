using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Controllers;

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
    /// 1) �������� ��� ������ ����� (OldSchools).
    /// GET: api/RSPO/oldschools
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
    /// 2) ������� ���� ����� �� RspoNumer.
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
    /// 3) ������� �� ������� API RSPO, ����� ������ ���� (page=?), 
    ///    �������� � OldSchools, ��������� ��������� � NewSchools, 
    ///    � ������� ������ NewSchools ������������ (������).
    /// GET: api/RSPO/fetch-and-compare?page=1
    /// </summary>
    [HttpGet("new-schools/fetch-and-compare")]
    public async Task<IActionResult> FetchAndCompare(int page = 1)
    {
        try
        {
            // 3.1. ������� � ������� API, �������� newSchools
            var newSchools = await _service.FetchSchoolsFromApiAsync(page);

            // 3.2. �������� � OldSchools (��������� SubField, isDifferentObj, isNewObj)
            var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);

            // 3.3. ��������� � ������� NewSchools
            await _service.SaveNewSchoolsAsync(comparedList);

            // 3.4. ���������� �� �����
            return Ok(comparedList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }

    /// <summary>
    /// 4) ��������� ��������� �� ������ NewSchool:
    ///    ������ ����������, ����� ������������ �� ������ 
    ///    ������, ����� ���� ���������/������.
    ///    POST: api/RSPO/apply-changes
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

            // 4.1. ��������� ��������� � OldSchools (insert/update)
            await _service.ApplyChangesFromNewSchoolsAsync(newSchools);

            return Ok("Changes applied successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }
}