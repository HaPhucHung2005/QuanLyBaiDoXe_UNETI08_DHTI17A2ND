using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class ViTriDoXeFilterViewModel
    {
        // Danh sách dữ liệu hiển thị trên trang hiện tại
        public List<ViTriDoXe> DanhSachViTri { get; set; } = new ();

        // Các tham số Lưu giữ điều kiện Lọc & Tìm kiếm
        public string? SearchTenViTri { get; set; }
        public string? SearchKhuVuc { get; set; }
        public int? SelectedMaLoaiPhuongTien { get; set; }
        public int? SelectedTang { get; set; }
        public string? SelectedTrangThai { get; set; }
        public string? SortOrder { get; set; }

        // Cấu hình Phân trang
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // SelectList phục vụ hiển thị Dropdown trên View
        public SelectList? LoaiPhuongTienList { get; set; }
        public SelectList? KhuVucList { get; set; }
        public SelectList? TangList { get; set; }
        public SelectList? TrangThaiList { get; set; }
    }
}
