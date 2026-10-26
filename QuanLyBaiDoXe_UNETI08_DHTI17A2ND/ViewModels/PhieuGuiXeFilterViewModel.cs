// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: 23203100006
// Nội dung thực hiện: Module 4 - ViewModel cho quản lý phiếu gửi xe
// =================================================================

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class PhieuGuiXeFilterViewModel
    {
        public List<PhieuGuiXe> DanhSach { get; set; } = new List<PhieuGuiXe>();
        public string? SearchBienSo { get; set; }
        public string? SearchChuXe { get; set; }
        public int? SelectedMaLoai { get; set; }
        public string? SelectedTrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public SelectList? LoaiList { get; set; }
        public SelectList? TrangThaiList { get; set; }
    }
}
