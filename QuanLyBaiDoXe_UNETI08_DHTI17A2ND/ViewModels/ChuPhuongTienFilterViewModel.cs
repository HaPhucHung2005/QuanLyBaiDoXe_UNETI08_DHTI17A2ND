// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: 23203100006
// Nội dung thực hiện: Module 3 - ViewModel cho chủ phương tiện
// =================================================================
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
}
