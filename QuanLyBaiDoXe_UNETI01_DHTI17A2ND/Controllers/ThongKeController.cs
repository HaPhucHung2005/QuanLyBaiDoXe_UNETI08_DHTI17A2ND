// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 4 - Dashboard và Thống kê bằng LINQ (Count, Sum, GroupBy, OrderBy, OrderByDescending)
// Quy ước: "lượt gửi" = phiếu đã thực sự có xe vào bãi (trạng thái Đang gửi hoặc Đã lấy xe).
// =================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Controllers
{
    public class ThongKeController : AuthControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ThongKe  (Dashboard)
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin được xem Dashboard.");

            var vm = new DashboardViewModel
            {
                TongLoaiPhuongTien = await _context.LoaiPhuongTien.CountAsync(),
                TongViTri = await _context.ViTriDoXe.CountAsync(),
                TongChuPhuongTien = await _context.ChuPhuongTien.CountAsync(),
                TongPhuongTien = await _context.PhuongTien.CountAsync(),
                TongPhieu = await _context.PhieuGuiXe.CountAsync(),
                PhieuChoXacNhan = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == TrangThaiPhieu.ChoXacNhan),
                XeDangGui = await _context.PhieuGuiXe.CountAsync(p => p.TrangThai == TrangThaiPhieu.DangGui),
                ViTriConTrong = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == TrangThaiViTri.ConTrong),
                ViTriDangSuDung = await _context.ViTriDoXe.CountAsync(v => v.TrangThai == TrangThaiViTri.DangSuDung)
            };
            return View(vm);
        }

        // GET: ThongKe/ThongKe
        public async Task<IActionResult> ThongKe()
        {
            if (!IsLoggedIn) return ChuaDangNhap();
            if (!IsAdmin) return KhongCoQuyen("Chỉ Admin được xem thống kê.");

            // Lượt gửi = đã có xe vào bãi
            var luotGui = _context.PhieuGuiXe.Where(p =>
                p.TrangThai == TrangThaiPhieu.DangGui || p.TrangThai == TrangThaiPhieu.DaLayXe);

            var vm = new ThongKeViewModel();

            // 1. Số phương tiện theo từng loại (truy vấn con, loại chưa có xe vẫn hiện 0)
            vm.PhuongTienTheoLoai = (await _context.LoaiPhuongTien
                .Select(l => new
                {
                    l.TenLoaiPhuongTien,
                    SoLuong = _context.PhuongTien.Count(p => p.MaLoaiPhuongTien == l.MaLoaiPhuongTien)
                })
                .OrderByDescending(x => x.SoLuong).ToListAsync())
                .Select(x => new ThongKeItem(x.TenLoaiPhuongTien, x.SoLuong)).ToList();

            // 2. Số vị trí theo từng trạng thái
            vm.ViTriTheoTrangThai = (await _context.ViTriDoXe
                .GroupBy(v => v.TrangThai)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong).ToListAsync())
                .Select(x => new ThongKeItem(x.Nhan, x.SoLuong)).ToList();

            // 3. Số phiếu theo từng trạng thái
            vm.PhieuTheoTrangThai = (await _context.PhieuGuiXe
                .GroupBy(p => p.TrangThai)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong).ToListAsync())
                .Select(x => new ThongKeItem(x.Nhan, x.SoLuong)).ToList();

            // 4. Số lượt gửi theo từng loại phương tiện
            vm.LuotGuiTheoLoai = (await luotGui
                .GroupBy(p => p.PhuongTien!.LoaiPhuongTien!.TenLoaiPhuongTien)
                .Select(g => new { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong).ToListAsync())
                .Select(x => new ThongKeItem(x.Nhan, x.SoLuong)).ToList();

            // 5. Số lượt gửi theo từng vị trí (tên vị trí chỉ duy nhất trong khu vực nên nhóm theo mã)
            vm.LuotGuiTheoViTri = (await luotGui
                .GroupBy(p => new { p.MaViTri, p.ViTriDoXe!.TenViTri, p.ViTriDoXe.KhuVuc })
                .Select(g => new { g.Key.TenViTri, g.Key.KhuVuc, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong).ThenBy(x => x.TenViTri).ToListAsync())
                .Select(x => new ThongKeItem($"{x.TenViTri} ({x.KhuVuc})", x.SoLuong)).ToList();

            // 6. Vị trí có số lượt sử dụng nhiều nhất (lấy tất cả vị trí đồng hạng)
            if (vm.LuotGuiTheoViTri.Count > 0)
            {
                int max = vm.LuotGuiTheoViTri.Max(x => x.SoLuong);
                vm.ViTriNhieuNhat = vm.LuotGuiTheoViTri.Where(x => x.SoLuong == max).ToList();
            }

            // 7. Số lượt gửi theo từng chủ phương tiện
            vm.LuotGuiTheoChu = (await luotGui
                .GroupBy(p => new { p.PhuongTien!.MaChuPhuongTien, p.PhuongTien.ChuPhuongTien!.HoTen })
                .Select(g => new { g.Key.HoTen, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong).ThenBy(x => x.HoTen).ToListAsync())
                .Select(x => new ThongKeItem(x.HoTen, x.SoLuong)).ToList();

            // 8. Số lượt gửi theo tháng (tính theo thời gian vào thực tế)
            vm.LuotGuiTheoThang = (await luotGui
                .Where(p => p.ThoiGianVaoThucTe != null)
                .GroupBy(p => new { p.ThoiGianVaoThucTe!.Value.Year, p.ThoiGianVaoThucTe.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, SoLuong = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync())
                .Select(x => new ThongKeItem($"{x.Month:D2}/{x.Year}", x.SoLuong)).ToList();

            // 9. Doanh thu theo tháng từ các phiếu Đã lấy xe (tính theo thời gian ra thực tế)
            vm.DoanhThuTheoThang = (await _context.PhieuGuiXe
                .Where(p => p.TrangThai == TrangThaiPhieu.DaLayXe && p.ThoiGianRaThucTe != null)
                .GroupBy(p => new { p.ThoiGianRaThucTe!.Value.Year, p.ThoiGianRaThucTe.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    SoPhieu = g.Count(),
                    DoanhThu = g.Sum(p => p.ThanhTien ?? 0m)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync())
                .Select(x => new ThongKeDoanhThu($"{x.Month:D2}/{x.Year}", x.SoPhieu, x.DoanhThu)).ToList();

            vm.TongDoanhThu = vm.DoanhThuTheoThang.Sum(x => x.DoanhThu);
            return View(vm);
        }
    }
}
