// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - MSSV
// Nội dung thực hiện: Module 1 - Đăng nhập, Đăng xuất, Lưu Session và Quản lý tài khoản
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class TaiKhoanController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. DANH SÁCH TÀI KHOẢN (ADMIN)
        // =================================================================
        // GET: TaiKhoan
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin mới có quyền truy cập Quản lý tài khoản.");

            var dsTaiKhoan = await _context.TaiKhoan
                .OrderByDescending(t => t.MaTaiKhoan)
                .ToListAsync();

            return View(dsTaiKhoan);
        }

        // =================================================================
        // 2. ĐĂNG NHẬP & ĐĂNG XUẤT (MODULE 1)
        // =================================================================
        // GET: TaiKhoan/Login
        public IActionResult Login()
        {
            if (IsLoggedIn)
            {
                return RedirectToAction("Index", IsAdmin ? "ThongKe" : "Home");
            }
            return View();
        }

        // POST: TaiKhoan/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.MatKhau == model.MatKhau);

            if (taiKhoan == null)
            {
                TempData["Loi"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                TempData["Loi"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên.";
                return View(model);
            }

            // 1. Lấy thông tin Họ tên từ ChuPhuongTien (nếu có)
            var chuXe = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == taiKhoan.MaTaiKhoan);
            string hoTen = chuXe?.HoTen ?? taiKhoan.TenDangNhap;

            // 2. Lưu Session Đăng nhập
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
            HttpContext.Session.SetString("HoTen", hoTen);

            TempData["ThongBao"] = $"Đăng nhập thành công! Chào mừng {hoTen}.";

            // 3. ĐIỀU HƯỚNG MẶC ĐỊNH THEO VAI TRÒ (ROLE-BASED REDIRECT)
            if (taiKhoan.VaiTro == "Admin")
            {
                // Admin mặc định chuyển trực tiếp về Dashboard Thống kê
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Khách hàng chuyển về Trang chủ
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: TaiKhoan/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ Session
            TempData["ThongBao"] = "Đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction("Login");
        }

        // =================================================================
        // 3. CHI TIẾT TÀI KHOẢN
        // =================================================================
        // GET: TaiKhoan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoan.FindAsync(id.Value);
            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        // =================================================================
        // 4. TẠO TÀI KHOẢN MỚI / ĐĂNG KÝ
        // =================================================================
        // GET: TaiKhoan/Create
        public IActionResult Create()
        {
            return View(new TaiKhoan { TrangThai = true, VaiTro = "Khách hàng" });
        }

        // POST: TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDangNhap,MatKhau,HoTen,Email,VaiTro,TrangThai")] TaiKhoan taiKhoan)
        {
            // Kiểm tra trùng Tên đăng nhập
            bool isExist = await _context.TaiKhoan.AnyAsync(t => t.TenDangNhap == taiKhoan.TenDangNhap);
            if (isExist)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tự động tạo Hồ sơ Chủ phương tiện nếu là Khách hàng đăng ký
                if (taiKhoan.VaiTro == "Khách hàng")
                {
                    var chuXe = new ChuPhuongTien
                    {
                        MaTaiKhoan = taiKhoan.MaTaiKhoan,
                        HoTen = string.IsNullOrEmpty(taiKhoan.TenDangNhap) ? "Khách hàng" : taiKhoan.TenDangNhap,
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    };
                    _context.Add(chuXe);
                    await _context.SaveChangesAsync();
                }

                TempData["ThongBao"] = $"Tạo tài khoản '{taiKhoan.TenDangNhap}' thành công!";

                if (!IsLoggedIn) return RedirectToAction(nameof(Login));

                return RedirectToAction(nameof(Index));
            }

            return View(taiKhoan);
        }

        // =================================================================
        // 5. CHỈNH SỬA TÀI KHOẢN (ADMIN)
        // =================================================================
        // GET: TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoan.FindAsync(id.Value);
            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        // POST: TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTaiKhoan,TenDangNhap,MatKhau,HoTen,Email,VaiTro,TrangThai")] TaiKhoan taiKhoan)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id != taiKhoan.MaTaiKhoan) return NotFound();

            // Nếu để trống mật khẩu khi sửa, bỏ qua validate Mật khẩu và giữ nguyên mật khẩu cũ
            if (string.IsNullOrWhiteSpace(taiKhoan.MatKhau))
            {
                ModelState.Remove("MatKhau");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var taiKhoanGoc = await _context.TaiKhoan.FindAsync(id);
                    if (taiKhoanGoc == null) return NotFound();

                    // Cập nhật các thông tin mới từ Form
                    taiKhoanGoc.HoTen = taiKhoan.HoTen;
                    taiKhoanGoc.Email = taiKhoan.Email;
                    taiKhoanGoc.VaiTro = taiKhoan.VaiTro;
                    taiKhoanGoc.TrangThai = taiKhoan.TrangThai;

                    // Chỉ đổi mật khẩu nếu người dùng nhập mật khẩu mới
                    if (!string.IsNullOrWhiteSpace(taiKhoan.MatKhau))
                    {
                        taiKhoanGoc.MatKhau = taiKhoan.MatKhau;
                    }

                    _context.Update(taiKhoanGoc);
                    await _context.SaveChangesAsync();

                    TempData["ThongBao"] = $"Cập nhật tài khoản '{taiKhoanGoc.TenDangNhap}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTaiKhoan)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(taiKhoan);
        }

        // =================================================================
        // 6. XÓA TÀI KHOẢN (ADMIN)
        // =================================================================
        // GET: TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoan.FindAsync(id.Value);
            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        // POST: TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan != null)
            {
                // Kiểm tra tài khoản hiện tại không tự xóa chính mình
                if (taiKhoan.MaTaiKhoan == MaTaiKhoan)
                {
                    TempData["Loi"] = "Bạn không thể tự xóa tài khoản đang đăng nhập hiện tại!";
                    return RedirectToAction(nameof(Index));
                }

                _context.TaiKhoan.Remove(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "Xóa tài khoản thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoan.Any(e => e.MaTaiKhoan == id);
        }
    }
}