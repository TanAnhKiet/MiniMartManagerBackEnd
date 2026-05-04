using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Function.Inventory;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiniMartManagementAPI.Controllers.Function.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ============================================================
        // SECTION 1: BÁO CÁO TỔNG QUAN TỒN KHO
        // ============================================================

        /// <summary>
        /// Lấy báo cáo tổng quan tồn kho:
        ///   - Tổng giá trị kho (tồn * giá nhập gần nhất)
        ///   - Danh sách sản phẩm sắp hết hàng
        ///   - Danh sách sản phẩm tồn kho lâu ngày (dead stock)
        /// GET /api/Inventory/Report?lowStockThreshold=5&deadStockDays=30
        /// </summary>
        [HttpGet("Report")]
        public async Task<IActionResult> GetInventoryReport(
            [FromQuery] int lowStockThreshold = 5,
            [FromQuery] int deadStockDays = 30)
        {
            // Lấy tất cả sản phẩm
            var products = await _unitOfWork.ProductRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(p => p.Category)
                .ToListAsync();

            // Lấy giá nhập gần nhất cho từng sản phẩm
            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice
                );

            // Lấy ngày cuối cùng mỗi sản phẩm được bán
            var orderItems = await _unitOfWork.OrderItemRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.Status == OrderStatus.Completed)
                .ToListAsync();

            var lastSoldMap = orderItems
                .GroupBy(oi => oi.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(oi => oi.Order.CreatedAt)
                );

            // 1. Tổng giá trị kho
            decimal totalInventoryValue = products.Sum(p =>
                p.Quantity * (costMap.TryGetValue(p.Id, out var cp) ? cp : 0));

            // 2. Danh sách sắp hết hàng
            var lowStockProducts = products
                .Where(p => p.Quantity <= lowStockThreshold)
                .Select(p => new LowStockProductDto
                {
                    ProductName = p.Name,
                    CurrentStock = p.Quantity,
                    MinThreshold = lowStockThreshold
                })
                .OrderBy(p => p.CurrentStock)
                .ToList();

            // 3. Sản phẩm tồn kho lâu ngày
            var cutoffDate = DateTime.Now.AddDays(-deadStockDays);
            var deadStockProducts = products
                .Where(p => p.Quantity > 0 &&
                            (!lastSoldMap.ContainsKey(p.Id) || lastSoldMap[p.Id] < cutoffDate))
                .Select(p => new DeadStockProductDto
                {
                    ProductName = p.Name,
                    CurrentStock = p.Quantity,
                    LastSoldDate = lastSoldMap.TryGetValue(p.Id, out var lastSold) ? lastSold : null,
                    DaysInStock = lastSoldMap.TryGetValue(p.Id, out var ls)
                        ? (int)(DateTime.Now - ls).TotalDays
                        : (int)(DateTime.Now - p.CreatedAt).TotalDays
                })
                .OrderByDescending(p => p.DaysInStock)
                .ToList();

            var result = new InventoryReport
            {
                TotalInventoryValue = totalInventoryValue,
                LowStockProducts = lowStockProducts,
                DeadStockProducts = deadStockProducts
            };

            return Ok(result);
        }

        // ============================================================
        // SECTION 2: DANH SÁCH SẢN PHẨM TỒN KHO (có lọc, phân trang)
        // ============================================================

        /// <summary>
        /// Lấy danh sách sản phẩm tồn kho có phân trang và lọc
        /// GET /api/Inventory/Products?pageIndex=1&pageSize=20&categoryId=xxx&lowStock=true
        /// </summary>
        [HttpGet("Products")]
        public async Task<IActionResult> GetInventoryProducts(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] bool? lowStock = null,
            [FromQuery] int lowStockThreshold = 5,
            [FromQuery] string? search = null)
        {
            var query = _unitOfWork.ProductRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            // Lọc theo danh mục
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Lọc sản phẩm sắp hết hàng
            if (lowStock.HasValue && lowStock.Value)
                query = query.Where(p => p.Quantity <= lowStockThreshold);

            // Tìm kiếm theo tên hoặc barcode
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Barcode.Contains(search));

            int totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy giá nhập gần nhất
            var productIds = products.Select(p => p.Id).ToList();
            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(ii => productIds.Contains(ii.ProductId))
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice
                );

            var items = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Barcode,
                p.Quantity,
                p.SellPrice,
                CostPrice = costMap.TryGetValue(p.Id, out var cp) ? cp : 0,
                StockValue = p.Quantity * (costMap.TryGetValue(p.Id, out var cp2) ? cp2 : 0),
                CategoryName = p.Category?.Name ?? "N/A",
                p.ExpiryDate,
                p.CreatedAt,
                IsLowStock = p.Quantity <= lowStockThreshold
            });

            return Ok(new
            {
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = items
            });
        }

        // ============================================================
        // SECTION 3: CHI TIẾT SẢN PHẨM TỒN KHO
        // ============================================================

        /// <summary>
        /// Lấy chi tiết tồn kho của một sản phẩm (lịch sử nhập, lịch sử bán)
        /// GET /api/Inventory/Products/{id}
        /// </summary>
        [HttpGet("Products/{id}")]
        public async Task<IActionResult> GetProductInventoryDetail(Guid id)
        {
            var product = await _unitOfWork.ProductRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { Message = $"Không tìm thấy sản phẩm với ID: {id}" });

            // Lịch sử nhập hàng
            var importHistory = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(ii => ii.Import)
                .Where(ii => ii.ProductId == id)
                .OrderByDescending(ii => ii.Import.CreatedAt)
                .Select(ii => new
                {
                    ii.Import.ImportCode,
                    ii.Import.CreatedAt,
                    ii.Quantity,
                    ii.CostPrice,
                    SubTotal = ii.Quantity * ii.CostPrice,
                    ii.ExpiryDate
                })
                .ToListAsync();

            // Lịch sử bán hàng
            var saleHistory = await _unitOfWork.OrderItemRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == id && oi.Order.Status == OrderStatus.Completed)
                .OrderByDescending(oi => oi.Order.CreatedAt)
                .Select(oi => new
                {
                    oi.Order.OrderCode,
                    oi.Order.CreatedAt,
                    oi.Quantity,
                    UnitPrice = oi.Price,
                    SubTotal = oi.Total
                })
                .ToListAsync();

            // Giá nhập gần nhất
            decimal latestCostPrice = importHistory.FirstOrDefault()?.CostPrice ?? 0;
            int totalImported = importHistory.Sum(i => i.Quantity);
            int totalSold = saleHistory.Sum(s => s.Quantity);

            var result = new
            {
                product.Id,
                product.Name,
                product.Barcode,
                product.Quantity,
                product.SellPrice,
                LatestCostPrice = latestCostPrice,
                StockValue = product.Quantity * latestCostPrice,
                CategoryName = product.Category?.Name ?? "N/A",
                product.ExpiryDate,
                product.CreatedAt,
                TotalImported = totalImported,
                TotalSold = totalSold,
                ImportHistory = importHistory,
                SaleHistory = saleHistory
            };

            return Ok(result);
        }

        // ============================================================
        // SECTION 4: THỐNG KÊ NHANH TỒN KHO
        // ============================================================

        /// <summary>
        /// Lấy thống kê nhanh tổng quan tồn kho (dùng cho dashboard cards)
        /// GET /api/Inventory/Summary
        /// </summary>
        [HttpGet("Summary")]
        public async Task<IActionResult> GetInventorySummary([FromQuery] int lowStockThreshold = 5)
        {
            var products = await _unitOfWork.ProductRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice
                );

            var result = new
            {
                TotalProducts = products.Count,
                TotalStockUnits = products.Sum(p => p.Quantity),
                TotalInventoryValue = products.Sum(p =>
                    p.Quantity * (costMap.TryGetValue(p.Id, out var cp) ? cp : 0)),
                OutOfStockCount = products.Count(p => p.Quantity == 0),
                LowStockCount = products.Count(p => p.Quantity > 0 && p.Quantity <= lowStockThreshold),
                LowStockThreshold = lowStockThreshold
            };

            return Ok(result);
        }
    }
}
