// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 4 - Quy tắc làm tròn giờ và công thức tính phí (dùng thống nhất toàn hệ thống)
// =================================================================
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Services
{
    /// <summary>
    /// Quy tắc nhóm thống nhất:
    ///   SoGioTinhPhi = làm tròn LÊN theo giờ (đủ hay lẻ đều tính 1 giờ), tối thiểu 1 giờ
    ///   ThanhTien    = SoGioTinhPhi x DonGiaTheoGio (đơn giá lấy từ dữ liệu hệ thống)
    /// Ví dụ: gửi 3 giờ 45 phút -> 4 giờ; gửi 20 phút -> 1 giờ.
    /// </summary>
    public static class QuyTacTinhPhi
    {
        public static int TinhSoGioTinhPhi(DateTime vao, DateTime ra)
        {
            var soGio = (ra - vao).TotalHours;
            return (int)Math.Max(1, Math.Ceiling(soGio));
        }

        public static decimal TinhThanhTien(DateTime vao, DateTime ra, decimal donGiaTheoGio)
            => TinhSoGioTinhPhi(vao, ra) * donGiaTheoGio;
    }
}
