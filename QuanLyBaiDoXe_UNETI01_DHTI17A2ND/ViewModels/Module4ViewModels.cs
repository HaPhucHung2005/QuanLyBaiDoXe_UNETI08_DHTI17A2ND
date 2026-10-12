// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 4 - ViewModel cho quản lý phiếu gửi xe, Dashboard và Thống kê
// =================================================================
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class PhieuGuiXeFilterViewModel
    {
        public List<PhieuGuiXe> DanhSach { get; set; } = new();
        public string? SearchBienSo { get; set; }
        public string? SearchChuXe { get; set; }
        public int? SelectedMaLoai { get; set; }
        public string? SelectedTrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public SelectList? LoaiList { get; set; }
        public SelectList? TrangThaiList { get; set; }
    }

    public class DashboardViewModel
    {
        public int TongLoaiPhuongTien { get; set; }
        public int TongViTri { get; set; }
        public int TongChuPhuongTien { get; set; }
        public int TongPhuongTien { get; set; }
        public int TongPhieu { get; set; }
        public int PhieuChoXacNhan { get; set; }
        public int XeDangGui { get; set; }
        public int ViTriConTrong { get; set; }
        public int ViTriDangSuDung { get; set; }
    }

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
