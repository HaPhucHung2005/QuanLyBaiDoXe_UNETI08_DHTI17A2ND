// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - Nghiệp vụ đăng ký gửi xe: kiểm tra chủ/phương tiện/vị trí/loại xe,
//                     kiểm tra lượt gửi đang hoạt động và trùng khoảng thời gian bằng EF Core + LINQ
// =================================================================
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services
{
    public class DangKyGuiXeService
    {
        private readonly ApplicationDbContext _context;

        public DangKyGuiXeService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vị trí đã có phiếu HỢP LỆ (Chờ xác nhận / Đang gửi) giao nhau về thời gian hay chưa.
        /// Hai khoảng [a1,a2) và [b1,b2) giao nhau khi a1 &lt; b2 và b1 &lt; a2.
        /// Phiếu Đã hủy / Đã lấy xe không được tính.
        /// </summary>
        public Task<bool> ViTriBiTrungLichAsync(int maViTri, DateTime vao, DateTime ra, int? boQuaMaPhieu = null)
        {
            return _context.PhieuGuiXe.AnyAsync(p =>
                p.MaViTri == maViTri
                && (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui)
                && (boQuaMaPhieu == null || p.MaPhieu != boQuaMaPhieu)
                && p.ThoiGianDuKienVao < ra
                && vao < p.ThoiGianDuKienRa);
        }

        /// <summary>Phương tiện đang có lượt gửi hoạt động (Chờ xác nhận / Đang gửi) hay chưa.</summary>
        public Task<bool> PhuongTienDangCoLuotGuiAsync(int maPhuongTien, int? boQuaMaPhieu = null)
        {
            return _context.PhieuGuiXe.AnyAsync(p =>
                p.MaPhuongTien == maPhuongTien
                && (p.TrangThai == TrangThaiPhieu.ChoXacNhan || p.TrangThai == TrangThaiPhieu.DangGui)
                && (boQuaMaPhieu == null || p.MaPhieu != boQuaMaPhieu));
        }

        /// <summary>Trả về danh sách lỗi nghiệp vụ; rỗng nghĩa là hợp lệ.</summary>
        public async Task<List<string>> KiemTraAsync(int maTaiKhoan, int maPhuongTien, int maViTri, DateTime vao, DateTime ra)
        {
            var loi = new List<string>();

            // 1. Chủ phương tiện đang hoạt động
            var chu = await _context.ChuPhuongTien.AsNoTracking()
                .FirstOrDefaultAsync(c => c.MaTaiKhoan == maTaiKhoan);
            if (chu == null)
            {
                loi.Add("Bạn chưa cập nhật thông tin chủ phương tiện. Vui lòng cập nhật thông tin cá nhân trước.");
                return loi;
            }
            if (!chu.TrangThai) loi.Add("Tài khoản chủ phương tiện đang bị khóa, không thể đăng ký gửi xe.");

            // 2. Phương tiện thuộc đúng khách hàng đang đăng nhập và đang hoạt động
            var pt = await _context.PhuongTien.AsNoTracking()
                .FirstOrDefaultAsync(p => p.MaPhuongTien == maPhuongTien);
            bool ptHopLe = pt != null && pt.MaChuPhuongTien == chu.MaChuPhuongTien;
            if (!ptHopLe) loi.Add("Phương tiện không tồn tại hoặc không thuộc tài khoản của bạn.");
            else if (!pt!.TrangThai) loi.Add("Phương tiện đang ngừng hoạt động.");

            // 3. Vị trí tồn tại và không Tạm ngừng
            var vt = await _context.ViTriDoXe.AsNoTracking()
                .FirstOrDefaultAsync(v => v.MaViTri == maViTri);
            if (vt == null) loi.Add("Vị trí đỗ không tồn tại.");
            else if (vt.TrangThai == TrangThaiViTri.TamNgung) loi.Add($"Vị trí {vt.TenViTri} đang tạm ngừng hoạt động.");

            // 4. Loại phương tiện phù hợp với vị trí
            if (ptHopLe && vt != null && pt!.MaLoaiPhuongTien != vt.MaLoaiPhuongTien)
                loi.Add("Loại phương tiện không phù hợp với vị trí đỗ đã chọn.");

            // 5. Thời gian
            bool thoiGianHopLe = true;
            if (ra <= vao)
            {
                loi.Add("Thời gian dự kiến ra phải sau thời gian dự kiến vào.");
                thoiGianHopLe = false;
            }
            if (vao < DateTime.Now.AddMinutes(-5))
            {
                loi.Add("Thời gian dự kiến vào không được ở trong quá khứ.");
                thoiGianHopLe = false;
            }

            // 6. Một phương tiện không có hai lượt gửi hoạt động đồng thời
            if (ptHopLe && await PhuongTienDangCoLuotGuiAsync(maPhuongTien))
                loi.Add("Phương tiện này đang có một lượt gửi xe chưa hoàn tất (Chờ xác nhận hoặc Đang gửi).");

            // 7. Vị trí không trùng khoảng thời gian với phiếu hợp lệ khác
            if (vt != null && thoiGianHopLe && await ViTriBiTrungLichAsync(maViTri, vao, ra))
                loi.Add($"Vị trí {vt.TenViTri} đã có phiếu gửi xe khác trong khoảng thời gian này.");

            return loi;
        }

        /// <summary>Kiểm tra + tạo phiếu trong một transaction để hạn chế đăng ký trùng khi nhiều người cùng bấm.</summary>
        public async Task<(PhieuGuiXe? Phieu, List<string> Loi)> TaoPhieuAsync(
            int maTaiKhoan, int maPhuongTien, int maViTri, DateTime vao, DateTime ra)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var loi = await KiemTraAsync(maTaiKhoan, maPhuongTien, maViTri, vao, ra);
            if (loi.Count > 0) return (null, loi);

            // Đơn giá lấy từ LoaiPhuongTien, không cho khách hàng tự nhập
            var pt = await _context.PhuongTien.Include(p => p.LoaiPhuongTien)
                .FirstAsync(p => p.MaPhuongTien == maPhuongTien);

            var phieu = new PhieuGuiXe
            {
                MaPhuongTien = maPhuongTien,
                MaViTri = maViTri,
                NgayDangKy = DateTime.Now,           // hệ thống xác định
                ThoiGianDuKienVao = vao,
                ThoiGianDuKienRa = ra,
                DonGiaTheoGio = pt.LoaiPhuongTien!.DonGiaTheoGio,
                ThanhTien = null,                    // tính khi xe ra
                TrangThai = TrangThaiPhieu.ChoXacNhan
            };
            _context.PhieuGuiXe.Add(phieu);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return (phieu, loi);
        }
    }
}
