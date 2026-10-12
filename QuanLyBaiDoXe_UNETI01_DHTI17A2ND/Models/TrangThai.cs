// =================================================================
// Dùng chung Module 3 và Module 4
// Họ và tên: Trần Ngọc Minh - Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Hằng số trạng thái phiếu gửi xe và trạng thái vị trí đỗ
//                     (tránh viết chuỗi "cứng" rải rác trong Controller/Service)
// =================================================================
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public static class TrangThaiPhieu
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DangGui = "Đang gửi";
        public const string DaLayXe = "Đã lấy xe";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa = { ChoXacNhan, DangGui, DaLayXe, DaHuy };
    }

    public static class TrangThaiViTri
    {
        public const string ConTrong = "Còn trống";
        public const string DangSuDung = "Đang sử dụng";
        public const string TamNgung = "Tạm ngừng";
    }
}
