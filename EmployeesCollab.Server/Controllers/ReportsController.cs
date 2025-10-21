using Microsoft.AspNetCore.Mvc;
using EmployeesCollab.Contracts;
using EmployeesCollab.Models;

namespace EmployeesCollab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] FileDataDTO input)
        {
            try
            {
                if (input.FileContent == null || input.FileContent.Length <= 0)
                {
                    return BadRequest("File is not uploaded.");
                }

                var result = await _reportService.FindPairCollab(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("There was and error, please try again.");
            }
            
        }
    }
}
