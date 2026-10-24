// =================================================================
// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Quản lý Loại phương tiện & Phân quyền Admin
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class LoaiPhuongTienController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoaiPhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. DANH SÁCH LOẠI PHƯƠNG TIỆN (ADMIN)
        // =================================================================
        // GET: LoaiPhuongTien
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin mới có quyền truy cập Quản lý loại phương tiện.");

            var dsLoaiXe = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTriDoXe)
                .OrderByDescending(l => l.MaLoaiPhuongTien)
                .ToListAsync();
            return View(dsLoaiXe);
        }

        // =================================================================
        // 2. CHI TIẾT LOẠI PHƯƠNG TIỆN
        // =================================================================
        // GET: LoaiPhuongTien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var loaiphuongtien = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == id);

            if (loaiphuongtien == null) return NotFound();

            return View(loaiphuongtien);
        }

        // =================================================================
        // 3. THÊM MỚI LOẠI PHƯƠNG TIỆN
        // =================================================================
        // GET: LoaiPhuongTien/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();
            return View(new LoaiPhuongTien { TrangThai = true });
        }

        // POST: LoaiPhuongTien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai")] LoaiPhuongTien loaiphuongtien)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            // Kiểm tra trùng tên loại phương tiện
            bool isExist = await _context.LoaiPhuongTien.AnyAsync(l => l.TenLoaiPhuongTien == loaiphuongtien.TenLoaiPhuongTien);
            if (isExist)
            {
                ModelState.AddModelError("TenLoaiPhuongTien", "Tên loại phương tiện này đã tồn tại trong hệ thống.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loaiphuongtien);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Thêm mới loại phương tiện '{loaiphuongtien.TenLoaiPhuongTien}' thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiphuongtien);
        }

        // =================================================================
        // 4. CHỈNH SỬA LOẠI PHƯƠNG TIỆN
        // =================================================================
        // GET: LoaiPhuongTien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(id);
            if (loaiphuongtien == null) return NotFound();

            return View(loaiphuongtien);
        }

        // POST: LoaiPhuongTien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai")] LoaiPhuongTien loaiphuongtien)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id != loaiphuongtien.MaLoaiPhuongTien) return NotFound();

            // Kiểm tra trùng tên loại phương tiện với các bản ghi khác
            bool isExist = await _context.LoaiPhuongTien.AnyAsync(l => l.TenLoaiPhuongTien == loaiphuongtien.TenLoaiPhuongTien && l.MaLoaiPhuongTien != id);
            if (isExist)
            {
                ModelState.AddModelError("TenLoaiPhuongTien", "Tên loại phương tiện này đã trùng với một loại phương tiện khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiphuongtien);
                    await _context.SaveChangesAsync();
                    TempData["ThongBao"] = $"Cập nhật loại phương tiện '{loaiphuongtien.TenLoaiPhuongTien}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiPhuongTienExists(loaiphuongtien.MaLoaiPhuongTien)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(loaiphuongtien);
        }

        // =================================================================
        // 5. XÓA LOẠI PHƯƠNG TIỆN
        // =================================================================
        // GET: LoaiPhuongTien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var loaiphuongtien = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == id);

            if (loaiphuongtien == null) return NotFound();

            return View(loaiphuongtien);
        }

        // POST: LoaiPhuongTien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(id);
            if (loaiphuongtien != null)
            {
                // Ràng buộc nghiệp vụ: Kiểm tra xem có Vị trí đỗ hoặc Phương tiện nào đang dùng loại này không
                bool hasViTri = await _context.ViTriDoXe.AnyAsync(v => v.MaLoaiPhuongTien == id);
                bool hasPhuongTien = await _context.PhuongTien.AnyAsync(p => p.MaLoaiPhuongTien == id);

                if (hasViTri || hasPhuongTien)
                {
                    TempData["Loi"] = $"Không thể xóa '{loaiphuongtien.TenLoaiPhuongTien}' vì đang có Vị trí đỗ hoặc Phương tiện liên quan!";
                    return RedirectToAction(nameof(Index));
                }

                _context.LoaiPhuongTien.Remove(loaiphuongtien);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Xóa loại phương tiện '{loaiphuongtien.TenLoaiPhuongTien}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoaiPhuongTienExists(int id)
        {
            return _context.LoaiPhuongTien.Any(e => e.MaLoaiPhuongTien == id);
        }
    }
}