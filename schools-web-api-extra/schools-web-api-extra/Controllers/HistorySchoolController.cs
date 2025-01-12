using Microsoft.AspNetCore.Mvc;
using schools_web_api_extra.Interface;

namespace schools_web_api_extra.Controllers;

public class HistorySchoolController : ControllerBase
{
    private readonly IHistoryService _service;

    public HistorySchoolController(IHistoryService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Retrieve the change history for a given RspoNumer.
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