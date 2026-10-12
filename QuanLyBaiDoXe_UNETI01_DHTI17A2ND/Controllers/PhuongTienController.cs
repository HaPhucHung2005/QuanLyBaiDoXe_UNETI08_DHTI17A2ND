// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Controller Quản lý Phương tiện (Module 3)
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

public class PhuongTienController : Controller
{
    private readonly ApplicationDbContext _context;

    public PhuongTienController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PhuongTien (Admin xem tất cả + tìm kiếm | Khách hàng xem xe của mình)
    public async Task<IActionResult> Index(string? searchBienSo, string? searchChu, int? selectedMaLoai, string? trangThai, int pageIndex = 1)
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");

        if (maTaiKhoan == null)
        {
            return RedirectToAction("Login", "TaiKhoan");
        }

        var query = _context.PhuongTien
            .Include(p => p.ChuPhuongTien)
            .Include(p => p.LoaiPhuongTien)
            .AsQueryable();

        // Nếu là Khách hàng: Lấy đúng xe thuộc về chủ phương tiện đăng nhập
        if (vaiTro != "Admin")
        {
            var chuPhuongTien = await _context.ChuPhuongTien
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan.Value);

            if (chuPhuongTien == null)
            {
                return NotFound("Không tìm thấy thông tin chủ phương tiện của bạn.");
            }

            query = query.Where(p => p.MaChuPhuongTien == chuPhuongTien.MaChuPhuongTien);
        }
        else
        {
            // Bộ lọc nâng cao dành cho Admin
            if (!string.IsNullOrWhiteSpace(searchChu))
            {
                var termChu = searchChu.Trim().ToLower();
                query = query.Where(p => p.ChuPhuongTien!.HoTen.ToLower().Contains(termChu)
                                      || p.ChuPhuongTien.SoDienThoai.Contains(termChu));
            }
        }

        // 1. Lọc theo Biển số xe hoặc Nhãn hiệu
        if (!string.IsNullOrWhiteSpace(searchBienSo))
        {
            var termBS = searchBienSo.Trim().ToLower();
            query = query.Where(p => p.BienSoXe.ToLower().Contains(termBS)
                                  || (p.NhanHieu != null && p.NhanHieu.ToLower().Contains(termBS)));
        }

        // 2. Lọc theo Loại phương tiện
        if (selectedMaLoai.HasValue && selectedMaLoai.Value > 0)
        {
            query = query.Where(p => p.MaLoaiPhuongTien == selectedMaLoai.Value);
        }

        // 3. Lọc theo Trạng thái hoạt động
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            if (trangThai == "hoatdong")
            {
                query = query.Where(p => p.TrangThai == TrangThaiHoatDong.HoatDong);
            }
            else if (trangThai == "tamngung")
            {
                query = query.Where(p => p.TrangThai == TrangThaiHoatDong.TamNgung);
            }
        }

        var dsPhuongTien = await query.OrderByDescending(p => p.MaPhuongTien).ToListAsync();

        // Lấy danh sách loại phương tiện nạp vào DropdownFilter
        var dsLoaiXe = await _context.LoaiPhuongTien
            .Where(l => l.TrangThai == TrangThaiHoatDong.HoatDong)
            .ToListAsync();

        var viewModel = new PhuongTienFilterViewModel
        {
            DanhSach = dsPhuongTien,
            SearchBienSo = searchBienSo,
            SearchChu = searchChu,
            SelectedMaLoai = selectedMaLoai,
            TrangThai = trangThai,
            PageIndex = pageIndex,
            TotalPages = 1,
            LoaiList = new SelectList(dsLoaiXe, "MaLoaiPhuongTien", "TenLoaiPhuongTien", selectedMaLoai)
        };

        ViewBag.Keyword = searchBienSo;

        return View(viewModel);
    }

    // GET: PhuongTien/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var phuongtien = await _context.PhuongTien
            .Include(p => p.ChuPhuongTien)
            .Include(p => p.LoaiPhuongTien)
            .FirstOrDefaultAsync(m => m.MaPhuongTien == id.Value);

        if (phuongtien == null)
        {
            return NotFound();
        }

        // Phân quyền: Khách hàng không được xem chi tiết xe người khác
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            var chuPhuongTien = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
            if (chuPhuongTien == null || phuongtien.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
            {
                return Forbid();
            }
        }

        return View(phuongtien);
    }

    // GET: PhuongTien/Create
    public async Task<IActionResult> Create()
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        if (maTaiKhoan == null)
        {
            return RedirectToAction("Login", "TaiKhoan");
        }

        // Nạp danh sách Loại phương tiện đang hoạt động vào DropdownList
        ViewData["MaLoaiPhuongTien"] = new SelectList(
            await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(),
            "MaLoaiPhuongTien",
            "TenLoaiPhuongTien"
        );

        return View();
    }

    // POST: PhuongTien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaLoaiPhuongTien,BienSoXe,NhanHieu,MauSac,MoTa,TrangThai")] PhuongTien phuongtien)
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
            return NotFound("Không tìm thấy chủ phương tiện.");
        }

        // Kiểm tra nghiệp vụ: Biển số xe không được trùng
        bool isExist = await _context.PhuongTien
            .AnyAsync(p => p.BienSoXe.Trim().ToLower() == phuongtien.BienSoXe.Trim().ToLower());

        if (isExist)
        {
            ModelState.AddModelError("BienSoXe", "Biển số xe này đã tồn tại trong hệ thống.");
        }

        // Gán MaChuPhuongTien từ chủ phương tiện đang đăng nhập
        phuongtien.MaChuPhuongTien = chuPhuongTien.MaChuPhuongTien;

        // Loại bỏ kiểm tra ModelState cho các đối tượng Navigation
        ModelState.Remove("ChuPhuongTien");
        ModelState.Remove("LoaiPhuongTien");
        ModelState.Remove("PhieuGuiXes");

        if (ModelState.IsValid)
        {
            _context.Add(phuongtien);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm phương tiện mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewData["MaLoaiPhuongTien"] = new SelectList(
            await _context.LoaiPhuongTien.Where(l => l.TrangThai == TrangThaiHoatDong.HoatDong).ToListAsync(),
            "MaLoaiPhuongTien",
            "TenLoaiPhuongTien",
            phuongtien.MaLoaiPhuongTien
        );

        return View(phuongtien);
    }

    // GET: PhuongTien/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var phuongtien = await _context.PhuongTien.FindAsync(id.Value);
        if (phuongtien == null)
        {
            return NotFound();
        }

        // Kiểm tra quyền sửa
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");

        if (vaiTro != "Admin")
        {
            var chuPhuongTien = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
            if (chuPhuongTien == null || phuongtien.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
            {
                return Forbid();
            }
        }

        ViewData["MaLoaiPhuongTien"] = new SelectList(
            await _context.LoaiPhuongTien.Where(l => l.TrangThai == TrangThaiHoatDong.HoatDong).ToListAsync(),
            "MaLoaiPhuongTien",
            "TenLoaiPhuongTien",
            phuongtien.MaLoaiPhuongTien
        );
        return View(phuongtien);
    }

    // POST: PhuongTien/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MaPhuongTien,MaChuPhuongTien,MaLoaiPhuongTien,BienSoXe,NhanHieu,MauSac,MoTa,TrangThai")] PhuongTien phuongtien)
    {
        if (id != phuongtien.MaPhuongTien)
        {
            return NotFound();
        }

        // Kiểm tra trùng Biển số xe (Trừ chính nó ra)
        bool isExist = await _context.PhuongTien
            .AnyAsync(p => p.BienSoXe.Trim().ToLower() == phuongtien.BienSoXe.Trim().ToLower() && p.MaPhuongTien != id);

        if (isExist)
        {
            ModelState.AddModelError("BienSoXe", "Biển số xe này đã thuộc về một phương tiện khác.");
        }

        ModelState.Remove("ChuPhuongTien");
        ModelState.Remove("LoaiPhuongTien");
        ModelState.Remove("PhieuGuiXes");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(phuongtien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin phương tiện thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhuongTienExists(phuongtien.MaPhuongTien))
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

        ViewData["MaLoaiPhuongTien"] = new SelectList(
            await _context.LoaiPhuongTien.Where(l => l.TrangThai == TrangThaiHoatDong.HoatDong).ToListAsync(),
            "MaLoaiPhuongTien",
            "TenLoaiPhuongTien",
            phuongtien.MaLoaiPhuongTien
        );

        return View(phuongtien);
    }

    // GET: PhuongTien/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var phuongtien = await _context.PhuongTien
            .Include(p => p.ChuPhuongTien)
            .Include(p => p.LoaiPhuongTien)
            .FirstOrDefaultAsync(m => m.MaPhuongTien == id.Value);

        if (phuongtien == null)
        {
            return NotFound();
        }

        // Kiểm tra quyền xóa
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            var chuPhuongTien = await _context.ChuPhuongTien.FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
            if (chuPhuongTien == null || phuongtien.MaChuPhuongTien != chuPhuongTien.MaChuPhuongTien)
            {
                return Forbid();
            }
        }

        return View(phuongtien);
    }

    // POST: PhuongTien/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var phuongtien = await _context.PhuongTien.FindAsync(id);
        if (phuongtien == null)
        {
            return NotFound();
        }

        // Nghiệp vụ bắt buộc: Không cho phép xóa nếu phương tiện đang có lượt gửi xe hoạt động (Chờ xác nhận hoặc Đang gửi)
        bool isParkingActive = await _context.PhieuGuiXe
            .AnyAsync(p => p.MaPhuongTien == id
                        && (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui));

        if (isParkingActive)
        {
            TempData["Error"] = "Không thể xóa phương tiện này vì đang có lượt gửi xe chưa hoàn tất!";
            return RedirectToAction(nameof(Index));
        }

        _context.PhuongTien.Remove(phuongtien);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa phương tiện thành công!";

        return RedirectToAction(nameof(Index));
    }

    private bool PhuongTienExists(int id)
    {
        return _context.PhuongTien.Any(e => e.MaPhuongTien == id);
    }
}