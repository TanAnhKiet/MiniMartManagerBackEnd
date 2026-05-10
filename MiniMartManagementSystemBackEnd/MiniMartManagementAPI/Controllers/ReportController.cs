using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMartManagementAPI.Service.Report;

namespace MiniMartManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "RootAdmin, Manager")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("ExportOrdersToExcel")]
        public async Task<IActionResult> ExportOrdersToExcel([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var fileBytes = await _reportService.ExportOrdersToExcel(fromDate, toDate);
            string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("GetRevenueStats")]
        public async Task<IActionResult> GetRevenueStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            return Ok(await _reportService.GetRevenueStats(fromDate, toDate));
        }
    }
}
