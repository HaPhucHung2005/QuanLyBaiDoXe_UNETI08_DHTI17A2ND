// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - ViewModel cho chủ phương tiện, phương tiện, đăng ký gửi xe, lịch sử gửi xe
// =================================================================
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class ChuPhuongTienFilterViewModel
    {
        public List<ChuPhuongTien> DanhSach { get; set; } = new();
        public string? Search { get; set; }
        public string? TrangThai { get; set; }   // "hoatdong" | "khoa"
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
    }

    public class PhuongTienFilterViewModel
    {
        public List<PhuongTien> DanhSach { get; set; } = new();
        public string? SearchBienSo { get; set; }
        public string? SearchChu { get; set; }          // chỉ Admin
        public int? SelectedMaLoai { get; set; }
        public string? TrangThai { get; set; }
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public SelectList? LoaiList { get; set; }
    }

    public class DangKyGuiXeViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn phương tiện")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phương tiện")]
        [Display(Name = "Phương tiện")]
        public int MaPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vị trí đỗ")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn vị trí đỗ")]
        [Display(Name = "Vị trí đỗ")]
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến vào")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian dự kiến vào")]
        public DateTime ThoiGianDuKienVao { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến ra")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian dự kiến ra")]
        public DateTime ThoiGianDuKienRa { get; set; }

        // Chỉ để hiển thị (không bind) - đơn giá do hệ thống lấy từ LoaiPhuongTien
        public List<PhuongTien> DanhSachPhuongTien { get; set; } = new();
        public List<ViTriDoXe> DanhSachViTri { get; set; } = new();
    }

    public class LichSuGuiXeViewModel
    {
        public List<PhieuGuiXe> DanhSach { get; set; } = new();
        public string? TrangThai { get; set; }
        public Dictionary<string, int> SoLuongTheoTrangThai { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
