using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function.Report
{
    public class DashboardAnalytics
    {
        // Thông số tổng hợp (Card Overview)
        public decimal TotalRevenueAllTime { get; set; }
        public decimal TotalProfitAllTime { get; set; }

        // Dữ liệu cho 3 biểu đồ
        public RevenueProfitTrend? TrendChart { get; set; }
        public List<TopProductProfit> TopProducts { get; set; } = new List<TopProductProfit>();
        public List<CategoryShare> CategoryShares { get; set; } = new List<CategoryShare>();
    }
}
