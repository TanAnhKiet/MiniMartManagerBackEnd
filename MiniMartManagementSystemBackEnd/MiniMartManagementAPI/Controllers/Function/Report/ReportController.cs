using AutoMapper;
using BackEnd.Core.Domain.Entities;
using BackEnd.Core.Models.Function.Report;
using BackEnd.Core.SeedWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiniMartManagementAPI.Controllers.Function.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ============================================================
        // SECTION 1: BÁO CÁO DOANH THU / LỢI NHUẬN
        // ============================================================

        /// <summary>
        /// Báo cáo doanh thu theo ngày (trong khoảng thời gian)
        /// GET /api/Report/Sales/Daily?from=2024-01-01&to=2024-01-31
        /// </summary>
        [HttpGet("Sales/Daily")]
        public async Task<IActionResult> GetDailySalesReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

            var orders = await _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed
                         && o.CreatedAt.Date >= from.Date
                         && o.CreatedAt.Date <= to.Date)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            // Tính tổng giá vốn qua ImportItems (lấy CostPrice gần nhất cho từng sản phẩm)
            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice);

            // Build chart data theo ngày
            var groupByDay = orders
                .GroupBy(o => o.CreatedAt.Date)
                .OrderBy(g => g.Key);

            decimal totalRevenue = 0, totalProfit = 0;
            int totalInvoices = orders.Count;
            var revenueChart = new List<ChartDataDto>();

            foreach (var day in groupByDay)
            {
                decimal dayRevenue = day.Sum(o => o.FinalAmount);
                decimal dayCost = day.SelectMany(o => o.Items)
                    .Sum(oi => (costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0) * oi.Quantity);
                decimal dayProfit = dayRevenue - dayCost;

                totalRevenue += dayRevenue;
                totalProfit += dayProfit;

                revenueChart.Add(new ChartDataDto
                {
                    Label = day.Key.ToString("dd/MM"),
                    Value = dayRevenue
                });
            }

            var result = new SalesReportDto
            {
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                TotalInvoices = totalInvoices,
                RevenueChart = revenueChart
            };

            return Ok(result);
        }

        /// <summary>
        /// Báo cáo doanh thu theo tháng
        /// GET /api/Report/Sales/Monthly?year=2024
        /// </summary>
        [HttpGet("Sales/Monthly")]
        public async Task<IActionResult> GetMonthlySalesReport([FromQuery] int year)
        {
            if (year <= 0) year = DateTime.Now.Year;

            var orders = await _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed
                         && o.CreatedAt.Year == year)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice);

            var groupByMonth = orders
                .GroupBy(o => o.CreatedAt.Month)
                .OrderBy(g => g.Key);

            decimal totalRevenue = 0, totalProfit = 0;
            int totalInvoices = orders.Count;
            var revenueChart = new List<ChartDataDto>();

            foreach (var month in groupByMonth)
            {
                decimal monthRevenue = month.Sum(o => o.FinalAmount);
                decimal monthCost = month.SelectMany(o => o.Items)
                    .Sum(oi => (costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0) * oi.Quantity);
                decimal monthProfit = monthRevenue - monthCost;

                totalRevenue += monthRevenue;
                totalProfit += monthProfit;

                revenueChart.Add(new ChartDataDto
                {
                    Label = $"Tháng {month.Key}",
                    Value = monthRevenue
                });
            }

            var result = new SalesReportDto
            {
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                TotalInvoices = totalInvoices,
                RevenueChart = revenueChart
            };

            return Ok(result);
        }

        /// <summary>
        /// Báo cáo doanh thu theo năm (nhiều năm)
        /// GET /api/Report/Sales/Yearly
        /// </summary>
        [HttpGet("Sales/Yearly")]
        public async Task<IActionResult> GetYearlySalesReport()
        {
            var orders = await _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice);

            var groupByYear = orders
                .GroupBy(o => o.CreatedAt.Year)
                .OrderBy(g => g.Key);

            decimal totalRevenue = 0, totalProfit = 0;
            int totalInvoices = orders.Count;
            var revenueChart = new List<ChartDataDto>();

            foreach (var year in groupByYear)
            {
                decimal yearRevenue = year.Sum(o => o.FinalAmount);
                decimal yearCost = year.SelectMany(o => o.Items)
                    .Sum(oi => (costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0) * oi.Quantity);
                decimal yearProfit = yearRevenue - yearCost;

                totalRevenue += yearRevenue;
                totalProfit += yearProfit;

                revenueChart.Add(new ChartDataDto
                {
                    Label = year.Key.ToString(),
                    Value = yearRevenue
                });
            }

            var result = new SalesReportDto
            {
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                TotalInvoices = totalInvoices,
                RevenueChart = revenueChart
            };

            return Ok(result);
        }

        // ============================================================
        // SECTION 2: DASHBOARD ANALYTICS (Tổng quan tất cả thời gian)
        // ============================================================

        /// <summary>
        /// Lấy dữ liệu Dashboard Analytics tổng hợp
        /// GET /api/Report/Dashboard
        /// </summary>
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboardAnalytics()
        {
            var orders = await _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .ToListAsync();

            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice);

            // Tổng doanh thu và lợi nhuận all-time
            decimal totalRevenue = orders.Sum(o => o.FinalAmount);
            decimal totalCost = orders.SelectMany(o => o.Items)
                .Sum(oi => (costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0) * oi.Quantity);
            decimal totalProfit = totalRevenue - totalCost;

            // Trend chart: 12 tháng gần nhất
            var trendChart = BuildRevenueProfitTrend(orders, costMap);

            // Top 5 sản phẩm lợi nhuận cao nhất
            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => new { oi.ProductId, oi.Product?.Name })
                .Select(g => new TopProductProfit
                {
                    ProductName = g.Key.Name,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalProfit = g.Sum(x =>
                        (x.Price - (costMap.TryGetValue(x.ProductId, out var cp) ? cp : 0)) * x.Quantity)
                })
                .OrderByDescending(x => x.TotalProfit)
                .Take(5)
                .ToList();

            // Tỷ lệ doanh thu theo danh mục
            var categoryRevenues = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.Product?.Category?.Name ?? "Không phân loại")
                .Select(g => new { CategoryName = g.Key, Revenue = g.Sum(x => x.Total) })
                .ToList();

            var categoryShares = categoryRevenues.Select(c => new CategoryShare
            {
                CategoryName = c.CategoryName,
                TotalRevenue = c.Revenue,
                Percentage = totalRevenue > 0 ? (double)(c.Revenue / totalRevenue * 100) : 0
            }).ToList();

            var result = new DashboardAnalytics
            {
                TotalRevenueAllTime = totalRevenue,
                TotalProfitAllTime = totalProfit,
                TrendChart = trendChart,
                TopProducts = topProducts,
                CategoryShares = categoryShares
            };

            return Ok(result);
        }

        // ============================================================
        // SECTION 3: DANH SÁCH & CHI TIẾT HÓA ĐƠN (ORDER)
        // ============================================================

        /// <summary>
        /// Lấy danh sách hóa đơn (có phân trang, lọc theo ngày, trạng thái)
        /// GET /api/Report/Orders?pageIndex=1&pageSize=20&from=2024-01-01&to=2024-12-31&status=Completed
        /// </summary>
        [HttpGet("Orders")]
        public async Task<IActionResult> GetOrderList(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? status = null)
        {
            var query = _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(o => o.Employee)
                .Include(o => o.Store)
                .AsQueryable();

            // Filter theo khoảng thời gian
            if (from.HasValue)
                query = query.Where(o => o.CreatedAt.Date >= from.Value.Date);
            if (to.HasValue)
                query = query.Where(o => o.CreatedAt.Date <= to.Value.Date);

            // Filter theo trạng thái
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);

            int totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = orders.Select(o => new
            {
                o.Id,
                o.OrderCode,
                o.Status,
                o.TotalAmount,
                o.FinalAmount,
                o.PaymentMethod,
                o.CreatedAt,
                EmployeeName = o.Employee?.FullName ?? "N/A",
                StoreName = o.Store?.Name ?? "N/A"
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

        /// <summary>
        /// Lấy chi tiết một hóa đơn theo ID (bao gồm danh sách sản phẩm)
        /// GET /api/Report/Orders/{id}
        /// </summary>
        [HttpGet("Orders/{id}")]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var order = await _unitOfWork.OrderRepository
                .GetQueryable()
                .AsNoTracking()
                .Include(o => o.Employee)
                .Include(o => o.Store)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { Message = $"Không tìm thấy hóa đơn với ID: {id}" });

            // Lấy giá vốn cho từng sản phẩm trong đơn hàng
            var productIds = order.Items.Select(oi => oi.ProductId).Distinct().ToList();
            var importItems = await _unitOfWork.ImportItemRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(ii => productIds.Contains(ii.ProductId))
                .ToListAsync();

            var costMap = importItems
                .GroupBy(ii => ii.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(ii => ii.ExpiryDate).First().CostPrice);

            var orderDetail = new
            {
                order.Id,
                order.OrderCode,
                order.Status,
                order.TotalAmount,
                order.FinalAmount,
                order.PaymentMethod,
                order.CreatedAt,
                EmployeeName = order.Employee?.FullName ?? "N/A",
                StoreName = order.Store?.Name ?? "N/A",
                Items = order.Items.Select(oi => new
                {
                    oi.Id,
                    oi.ProductId,
                    ProductName = oi.Product?.Name ?? "N/A",
                    CategoryName = oi.Product?.Category?.Name ?? "N/A",
                    oi.Quantity,
                    UnitPrice = oi.Price,
                    CostPrice = costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0,
                    Profit = (oi.Price - (costMap.TryGetValue(oi.ProductId, out var cp2) ? cp2 : 0)) * oi.Quantity,
                    SubTotal = oi.Total
                })
            };

            return Ok(orderDetail);
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        private RevenueProfitTrend BuildRevenueProfitTrend(
            List<OrderEntity> orders,
            Dictionary<Guid, decimal> costMap)
        {
            var now = DateTime.Now;
            var trend = new RevenueProfitTrend();

            for (int i = 11; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthOrders = orders
                    .Where(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month)
                    .ToList();

                decimal revenue = monthOrders.Sum(o => o.FinalAmount);
                decimal cost = monthOrders.SelectMany(o => o.Items)
                    .Sum(oi => (costMap.TryGetValue(oi.ProductId, out var cp) ? cp : 0) * oi.Quantity);

                trend.Labels.Add(month.ToString("MM/yyyy"));
                trend.RevenueData.Add(revenue);
                trend.ProfitData.Add(revenue - cost);
            }

            return trend;
        }
    }
}
