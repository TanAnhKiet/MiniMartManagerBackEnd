using BackEnd.Core.SeedWorks;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;

namespace MiniMartManagementAPI.Service.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportOrdersToExcel(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.OrderRepository.GetQueryable()
                .Include(o => o.Employee)
                .AsQueryable();

            if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate.Value);

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders Report");
                worksheet.Cells[1, 1].Value = "Mã Hóa Đơn";
                worksheet.Cells[1, 2].Value = "Ngày Tạo";
                worksheet.Cells[1, 3].Value = "Nhân Viên";
                worksheet.Cells[1, 4].Value = "Tổng Tiền";
                worksheet.Cells[1, 5].Value = "Thành Tiền";
                worksheet.Cells[1, 6].Value = "Phương Thức";
                worksheet.Cells[1, 7].Value = "Trạng Thái";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                for (int i = 0; i < orders.Count; i++)
                {
                    var order = orders[i];
                    worksheet.Cells[i + 2, 1].Value = order.OrderCode;
                    worksheet.Cells[i + 2, 2].Value = order.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[i + 2, 3].Value = order.Employee?.FullName ?? "N/A";
                    worksheet.Cells[i + 2, 4].Value = order.TotalAmount;
                    worksheet.Cells[i + 2, 5].Value = order.FinalAmount;
                    worksheet.Cells[i + 2, 6].Value = order.PaymentMethod;
                    worksheet.Cells[i + 2, 7].Value = order.Status.ToString();
                }
                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public async Task<object> GetRevenueStats(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.OrderRepository.GetQueryable().Where(o => o.Status == BackEnd.Core.Domain.Entities.OrderStatus.Completed);
            if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate.Value);

            var totalRevenue = await query.SumAsync(o => o.FinalAmount);
            var orderCount = await query.CountAsync();

            return new { TotalRevenue = totalRevenue, OrderCount = orderCount };
        }
    }
}
