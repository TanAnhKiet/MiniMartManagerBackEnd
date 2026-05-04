using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function.Report
{
    public class RevenueProfitTrend
    {
        // Các mốc thời gian trên trục X (ví dụ: "01/05", "02/05"...)
        public List<string> Labels { get; set; } = new List<string>();

        // Tập dữ liệu cho đường Doanh thu
        public List<decimal> RevenueData { get; set; } = new List<decimal>();

        // Tập dữ liệu cho đường Lợi nhuận
        public List<decimal> ProfitData { get; set; } = new List<decimal>();
    }
}
