using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using schools_web_api_extra.DTOs;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Models;

namespace schools_web_api_extra.Controllers;

[ApiController]
[Route("api/rspo/new-school")]
public class NewSchoolController : ControllerBase
{
    private readonly INewSchoolService _service;

    public NewSchoolController(INewSchoolService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Retrieve all new schools (NewSchools).
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
    /// Fetch data from an external RSPO API (page=?),
    /// and return current progress in real time
    /// GET: api/RSPO/new-schools/fetch?page=1
    /// </summary>
    [HttpGet("new-schools/fetch")]
    public async Task Fetch(CancellationToken cancellationToken)
    {
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";

        try
        {
            await _service.DeleteAllNewSchoolAsync();

            var newSchools = await _service.FetchSchoolsFromApiAsync(
                async (page, progress) =>
                {
                    var progressMessage = new { page, progress };
                    await Response.WriteAsync($"{{ data: {JsonConvert.SerializeObject(progressMessage)} }}");
                    await Response.Body.FlushAsync(cancellationToken);
                }, cancellationToken);

            await _service.SaveNewSchoolsAsync(newSchools);

            await Response.WriteAsync("{ data: {\"message\": \"Fetch complete\"} }", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await Response.WriteAsync($"{{data: {{\"error\": \"{e.Message}\"}}}}");
            await Response.Body.FlushAsync(cancellationToken);
        }
        finally
        {
            Response.Body.Close();
        }
    }
    
    /// <summary>
    ///    Compare NewSchools with OldSchools, save the result in NewSchools,
    ///    and return the list of NewSchools to the frontend.
    /// GET: api/RSPO/new-schools/compare
    /// </summary>
    [HttpGet("new-schools/compare")]
    public async Task<IActionResult> Compare()
    {
        try
        {
            var newSchools = (await _service.GetAllNewSchoolAsync()).ToList();

            await _service.DeleteAllNewSchoolAsync();

            var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);

            await _service.SaveNewSchoolsAsync(comparedList);

            return Ok(comparedList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }

    /// <summary>
    /// Fetch data from an external RSPO API (page=?),
    ///    compare it with OldSchools, save the result in NewSchools,
    ///    and return the list of NewSchools to the frontend.
    /// GET: api/RSPO/new-schools/fetch-and-compare?page=1
    /// </summary>
    // [HttpGet("new-schools/fetch-and-compare")]
    // public async Task<IActionResult> FetchAndCompare()
    // {
    //     try
    //     {
    //         await _service.DeleteAllNewSchoolAsync();
    //         // 3.1. Fetch data from the external API, get newSchools
    //         var newSchools = await _service.FetchSchoolsFromApiAsync();

    //         // 3.2. Compare with OldSchools (populate SubField, isDifferentObj, isNewObj)
    //         var comparedList = await _service.CompareWithOldSchoolsAsync(newSchools);

    //         // 3.3. Save to the NewSchools table
    //         await _service.SaveNewSchoolsAsync(comparedList);

    //         // 3.4. Return the result
    //         return Ok(comparedList);
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"Error occured: {ex.Message}");
    //     }
    // }
    
    /// <summary>
    /// Get filtered records from newschools.
    /// GET: api/RSPO/new-school/filters
    /// </summary>
    [HttpGet("new-schools/filters")]
    public async Task<IActionResult> GetOldSchoolsByFilters([FromQuery] FiltersDTO filters)
    {
        try
        {
            var newSchools = (await _service.GetAllNewSchoolAsync()).ToList();
            var filterProperties = typeof(FiltersDTO).GetProperties();
            var filteredSchools = new List<NewSchool>();

            foreach (var property in filterProperties)
            {
                if (property.GetValue(filters) is null)
                    continue;

                var desiredValue = property.GetValue(filters);

                filteredSchools = newSchools.Where(o => Equals(o.GetType().GetProperty(property.Name)?.GetValue(o), desiredValue)).ToList();
            }

            return Ok(filteredSchools);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a newschool record by rsponumer
    /// DELETE: api/RSPO/new-school/{rsponumer} 
    /// </summary>
    /// <param name="rsponumer"></param>
    /// <returns></returns>
    [HttpDelete("new-schools/{rsponumer}")]
    public async Task<IActionResult> DeleteNewSchoolByRspoNumer(string rsponumer)
    {
        try
        {
            await _service.DeleteNewSchoolAsync(rsponumer);
            return Ok($"NewSchool with rsponumer = {rsponumer} has been successfully deleted");
        }
        catch (Exception e)
        {
            return StatusCode(505, $"Error occured: {e.Message}");
        }
    }
}