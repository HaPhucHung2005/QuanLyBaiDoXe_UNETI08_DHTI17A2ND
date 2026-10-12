// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 4 - Xử lý trạng thái phiếu: xác nhận xe vào, xác nhận xe ra + tính phí, hủy phiếu
//   Chờ xác nhận -> Đang gửi -> Đã lấy xe ; Chờ xác nhận -> Đã hủy
// =================================================================
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services
{
    public record KetQuaXuLy(bool ThanhCong, string ThongBao);

    public class PhieuGuiXeService
    {
        private readonly ApplicationDbContext _context;
        private readonly DangKyGuiXeService _dangKy;

        public PhieuGuiXeService(ApplicationDbContext context, DangKyGuiXeService dangKy)
        {
            _context = context;
            _dangKy = dangKy;
        }

        private static KetQuaXuLy Loi(string msg) => new(false, msg);

        // ---------------------------------------------------------------
        // XÁC NHẬN XE VÀO: Chờ xác nhận -> Đang gửi
        // ---------------------------------------------------------------
        public async Task<KetQuaXuLy> XacNhanVaoAsync(int maPhieu)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu == null) return Loi("Không tìm thấy phiếu gửi xe.");
            if (phieu.TrangThai == TrangThaiPhieu.DaHuy) return Loi("Phiếu đã hủy, không thể xác nhận xe vào.");
            if (phieu.TrangThai != TrangThaiPhieu.ChoXacNhan)
                return Loi($"Phiếu đang ở trạng thái '{phieu.TrangThai}'. Chỉ phiếu 'Chờ xác nhận' mới được xác nhận xe vào.");

            var now = DateTime.Now;
            if (phieu.ThoiGianDuKienRa <= now)
                return Loi("Phiếu đã quá thời gian dự kiến ra nên không còn hợp lệ. Vui lòng hủy phiếu và đăng ký lại.");

            var pt = phieu.PhuongTien;
            var vt = phieu.ViTriDoXe;
            if (pt == null || vt == null) return Loi("Dữ liệu phương tiện hoặc vị trí của phiếu không còn tồn tại.");

            if (!pt.TrangThai) return Loi("Phương tiện đang ngừng hoạt động.");
            if (vt.TrangThai == TrangThaiViTri.TamNgung) return Loi($"Vị trí {vt.TenViTri} đang tạm ngừng.");
            if (pt.MaLoaiPhuongTien != vt.MaLoaiPhuongTien) return Loi("Loại phương tiện không còn phù hợp với vị trí đỗ.");

            // Không có lượt gửi khác đang chiếm vị trí
            bool viTriBiChiem = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaPhieu != maPhieu && p.MaViTri == phieu.MaViTri && p.TrangThai == TrangThaiPhieu.DangGui);
            if (viTriBiChiem) return Loi($"Vị trí {vt.TenViTri} đang có xe khác chiếm chỗ.");

            // Phương tiện không có lượt gửi khác đang hoạt động
            bool xeDangGui = await _context.PhieuGuiXe.AnyAsync(p =>
                p.MaPhieu != maPhieu && p.MaPhuongTien == phieu.MaPhuongTien && p.TrangThai == TrangThaiPhieu.DangGui);
            if (xeDangGui) return Loi("Phương tiện này đang có một lượt gửi khác ở trạng thái Đang gửi.");

            // Không trùng lịch với phiếu hợp lệ khác trên cùng vị trí
            if (await _dangKy.ViTriBiTrungLichAsync(phieu.MaViTri, phieu.ThoiGianDuKienVao, phieu.ThoiGianDuKienRa, maPhieu))
                return Loi($"Vị trí {vt.TenViTri} bị trùng lịch với một phiếu hợp lệ khác.");

            phieu.ThoiGianVaoThucTe = now;
            phieu.TrangThai = TrangThaiPhieu.DangGui;
            vt.TrangThai = TrangThaiViTri.DangSuDung;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return new KetQuaXuLy(true, $"Đã xác nhận xe {pt.BienSoXe} vào vị trí {vt.TenViTri} lúc {now:dd/MM/yyyy HH:mm}.");
        }

        // ---------------------------------------------------------------
        // XÁC NHẬN XE RA + TÍNH PHÍ: Đang gửi -> Đã lấy xe
        // ---------------------------------------------------------------
        public async Task<KetQuaXuLy> XacNhanRaAsync(int maPhieu)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var phieu = await _context.PhieuGuiXe
                .Include(p => p.PhuongTien)
                .Include(p => p.ViTriDoXe)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu == null) return Loi("Không tìm thấy phiếu gửi xe.");
            if (phieu.TrangThai != TrangThaiPhieu.DangGui)
                return Loi($"Phiếu đang ở trạng thái '{phieu.TrangThai}'. Chỉ phiếu 'Đang gửi' mới được xác nhận xe ra.");
            if (phieu.ThoiGianVaoThucTe == null) return Loi("Phiếu chưa có thời gian vào thực tế.");

            var ra = DateTime.Now;
            if (ra <= phieu.ThoiGianVaoThucTe.Value)
                return Loi("Thời gian ra thực tế phải sau thời gian vào thực tế.");

            int soGio = QuyTacTinhPhi.TinhSoGioTinhPhi(phieu.ThoiGianVaoThucTe.Value, ra);
            decimal thanhTien = soGio * phieu.DonGiaTheoGio; // ThanhTien = SoGioTinhPhi x DonGiaTheoGio

            phieu.ThoiGianRaThucTe = ra;
            phieu.ThanhTien = thanhTien;
            phieu.TrangThai = TrangThaiPhieu.DaLayXe;

            // Trả vị trí về "Còn trống" nếu không còn xe khác chiếm và vị trí không bị Admin tạm ngừng
            var vt = phieu.ViTriDoXe;
            if (vt != null && vt.TrangThai == TrangThaiViTri.DangSuDung)
            {
                bool conXeKhac = await _context.PhieuGuiXe.AnyAsync(p =>
                    p.MaPhieu != maPhieu && p.MaViTri == phieu.MaViTri && p.TrangThai == TrangThaiPhieu.DangGui);
                if (!conXeKhac) vt.TrangThai = TrangThaiViTri.ConTrong;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return new KetQuaXuLy(true,
                $"Xe {phieu.PhuongTien?.BienSoXe} đã ra. Tính phí {soGio} giờ x {phieu.DonGiaTheoGio:N0} = {thanhTien:N0} VNĐ.");
        }

        // ---------------------------------------------------------------
        // HỦY PHIẾU: chỉ hủy được phiếu "Chờ xác nhận"
        // ---------------------------------------------------------------
        public async Task<KetQuaXuLy> HuyAsync(int maPhieu)
        {
            var phieu = await _context.PhieuGuiXe.FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);
            if (phieu == null) return Loi("Không tìm thấy phiếu gửi xe.");

            if (phieu.TrangThai == TrangThaiPhieu.DaHuy) return Loi("Phiếu này đã được hủy trước đó.");
            if (phieu.TrangThai == TrangThaiPhieu.DangGui)
                return Loi("Xe đang gửi trong bãi, không thể hủy. Hãy xác nhận xe ra để hoàn tất phiếu.");
            if (phieu.TrangThai == TrangThaiPhieu.DaLayXe) return Loi("Phiếu đã hoàn tất (Đã lấy xe), không thể hủy.");

            phieu.TrangThai = TrangThaiPhieu.DaHuy;
            await _context.SaveChangesAsync();
            return new KetQuaXuLy(true, $"Đã hủy phiếu #{phieu.MaPhieu}.");
        }
    }
}
