// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - Quản lý phương tiện: Khách hàng thêm/sửa/xem/xóa phương tiện của mình,
//                     Admin xem và quản lý toàn bộ; kiểm tra biển số trùng, quyền sở hữu, lượt gửi hoạt động
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class PhuongTienController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public PhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Task<ChuPhuongTien?> LayChuHienTaiAsync()
        {
            int maTK = MaTaiKhoan ?? 0;
            return _context.ChuPhuongTien.AsNoTracking().FirstOrDefaultAsync(c => c.MaTaiKhoan == maTK);
        }

        private async Task NapDanhSachChonAsync(int? maLoai = null, int? maChu = null)
        {
            ViewData["MaLoaiPhuongTien"] = new SelectList(
                await _context.LoaiPhuongTien.Where(l => l.TrangThai || l.MaLoaiPhuongTien == maLoai).ToListAsync(),
                "MaLoaiPhuongTien", "TenLoaiPhuongTien", maLoai);

            if (IsAdmin)
            {
                ViewData["MaChuPhuongTien"] = new SelectList(
                    await _context.ChuPhuongTien.Where(c => c.TrangThai || c.MaChuPhuongTien == maChu).OrderBy(c => c.HoTen).ToListAsync(),
                    "MaChuPhuongTien", "HoTen", maChu);
            }
        }

        // Kiểm tra chủ hiện tại của khách hàng; trả về null nếu hợp lệ, ngược lại trả về action chuyển hướng
        private async Task<(ChuPhuongTien? Chu, IActionResult? ChuyenHuong)> YeuCauChuHoatDongAsync()
        {
            var chu = await LayChuHienTaiAsync();
            if (chu == null)
            {
                TempData["Loi"] = "Bạn cần cập nhật thông tin cá nhân trước khi quản lý phương tiện.";
                return (null, RedirectToAction("ThongTinCaNhan", "ChuPhuongTien"));
            }
            if (!chu.TrangThai)
            {
                TempData["Loi"] = "Tài khoản chủ phương tiện của bạn đang bị khóa.";
                return (null, RedirectToAction(nameof(Index)));
            }
            return (chu, null);
        }

        // GET: PhuongTien - Admin thấy tất cả; khách hàng chỉ thấy phương tiện của mình
        public async Task<IActionResult> Index(string? searchBienSo, string? searchChu, int? selectedMaLoai,
                                               string? trangThai, string? sortOrder, int pageIndex = 1)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var query = _context.PhuongTien
                .Include(p => p.ChuPhuongTien)
                .Include(p => p.LoaiPhuongTien)
                .AsQueryable();

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null)
                {
                    TempData["Loi"] = "Bạn cần cập nhật thông tin cá nhân trước khi quản lý phương tiện.";
                    return RedirectToAction("ThongTinCaNhan", "ChuPhuongTien");
                }
                query = query.Where(p => p.MaChuPhuongTien == chu.MaChuPhuongTien);
                searchChu = null;
            }

            if (!string.IsNullOrWhiteSpace(searchBienSo))
            {
                var kw = searchBienSo.Trim();
                query = query.Where(p => p.BienSoXe.Contains(kw) || (p.NhanHieu != null && p.NhanHieu.Contains(kw)));
            }
            if (!string.IsNullOrWhiteSpace(searchChu))
            {
                var kw = searchChu.Trim();
                query = query.Where(p => p.ChuPhuongTien!.HoTen.Contains(kw));
            }
            if (selectedMaLoai.HasValue && selectedMaLoai > 0)
                query = query.Where(p => p.MaLoaiPhuongTien == selectedMaLoai.Value);
            if (trangThai == "hoatdong") query = query.Where(p => p.TrangThai);
            else if (trangThai == "ngung") query = query.Where(p => !p.TrangThai);

            query = sortOrder switch
            {
                "bien_desc" => query.OrderByDescending(p => p.BienSoXe),
                _ => query.OrderBy(p => p.BienSoXe)
            };

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(totalPages, 1)));

            var vm = new PhuongTienFilterViewModel
            {
                DanhSach = await query.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToListAsync(),
                SearchBienSo = searchBienSo,
                SearchChu = searchChu,
                SelectedMaLoai = selectedMaLoai,
                TrangThai = trangThai,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                LoaiList = new SelectList(await _context.LoaiPhuongTien.ToListAsync(),
                    "MaLoaiPhuongTien", "TenLoaiPhuongTien", selectedMaLoai)
            };
            return View(vm);
        }

        // GET: PhuongTien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var pt = await _context.PhuongTien
                .Include(p => p.ChuPhuongTien)
                .Include(p => p.LoaiPhuongTien)
                .FirstOrDefaultAsync(p => p.MaPhuongTien == id);
            if (pt == null) return NotFound();

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null || pt.MaChuPhuongTien != chu.MaChuPhuongTien) return NotFound();
            }
            return View(pt);
        }

        // GET: PhuongTien/Create
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin)
            {
                var (_, chuyenHuong) = await YeuCauChuHoatDongAsync();
                if (chuyenHuong != null) return chuyenHuong;
            }
            await NapDanhSachChonAsync();
            return View(new PhuongTien());
        }

        // POST: PhuongTien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaChuPhuongTien,MaLoaiPhuongTien,BienSoXe,NhanHieu,MauSac,MoTa,TrangThai")] PhuongTien phuongTien)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            if (!IsAdmin)
            {
                var (chu, chuyenHuong) = await YeuCauChuHoatDongAsync();
                if (chuyenHuong != null) return chuyenHuong;
                phuongTien.MaChuPhuongTien = chu!.MaChuPhuongTien;   // luôn gán theo tài khoản đăng nhập
                ModelState.Remove(nameof(PhuongTien.MaChuPhuongTien));
            }

            phuongTien.BienSoXe = (phuongTien.BienSoXe ?? "").Trim().ToUpper();
            ModelState.Remove(nameof(PhuongTien.BienSoXe));
            if (string.IsNullOrEmpty(phuongTien.BienSoXe))
                ModelState.AddModelError(nameof(PhuongTien.BienSoXe), "Biển số xe không được để trống");
            else if (await _context.PhuongTien.AnyAsync(p => p.BienSoXe == phuongTien.BienSoXe))
                ModelState.AddModelError(nameof(PhuongTien.BienSoXe), "Biển số xe đã tồn tại trong hệ thống.");

            if (ModelState.IsValid)
            {
                _context.Add(phuongTien);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Đã thêm phương tiện {phuongTien.BienSoXe}.";
                return RedirectToAction(nameof(Index));
            }
            await NapDanhSachChonAsync(phuongTien.MaLoaiPhuongTien, phuongTien.MaChuPhuongTien);
            return View(phuongTien);
        }

        // GET: PhuongTien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var pt = await _context.PhuongTien.FindAsync(id);
            if (pt == null) return NotFound();

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null || pt.MaChuPhuongTien != chu.MaChuPhuongTien) return NotFound();
            }
            await NapDanhSachChonAsync(pt.MaLoaiPhuongTien, pt.MaChuPhuongTien);
            return View(pt);
        }

        // POST: PhuongTien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaPhuongTien,MaChuPhuongTien,MaLoaiPhuongTien,BienSoXe,NhanHieu,MauSac,MoTa,TrangThai")] PhuongTien phuongTien)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id != phuongTien.MaPhuongTien) return NotFound();

            // Lấy bản gốc trong DB để kiểm tra quyền sở hữu (không tin dữ liệu form gửi lên)
            var goc = await _context.PhuongTien.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhuongTien == id);
            if (goc == null) return NotFound();

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null || goc.MaChuPhuongTien != chu.MaChuPhuongTien) return NotFound();
                if (!chu.TrangThai)
                {
                    TempData["Loi"] = "Tài khoản chủ phương tiện của bạn đang bị khóa.";
                    return RedirectToAction(nameof(Index));
                }
                phuongTien.MaChuPhuongTien = goc.MaChuPhuongTien;
                ModelState.Remove(nameof(PhuongTien.MaChuPhuongTien));
            }

            phuongTien.BienSoXe = (phuongTien.BienSoXe ?? "").Trim().ToUpper();
            ModelState.Remove(nameof(PhuongTien.BienSoXe));
            if (string.IsNullOrEmpty(phuongTien.BienSoXe))
                ModelState.AddModelError(nameof(PhuongTien.BienSoXe), "Biển số xe không được để trống");
            else if (await _context.PhuongTien.AnyAsync(p => p.BienSoXe == phuongTien.BienSoXe && p.MaPhuongTien != id))
                ModelState.AddModelError(nameof(PhuongTien.BienSoXe), "Biển số xe đã được dùng cho phương tiện khác.");

            // Đổi loại xe khi đang có lượt gửi hoạt động sẽ làm phiếu không còn khớp vị trí đỗ
            if (phuongTien.MaLoaiPhuongTien != goc.MaLoaiPhuongTien &&
                await _context.PhieuGuiXe.AnyAsync(p => p.MaPhuongTien == id &&
                    (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui)))
            {
                ModelState.AddModelError(nameof(PhuongTien.MaLoaiPhuongTien),
                    "Không thể đổi loại phương tiện khi đang có lượt gửi xe hoạt động.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuongTien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.PhuongTien.AnyAsync(p => p.MaPhuongTien == id)) return NotFound();
                    throw;
                }
                TempData["ThongBao"] = "Đã cập nhật phương tiện.";
                return RedirectToAction(nameof(Index));
            }
            await NapDanhSachChonAsync(phuongTien.MaLoaiPhuongTien, phuongTien.MaChuPhuongTien);
            return View(phuongTien);
        }

        // GET: PhuongTien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var pt = await _context.PhuongTien
                .Include(p => p.ChuPhuongTien)
                .Include(p => p.LoaiPhuongTien)
                .FirstOrDefaultAsync(p => p.MaPhuongTien == id);
            if (pt == null) return NotFound();

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null || pt.MaChuPhuongTien != chu.MaChuPhuongTien) return NotFound();
            }
            return View(pt);
        }

        // POST: PhuongTien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var pt = await _context.PhuongTien.FindAsync(id);
            if (pt == null) return RedirectToAction(nameof(Index));

            if (!IsAdmin)
            {
                var chu = await LayChuHienTaiAsync();
                if (chu == null || pt.MaChuPhuongTien != chu.MaChuPhuongTien) return NotFound();
            }

            // Không xóa khi đang có lượt gửi hoạt động
            if (await _context.PhieuGuiXe.AnyAsync(p => p.MaPhuongTien == id &&
                    (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui)))
            {
                TempData["Loi"] = "Không thể xóa: phương tiện đang có lượt gửi xe hoạt động.";
                return RedirectToAction(nameof(Index));
            }
            // Đã có lịch sử gửi xe -> khóa ngoại Restrict không cho xóa, hướng dẫn chuyển sang ngừng hoạt động
            if (await _context.PhieuGuiXe.AnyAsync(p => p.MaPhuongTien == id))
            {
                TempData["Loi"] = "Không thể xóa: phương tiện đã có lịch sử gửi xe. Hãy bỏ chọn 'Đang hoạt động' ở trang Sửa để ngừng sử dụng.";
                return RedirectToAction(nameof(Index));
            }

            _context.PhuongTien.Remove(pt);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã xóa phương tiện {pt.BienSoXe}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
