-- =================================================================================
-- MOCK DATA SCRIPT FOR MINIMART MANAGEMENT SYSTEM
-- =================================================================================
-- Script này tự động lấy StoreId và RoleId đã được seed sẵn bởi hệ thống
-- để chèn các dữ liệu mẫu (mỗi bảng từ 5-10 dòng)
-- Mật khẩu mặc định cho tất cả user được tạo là: Admin@123
-- =================================================================================

-- Khai báo biến để sử dụng các dữ liệu đã seed
DECLARE @StoreId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Stores);
IF @StoreId IS NULL
BEGIN
    SET @StoreId = NEWID();
    INSERT INTO Stores (Id, Name, Address, Phone, Email, CreatedAt) 
    VALUES (@StoreId, N'Chut chut Store', N'Hà Nội', '0123456789', 'ChutChut99@gmail.com', GETUTCDATE());
END

DECLARE @RoleManager UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'Manager');
IF @RoleManager IS NULL
BEGIN
    SET @RoleManager = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, DisplayName) 
    VALUES (@RoleManager, 'Manager', 'MANAGER', N'Quản lý');
END

DECLARE @RoleStaff UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'Staff');
IF @RoleStaff IS NULL
BEGIN
    SET @RoleStaff = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, DisplayName) 
    VALUES (@RoleStaff, 'Staff', 'STAFF', N'Nhân viên');
END

DECLARE @RoleSysEng UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'SystemEngineer');
IF @RoleSysEng IS NULL
BEGIN
    SET @RoleSysEng = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, DisplayName) 
    VALUES (@RoleSysEng, 'SystemEngineer', 'SYSTEMENGINEER', N'Kỹ sư hệ thống');
END

-- Dummy Password Hash cho "Admin@123"
DECLARE @DefaultPasswordHash NVARCHAR(MAX) = 'AQAAAAEAACcQAAAAEFbU/sQ4Yh9nU7e8P+Nn9Xb3h0o1Jg+kXzGkHk8sFzNf6q9m6qVfG7X5L/vPz2q+Hg==';

-- =================================================================================
-- 1. BẢNG: AppUser
-- =================================================================================
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, IsActive, CreatedAt, RefreshTokenExpiryTime, FullName)
VALUES 
('11111111-0000-0000-0000-000000000001', 'manager01', 'MANAGER01', 'manager01@minimart.vn', 'MANAGER01@MINIMART.VN', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), N'Nguyễn Văn Quản Lý'),
('11111111-0000-0000-0000-000000000002', 'staff01', 'STAFF01', 'staff01@minimart.vn', 'STAFF01@MINIMART.VN', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), N'Trần Thị Nhân Viên 1'),
('11111111-0000-0000-0000-000000000003', 'staff02', 'STAFF02', 'staff02@minimart.vn', 'STAFF02@MINIMART.VN', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), N'Lê Minh Nhân Viên 2'),
('11111111-0000-0000-0000-000000000004', 'staff03', 'STAFF03', 'staff03@minimart.vn', 'STAFF03@MINIMART.VN', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), N'Phạm Thị Nhân Viên 3'),
('11111111-0000-0000-0000-000000000005', 'staff04', 'STAFF04', 'staff04@minimart.vn', 'STAFF04@MINIMART.VN', 1, @DefaultPasswordHash, NEWID(), NEWID(), 0, 0, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), N'Hoàng Văn Nhân Viên 4');

-- =================================================================================
-- 2. BẢNG: AppUserRole
-- =================================================================================
INSERT INTO AppUserRole (UserId, RoleId)
VALUES 
('11111111-0000-0000-0000-000000000001', @RoleManager),
('11111111-0000-0000-0000-000000000002', @RoleStaff),
('11111111-0000-0000-0000-000000000003', @RoleStaff),
('11111111-0000-0000-0000-000000000004', @RoleStaff),
('11111111-0000-0000-0000-000000000005', @RoleStaff);

-- =================================================================================
-- 3. BẢNG: Employee
-- =================================================================================
INSERT INTO Employee (Id, EmployeeCode, FullName, Address, PhoneNumber, DateOfBirth, Position, IsActive, AppUserId, StoreId, CreatedAt)
VALUES 
('22222222-0000-0000-0000-000000000001', 'NV002', N'Nguyễn Văn Quản Lý', N'Hà Nội', '0912345678', '1990-05-10', N'Quản lý', 1, '11111111-0000-0000-0000-000000000001', @StoreId, GETUTCDATE()),
('22222222-0000-0000-0000-000000000002', 'NV003', N'Trần Thị Nhân Viên 1', N'Hà Nội', '0923456789', '1995-08-15', N'Nhân viên bán hàng', 1, '11111111-0000-0000-0000-000000000002', @StoreId, GETUTCDATE()),
('22222222-0000-0000-0000-000000000003', 'NV004', N'Lê Minh Nhân Viên 2', N'Hà Nội', '0934567890', '1998-11-20', N'Nhân viên bán hàng', 1, '11111111-0000-0000-0000-000000000003', @StoreId, GETUTCDATE()),
('22222222-0000-0000-0000-000000000004', 'NV005', N'Phạm Thị Nhân Viên 3', N'Hà Nội', '0945678901', '1996-02-25', N'Nhân viên kho', 1, '11111111-0000-0000-0000-000000000004', @StoreId, GETUTCDATE()),
('22222222-0000-0000-0000-000000000005', 'NV006', N'Hoàng Văn Nhân Viên 4', N'Hà Nội', '0956789012', '1999-07-30', N'Nhân viên bán hàng', 1, '11111111-0000-0000-0000-000000000005', @StoreId, GETUTCDATE());

-- =================================================================================
-- 4. BẢNG: Suppliers
-- =================================================================================
INSERT INTO Suppliers (Id, Name, Phone, Address, CreatedAt)
VALUES
('33333333-0000-0000-0000-000000000001', N'Công ty CP Sữa Vinamilk', '0241234567', N'10 Tân Trào, Quận 7, TP.HCM', GETUTCDATE()),
('33333333-0000-0000-0000-000000000002', N'Công ty Nước Giải Khát Coca-Cola', '0242345678', N'KCN Biên Hòa, Đồng Nai', GETUTCDATE()),
('33333333-0000-0000-0000-000000000003', N'Công ty CP Bánh Kẹo Hải Châu', '0243456789', N'15 Mạc Thị Bưởi, Hà Nội', GETUTCDATE()),
('33333333-0000-0000-0000-000000000004', N'Công ty TNHH Unilever Việt Nam', '0244567890', N'156 Nguyễn Lương Bằng, Quận 7, TP.HCM', GETUTCDATE()),
('33333333-0000-0000-0000-000000000005', N'Công ty TNHH Dầu Thực Vật Cái Lân', '0245678901', N'Cái Lân, Hạ Long, Quảng Ninh', GETUTCDATE());

-- =================================================================================
-- 5. BẢNG: Category
-- =================================================================================
INSERT INTO Category (Id, Name, CreatedAt)
VALUES
('44444444-0000-0000-0000-000000000001', N'Sữa & Chế phẩm từ sữa', GETUTCDATE()),
('44444444-0000-0000-0000-000000000002', N'Đồ uống & Nước giải khát', GETUTCDATE()),
('44444444-0000-0000-0000-000000000003', N'Bánh kẹo', GETUTCDATE()),
('44444444-0000-0000-0000-000000000004', N'Chăm sóc cá nhân', GETUTCDATE()),
('44444444-0000-0000-0000-000000000005', N'Gia vị & Thực phẩm khô', GETUTCDATE());

-- =================================================================================
-- 6. BẢNG: Products
-- =================================================================================
INSERT INTO Products (Id, StoreId, Name, Barcode, CategoryId, SellPrice, Quantity, ExpiryDate, CreatedAt)
VALUES
('55555555-0000-0000-0000-000000000001', @StoreId, N'Sữa Tươi Vinamilk 1L', '8934563102001', '44444444-0000-0000-0000-000000000001', 35000, 100, '2026-01-01', GETUTCDATE()),
('55555555-0000-0000-0000-000000000002', @StoreId, N'Nước Ngọt Coca-Cola 330ml', '8934563102002', '44444444-0000-0000-0000-000000000002', 12000, 250, '2027-01-01', GETUTCDATE()),
('55555555-0000-0000-0000-000000000003', @StoreId, N'Bánh Quy ChocoPie', '8934563102003', '44444444-0000-0000-0000-000000000003', 55000, 80, '2025-10-01', GETUTCDATE()),
('55555555-0000-0000-0000-000000000004', @StoreId, N'Dầu Gội Clear Men 170g', '8934563102004', '44444444-0000-0000-0000-000000000004', 62000, 45, '2027-12-01', GETUTCDATE()),
('55555555-0000-0000-0000-000000000005', @StoreId, N'Dầu Ăn Neptune 1L', '8934563102005', '44444444-0000-0000-0000-000000000005', 48000, 120, '2026-06-01', GETUTCDATE());

-- =================================================================================
-- 7. BẢNG: Imports
-- =================================================================================
INSERT INTO Imports (Id, ImportCode, StoreId, SupplierId, TotalAmount, CreatedAt, Status)
VALUES
('66666666-0000-0000-0000-000000000001', 'IMP-2024010101', @StoreId, '33333333-0000-0000-0000-000000000001', 2500000, '2024-01-01', 'Completed'),
('66666666-0000-0000-0000-000000000002', 'IMP-2024011501', @StoreId, '33333333-0000-0000-0000-000000000002', 2000000, '2024-01-15', 'Completed'),
('66666666-0000-0000-0000-000000000003', 'IMP-2024020101', @StoreId, '33333333-0000-0000-0000-000000000003', 3200000, '2024-02-01', 'Completed'),
('66666666-0000-0000-0000-000000000004', 'IMP-2024021501', @StoreId, '33333333-0000-0000-0000-000000000004', 1860000, '2024-02-15', 'Completed'),
('66666666-0000-0000-0000-000000000005', 'IMP-2024030101', @StoreId, '33333333-0000-0000-0000-000000000005', 4000000, '2024-03-01', 'Completed');

-- =================================================================================
-- 8. BẢNG: ImportItems
-- =================================================================================
INSERT INTO ImportItems (Id, ImportId, ProductId, Quantity, CostPrice, ExpiryDate)
VALUES
('77777777-0000-0000-0000-000000000001', '66666666-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000001', 100, 25000, '2026-01-01'),
('77777777-0000-0000-0000-000000000002', '66666666-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000002', 250, 8000, '2027-01-01'),
('77777777-0000-0000-0000-000000000003', '66666666-0000-0000-0000-000000000003', '55555555-0000-0000-0000-000000000003', 80, 40000, '2025-10-01'),
('77777777-0000-0000-0000-000000000004', '66666666-0000-0000-0000-000000000004', '55555555-0000-0000-0000-000000000004', 45, 41333, '2027-12-01'),
('77777777-0000-0000-0000-000000000005', '66666666-0000-0000-0000-000000000005', '55555555-0000-0000-0000-000000000005', 120, 33333, '2026-06-01');

-- =================================================================================
-- 9. BẢNG: Orders
-- (Lưu ý: Enum OrderStatus -> Pending:0, Completed:1, Cancelled:2)
-- =================================================================================
INSERT INTO Orders (Id, OrderCode, StoreId, EmployeeId, Status, TotalAmount, FinalAmount, PaymentMethod, CreatedAt)
VALUES
('88888888-0000-0000-0000-000000000001', 'HD2024050101', @StoreId, '22222222-0000-0000-0000-000000000002', 1, 94000, 94000, 'Cash', '2024-05-01 10:15'),
('88888888-0000-0000-0000-000000000002', 'HD2024050201', @StoreId, '22222222-0000-0000-0000-000000000003', 1, 155000, 155000, 'VNPay', '2024-05-02 14:30'),
('88888888-0000-0000-0000-000000000003', 'HD2024050301', @StoreId, '22222222-0000-0000-0000-000000000002', 1, 60000, 60000, 'MoMo', '2024-05-03 09:45'),
('88888888-0000-0000-0000-000000000004', 'HD2024050401', @StoreId, '22222222-0000-0000-0000-000000000003', 1, 110000, 110000, 'Cash', '2024-05-04 18:20'),
('88888888-0000-0000-0000-000000000005', 'HD2024050501', @StoreId, '22222222-0000-0000-0000-000000000005', 1, 144000, 144000, 'Cash', '2024-05-05 11:10');

-- =================================================================================
-- 10. BẢNG: OrderItems
-- =================================================================================
INSERT INTO OrderItems (Id, OrderId, ProductId, Price, Quantity, Total)
VALUES
('99999999-0000-0000-0000-000000000001', '88888888-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000001', 35000, 2, 70000),
('99999999-0000-0000-0000-000000000002', '88888888-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000002', 12000, 2, 24000),
('99999999-0000-0000-0000-000000000003', '88888888-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000003', 55000, 1, 55000),
('99999999-0000-0000-0000-000000000004', '88888888-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000005', 48000, 2, 96000),
('99999999-0000-0000-0000-000000000005', '88888888-0000-0000-0000-000000000003', '55555555-0000-0000-0000-000000000002', 12000, 5, 60000),
('99999999-0000-0000-0000-000000000006', '88888888-0000-0000-0000-000000000004', '55555555-0000-0000-0000-000000000003', 55000, 2, 110000),
('99999999-0000-0000-0000-000000000007', '88888888-0000-0000-0000-000000000005', '55555555-0000-0000-0000-000000000005', 48000, 3, 144000);

-- =================================================================================
-- 11. BẢNG: StorePaymentAccounts
-- =================================================================================
INSERT INTO StorePaymentAccounts (Id, StoreId, Provider, AccountName, MerchantId, PublicKey, SecretKey, BaseUrl, ReturnUrl, IsActive, CreatedAt)
VALUES
('00000000-0000-0000-0000-000000000001', @StoreId, 'VNPay', N'Chut Chut Store VNPay', 'TKD12CJH', '', 'RAPEX52LN0Q1L25OV5G9G29KG8B627IF', 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html', 'https://localhost:7289/api/Payment/PaymentCallback', 1, GETUTCDATE()),
('00000000-0000-0000-0000-000000000002', @StoreId, 'MoMo', N'Chut Chut Store MoMo', 'MOMO_CHUTCHUT', 'momo_public_key_mock', 'momo_secret_key_mock', 'https://test-payment.momo.vn/v2/gateway/api/create', 'https://localhost:7289/api/Payment/MomoCallback', 1, GETUTCDATE()),
('00000000-0000-0000-0000-000000000003', @StoreId, 'ZaloPay', N'Chut Chut Store ZaloPay', 'ZALO_CHUTCHUT', 'zalo_public_key_mock', 'zalo_secret_key_mock', '', '', 0, GETUTCDATE()),
('00000000-0000-0000-0000-000000000004', @StoreId, 'BankTransfer', N'Chut Chut Store VCB', '999988887777', 'vcb_public_key_mock', 'vcb_secret_key_mock', '', '', 1, GETUTCDATE()),
('00000000-0000-0000-0000-000000000005', @StoreId, 'Stripe', N'Chut Chut Store Stripe', 'STRIPE_CHUTCHUT', 'stripe_public_key_mock', 'stripe_secret_key_mock', '', '', 0, GETUTCDATE());
