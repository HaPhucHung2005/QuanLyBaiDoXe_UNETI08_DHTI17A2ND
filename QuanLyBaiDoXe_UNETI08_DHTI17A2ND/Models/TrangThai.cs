// =================================================================
// Dùng chung toan bo Module
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - 23203100006
// Nội dung thực hiện: Hằng số trạng thái phiếu gửi xe và trạng thái vị trí đỗ
//                     (tránh viết chuỗi "cứng" rải rác trong Controller/Service)
// =================================================================

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    /// <summary>
    /// Trạng thái hoạt động của Loại phương tiện (Module 1) và Phương tiện (Module 3)
    /// </summary>
    /// 
    public static class TrangThaiHoatDong
    {
        public const bool HoatDong = true;
        public const bool TamNgung = false;
    }
    /// <summary>
    /// Trạng thái Phiếu gửi xe (Module 3)
    /// </summary>
    public static class TrangThaiPhieu
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DangGui = "Đang gửi";
        public const string DaLayXe = "Đã lấy xe";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa = { ChoXacNhan, DangGui, DaLayXe, DaHuy };
    }
    /// <summary>
    /// Trạng thái Vị trí đỗ xe (Module 2)
    /// </summary>
    public static class TrangThaiViTri
    {
        public const string ConTrong = "Còn trống";
        public const string DangSuDung = "Đang sử dụng";
        public const string TamNgung = "Tạm ngừng";
        public static readonly string[] TatCa = { ConTrong, DangSuDung, TamNgung };
    }
    /// <summary>
    /// Trạng thái Tài khoản / Chủ phương tiện (Module 3)
    /// </summary>
    public static class TrangThaiTaiKhoan
    {
        public const bool DangHoatDong = true;
        public const bool DaKhoa = false;
    }
}
