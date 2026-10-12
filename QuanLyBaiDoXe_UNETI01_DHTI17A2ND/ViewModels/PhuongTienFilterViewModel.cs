// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - ViewModel cho phương tiện
// =================================================================
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class PhuongTienFilterViewModel
    {
        public List<PhuongTien> DanhSach { get; set; } = new();
        public string? SearchBienSo { get; set; }
        public string? SearchChu { get; set; }          // Dùng cho Admin tìm theo tên/SĐT chủ xe
        public int? SelectedMaLoai { get; set; }
        public string? TrangThai { get; set; }
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public SelectList? LoaiList { get; set; }
    }
}
