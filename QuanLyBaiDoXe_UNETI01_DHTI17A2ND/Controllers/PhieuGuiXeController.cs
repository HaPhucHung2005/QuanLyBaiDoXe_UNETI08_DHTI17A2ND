// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - [MSSV]
// Nội dung thực hiện: Controller Đăng ký và Quản lý Phiếu gửi xe (Module 3)
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
    public class PhieuGuiXeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuGuiXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PhieuGuiXe (Lịch sử gửi xe cho Khách hàng | Quản lý cho Admin)
        public async Task<IActionResult> Index(string? trangThai,int pageIndex = 1)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (maTaiKhoan == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var query = _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .AsQueryable();

            // Nếu là Khách hàng: Chỉ xem phiếu gửi thuộc phương tiện của chính mình
            if (vaiTro != "Admin")
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan.Value);

                if (chuPhuongTien == null)
                {
                    return NotFound("Không tìm thấy thông tin chủ phương tiện.");
                }

                query = query.Where(p => p.PhuongTien!.MaChuPhuongTien == chuPhuongTien.MaChuPhuongTien);
            }

            // Thống kê số lượng phiếu theo từng trạng thái (Trước khi lọc theo trangThai)
            var soLuongTheoTrangThai = await query
                .GroupBy(p => p.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.TrangThai, x => x.SoLuong);

            // Lọc theo trạng thái nếu có chọn filter
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            var dsPhieu = await query.OrderByDescending(p => p.NgayDangKy).ToListAsync();

            // 💡 ĐÓNG GÓI VÀO ViewModel TRƯỚC KHI TRẢ VỀ VIEW
            var viewModel = new LichSuGuiXeViewModel
            {
                DanhSach = dsPhieu,
                TrangThai = trangThai,
                SoLuongTheoTrangThai = soLuongTheoTrangThai,
                PageIndex = pageIndex,
                TotalPages = 1
            };

            ViewBag.CurrentTrangThai = trangThai;

            // SỬA DÒNG NÀY: Trả về viewModel thay vì dsPhieu/query
            return View(viewModel);
        }

        // GET: PhieuGuiXe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuguixe = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaPhieu == id.Value);

            if (phieuguixe == null)
            {
                return NotFound();
            }

            // Phân quyền: Khách hàng chỉ xem được chi tiết phiếu của mình
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin")
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);

                if (chuPhuongTien == null || phieuguixe.PhuongTien?.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
                {
                    return Forbid();
                }
            }

            return View(phieuguixe);
        }

        // GET: PhieuGuiXe/Create (Đăng ký gửi xe)
        public async Task<IActionResult> Create()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var chuPhuongTien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan.Value);

            if (chuPhuongTien == null)
            {
                return NotFound("Không tìm thấy thông tin chủ phương tiện của bạn.");
            }

            // Lấy danh sách phương tiện đang hoạt động của khách hàng
            var dsXeCuaToi = await _context.PhuongTien
                .Where(p => p.MaChuPhuongTien == chuPhuongTien.MaChuPhuongTien && p.TrangThai)
                .ToListAsync();

            ViewData["MaPhuongTien"] = new SelectList(dsXeCuaToi, "MaPhuongTien", "BienSoXe");

            // Khởi tạo thời gian dự kiến mặc định
            var defaultModel = new PhieuGuiXe
            {
                ThoiGianDuKienVao = DateTime.Now,
                ThoiGianDuKienRa = DateTime.Now.AddHours(2),
            };

            return View(defaultModel);
        }

        // API Ajax: Lấy danh sách Vị trí đỗ phù hợp với loại phương tiện đã chọn
        [HttpGet]
        public async Task<JsonResult> GetViTriByPhuongTien(int maPhuongTien)
        {
            var xe = await _context.PhuongTien.FindAsync(maPhuongTien);
            if (xe == null)
            {
                return Json(null);
            }

            var listViTri = await _context.ViTriDoXe
                .Where(v => v.MaLoaiPhuongTien == xe.MaLoaiPhuongTien && v.TrangThai != "Tạm ngừng")
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
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var chuPhuongTien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan.Value);

            if (chuPhuongTien == null || !chuPhuongTien.TrangThai)
            {
                ModelState.AddModelError("", "Tài khoản hoặc chủ phương tiện đang ở trạng thái bị khóa.");
            }

            var xe = await _context.PhuongTien.FindAsync(phieuguixe.MaPhuongTien);
            if (xe == null || xe.MaChuPhuongTien != chuPhuongTien?.MaChuPhuongTien)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện không hợp lệ hoặc không thuộc về bạn.");
            }

            // 1. Kiểm tra nghiệp vụ thời gian: Thời gian dự kiến ra phải sau thời gian dự kiến vào
            if (phieuguixe.ThoiGianDuKienRa <= phieuguixe.ThoiGianDuKienVao)
            {
                ModelState.AddModelError("ThoiGianDuKienRa", "Thời gian dự kiến ra phải lớn hơn thời gian dự kiến vào.");
            }

            // 2. Kiểm tra phương tiện không có 2 lượt gửi active đồng thời (Chờ xác nhận hoặc Đang gửi)
            bool isVehicleActive = await _context.PhieuGuiXe
                .AnyAsync(p => p.MaPhuongTien == phieuguixe.MaPhuongTien
                            && (p.TrangThai == "Chờ xác nhận" || p.TrangThai == "Đang gửi"));

            if (isVehicleActive)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện này đang có 1 lượt gửi xe chưa hoàn tất!");
            }

            // 3. NGHIỆP VỤ BẮT BUỘC (Mục 7.5): Kiểm tra vị trí đỗ bị trùng khoảng thời gian đăng ký bằng LINQ
            bool isLocationOverlap = await _context.PhieuGuiXe
                .AnyAsync(p => p.MaViTri == phieuguixe.MaViTri
                            && p.TrangThai != "Đã hủy"
                            && p.TrangThai != "Đã lấy xe"
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
                // Lấy đơn giá theo giờ tự động từ LoaiPhuongTien
                var loaiXe = await _context.LoaiPhuongTien.FindAsync(xe!.MaLoaiPhuongTien);

                phieuguixe.NgayDangKy = DateTime.Now;
                phieuguixe.DonGiaTheoGio = loaiXe?.DonGiaTheoGio ?? 0;
                phieuguixe.TrangThai = "Chờ xác nhận";

                _context.Add(phieuguixe);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký gửi xe thành công! Vui lòng chờ xác nhận từ Admin.";
                return RedirectToAction(nameof(Index));
            }

            var dsXeCuaToi = await _context.PhuongTien
                .Where(p => p.MaChuPhuongTien == chuPhuongTien!.MaChuPhuongTien && p.TrangThai)
                .ToListAsync();

            ViewData["MaPhuongTien"] = new SelectList(dsXeCuaToi, "MaPhuongTien", "BienSoXe", phieuguixe.MaPhuongTien);
            return View(phieuguixe);
        }

        // GET: PhieuGuiXe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuguixe = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaPhieu == id.Value);

            if (phieuguixe == null)
            {
                return NotFound();
            }

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin")
            {
                var chuPhuongTien = await _context.ChuPhuongTien
                    .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);

                if (chuPhuongTien == null || phieuguixe.PhuongTien?.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
                {
                    return Forbid();
                }
            }

            return View(phieuguixe);
        }

        // POST: PhieuGuiXe/Delete/5 (Khách hàng có thể hủy phiếu khi còn ở trạng thái Chờ xác nhận)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var phieu = await _context.PhieuGuiXe.FindAsync(id);
            if (phieu == null)
            {
                return NotFound();
            }

            if (phieu.TrangThai != "Chờ xác nhận")
            {
                TempData["Error"] = "Chỉ được phép hủy phiếu đang ở trạng thái 'Chờ xác nhận'!";
                return RedirectToAction(nameof(Index));
            }

            phieu.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã hủy phiếu gửi xe thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: PhieuGuiXe/Edit/5 (Quản trị viên chỉnh sửa thông tin/trạng thái phiếu)
        public async Task<IActionResult> Edit(int? id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var phieuguixe = await _context.PhieuGuiXe.FindAsync(id.Value);
            if (phieuguixe == null)
            {
                return NotFound();
            }

            ViewData["MaPhuongTien"] = new SelectList(_context.PhuongTien, "MaPhuongTien", "BienSoXe", phieuguixe.MaPhuongTien);
            ViewData["MaViTri"] = new SelectList(_context.ViTriDoXe, "MaViTri", "TenViTri", phieuguixe.MaViTri);
            return View(phieuguixe);
        }

        // POST: PhieuGuiXe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhieu,MaPhuongTien,MaViTri,NgayDangKy,ThoiGianDuKienVao,ThoiGianDuKienRa,ThoiGianVaoThucTe,ThoiGianRaThucTe,DonGiaTheoGio,ThanhTien,TrangThai")] PhieuGuiXe phieuguixe)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction(nameof(Index));
            }

            if (id != phieuguixe.MaPhieu)
            {
                return NotFound();
            }

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
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật phiếu gửi xe thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuGuiXeExists(phieuguixe.MaPhieu))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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