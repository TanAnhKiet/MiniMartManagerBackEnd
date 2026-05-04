using System.ComponentModel;

namespace BackEnd.Core.Constants
{
    public static class Permissions
    {
        // Nhóm chức năng Bán hàng
        public static class Sales
        {
            [Description("Tạo hóa đơn")]
            public const string CreateOrder = "Permissions.Sales.CreateOrder";

            [Description("Xử lý hóa đơn")]
            public const string ProcessOrder = "Permissions.Sales.ProcessOrder";

            [Description("Thanh toán")]
            public const string Payment = "Permissions.Sales.Payment";
        }

        // Nhóm chức năng Nhập hàng
        public static class Inventory
        {
            [Description("Cập nhật nhà cung cấp")]
            public const string UpdateSupplier = "Permissions.Inventory.UpdateSupplier";

            [Description("Tạo phiếu nhập")]
            public const string CreateImportTicket = "Permissions.Inventory.CreateImportTicket";

            [Description("Tạo lô hàng")]
            public const string CreateBatch = "Permissions.Inventory.CreateBatch";
        }

        // Nhóm chức năng Thống kê
        public static class Statistics
        {
            [Description("Thống kê sản phẩm bán chạy")]
            public const string BestSellers = "Permissions.Statistics.BestSellers";

            [Description("Thống kê tồn kho")]
            public const string InventoryStock = "Permissions.Statistics.InventoryStock";

            [Description("Thống kê doanh thu")]
            public const string Revenue = "Permissions.Statistics.Revenue";

            [Description("Thống kê lợi nhuận")]
            public const string Profit = "Permissions.Statistics.Profit";
        }

        // Nhóm chức năng Quản lý
        public static class Management
        {
            [Description("Cập nhật danh mục")]
            public const string UpdateCategory = "Permissions.Management.UpdateCategory";

            [Description("Cập nhật thông tin cửa hàng")]
            public const string UpdateStoreInfo = "Permissions.Management.UpdateStoreInfo";

            [Description("Cập nhật hàng hóa")]
            public const string UpdateProduct = "Permissions.Management.UpdateProduct";

            [Description("Quản lý nhân viên")]
            public const string UpdateEmployee = "Permissions.Management.UpdateEmployee";
        }

        // Nhóm chức năng Hệ thống
        public static class System
        {
            [Description("Cập nhật tài khoản thanh toán")]
            public const string ManagePayment = "Permissions.System.ManagePayment";

            [Description("Xem tài khoản thanh toán")]
            public const string ViewPayment = "Permissions.System.ViewPayment";

            [Description("Quản lý vai trò (Roles)")]
            public const string ManageRoles = "Permissions.System.ManageRoles";
        }

        public static class Actions
        {
            [Description("Thêm mới")]
            public const string Create = "Permissions.Actions.Create";

            [Description("Xem")]
            public const string Read = "Permissions.Actions.Read";

            [Description("Cập nhật")]
            public const string Update = "Permissions.Actions.Update";

            [Description("Xóa")]
            public const string Delete = "Permissions.Actions.Delete";
        }
    }
}