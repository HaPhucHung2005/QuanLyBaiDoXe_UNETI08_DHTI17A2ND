// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: 23203100006
// Nội dung thực hiện: Module 4 - ViewModel cho Thống kê
// =================================================================

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public record ThongKeItem(string Nhan, int SoLuong);
    public record ThongKeBang(string TieuDe, List<ThongKeItem> Du);
    public record ThongKeDoanhThu(string Thang, int SoPhieu, decimal DoanhThu);
    public class ThongKeViewModel
    {
        public List<ThongKeItem> PhuongTienTheoLoai { get; set; } = new();
        public List<ThongKeItem> ViTriTheoTrangThai { get; set; } = new();
        public List<ThongKeItem> PhieuTheoTrangThai { get; set; } = new();
        public List<ThongKeItem> LuotGuiTheoLoai { get; set; } = new();
        public List<ThongKeItem> LuotGuiTheoViTri { get; set; } = new();
        public List<ThongKeItem> ViTriNhieuNhat { get; set; } = new();   // có thể nhiều vị trí đồng hạng
        public List<ThongKeItem> LuotGuiTheoChu { get; set; } = new();
        public List<ThongKeItem> LuotGuiTheoThang { get; set; } = new();
        public List<ThongKeDoanhThu> DoanhThuTheoThang { get; set; } = new();
        public decimal TongDoanhThu { get; set; }
    }
}
