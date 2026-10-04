// =================================================================
// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Quản lý Loại phương tiện & Phân quyền Admin
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class LoaiPhuongTienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiPhuongTienController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return !string.IsNullOrEmpty(vaiTro) && vaiTro == "Admin";
        }

        // GET: LoaiPhuongTien
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");
            return View(await _context.LoaiPhuongTien.Include(l => l.DanhSachViTriDoXe).ToListAsync());
        }

        // GET: LoaiPhuongTien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            if (id == null)
            {
                return NotFound();
            }

            var loaiphuongtien = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == id);

            if (loaiphuongtien == null)
            {
                return NotFound();
            }

            return View(loaiphuongtien);
        }

        // GET: LoaiPhuongTien/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");
            return View();
        }

        // POST: LoaiPhuongTien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai")] LoaiPhuongTien loaiphuongtien)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            if (await _context.LoaiPhuongTien.AnyAsync(l => l.TenLoaiPhuongTien == loaiphuongtien.TenLoaiPhuongTien))
            {
                ModelState.AddModelError("TenLoaiPhuongTien", "Tên loại phương tiện đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loaiphuongtien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiphuongtien);
        }

        // GET: LoaiPhuongTien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            if (id == null)
            {
                return NotFound();
            }

            var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(id);
            if (loaiphuongtien == null)
            {
                return NotFound();
            }
            return View(loaiphuongtien);
        }

        // POST: LoaiPhuongTien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai")] LoaiPhuongTien loaiphuongtien)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            if (id != loaiphuongtien.MaLoaiPhuongTien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiphuongtien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiPhuongTienExists(loaiphuongtien.MaLoaiPhuongTien))
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
            return View(loaiphuongtien);
        }

        // GET: LoaiPhuongTien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            if (id == null)
            {
                return NotFound();
            }

            var loaiphuongtien = await _context.LoaiPhuongTien
                .Include(l => l.DanhSachViTriDoXe)
                .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == id);

            if (loaiphuongtien == null)
            {
                return NotFound();
            }

            return View(loaiphuongtien);
        }

        // POST: LoaiPhuongTien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");

            var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(id);
            if (loaiphuongtien != null)
            {
                _context.LoaiPhuongTien.Remove(loaiphuongtien);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoaiPhuongTienExists(int id)
        {
            return _context.LoaiPhuongTien.Any(e => e.MaLoaiPhuongTien == id);
        }
    }
}