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
    VALUES (@StoreId, N'MiniMart Chut Chut', N'Số 123 Đường Cầu Giấy, Hà Nội', '0123456789', 'contact@minimartchutchut.vn', GETUTCDATE());
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
-- 1. BẢNG: AspNetUsers
-- =================================================================================
DELETE FROM AspNetUsers WHERE UserName IN ('manager01', 'staff01', 'staff02', 'staff03', 'staff04');
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
DELETE FROM AppUserRole WHERE UserId IN ('11111111-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000003', '11111111-0000-0000-0000-000000000004', '11111111-0000-0000-0000-000000000005');
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
DELETE FROM Employee WHERE EmployeeCode IN ('NV002', 'NV003', 'NV004', 'NV005', 'NV006');
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
SELECT Id, Name, Phone, Address, CreatedAt FROM (
    VALUES
    ('33333333-0000-0000-0000-000000000001', N'Công ty CP Sữa Vinamilk', '0241234567', N'10 Tân Trào, Quận 7, TP.HCM', GETUTCDATE()),
    ('33333333-0000-0000-0000-000000000002', N'Công ty Nước Giải Khát Coca-Cola', '0242345678', N'KCN Biên Hòa, Đồng Nai', GETUTCDATE()),
    ('33333333-0000-0000-0000-000000000003', N'Công ty CP Bánh Kẹo Hải Châu', '0243456789', N'15 Mạc Thị Bưởi, Hà Nội', GETUTCDATE()),
    ('33333333-0000-0000-0000-000000000004', N'Công ty TNHH Unilever Việt Nam', '0244567890', N'156 Nguyễn Lương Bằng, Quận 7, TP.HCM', GETUTCDATE()),
    ('33333333-0000-0000-0000-000000000005', N'Công ty TNHH Dầu Thực Vật Cái Lân', '0245678901', N'Cái Lân, Hạ Long, Quảng Ninh', GETUTCDATE())
) AS Source(Id, Name, Phone, Address, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Suppliers WHERE Suppliers.Id = Source.Id);

-- =================================================================================
-- 5. BẢNG: Category
-- =================================================================================
INSERT INTO Category (Id, Name, CreatedAt)
SELECT Id, Name, CreatedAt FROM (
    VALUES
    ('44444444-0000-0000-0000-000000000001', N'Sữa & Chế phẩm từ sữa', GETUTCDATE()),
    ('44444444-0000-0000-0000-000000000002', N'Đồ uống & Nước giải khát', GETUTCDATE()),
    ('44444444-0000-0000-0000-000000000003', N'Bánh kẹo', GETUTCDATE()),
    ('44444444-0000-0000-0000-000000000004', N'Chăm sóc cá nhân', GETUTCDATE()),
    ('44444444-0000-0000-0000-000000000005', N'Gia vị & Thực phẩm khô', GETUTCDATE())
) AS Source(Id, Name, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Category WHERE Category.Id = Source.Id);

-- =================================================================================
-- 6. BẢNG: Products
-- =================================================================================
INSERT INTO Products (Id, StoreId, Name, Barcode, CategoryId, SellPrice, Quantity, WarningThreshold, ExpiryDate, CreatedAt)
SELECT Id, StoreId, Name, Barcode, CategoryId, SellPrice, Quantity, WarningThreshold, ExpiryDate, CreatedAt FROM (
    VALUES
    ('55555555-0000-0000-0000-000000000001', @StoreId, N'Sữa Tươi Vinamilk 1L', '8934563102001', '44444444-0000-0000-0000-000000000001', 35000, 100, 10, '2026-01-01', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000002', @StoreId, N'Nước Ngọt Coca-Cola 330ml', '8934563102002', '44444444-0000-0000-0000-000000000002', 12000, 250, 20, '2027-01-01', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000003', @StoreId, N'Bánh Quy ChocoPie', '8934563102003', '44444444-0000-0000-0000-000000000003', 55000, 80, 5, '2025-10-01', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000004', @StoreId, N'Dầu Gội Clear Men 170g', '8934563102004', '44444444-0000-0000-0000-000000000004', 62000, 45, 5, '2027-12-01', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000005', @StoreId, N'Dầu Ăn Neptune 1L', '8934563102005', '44444444-0000-0000-0000-000000000005', 48000, 120, 15, '2026-06-01', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000006', @StoreId, N'Sữa Chua Vinamilk Có Đường', '8934563102006', '44444444-0000-0000-0000-000000000001', 6000, 200, 50, '2024-12-31', GETUTCDATE()),
    ('55555555-0000-0000-0000-000000000007', @StoreId, N'Nước Suối Lavie 500ml', '8934563102007', '44444444-0000-0000-0000-000000000002', 5000, 500, 100, '2026-05-20', GETUTCDATE())
) AS Source(Id, StoreId, Name, Barcode, CategoryId, SellPrice, Quantity, WarningThreshold, ExpiryDate, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Products.Id = Source.Id);

-- =================================================================================
-- 7. BẢNG: Imports
-- =================================================================================
INSERT INTO Imports (Id, ImportCode, StoreId, SupplierId, EmployeeId, TotalAmount, CreatedAt, Status)
SELECT Id, ImportCode, StoreId, SupplierId, EmployeeId, TotalAmount, CreatedAt, Status FROM (
    VALUES
    ('66666666-0000-0000-0000-000000000001', 'IMP-2024010101', @StoreId, '33333333-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000001', 2500000, '2024-01-01', 'Completed'),
    ('66666666-0000-0000-0000-000000000002', 'IMP-2024011501', @StoreId, '33333333-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000001', 2000000, '2024-01-15', 'Completed'),
    ('66666666-0000-0000-0000-000000000003', 'IMP-2024020101', @StoreId, '33333333-0000-0000-0000-000000000003', '22222222-0000-0000-0000-000000000004', 3200000, '2024-02-01', 'Completed'),
    ('66666666-0000-0000-0000-000000000004', 'IMP-2024021501', @StoreId, '33333333-0000-0000-0000-000000000004', '22222222-0000-0000-0000-000000000004', 1860000, '2024-02-15', 'Completed'),
    ('66666666-0000-0000-0000-000000000005', 'IMP-2024030101', @StoreId, '33333333-0000-0000-0000-000000000005', '22222222-0000-0000-0000-000000000001', 4000000, '2024-03-01', 'Completed')
) AS Source(Id, ImportCode, StoreId, SupplierId, EmployeeId, TotalAmount, CreatedAt, Status)
WHERE NOT EXISTS (SELECT 1 FROM Imports WHERE Imports.Id = Source.Id);

-- =================================================================================
-- 8. BẢNG: ImportItems
-- =================================================================================
INSERT INTO ImportItems (Id, ImportId, ProductId, Quantity, CostPrice, ExpiryDate)
SELECT Id, ImportId, ProductId, Quantity, CostPrice, ExpiryDate FROM (
    VALUES
    ('77777777-0000-0000-0000-000000000001', '66666666-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000001', 100, 25000, '2026-01-01'),
    ('77777777-0000-0000-0000-000000000002', '66666666-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000002', 250, 8000, '2027-01-01'),
    ('77777777-0000-0000-0000-000000000003', '66666666-0000-0000-0000-000000000003', '55555555-0000-0000-0000-000000000003', 80, 40000, '2025-10-01'),
    ('77777777-0000-0000-0000-000000000004', '66666666-0000-0000-0000-000000000004', '55555555-0000-0000-0000-000000000004', 45, 41333, '2027-12-01'),
    ('77777777-0000-0000-0000-000000000005', '66666666-0000-0000-0000-000000000005', '55555555-0000-0000-0000-000000000005', 120, 33333, '2026-06-01')
) AS Source(Id, ImportId, ProductId, Quantity, CostPrice, ExpiryDate)
WHERE NOT EXISTS (SELECT 1 FROM ImportItems WHERE ImportItems.Id = Source.Id);

-- =================================================================================
-- 9. BẢNG: Orders
-- (Lưu ý: Enum OrderStatus -> Pending:0, Completed:1, Cancelled:2)
-- =================================================================================
INSERT INTO Orders (Id, OrderCode, StoreId, EmployeeId, Status, TotalAmount, FinalAmount, PaymentMethod, CreatedAt)
SELECT Id, OrderCode, StoreId, EmployeeId, Status, TotalAmount, FinalAmount, PaymentMethod, CreatedAt FROM (
    VALUES
    ('88888888-0000-0000-0000-000000000001', 'HD2024050101', @StoreId, '22222222-0000-0000-0000-000000000002', 1, 94000, 94000, 'Cash', '2024-05-01 10:15'),
    ('88888888-0000-0000-0000-000000000002', 'HD2024050201', @StoreId, '22222222-0000-0000-0000-000000000003', 1, 155000, 155000, 'VNPay', '2024-05-02 14:30'),
    ('88888888-0000-0000-0000-000000000003', 'HD2024050301', @StoreId, '22222222-0000-0000-0000-000000000002', 1, 60000, 60000, 'MoMo', '2024-05-03 09:45'),
    ('88888888-0000-0000-0000-000000000004', 'HD2024050401', @StoreId, '22222222-0000-0000-0000-000000000003', 1, 110000, 110000, 'Cash', '2024-05-04 18:20'),
    ('88888888-0000-0000-0000-000000000005', 'HD2024050501', @StoreId, '22222222-0000-0000-0000-000000000005', 1, 144000, 144000, 'Cash', '2024-05-05 11:10')
) AS Source(Id, OrderCode, StoreId, EmployeeId, Status, TotalAmount, FinalAmount, PaymentMethod, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE Orders.Id = Source.Id);

-- =================================================================================
-- 10. BẢNG: OrderItems
-- =================================================================================
INSERT INTO OrderItems (Id, OrderId, ProductId, Price, Quantity, Total)
SELECT Id, OrderId, ProductId, Price, Quantity, Total FROM (
    VALUES
    ('99999999-0000-0000-0000-000000000001', '88888888-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000001', 35000, 2, 70000),
    ('99999999-0000-0000-0000-000000000002', '88888888-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000002', 12000, 2, 24000),
    ('99999999-0000-0000-0000-000000000003', '88888888-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000003', 55000, 1, 55000),
    ('99999999-0000-0000-0000-000000000004', '88888888-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000005', 48000, 2, 96000),
    ('99999999-0000-0000-0000-000000000005', '88888888-0000-0000-0000-000000000003', '55555555-0000-0000-0000-000000000002', 12000, 5, 60000),
    ('99999999-0000-0000-0000-000000000006', '88888888-0000-0000-0000-000000000004', '55555555-0000-0000-0000-000000000003', 55000, 2, 110000),
    ('99999999-0000-0000-0000-000000000007', '88888888-0000-0000-0000-000000000005', '55555555-0000-0000-0000-000000000005', 48000, 3, 144000)
) AS Source(Id, OrderId, ProductId, Price, Quantity, Total)
WHERE NOT EXISTS (SELECT 1 FROM OrderItems WHERE OrderItems.Id = Source.Id);

-- =================================================================================
-- 11. BẢNG: StorePaymentAccounts
-- =================================================================================
INSERT INTO StorePaymentAccounts (Id, StoreId, Provider, AccountName, MerchantId, PublicKey, SecretKey, BaseUrl, ReturnUrl, IsActive, CreatedAt)
SELECT Id, StoreId, Provider, AccountName, MerchantId, PublicKey, SecretKey, BaseUrl, ReturnUrl, IsActive, CreatedAt FROM (
    VALUES
    ('00000000-0000-0000-0000-000000000001', @StoreId, 'VNPay', N'VNPay Sandbox', 'TKD12CJH', '', 'RAPEX52LN0Q1L25OV5G9G29KG8B627IF', 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html', 'https://localhost:7289/api/Payment/PaymentCallback', 1, GETUTCDATE()),
    ('00000000-0000-0000-0000-000000000002', @StoreId, 'MoMo', N'MoMo Test', 'MOMO_CHUTCHUT', 'momo_public_key_mock', 'momo_secret_key_mock', 'https://test-payment.momo.vn/v2/gateway/api/create', 'https://localhost:7289/api/Payment/MomoCallback', 1, GETUTCDATE()),
    ('00000000-0000-0000-0000-000000000004', @StoreId, 'BankTransfer', N'Vietcombank', '999988887777', 'vcb_public_key_mock', 'vcb_secret_key_mock', '', '', 1, GETUTCDATE())
) AS Source(Id, StoreId, Provider, AccountName, MerchantId, PublicKey, SecretKey, BaseUrl, ReturnUrl, IsActive, CreatedAt)
WHERE NOT EXISTS (SELECT 1 FROM StorePaymentAccounts WHERE StorePaymentAccounts.Id = Source.Id);

-- =================================================================================
-- 12. BẢNG: Promotions
-- (Type: PERCENT:0, FIXED:1, BUY_X_GET_Y:2 | Scope: BAN_HANG:0, NHAP_HANG:1 | Status: ACTIVE:0, INACTIVE:1)
-- =================================================================================
INSERT INTO Promotions (Id, Name, Type, DiscountValue, BuyQuantity, GetQuantity, ProductId, Scope, StartDate, EndDate, Status, StoreId, CreatedAt, CreatedById)
SELECT Id, Name, Type, DiscountValue, BuyQuantity, GetQuantity, ProductId, Scope, StartDate, EndDate, Status, StoreId, CreatedAt, CreatedById FROM (
    VALUES
    ('AAAA1111-0000-0000-0000-000000000001', N'Giảm giá khai trương 10%', 0, 10, NULL, NULL, NULL, 0, '2024-01-01', '2026-12-31', 0, @StoreId, GETUTCDATE(), '22222222-0000-0000-0000-000000000001'),
    ('AAAA1111-0000-0000-0000-000000000002', N'Mua 2 tặng 1 Sữa Vinamilk', 2, NULL, 2, 1, '55555555-0000-0000-0000-000000000001', 0, '2024-05-01', '2024-05-31', 0, @StoreId, GETUTCDATE(), '22222222-0000-0000-0000-000000000001'),
    ('AAAA1111-0000-0000-0000-000000000003', N'Giảm 5000đ cho Bánh ChocoPie', 1, 5000, NULL, NULL, '55555555-0000-0000-0000-000000000003', 0, '2024-05-10', '2024-06-10', 0, @StoreId, GETUTCDATE(), '22222222-0000-0000-0000-000000000001'),
    ('AAAA1111-0000-0000-0000-000000000004', N'Ưu đãi nhập hàng Unilever', 0, 5, NULL, NULL, NULL, 1, '2024-01-01', '2024-12-31', 0, @StoreId, GETUTCDATE(), '22222222-0000-0000-0000-000000000001')
) AS Source(Id, Name, Type, DiscountValue, BuyQuantity, GetQuantity, ProductId, Scope, StartDate, EndDate, Status, StoreId, CreatedAt, CreatedById)
WHERE NOT EXISTS (SELECT 1 FROM Promotions WHERE Promotions.Id = Source.Id);
