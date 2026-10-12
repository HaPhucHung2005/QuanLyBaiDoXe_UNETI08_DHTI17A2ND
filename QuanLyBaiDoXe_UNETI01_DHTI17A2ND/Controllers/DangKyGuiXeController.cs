// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - Khách hàng đăng ký gửi xe, xem danh sách phiếu / lịch sử gửi xe / chi tiết phiếu của mình
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class DangKyGuiXeController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DangKyGuiXeService _dangKy;
        private const int PageSize = 10;

        public DangKyGuiXeController(ApplicationDbContext context, DangKyGuiXeService dangKy)
        {
            _context = context;
            _dangKy = dangKy;
        }

        private async Task NapDanhSachAsync(DangKyGuiXeViewModel vm, int maChu)
        {
            vm.DanhSachPhuongTien = await _context.PhuongTien
                .Include(p => p.LoaiPhuongTien)
                .Where(p => p.MaChuPhuongTien == maChu && p.TrangThai)
                .OrderBy(p => p.BienSoXe).ToListAsync();

            vm.DanhSachViTri = await _context.ViTriDoXe
                .Include(v => v.LoaiPhuongTien)
                .Where(v => v.TrangThai != TrangThaiViTri.TamNgung)
                .OrderBy(v => v.KhuVuc).ThenBy(v => v.TenViTri).ToListAsync();
        }

        // GET: DangKyGuiXe/DangKy?maViTri=3
        [HttpGet]
        public async Task<IActionResult> DangKy(int? maViTri)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (IsAdmin) return KhongCoQuyen("Chức năng đăng ký gửi xe dành cho khách hàng.");

            int maTK = MaTaiKhoan!.Value;
            var chu = await _context.ChuPhuongTien.AsNoTracking().FirstOrDefaultAsync(c => c.MaTaiKhoan == maTK);
            if (chu == null)
            {
                TempData["Loi"] = "Bạn cần cập nhật thông tin cá nhân trước khi đăng ký gửi xe.";
                return RedirectToAction("ThongTinCaNhan", "ChuPhuongTien");
            }
            if (!chu.TrangThai)
            {
                TempData["Loi"] = "Tài khoản chủ phương tiện của bạn đang bị khóa.";
                return RedirectToAction("Index", "ViTriDoXe");
            }

            var gio = DateTime.Now.AddHours(1);
            var vao = new DateTime(gio.Year, gio.Month, gio.Day, gio.Hour, 0, 0);
            var vm = new DangKyGuiXeViewModel
            {
                MaViTri = maViTri ?? 0,
                ThoiGianDuKienVao = vao,
                ThoiGianDuKienRa = vao.AddHours(2)
            };
            await NapDanhSachAsync(vm, chu.MaChuPhuongTien);
            return View(vm);
        }

        // POST: DangKyGuiXe/DangKy
        // Chỉ bind 4 trường khách hàng được nhập; đơn giá, thành tiền, ngày đăng ký, trạng thái do hệ thống gán.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(
            [Bind("MaPhuongTien,MaViTri,ThoiGianDuKienVao,ThoiGianDuKienRa")] DangKyGuiXeViewModel vm)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (IsAdmin) return KhongCoQuyen("Chức năng đăng ký gửi xe dành cho khách hàng.");

            int maTK = MaTaiKhoan!.Value;
            var chu = await _context.ChuPhuongTien.AsNoTracking().FirstOrDefaultAsync(c => c.MaTaiKhoan == maTK);
            if (chu == null) return RedirectToAction("ThongTinCaNhan", "ChuPhuongTien");

            if (ModelState.IsValid)
            {
                var (phieu, loi) = await _dangKy.TaoPhieuAsync(
                    maTK, vm.MaPhuongTien, vm.MaViTri, vm.ThoiGianDuKienVao, vm.ThoiGianDuKienRa);

                if (phieu != null)
                {
                    TempData["ThongBao"] = "Đăng ký gửi xe thành công. Phiếu đang chờ Admin xác nhận khi xe vào bãi.";
                    return RedirectToAction(nameof(ChiTiet), new { id = phieu.MaPhieu });
                }
                foreach (var l in loi) ModelState.AddModelError(string.Empty, l);
            }

            await NapDanhSachAsync(vm, chu.MaChuPhuongTien);
            return View(vm);
        }

        // GET: DangKyGuiXe/LichSu?trangThai=Đang gửi&pageIndex=1
        public async Task<IActionResult> LichSu(string? trangThai, int pageIndex = 1)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (IsAdmin) return RedirectToAction("Index", "PhieuGuiXe");

            int maTK = MaTaiKhoan!.Value;

            // Khách hàng chỉ thấy phiếu thuộc phương tiện của chính mình
            var goc = _context.PhieuGuiXe.Where(p => p.PhuongTien!.ChuPhuongTien!.MaTaiKhoan == maTK);

            var dem = await goc.GroupBy(p => p.TrangThai)
                .Select(g => new { g.Key, SoLuong = g.Count() }).ToListAsync();

            var query = goc
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(trangThai) && TrangThaiPhieu.TatCa.Contains(trangThai))
                query = query.Where(p => p.TrangThai == trangThai);

            query = query.OrderByDescending(p => p.NgayDangKy);

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(totalPages, 1)));

            var vm = new LichSuGuiXeViewModel
            {
                DanhSach = await query.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToListAsync(),
                TrangThai = trangThai,
                SoLuongTheoTrangThai = dem.ToDictionary(x => x.Key, x => x.SoLuong),
                PageIndex = pageIndex,
                TotalPages = totalPages
            };
            return View(vm);
        }

        // GET: DangKyGuiXe/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();
            if (IsAdmin) return RedirectToAction("Details", "PhieuGuiXe", new { id });

            int maTK = MaTaiKhoan!.Value;
            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)!.ThenInclude(x => x!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id && p.PhuongTien!.ChuPhuongTien!.MaTaiKhoan == maTK);

            if (phieu == null) return NotFound(); // phiếu của người khác -> không tiết lộ
            return View(phieu);
        }
    }
}
