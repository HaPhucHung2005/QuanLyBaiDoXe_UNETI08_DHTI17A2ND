// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh - Lương Chính Nguyên
// Mã sinh viên: 23203100001 - MSSV - MSSV
// Nội dung thực hiện: Cấu hình hệ thống Program.cs (.NET Core, Session, DbContext SQL Server & Culture)
// =================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. CẤU HÌNH DBCONTEXT VỚI SQL SERVER
// Lấy 2 chuỗi kết nối từ appsettings.json
var localDbString = builder.Configuration.GetConnectionString("LocalDb");
var localhostString = builder.Configuration.GetConnectionString("LocalhostDb");

// Lấy chuỗi kết nối khả dụng (Ưu tiên LocalDb, nếu không có sẽ lấy LocalhostDb)
var connectionString = localDbString
    ?? localhostString
    ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'LocalDb' hoặc 'LocalhostDb' trong appsettings.json.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        // Tự động kết nối lại nếu gặp sự cố chập chờn
        sqlServerOptions.EnableRetryOnFailure();
    });
});

// 2. ĐĂNG KÝ SERVICE SESSION VÀ IN-MEMORY CACHE
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 3. THIẾT LẬP CULTURINFO (Định dạng dấu chấm phân cách số thập phân)
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 4. KÍCH HOẠT MIDDLEWARE SESSION (Đặt trước UseAuthorization)
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

// 5. ĐỊNH TUYẾN MẶC ĐỊNH VỀ TRANG ĐĂNG NHẬP (TAIKHOAN/LOGIN)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TaiKhoan}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
