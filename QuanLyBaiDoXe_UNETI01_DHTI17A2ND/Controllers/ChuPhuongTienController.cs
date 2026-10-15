// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Controller Quản lý Chủ phương tiện (Module 3)
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

public class ChuPhuongTienController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChuPhuongTienController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CHUPHUONGTIENS (Admin xem danh sách & Tìm kiếm)
    public async Task<IActionResult> Index(string? keyword, string? trangThai, int pageIndex = 1)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        var query = _context.ChuPhuongTien
            .Include(c => c.TaiKhoan)
            .AsQueryable();

        // 1. Lọc theo từ khóa tìm kiếm (Họ tên, SĐT, Email)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(c => c.HoTen.ToLower().Contains(term)
                                  || c.SoDienThoai.Contains(term)
                                  || (c.Email != null && c.Email.ToLower().Contains(term)));
        }

        // 2. Lọc theo Trạng thái (trangThai là string nhận từ QueryString/URL)
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            // Chuyển chuỗi tham số về chữ thường để so sánh
            string val = trangThai.Trim().ToLowerInvariant();

            if (val == "true" || val == "1" || val == "hoatdong")
            {
                query = query.Where(c => c.TrangThai == true);
            }
            else if (val == "false" || val == "0" || val == "khoa" || val == "dakhoa")
            {
                query = query.Where(c => c.TrangThai == false);
            }
        }

        var dsChuPhuongTien = await query
            .OrderByDescending(c => c.MaChuPhuongTien)
            .ToListAsync();

        var viewModel = new ChuPhuongTienFilterViewModel
        {
            DanhSach = dsChuPhuongTien,
            Search = keyword,
            TrangThai = trangThai,
            PageIndex = pageIndex,
            TotalPages = 1
        };

        ViewBag.Keyword = keyword;
        ViewBag.TrangThai = trangThai;
        return View(viewModel);
    }

    // GET: CHUPHUONGTIENS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        var maTaiKhoanSession = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTroSession = HttpContext.Session.GetString("VaiTro");

        if (maTaiKhoanSession == null)
        {
            return RedirectToAction("Login", "TaiKhoan");
        }

        ChuPhuongTien? chuPhuongTien;

        if (vaiTroSession == "Admin" && id.HasValue)
        {
            chuPhuongTien = await _context.ChuPhuongTien
                .Include(c => c.TaiKhoan)
                .Include(c => c.PhuongTiens)
                .FirstOrDefaultAsync(m => m.MaChuPhuongTien == id.Value);
        }
        else
        {
            chuPhuongTien = await _context.ChuPhuongTien
                .Include(c => c.TaiKhoan)
                .Include(c => c.PhuongTiens)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == maTaiKhoanSession.Value);
        }

        if (chuPhuongTien == null)
        {
            return NotFound();
        }

        return View(chuPhuongTien);
    }

    // GET: CHUPHUONGTIENS/Create
    public IActionResult Create()
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["MaTaiKhoan"] = new SelectList(
            _context.TaiKhoan.Where(t => t.VaiTro == "Khách hàng"),
            "MaTaiKhoan",
            "TenDangNhap"
        );

        return View();
    }

    // POST: CHUPHUONGTIENS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaChuPhuongTien,MaTaiKhoan,HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi,NgayDangKy,TrangThai,TaiKhoan,PhuongTiens")] ChuPhuongTien chuphuongtien)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        // Loại bỏ Validation cho Navigation properties
        ModelState.Remove("TaiKhoan");
        ModelState.Remove("PhuongTiens");

        // Kiểm tra trùng Số điện thoại
        bool phoneExists = await _context.ChuPhuongTien.AnyAsync(c => c.SoDienThoai == chuphuongtien.SoDienThoai);
        if (phoneExists)
        {
            ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được đăng ký bởi chủ phương tiện khác.");
        }

        if (ModelState.IsValid)
        {
            chuphuongtien.NgayDangKy = DateTime.Now;
            _context.Add(chuphuongtien);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm mới chủ phương tiện thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewData["MaTaiKhoan"] = new SelectList(
            _context.TaiKhoan.Where(t => t.VaiTro == "Khách hàng"),
            "MaTaiKhoan",
            "TenDangNhap",
            chuphuongtien.MaTaiKhoan
        );

        return View(chuphuongtien);
    }

    // GET: CHUPHUONGTIENS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        var maTaiKhoanSession = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTroSession = HttpContext.Session.GetString("VaiTro");

        if (maTaiKhoanSession == null)
        {
            return RedirectToAction("Login", "TaiKhoan");
        }

        ChuPhuongTien? chuphuongtien;

        if (vaiTroSession == "Admin" && id.HasValue)
        {
            chuphuongtien = await _context.ChuPhuongTien.FindAsync(id.Value);
        }
        else
        {
            chuphuongtien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoanSession.Value);
        }

        if (chuphuongtien == null)
        {
            return NotFound();
        }

        ViewData["MaTaiKhoan"] = new SelectList(
            _context.TaiKhoan.Where(t => t.VaiTro == "Khách hàng"),
            "MaTaiKhoan",
            "TenDangNhap",
            chuphuongtien.MaTaiKhoan
        );

        return View(chuphuongtien);
    }

    // POST: CHUPHUONGTIENS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("MaChuPhuongTien,MaTaiKhoan,HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi,NgayDangKy,TrangThai,TaiKhoan,PhuongTiens")] ChuPhuongTien chuphuongtien)
    {
        if (id != chuphuongtien.MaChuPhuongTien)
        {
            return NotFound();
        }

        ModelState.Remove("TaiKhoan");
        ModelState.Remove("PhuongTiens");

        // Kiểm tra trùng SĐT với chủ phương tiện khác
        bool phoneExists = await _context.ChuPhuongTien
            .AnyAsync(c => c.SoDienThoai == chuphuongtien.SoDienThoai && c.MaChuPhuongTien != id);

        if (phoneExists)
        {
            ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi chủ phương tiện khác.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(chuphuongtien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin chủ phương tiện thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChuPhuongTienExists(chuphuongtien.MaChuPhuongTien))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var vaiTroSession = HttpContext.Session.GetString("VaiTro");
            if (vaiTroSession == "Admin")
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Details));
        }

        ViewData["MaTaiKhoan"] = new SelectList(
            _context.TaiKhoan.Where(t => t.VaiTro == "Khách hàng"),
            "MaTaiKhoan",
            "TenDangNhap",
            chuphuongtien.MaTaiKhoan
        );

        return View(chuphuongtien);
    }

    // GET: CHUPHUONGTIENS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var chuphuongtien = await _context.ChuPhuongTien
            .Include(c => c.TaiKhoan)
            .Include(c => c.PhuongTiens)
            .FirstOrDefaultAsync(m => m.MaChuPhuongTien == id.Value);

        if (chuphuongtien == null)
        {
            return NotFound();
        }

        return View(chuphuongtien);
    }

    // POST: CHUPHUONGTIENS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        var chuphuongtien = await _context.ChuPhuongTien
            .Include(c => c.PhuongTiens)
            .FirstOrDefaultAsync(c => c.MaChuPhuongTien == id);

        if (chuphuongtien != null)
        {
            // Kiểm tra ràng buộc: Không cho xóa nếu chủ phương tiện đang có xe trong CSDL
            if (chuphuongtien.PhuongTiens != null && chuphuongtien.PhuongTiens.Any())
            {
                TempData["Error"] = "Không thể xóa chủ phương tiện này vì họ đang sở hữu phương tiện trong hệ thống!";
                return RedirectToAction(nameof(Index));
            }

            _context.ChuPhuongTien.Remove(chuphuongtien);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa chủ phương tiện thành công!";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ChuPhuongTienExists(int id)
    {
        return _context.ChuPhuongTien.Any(e => e.MaChuPhuongTien == id);
    }
}
