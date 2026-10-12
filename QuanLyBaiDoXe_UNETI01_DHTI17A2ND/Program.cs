using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. CẤU HÌNH DBCONTEXT VỚI SQL SERVER
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext") 
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(connectionString));

// Module 3 & 4: Service nghiệp vụ đăng ký gửi xe, xác nhận vào/ra, tính phí
builder.Services.AddScoped<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services.DangKyGuiXeService>();
builder.Services.AddScoped<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services.PhieuGuiXeService>();

// 2. ĐĂNG KÝ SERVICE SESSION VÀ IN-MEMORY CACHE
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 3. KÍCH HOẠT MIDDLEWARE SESSION (QUAN TRỌNG: Đặt trước UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// 4. ĐỊNH TUYẾN MẶC ĐỊNH VỀ TRANG ĐĂNG NHẬP (TAIKHOAN/LOGIN)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TaiKhoan}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
