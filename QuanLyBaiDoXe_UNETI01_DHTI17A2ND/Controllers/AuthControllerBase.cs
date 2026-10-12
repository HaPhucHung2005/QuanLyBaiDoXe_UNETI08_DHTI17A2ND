// =================================================================
// Dùng chung Module 3 và Module 4
// Họ và tên: Trần Ngọc Minh - Hà Phúc Hưng
// Mã sinh viên: [điền MSSV] - 23203100001
// Nội dung thực hiện: Lớp cơ sở kiểm tra đăng nhập / phân quyền bằng Session cho các Controller Module 3-4
// =================================================================
using Microsoft.AspNetCore.Mvc;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public abstract class AuthControllerBase : Controller
    {
        protected int? MaTaiKhoan => HttpContext.Session.GetInt32("MaTaiKhoan");
        protected bool IsLoggedIn => MaTaiKhoan != null;
        protected bool IsAdmin => IsLoggedIn && HttpContext.Session.GetString("VaiTro") == "Admin";

        protected IActionResult ChuaDangNhap() => RedirectToAction("Login", "TaiKhoan");

        protected IActionResult KhongCoQuyen(string? thongBao = null)
        {
            TempData["Loi"] = thongBao ?? "Bạn không có quyền thực hiện chức năng này.";
            return RedirectToAction("Index", "ViTriDoXe");
        }
    }
}