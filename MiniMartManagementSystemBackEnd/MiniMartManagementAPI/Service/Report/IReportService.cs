namespace MiniMartManagementAPI.Service.Report
{
    public interface IReportService
    {
        Task<byte[]> ExportOrdersToExcel(DateTime? fromDate, DateTime? toDate);
        Task<object> GetRevenueStats(DateTime? fromDate, DateTime? toDate);
    }
}
