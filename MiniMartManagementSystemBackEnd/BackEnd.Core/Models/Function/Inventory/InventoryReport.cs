using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function.Inventory
{
    public class InventoryReport
    {
        // 1. Tổng giá trị kho = Sum(Số lượng tồn * Giá nhập gần nhất)
        public decimal TotalInventoryValue { get; set; }

        // 2. Danh sách sắp hết hàng
        public List<LowStockProductDto> LowStockProducts { get; set; } = new List<LowStockProductDto>();

        // 3. Sản phẩm tồn kho lâu ngày (Dead Stock)
        public List<DeadStockProductDto> DeadStockProducts { get; set; } = new List<DeadStockProductDto>();
    }

    public class LowStockProductDto
    {
        public string? ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int MinThreshold { get; set; } // Ngưỡng báo động (ví dụ < 5)
    }

    public class DeadStockProductDto
    {
        public string? ProductName { get; set; }
        public int CurrentStock { get; set; }
        public DateTime? LastSoldDate { get; set; } // Ngày cuối cùng bán được
        public int DaysInStock { get; set; } // Số ngày chưa bán được miếng nào
    }
}
