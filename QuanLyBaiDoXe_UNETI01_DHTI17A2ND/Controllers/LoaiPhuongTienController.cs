// =================================================================
// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Quản lý Loại phương tiện & Phân quyền Admin
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;

public class LoaiPhuongTienController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoaiPhuongTienController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsAdmin() => HttpContext.Session.GetString("VaiTro") == "Admin";

    // GET: LOAIPHUONGTIENS
    public async Task<IActionResult> Index()    
    {
        if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");
        return View(await _context.LoaiPhuongTien.ToListAsync());
    }

    // GET: LOAIPHUONGTIENS/Details/5
    public async Task<IActionResult> Details(int? maloaiphuongtien)
    {
        if (maloaiphuongtien == null)
        {
            return NotFound();
        }

        var loaiphuongtien = await _context.LoaiPhuongTien
            .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == maloaiphuongtien);
        if (loaiphuongtien == null)
        {
            return NotFound();
        }

        return View(loaiphuongtien);
    }

    // GET: LOAIPHUONGTIENS/Create
    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "TaiKhoan");
        return View();
    }

    // POST: LOAIPHUONGTIENS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai,DanhSachViTriDoXe")] LoaiPhuongTien loaiphuongtien)
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

    // GET: LOAIPHUONGTIENS/Edit/5
    public async Task<IActionResult> Edit(int? maloaiphuongtien)
    {
        if (maloaiphuongtien == null)
        {
            return NotFound();
        }

        var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(maloaiphuongtien);
        if (loaiphuongtien == null)
        {
            return NotFound();
        }
        return View(loaiphuongtien);
    }

    // POST: LOAIPHUONGTIENS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? maloaiphuongtien, [Bind("MaLoaiPhuongTien,TenLoaiPhuongTien,DonGiaTheoGio,MoTa,TrangThai,DanhSachViTriDoXe")] LoaiPhuongTien loaiphuongtien)
    {
        if (maloaiphuongtien != loaiphuongtien.MaLoaiPhuongTien)
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

    // GET: LOAIPHUONGTIENS/Delete/5
    public async Task<IActionResult> Delete(int? maloaiphuongtien)
    {
        if (maloaiphuongtien == null)
        {
            return NotFound();
        }

        var loaiphuongtien = await _context.LoaiPhuongTien
            .FirstOrDefaultAsync(m => m.MaLoaiPhuongTien == maloaiphuongtien);
        if (loaiphuongtien == null)
        {
            return NotFound();
        }

        return View(loaiphuongtien);
    }

    // POST: LOAIPHUONGTIENS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? maloaiphuongtien)
    {
        var loaiphuongtien = await _context.LoaiPhuongTien.FindAsync(maloaiphuongtien);
        if (loaiphuongtien != null)
        {
            _context.LoaiPhuongTien.Remove(loaiphuongtien);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LoaiPhuongTienExists(int? maloaiphuongtien)
    {
        return _context.LoaiPhuongTien.Any(e => e.MaLoaiPhuongTien == maloaiphuongtien);
    }
}
