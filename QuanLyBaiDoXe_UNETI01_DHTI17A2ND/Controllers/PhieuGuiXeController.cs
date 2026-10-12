// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 4 - Admin quản lý phiếu gửi xe: tìm kiếm, lọc, sắp xếp, phân trang,
//                     xem chi tiết, xác nhận xe vào, xác nhận xe ra + tính phí, hủy phiếu
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class PhieuGuiXeController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PhieuGuiXeService _service;
        private const int PageSize = 10;

        public PhieuGuiXeController(ApplicationDbContext context, PhieuGuiXeService service)
        {
            _context = context;
            _service = service;
        }

        // GET: PhieuGuiXe  (Admin)
        // Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị
        public async Task<IActionResult> Index(string? searchBienSo, string? searchChuXe, int? selectedMaLoai,
            string? selectedTrangThai, DateTime? tuNgay, DateTime? denNgay, string? sortOrder, int pageIndex = 1)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin được quản lý phiếu gửi xe.");

            var query = _context.PhieuGuiXe
                .Include(p => p.PhuongTien)!.ThenInclude(x => x!.ChuPhuongTien)
                .Include(p => p.PhuongTien)!.ThenInclude(x => x!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchBienSo))
            {
                var kw = searchBienSo.Trim();
                query = query.Where(p => p.PhuongTien!.BienSoXe.Contains(kw));
            }
            if (!string.IsNullOrWhiteSpace(searchChuXe))
            {
                var kw = searchChuXe.Trim();
                query = query.Where(p => p.PhuongTien!.ChuPhuongTien!.HoTen.Contains(kw));
            }

            // Lọc
            if (selectedMaLoai.HasValue && selectedMaLoai > 0)
                query = query.Where(p => p.PhuongTien!.MaLoaiPhuongTien == selectedMaLoai.Value);
            if (!string.IsNullOrWhiteSpace(selectedTrangThai))
                query = query.Where(p => p.TrangThai == selectedTrangThai);
            if (tuNgay.HasValue)
            {
                var tu = tuNgay.Value.Date;
                query = query.Where(p => p.NgayDangKy >= tu);
            }
            if (denNgay.HasValue)
            {
                var den = denNgay.Value.Date.AddDays(1);   // bao gồm cả ngày kết thúc
                query = query.Where(p => p.NgayDangKy < den);
            }

            // Sắp xếp
            query = sortOrder switch
            {
                "ngay_asc" => query.OrderBy(p => p.NgayDangKy),
                "tien_desc" => query.OrderByDescending(p => p.ThanhTien),
                "tien_asc" => query.OrderBy(p => p.ThanhTien),
                _ => query.OrderByDescending(p => p.NgayDangKy)
            };

            // Phân trang trên truy vấn EF Core (Skip/Take)
            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(totalPages, 1)));

            var vm = new PhieuGuiXeFilterViewModel
            {
                DanhSach = await query.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToListAsync(),
                SearchBienSo = searchBienSo,
                SearchChuXe = searchChuXe,
                SelectedMaLoai = selectedMaLoai,
                SelectedTrangThai = selectedTrangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                TotalItems = total,
                LoaiList = new SelectList(await _context.LoaiPhuongTien.ToListAsync(),
                    "MaLoaiPhuongTien", "TenLoaiPhuongTien", selectedMaLoai),
                TrangThaiList = new SelectList(TrangThaiPhieu.TatCa, selectedTrangThai)
            };
            return View(vm);
        }

        // GET: PhieuGuiXe/Details/5  (Admin)
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin được quản lý phiếu gửi xe.");
            if (id == null) return NotFound();

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)!.ThenInclude(x => x!.ChuPhuongTien)
                .Include(p => p.PhuongTien)!.ThenInclude(x => x!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);
            if (phieu == null) return NotFound();
            return View(phieu);
        }

        // ---- Các thao tác đổi trạng thái đều dùng POST + AntiForgery, kiểm tra Admin tại Controller ----

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanVao(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();
            return Ket(await _service.XacNhanVaoAsync(id), id);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanRa(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();
            return Ket(await _service.XacNhanRaAsync(id), id);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();
            return Ket(await _service.HuyAsync(id), id);
        }

        private IActionResult Ket(KetQuaXuLy kq, int id)
        {
            TempData[kq.ThanhCong ? "ThongBao" : "Loi"] = kq.ThongBao;
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
