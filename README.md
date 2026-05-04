# MiniMart Management System - BackEnd

Hệ thống quản lý cửa hàng tiện lợi MiniMart (Phần Backend) được xây dựng trên nền tảng .NET hiện đại, hỗ trợ quản lý toàn diện các hoạt động vận hành của cửa hàng.

## 🚀 Giới thiệu chung
Project này cung cấp các API mạnh mẽ để quản lý kho hàng, nhân viên, nhà cung cấp, và quy trình bán hàng. Hệ thống tích hợp các giải pháp thanh toán điện tử hiện đại và hỗ trợ cập nhật dữ liệu thời gian thực.

## ✨ Chức năng chính
- **Quản lý Kho hàng**: Theo dõi tồn kho, danh mục sản phẩm, quản lý mã vạch (barcode) và hạn sử dụng.
- **Quản lý Bán hàng (POS)**: Xử lý đơn hàng, tính toán tiền hàng, chiết khấu và quản lý trạng thái đơn hàng.
- **Thanh toán Trực tuyến**: Tích hợp cổng thanh toán VNPay (hỗ trợ mở rộng MoMo, ZaloPay).
- **Quản lý Nhập hàng**: Quản lý nhà cung cấp, hóa đơn nhập kho và cập nhật tồn kho tự động.
- **Hệ thống Phân quyền**: Quản lý nhân viên theo vai trò (RootAdmin, Manager, Staff) bằng ASP.NET Core Identity.
- **Thông báo thời gian thực**: Sử dụng SignalR để cập nhật trạng thái kho và thanh toán ngay lập tức cho Frontend.

## 🛠 Công nghệ áp dụng
- **Ngôn ngữ**: C# / .NET 8
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core (SQL Server)
- **Bảo mật**: JWT Authentication & Role-based Authorization
- **Real-time**: SignalR
- **Thư viện hỗ trợ**: AutoMapper, Newtonsoft.Json, Microsoft Identity
- **Thanh toán**: VNPay Integration

## 📂 Cấu trúc thư mục
```text
MiniMartManagementSystem/BackEnd/
├── MiniMartManagementSystemBackEnd/
│   ├── MiniMartManagementAPI/      # Dự án API chính (Controllers, Services, Hubs)
│   ├── BackEnd.Core/               # Lớp Domain (Entities, DTOs, Interfaces, Constants)
│   ├── BackEnd.Data/               # Lớp Data Access (DbContext, Repositories, Migrations, Seeding)
│   └── MiniMartManagementSystemBackEnd.slnx  # File Solution
├── MockData.sql                    # Script SQL tạo dữ liệu mẫu cho database
└── README.md                       # Tài liệu hướng dẫn project
```

## ⚙️ Hướng dẫn vận hành

### 1. Yêu cầu hệ thống
- .NET 8 SDK
- SQL Server (LocalDB hoặc SQLEXPRESS)
- Visual Studio 2022 hoặc VS Code

### 2. Cấu hình Database
- Mở file `MiniMartManagementAPI/appsettings.json` (hoặc `appsettings.Development.json`).
- Cập nhật chuỗi kết nối `DefaultConnection` phù hợp với Server SQL của bạn:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MiniMartManagementDB;Integrated Security=True;..."
  }
  ```

### 3. Khởi tạo dữ liệu
Hệ thống sẽ tự động tạo Database và Seed dữ liệu cơ bản (User admin, Roles, Store) khi bạn chạy ứng dụng lần đầu.
Nếu muốn nạp thêm dữ liệu mẫu phong phú hơn (Sản phẩm, Nhà cung cấp, Đơn hàng), hãy thực thi file `MockData.sql` trong SQL Server Management Studio.

### 4. Chạy ứng dụng
Sử dụng terminal tại thư mục dự án:
```bash
dotnet restore
dotnet run --project MiniMartManagementAPI
```
Hoặc nhấn **F5** trong Visual Studio. 
Truy cập `https://localhost:7289/swagger` để xem tài liệu API.

## 💳 Cấu hình VNPay
Các thông tin cấu hình VNPay (TmnCode, HashSecret) hiện đã được chuyển vào quản lý trong Database (bảng `StorePaymentAccounts`) để linh hoạt cho từng chi nhánh. Bạn có thể thay đổi trực tiếp trong DB hoặc thông qua trang Settings trên UI.

---
© 2026 Tan Anh Kiet - MiniMart Management System Project.
