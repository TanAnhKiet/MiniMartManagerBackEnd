using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function.Report
{
    public class CategoryShare
    {
        public string? CategoryName { get; set; }
        public decimal TotalRevenue { get; set; }
        public double Percentage { get; set; } // Phần trăm (để hiển thị tooltip)
    }
}
