// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - [MSSV]
// Nội dung thực hiện: Controller Đăng ký và Quản lý Phiếu gửi xe (Module 3 & Module 4)
// =================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class PhieuGuiXeController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhieuGuiXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. DANH SÁCH & LỌC PHIẾU GỬI XE (MODULE 3 & 4)
        // =================================================================
        // GET: PhieuGuiXe (Lịch sử gửi xe cho Khách hàng | Quản lý cho Admin)
        public async Task<IActionResult> Index(PhieuGuiXeFilterViewModel filter)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var query = _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                    .ThenInclude(p => p!.ChuPhuongTien)
                .Include(p => p.PhuongTien)
                    .ThenInclude(p => p!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .AsQueryable();

            // Nếu là Khách hàng: Chỉ xem phiếu thuộc các phương tiện của chính mình
            if (!IsAdmin)
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

                if (chuPhuongTien == null)
                {
                    return NotFound("Không tìm thấy thông tin chủ phương tiện.");
                }

                query = query.Where(p => p.PhuongTien!.MaChuPhuongTien == chuPhuongTien.MaChuPhuongTien);
            }

            // 1. Tìm kiếm theo biển số xe
            if (!string.IsNullOrWhiteSpace(filter.SearchBienSo))
            {
                var termBS = filter.SearchBienSo.Trim().ToLower();
                query = query.Where(p => p.PhuongTien!.BienSoXe.ToLower().Contains(termBS));
            }

            // 2. Tìm kiếm theo tên chủ phương tiện (Admin)
            if (!string.IsNullOrWhiteSpace(filter.SearchChuXe))
            {
                var termChu = filter.SearchChuXe.Trim().ToLower();
                query = query.Where(p => p.PhuongTien!.ChuPhuongTien!.HoTen.ToLower().Contains(termChu));
            }

            // 3. Lọc theo loại phương tiện
            if (filter.SelectedMaLoai.HasValue && filter.SelectedMaLoai > 0)
            {
                query = query.Where(p => p.PhuongTien!.MaLoaiPhuongTien == filter.SelectedMaLoai.Value);
            }

            // 4. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(filter.SelectedTrangThai))
            {
                query = query.Where(p => p.TrangThai == filter.SelectedTrangThai);
            }

            // 5. Lọc theo khoảng thời gian đăng ký (Từ ngày - Đến ngày)
            if (filter.TuNgay.HasValue)
            {
                query = query.Where(p => p.NgayDangKy >= filter.TuNgay.Value.Date);
            }
            if (filter.DenNgay.HasValue)
            {
                query = query.Where(p => p.NgayDangKy <= filter.DenNgay.Value.Date.AddDays(1).AddTicks(-1));
            }

            filter.TotalItems = await query.CountAsync();

            // Xử lý phân trang
            if (filter.PageSize <= 0) filter.PageSize = 10;
            if (filter.PageIndex <= 0) filter.PageIndex = 1;
            filter.TotalPages = (int)Math.Ceiling((double)filter.TotalItems / filter.PageSize);

            filter.DanhSach = await query
                .OrderByDescending(p => p.NgayDangKy)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Khởi tạo SelectList cho bộ lọc
            var dsLoai = await _context.LoaiPhuongTien.ToListAsync();
            filter.LoaiList = new SelectList(dsLoai, "MaLoaiPhuongTien", "TenLoaiPhuongTien", filter.SelectedMaLoai);
            filter.TrangThaiList = new SelectList(TrangThaiPhieu.TatCa, filter.SelectedTrangThai);

            return View(filter);
        }

        // =================================================================
        // 2. XEM CHI TIẾT PHIẾU (MODULE 3)
        // =================================================================
        // GET: PhieuGuiXe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var phieuguixe = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                    .ThenInclude(p => p!.ChuPhuongTien)
                .Include(p => p.PhuongTien)
                    .ThenInclude(p => p!.LoaiPhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaPhieu == id.Value);

            if (phieuguixe == null) return NotFound();

            // Phân quyền: Khách hàng chỉ xem được chi tiết phiếu của chính mình
            if (!IsAdmin)
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

                if (chuPhuongTien == null || phieuguixe.PhuongTien?.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
                {
                    return Forbid();
                }
            }

            return View(phieuguixe);
        }

        // =================================================================
        // 3. ĐĂNG KÝ GỬI XE MỚI (MODULE 3)
        // =================================================================
        // GET: PhieuGuiXe/Create
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var chuPhuongTien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

            if (chuPhuongTien == null)
            {
                return NotFound("Không tìm thấy thông tin chủ phương tiện của bạn.");
            }

            var dsXeCuaToi = await _context.PhuongTien
                .Where(p => p.MaChuPhuongTien == chuPhuongTien.MaChuPhuongTien && p.TrangThai)
                .ToListAsync();

            ViewData["MaPhuongTien"] = new SelectList(dsXeCuaToi, "MaPhuongTien", "BienSoXe");

            var defaultModel = new PhieuGuiXe
            {
                ThoiGianDuKienVao = DateTime.Now,
                ThoiGianDuKienRa = DateTime.Now.AddHours(2)
            };

            return View(defaultModel);
        }

        // API Ajax: Lấy danh sách Vị trí đỗ phù hợp theo xe chọn
        [HttpGet]
        public async Task<JsonResult> GetViTriByPhuongTien(int maPhuongTien)
        {
            var xe = await _context.PhuongTien.FindAsync(maPhuongTien);
            if (xe == null) return Json(null);

            var listViTri = await _context.ViTriDoXe
                .Where(v => v.MaLoaiPhuongTien == xe.MaLoaiPhuongTien && v.TrangThai != TrangThaiViTri.TamNgung)
                .OrderBy(v => v.TenViTri)
                .Select(v => new
                {
                    v.MaViTri,
                    v.TenViTri,
                    v.KhuVuc,
                    v.Tang
                })
                .ToListAsync();

            return Json(listViTri);
        }

        // POST: PhieuGuiXe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhuongTien,MaViTri,ThoiGianDuKienVao,ThoiGianDuKienRa")] PhieuGuiXe phieuguixe)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var chuPhuongTien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

            if (chuPhuongTien == null || !chuPhuongTien.TrangThai)
            {
                ModelState.AddModelError("", "Tài khoản hoặc chủ phương tiện đang ở trạng thái bị khóa.");
            }

            var xe = await _context.PhuongTien.FindAsync(phieuguixe.MaPhuongTien);
            if (xe == null || xe.MaChuPhuongTien != chuPhuongTien?.MaChuPhuongTien)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện không hợp lệ hoặc không thuộc về bạn.");
            }

            // 1. Kiểm tra thời gian: Dự kiến ra phải sau thời gian dự kiến vào
            if (phieuguixe.ThoiGianDuKienRa <= phieuguixe.ThoiGianDuKienVao)
            {
                ModelState.AddModelError("ThoiGianDuKienRa", "Thời gian dự kiến ra phải lớn hơn thời gian dự kiến vào.");
            }

            // 2. Kiểm tra phương tiện không có lượt gửi active (Chờ xác nhận / Đang gửi)
            bool isVehicleActive = await _context.PhieuGuiXe
                .AnyAsync(p => p.MaPhuongTien == phieuguixe.MaPhuongTien
                            && (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui));

            if (isVehicleActive)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện này đang có 1 lượt gửi xe chưa hoàn tất!");
            }

            // 3. Kiểm tra vị trí đỗ trùng khoảng thời gian đăng ký
            bool isLocationOverlap = await _context.PhieuGuiXe
                .AnyAsync(p => p.MaViTri == phieuguixe.MaViTri
                            && p.TrangThai != TrangThaiPhieu.DaHuy
                            && p.TrangThai != TrangThaiPhieu.DaLayXe
                            && p.ThoiGianDuKienVao < phieuguixe.ThoiGianDuKienRa
                            && p.ThoiGianDuKienRa > phieuguixe.ThoiGianDuKienVao);

            if (isLocationOverlap)
            {
                ModelState.AddModelError("MaViTri", "Vị trí đỗ này đã có phiếu gửi bị trùng khoảng thời gian đăng ký!");
            }

            ModelState.Remove("PhuongTien");
            ModelState.Remove("ViTriDoXe");

            if (ModelState.IsValid)
            {
                var loaiXe = await _context.LoaiPhuongTien.FindAsync(xe!.MaLoaiPhuongTien);

                phieuguixe.NgayDangKy = DateTime.Now;
                phieuguixe.DonGiaTheoGio = loaiXe?.DonGiaTheoGio ?? 0;
                phieuguixe.TrangThai = TrangThaiPhieu.ChoXacNhan;

                _context.Add(phieuguixe);
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = "Đăng ký gửi xe thành công! Vui lòng chờ xác nhận từ Admin.";
                return RedirectToAction(nameof(Index));
            }

            var dsXeCuaToi = await _context.PhuongTien
                .Where(p => p.MaChuPhuongTien == chuPhuongTien!.MaChuPhuongTien && p.TrangThai)
                .ToListAsync();

            ViewData["MaPhuongTien"] = new SelectList(dsXeCuaToi, "MaPhuongTien", "BienSoXe", phieuguixe.MaPhuongTien);
            return View(phieuguixe);
        }

        // =================================================================
        // 4. XÁC NHẬN XE VÀO BÃI (MODULE 4 - ADMIN)
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanVao(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin mới có quyền xác nhận xe vào bãi.");

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            if (phieu.TrangThai != TrangThaiPhieu.ChoXacNhan)
            {
                TempData["Loi"] = "Chỉ xác nhận xe vào cho phiếu đang ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction(nameof(Index));
            }

            if (phieu.PhuongTien == null || !phieu.PhuongTien.TrangThai)
            {
                TempData["Loi"] = "Phương tiện không hợp lệ hoặc đang bị tạm ngừng hoạt động.";
                return RedirectToAction(nameof(Index));
            }

            if (phieu.ViTriDoXe == null || phieu.ViTriDoXe.TrangThai == TrangThaiViTri.TamNgung)
            {
                TempData["Loi"] = "Vị trí đỗ hiện đang bị khóa hoặc bảo trì.";
                return RedirectToAction(nameof(Index));
            }

            bool viTriOccupied = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaViTri == phieu.MaViTri &&
                p.MaPhieu != phieu.MaPhieu &&
                p.TrangThai == TrangThaiPhieu.DangGui);

            if (viTriOccupied)
            {
                TempData["Loi"] = "Vị trí đỗ này đang bị chiếm bởi một lượt gửi xe khác.";
                return RedirectToAction(nameof(Index));
            }

            bool phuongTienActive = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaPhuongTien == phieu.MaPhuongTien &&
                p.MaPhieu != phieu.MaPhieu &&
                p.TrangThai == TrangThaiPhieu.DangGui);

            if (phuongTienActive)
            {
                TempData["Loi"] = "Phương tiện này hiện đang có một lượt gửi khác trong bãi.";
                return RedirectToAction(nameof(Index));
            }

            phieu.ThoiGianVaoThucTe = DateTime.Now;
            phieu.TrangThai = TrangThaiPhieu.DangGui;
            phieu.ViTriDoXe.TrangThai = TrangThaiViTri.DangSuDung;

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Xác nhận xe {phieu.PhuongTien.BienSoXe} vào bãi thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 5. XÁC NHẬN XE RA BÃI & TÍNH PHÍ TỰ ĐỘNG (MODULE 4 - ADMIN)
        // =================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanRa(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin mới có quyền xác nhận xe ra bãi.");

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.DangGui)
            {
                TempData["Loi"] = "Phiếu gửi không hợp lệ hoặc xe chưa vào bãi.";
                return RedirectToAction(nameof(Index));
            }

            DateTime thoiGianRa = DateTime.Now;
            DateTime thoiGianVao = phieu.ThoiGianVaoThucTe ?? phieu.ThoiGianDuKienVao;

            if (thoiGianRa <= thoiGianVao)
            {
                TempData["Loi"] = "Thời gian ra thực tế phải lớn hơn thời gian vào thực tế.";
                return RedirectToAction(nameof(Index));
            }

            TimeSpan duration = thoiGianRa - thoiGianVao;
            int soGioTinhPhi = (int)Math.Ceiling(duration.TotalHours);
            if (soGioTinhPhi < 1) soGioTinhPhi = 1;

            decimal thanhTien = soGioTinhPhi * phieu.DonGiaTheoGio;

            phieu.ThoiGianRaThucTe = thoiGianRa;
            phieu.ThanhTien = thanhTien;
            phieu.TrangThai = TrangThaiPhieu.DaLayXe;

            bool remainingActive = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaViTri == phieu.MaViTri &&
                p.MaPhieu != phieu.MaPhieu &&
                p.TrangThai == TrangThaiPhieu.DangGui);

            if (!remainingActive && phieu.ViTriDoXe != null)
            {
                phieu.ViTriDoXe.TrangThai = TrangThaiViTri.ConTrong;
            }

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Xác nhận xe {phieu.PhuongTien?.BienSoXe} ra bãi thành công. Thành tiền: {thanhTien:N0} VNĐ ({soGioTinhPhi} giờ).";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 6. HỦY PHIẾU GỬI XE (MODULE 3 & 4)
        // =================================================================
        // GET: PhieuGuiXe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (id == null) return NotFound();

            var phieuguixe = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaPhieu == id.Value);

            if (phieuguixe == null) return NotFound();

            if (!IsAdmin)
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

                if (chuPhuongTien == null || phieuguixe.PhuongTien?.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
                {
                    return KhongCoQuyen("Bạn không thể hủy phiếu gửi xe của người khác.");
                }
            }

            return View(phieuguixe);
        }

        // POST: PhieuGuiXe/Delete/5 (Khách hàng có thể hủy phiếu khi còn ở trạng thái Chờ xác nhận)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null) return NotFound();

            if (!IsAdmin)
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == MaTaiKhoan);

                if (chuPhuongTien == null || phieu.PhuongTien?.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
                {
                    return KhongCoQuyen("Bạn không thể hủy phiếu gửi xe của người khác.");
                }
            }

            if (phieu.TrangThai != TrangThaiPhieu.ChoXacNhan)
            {
                TempData["Loi"] = "Chỉ được phép hủy phiếu đang ở trạng thái 'Chờ xác nhận'!";
                return RedirectToAction(nameof(Index));
            }

            phieu.TrangThai = TrangThaiPhieu.DaHuy;
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đã hủy phiếu gửi xe thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // 7. CHỈNH SỬA PHIẾU (ADMIN)
        // =================================================================
        // GET: PhieuGuiXe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var phieuguixe = await _context.PhieuGuiXe.FindAsync(id.Value);
            if (phieuguixe == null) return NotFound();

            ViewData["MaPhuongTien"] = new SelectList(_context.PhuongTien, "MaPhuongTien", "BienSoXe", phieuguixe.MaPhuongTien);
            ViewData["MaViTri"] = new SelectList(_context.ViTriDoXe, "MaViTri", "TenViTri", phieuguixe.MaViTri);
            return View(phieuguixe);
        }

        // POST: PhieuGuiXe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhieu,MaPhuongTien,MaViTri,NgayDangKy,ThoiGianDuKienVao,ThoiGianDuKienRa,ThoiGianVaoThucTe,ThoiGianRaThucTe,DonGiaTheoGio,ThanhTien,TrangThai")] PhieuGuiXe phieuguixe)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id != phieuguixe.MaPhieu) return NotFound();

            if (phieuguixe.ThoiGianDuKienRa <= phieuguixe.ThoiGianDuKienVao)
            {
                ModelState.AddModelError("ThoiGianDuKienRa", "Thời gian dự kiến ra phải lớn hơn thời gian dự kiến vào.");
            }

            ModelState.Remove("PhuongTien");
            ModelState.Remove("ViTriDoXe");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuguixe);

                    // Giải phóng vị trí đỗ nếu trạng thái chuyển về "Đã hủy" hoặc "Đã lấy xe"
                    if (phieuguixe.TrangThai == TrangThaiPhieu.DaHuy || phieuguixe.TrangThai == TrangThaiPhieu.DaLayXe)
                    {
                        var viTri = await _context.ViTriDoXe.FindAsync(phieuguixe.MaViTri);
                        if (viTri != null && viTri.TrangThai == TrangThaiViTri.DangSuDung)
                        {
                            viTri.TrangThai = TrangThaiViTri.ConTrong;
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["ThongBao"] = "Cập nhật phiếu gửi xe thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuGuiXeExists(phieuguixe.MaPhieu)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaPhuongTien"] = new SelectList(_context.PhuongTien, "MaPhuongTien", "BienSoXe", phieuguixe.MaPhuongTien);
            ViewData["MaViTri"] = new SelectList(_context.ViTriDoXe, "MaViTri", "TenViTri", phieuguixe.MaViTri);
            return View(phieuguixe);
        }

        private bool PhieuGuiXeExists(int id)
        {
            return _context.PhieuGuiXe.Any(e => e.MaPhieu == id);
        }
    }
}