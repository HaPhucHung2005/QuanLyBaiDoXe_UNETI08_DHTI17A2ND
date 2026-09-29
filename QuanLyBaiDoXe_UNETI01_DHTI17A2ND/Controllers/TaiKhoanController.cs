// =================================================================
// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Đăng nhập, Đăng xuất, Lưu Session và Kiểm tra Phân quyền
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
public class TaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaiKhoanController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TaiKhoan/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: TaiKhoan/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Kiểm tra tài khoản bằng EF Core và LINQ
        var user = await _context.TaiKhoan
            .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap && u.MatKhau == model.MatKhau);

        if (user == null)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
            return View(model);
        }

        if (!user.TrangThai)
        {
            ModelState.AddModelError("", "Tài khoản của bạn hiện đang bị khóa.");
            return View(model);
        }

        // Lưu thông tin vào Session
        HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
        HttpContext.Session.SetString("HoTen", user.HoTen);
        HttpContext.Session.SetString("VaiTro", user.VaiTro);

        if (user.VaiTro == "Admin")
            return RedirectToAction("Index", "LoaiPhuongTien");

        return RedirectToAction("Index", "ViTriDoXe");
    }

    // GET: TAIKHOANS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.TaiKhoan.ToListAsync());
    }

    // GET: TAIKHOANS/Details/5
    public async Task<IActionResult> Details(int? mataikhoan)
    {
        if (mataikhoan == null)
        {
            return NotFound();
        }

        var taikhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(m => m.MaTaiKhoan == mataikhoan);
        if (taikhoan == null)
        {
            return NotFound();
        }

        return View(taikhoan);
    }

    // GET: TAIKHOANS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TAIKHOANS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaTaiKhoan,TenDangNhap,MatKhau,HoTen,Email,VaiTro,TrangThai")] TaiKhoan taikhoan)
    {
        if (ModelState.IsValid)
        {
            _context.Add(taikhoan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(taikhoan);
    }

    // GET: TAIKHOANS/Edit/5
    public async Task<IActionResult> Edit(int? mataikhoan)
    {
        if (mataikhoan == null)
        {
            return NotFound();
        }

        var taikhoan = await _context.TaiKhoan.FindAsync(mataikhoan);
        if (taikhoan == null)
        {
            return NotFound();
        }
        return View(taikhoan);
    }

    // POST: TAIKHOANS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? mataikhoan, [Bind("MaTaiKhoan,TenDangNhap,MatKhau,HoTen,Email,VaiTro,TrangThai")] TaiKhoan taikhoan)
    {
        if (mataikhoan != taikhoan.MaTaiKhoan)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(taikhoan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaiKhoanExists(taikhoan.MaTaiKhoan))
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
        return View(taikhoan);
    }

    // GET: TAIKHOANS/Delete/5
    public async Task<IActionResult> Delete(int? mataikhoan)
    {
        if (mataikhoan == null)
        {
            return NotFound();
        }

        var taikhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(m => m.MaTaiKhoan == mataikhoan);
        if (taikhoan == null)
        {
            return NotFound();
        }

        return View(taikhoan);
    }

    // POST: TAIKHOANS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? mataikhoan)
    {
        var taikhoan = await _context.TaiKhoan.FindAsync(mataikhoan);
        if (taikhoan != null)
        {
            _context.TaiKhoan.Remove(taikhoan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TaiKhoanExists(int? mataikhoan)
    {
        return _context.TaiKhoan.Any(e => e.MaTaiKhoan == mataikhoan);
    }

    // GET: TaiKhoan/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
