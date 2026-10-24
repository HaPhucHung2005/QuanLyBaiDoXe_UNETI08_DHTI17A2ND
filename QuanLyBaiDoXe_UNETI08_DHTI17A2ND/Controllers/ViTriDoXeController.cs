// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - MSSV
// Nội dung thực hiện: Module 2 - Quản lý Vị trí đỗ xe, LINQ Tìm kiếm, Lọc, Sắp xếp và Phân trang
// =================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class ViTriDoXeController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ViTriDoXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. TÌM KIẾM + LỌC + SẮP XẾP + PHÂN TRANG (MODULE 2)
        // =================================================================
        // GET: ViTriDoXe
        public async Task<IActionResult> Index(
            string? searchTenViTri,
            string? searchKhuVuc,
            int? selectedMaLoaiPhuongTien,
            int? selectedTang,
            string? selectedTrangThai,
            string? sortOrder,
            int pageIndex = 1)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            int pageSize = 5; // Số bản ghi hiển thị trên một trang

            // Query gốc kèm Navigation Property để hiển thị Tên loại xe
            var query = _context.ViTriDoXe.Include(v => v.LoaiPhuongTien).AsQueryable();

            // 1. Tìm kiếm LINQ theo Tên vị trí và Khu vực
            if (!string.IsNullOrWhiteSpace(searchTenViTri))
            {
                query = query.Where(v => v.TenViTri.Contains(searchTenViTri));
            }
            if (!string.IsNullOrWhiteSpace(searchKhuVuc))
            {
                query = query.Where(v => v.KhuVuc.Contains(searchKhuVuc));
            }

            // 2. Lọc LINQ kết hợp
            if (selectedMaLoaiPhuongTien.HasValue && selectedMaLoaiPhuongTien > 0)
            {
                query = query.Where(v => v.MaLoaiPhuongTien == selectedMaLoaiPhuongTien.Value);
            }
            if (selectedTang.HasValue)
            {
                query = query.Where(v => v.Tang == selectedTang.Value);
            }
            if (!string.IsNullOrWhiteSpace(selectedTrangThai))
            {
                query = query.Where(v => v.TrangThai == selectedTrangThai);
            }

            // 3. Sắp xếp LINQ
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(v => v.TenViTri),
                "tang_asc" => query.OrderBy(v => v.Tang),
                "tang_desc" => query.OrderByDescending(v => v.Tang),
                _ => query.OrderBy(v => v.TenViTri), // Mặc định: Tên A -> Z
            };

            // 4. Phân trang LINQ Skip / Take trên Database
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, totalPages > 0 ? totalPages : 1));

            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            // Đóng gói vào ViTriDoXeFilterViewModel
            var model = new ViTriDoXeFilterViewModel
            {
                DanhSachViTri = items,
                SearchTenViTri = searchTenViTri,
                SearchKhuVuc = searchKhuVuc,
                SelectedMaLoaiPhuongTien = selectedMaLoaiPhuongTien,
                SelectedTang = selectedTang,
                SelectedTrangThai = selectedTrangThai,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                LoaiPhuongTienList = new SelectList(await _context.LoaiPhuongTien.Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", selectedMaLoaiPhuongTien),
                KhuVucList = new SelectList(await _context.ViTriDoXe.Select(v => v.KhuVuc).Distinct().ToListAsync(), searchKhuVuc),
                TangList = new SelectList(await _context.ViTriDoXe.Select(v => v.Tang).Distinct().ToListAsync(), selectedTang),
                TrangThaiList = new SelectList(TrangThaiViTri.TatCa, selectedTrangThai)
            };

            return View(model);
        }

        // =================================================================
        // 2. CHI TIẾT VỊ TRÍ ĐỖ XE
        // =================================================================
        // GET: ViTriDoXe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();

            if (id == null) return NotFound();

            var vitridoxe = await _context.ViTriDoXe
                .Include(v => v.LoaiPhuongTien)
                .FirstOrDefaultAsync(m => m.MaViTri == id);

            if (vitridoxe == null) return NotFound();

            return View(vitridoxe);
        }

        // =================================================================
        // 3. THÊM MỚI VỊ TRÍ ĐỖ XE (ADMIN)
        // =================================================================
        // GET: ViTriDoXe/Create (Admin)
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Quản trị viên mới có quyền thêm mới vị trí đỗ.");

            ViewData["MaLoaiPhuongTien"] = new SelectList(await _context.LoaiPhuongTien
                .Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien");

            return View(new ViTriDoXe { TrangThai = TrangThaiViTri.ConTrong, Tang = 1 });
        }

        // POST: ViTriDoXe/Create (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaViTri,TenViTri,MaLoaiPhuongTien,KhuVuc,Tang,TrangThai,MoTa")] ViTriDoXe vitridoxe)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            // Ràng buộc nghiệp vụ: Tên vị trí không trùng trong cùng khu vực
            if (await _context.ViTriDoXe.AnyAsync(v => v.TenViTri == vitridoxe.TenViTri && v.KhuVuc == vitridoxe.KhuVuc))
            {
                ModelState.AddModelError("TenViTri", "Tên vị trí đỗ xe trong khu vực này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(vitridoxe);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Thêm vị trí đỗ xe '{vitridoxe.TenViTri}' ({vitridoxe.KhuVuc}) thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoaiPhuongTien"] = new SelectList(await _context.LoaiPhuongTien
                .Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", vitridoxe.MaLoaiPhuongTien);

            return View(vitridoxe);
        }

        // =================================================================
        // 4. CHỈNH SỬA VỊ TRÍ ĐỖ XE (ADMIN)
        // =================================================================
        // GET: ViTriDoXe/Edit/5 (Admin)
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var vitridoxe = await _context.ViTriDoXe.FindAsync(id);
            if (vitridoxe == null) return NotFound();

            ViewData["MaLoaiPhuongTien"] = new SelectList(await _context.LoaiPhuongTien
                .Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", vitridoxe.MaLoaiPhuongTien);

            return View(vitridoxe);
        }

        // POST: ViTriDoXe/Edit/5 (Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaViTri,TenViTri,MaLoaiPhuongTien,KhuVuc,Tang,TrangThai,MoTa")] ViTriDoXe vitridoxe)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id != vitridoxe.MaViTri) return NotFound();

            // Ràng buộc nghiệp vụ: Tên vị trí không trùng với vị trí khác trong cùng khu vực
            if (await _context.ViTriDoXe.AnyAsync(v => v.TenViTri == vitridoxe.TenViTri && v.KhuVuc == vitridoxe.KhuVuc && v.MaViTri != id))
            {
                ModelState.AddModelError("TenViTri", "Tên vị trí đỗ xe trong khu vực này đã bị trùng.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vitridoxe);
                    await _context.SaveChangesAsync();
                    TempData["ThongBao"] = $"Cập nhật vị trí đỗ xe '{vitridoxe.TenViTri}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViTriDoXeExists(vitridoxe.MaViTri)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoaiPhuongTien"] = new SelectList(await _context.LoaiPhuongTien
                .Where(l => l.TrangThai).ToListAsync(), "MaLoaiPhuongTien", "TenLoaiPhuongTien", vitridoxe.MaLoaiPhuongTien);

            return View(vitridoxe);
        }

        // =================================================================
        // 5. XÓA VỊ TRÍ ĐỖ XE (ADMIN)
        // =================================================================
        // GET: ViTriDoXe/Delete/5 (Admin)
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            if (id == null) return NotFound();

            var vitridoxe = await _context.ViTriDoXe
                .Include(v => v.LoaiPhuongTien)
                .FirstOrDefaultAsync(m => m.MaViTri == id);

            if (vitridoxe == null) return NotFound();

            return View(vitridoxe);
        }

        // POST: ViTriDoXe/Delete/5 (Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen();

            var vitridoxe = await _context.ViTriDoXe.FindAsync(id);
            if (vitridoxe != null)
            {
                // Ràng buộc nghiệp vụ: Kiểm tra xem vị trí này có phiếu gửi xe nào liên quan không
                bool hasPhieuGui = await _context.PhieuGuiXe.AnyAsync(p => p.MaViTri == id);
                if (hasPhieuGui)
                {
                    TempData["Loi"] = $"Không thể xóa vị trí '{vitridoxe.TenViTri}' vì đã có Lịch sử gửi xe liên quan!";
                    return RedirectToAction(nameof(Index));
                }

                _context.ViTriDoXe.Remove(vitridoxe);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Xóa vị trí đỗ xe '{vitridoxe.TenViTri}' thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ViTriDoXeExists(int id)
        {
            return _context.ViTriDoXe.Any(e => e.MaViTri == id);
        }
    }
}