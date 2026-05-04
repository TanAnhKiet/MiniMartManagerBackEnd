namespace BackEnd.Core.Models.Function.Report
{
    // model dùng chung cho 3 báo cáo
    public class SalesReportDto
    {
        public decimal TotalRevenue { get; set; }     // Tổng doanh thu
        public decimal TotalProfit { get; set; }      // Tổng lợi nhuận gộp (Doanh thu - Giá vốn)
        public int TotalInvoices { get; set; }        // Tổng số hóa đơn
        public List<ChartDataDto> RevenueChart { get; set; } = new List<ChartDataDto>();// Dữ liệu cho 3 biểu đồ
    }

    public class ChartDataDto
    {
        public string? Label { get; set; } // Ngày/Tháng/Năm hoặc Tên Sản phẩm
        public decimal Value { get; set; } // Giá trị tiền hoặc số lượng
    }
}
