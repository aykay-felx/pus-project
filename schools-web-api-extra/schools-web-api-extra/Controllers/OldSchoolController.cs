using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Controllers;

[ApiController]
[Route("api/rspo/old-school")]
public class OldSchoolController : ControllerBase
{
    private readonly IOldSchoolService _oldSchoolService;
    private readonly INewSchoolService _newSchoolService;

    public OldSchoolController(IOldSchoolService oldSchoolService, INewSchoolService newSchoolService)
    {
        _oldSchoolService = oldSchoolService;
        _newSchoolService = newSchoolService;
    }
    
    /// <summary>
    /// Retrieve all old schools (OldSchools).
    /// GET: api/RSPO/old-schools
    /// </summary>
    [HttpGet("old-schools")]
    public async Task<IActionResult> GetAllOldSchools()
    {
        try
        {
            var oldSchools = await _oldSchoolService.GetAllOldSchoolsAsync();
            return Ok(oldSchools);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Delete a single school by RspoNumer.
    /// DELETE: api/RSPO/oldschools/{rspoNumer}
    /// </summary>
    [HttpDelete("oldschools/{rspoNumer}")]
    public async Task<IActionResult> DeleteOldSchool(string rspoNumer)
    {
        try
        {
            await _oldSchoolService.DeleteOldSchoolAsync(rspoNumer);
            return Ok($"School with RspoNumer={rspoNumer} deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Apply changes from the NewSchool list to OldSchools:
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
            if (newSchools[0].RspoNumer == "string")
            {
                return BadRequest("No data received.");
            }
            // Apply changes to OldSchools (insert/update)
            await _oldSchoolService.ApplyChangesFromNewSchoolsAsync(newSchools);

            foreach (var newSchool in newSchools)
            {
                await _newSchoolService.DeleteNewSchoolAsync(newSchool.RspoNumer);
            }

            return Ok("Changes applied successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }
}