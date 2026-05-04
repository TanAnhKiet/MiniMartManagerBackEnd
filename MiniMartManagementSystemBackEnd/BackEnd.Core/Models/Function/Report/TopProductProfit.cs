using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function.Report
{
    public class TopProductProfit
    {
        public string? ProductName { get; set; }
        public decimal TotalProfit { get; set; } // (Giá bán - Giá nhập) * Tổng số lượng bán
        public int TotalQuantitySold { get; set; } // Hiển thị thêm để biết bán được bao nhiêu
    }
}
