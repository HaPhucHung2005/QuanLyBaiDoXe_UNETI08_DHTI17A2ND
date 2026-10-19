================================================================================
BÀI TẬP LỚN: QUẢN LÝ BÃI ĐỖ XE VÀ ĐĂNG KÝ GỬI XE (ASP.NET CORE 10 MVC)
Lớp: DHTI17A2ND | Trường: ĐẠI HỌC KINH TẾ KỸ THUẬT CÔNG NGHIỆP - UNETI
================================================================================

1. LINK REPOSITORY GITHUB:
   - Link: https://github.com/HaPhucHung2005/QuanLyBaiDoXe_UNETI01_DHTI17A2ND

2. TÀI KHOẢN KIỂM THỬ (TEST ACCOUNTS):
   - Quản trị viên (Admin):
     + Tên đăng nhập: admin
     + Mật khẩu: 123456
   - Khách hàng (User):
     + Tên đăng nhập: khachhang1
     + Mật khẩu: 123456

3. HƯỚNG DẪN CÀI ĐẶT & TẠO DATABASE:
   CÁCH 1: Sử dụng File Script SQL (Khuyên dùng)
   - Mở SQL Server Management Studio (SSMS).
   - Mở file: 02_Database/QuanLyBaiDoXe_UNETI01_DB.sql
   - Đọc khối ghi chú ở đầu file và nhấn Execute để tạo Database kèm dữ liệu mẫu.

   CÁCH 2: Sử dụng EF Core Migration
   - Mở Terminal tại thư mục 01_SourceCode/QuanLyBaiDoXe_UNETI01_DHTI17A2ND
   - Chạy lệnh: dotnet ef database update

   CÁCH 3: Sử dụng Package Manager Console trong Visual Studio
   - Mở Solution QuanLyBaiDoXe_UNETI01_DHTI17A2ND.sln
   - Mở Package Manager Console (Tools -> NuGet Package Manager -> Package Manager Console)
   - Chạy lệnh: Update-Database

4. HƯỚNG DẪN CHẠY CHƯƠNG TRÌNH:
   - Cấu hình chuỗi kết nối trong appsettings.json khớp với SQL Server máy tính.
   - Chạy lệnh: dotnet run (hoặc nhấn F5/Ctrl+F5 trong Visual Studio).
   - Truy cập địa chỉ Localhost hiển thị trên màn hình.
================================================================================
