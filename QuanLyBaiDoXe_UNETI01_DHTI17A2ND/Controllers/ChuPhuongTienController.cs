// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - Quản lý chủ phương tiện: Admin xem/tìm kiếm/khóa-mở khóa,
//                     Khách hàng xem và cập nhật thông tin cá nhân của mình
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class ChuPhuongTienController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ChuPhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChuPhuongTien  (Admin) - tìm kiếm + lọc + sắp xếp + phân trang bằng LINQ
        public async Task<IActionResult> Index(string? search, string? trangThai, string? sortOrder, int pageIndex = 1)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin được xem danh sách chủ phương tiện.");

            var query = _context.ChuPhuongTien.Include(c => c.DanhSachPhuongTien).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim();
                query = query.Where(c => c.HoTen.Contains(kw) || c.SoDienThoai.Contains(kw) || c.Email.Contains(kw));
            }

            if (trangThai == "hoatdong") query = query.Where(c => c.TrangThai);
            else if (trangThai == "khoa") query = query.Where(c => !c.TrangThai);

            query = sortOrder switch
            {
                "ten_desc" => query.OrderByDescending(c => c.HoTen),
                "ngay_desc" => query.OrderByDescending(c => c.NgayDangKy),
                "ngay_asc" => query.OrderBy(c => c.NgayDangKy),
                _ => query.OrderBy(c => c.HoTen)
            };

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(totalPages, 1)));

            var vm = new ChuPhuongTienFilterViewModel
            {
                DanhSach = await query.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToListAsync(),
                Search = search,
                TrangThai = trangThai,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                TotalPages = totalPages
            };
            return View(vm);
        }

        // GET: ChuPhuongTien/Details/5  (Admin xem tất cả; khách hàng chỉ xem của mình)
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var chu = await _context.ChuPhuongTien
                .Include(c => c.TaiKhoan)
                .Include(c => c.DanhSachPhuongTien)!.ThenInclude(p => p.LoaiPhuongTien)
                .FirstOrDefaultAsync(c => c.MaChuPhuongTien == id);
            if (chu == null) return NotFound();

            if (!IsAdmin && chu.MaTaiKhoan != MaTaiKhoan) return NotFound(); // không lộ dữ liệu người khác
            return View(chu);
        }

        // POST: ChuPhuongTien/CapNhatTrangThai/5  (Admin) - khóa / mở khóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            var chu = await _context.ChuPhuongTien.FindAsync(id);
            if (chu == null) return NotFound();

            chu.TrangThai = !chu.TrangThai;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã {(chu.TrangThai ? "mở khóa" : "khóa")} chủ phương tiện {chu.HoTen}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ChuPhuongTien/ThongTinCaNhan  (Khách hàng)
        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (IsAdmin)
            {
                TempData["Loi"] = "Tài khoản Admin không có hồ sơ chủ phương tiện.";
                return RedirectToAction(nameof(Index));
            }

            int maTK = MaTaiKhoan!.Value;
            var chu = await _context.ChuPhuongTien.AsNoTracking().FirstOrDefaultAsync(c => c.MaTaiKhoan == maTK);
            if (chu == null)
            {
                // Lần đầu: gợi ý sẵn họ tên/email từ tài khoản
                var tk = await _context.TaiKhoan.AsNoTracking().FirstOrDefaultAsync(t => t.MaTaiKhoan == maTK);
                chu = new ChuPhuongTien
                {
                    MaTaiKhoan = maTK,
                    HoTen = tk?.HoTen ?? "",
                    Email = tk?.Email ?? "",
                    NgaySinh = new DateTime(2000, 1, 1),
                    GioiTinh = "Nam"
                };
            }
            return View(chu);
        }

        // POST: ChuPhuongTien/ThongTinCaNhan
        // Chỉ bind các trường được phép sửa: khách hàng KHÔNG được tự đổi trạng thái, ngày đăng ký, tài khoản.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinCaNhan(
            [Bind("HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi")] ChuPhuongTien model)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (IsAdmin) return KhongCoQuyen("Tài khoản Admin không có hồ sơ chủ phương tiện.");

            int maTK = MaTaiKhoan!.Value;
            if (!ModelState.IsValid)
            {
                model.MaTaiKhoan = maTK;
                return View(model);
            }

            var chu = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTK);
            if (chu == null)
            {
                chu = new ChuPhuongTien { MaTaiKhoan = maTK, NgayDangKy = DateTime.Now, TrangThai = true };
                _context.ChuPhuongTien.Add(chu);
            }

            chu.HoTen = model.HoTen.Trim();
            chu.NgaySinh = model.NgaySinh;
            chu.GioiTinh = model.GioiTinh;
            chu.SoDienThoai = model.SoDienThoai.Trim();
            chu.Email = model.Email.Trim();
            chu.DiaChi = model.DiaChi.Trim();

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Đã lưu thông tin cá nhân.";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
    }
}
