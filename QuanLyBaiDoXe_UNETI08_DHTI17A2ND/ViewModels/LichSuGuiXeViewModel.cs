// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: 23203100006
// Nội dung thực hiện: Module 3 - ViewModel cho lịch sử gửi xe
// =================================================================

using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class LichSuGuiXeViewModel
    {
        public List<PhieuGuiXe> DanhSach { get; set; } = new();
        public string? TrangThai { get; set; }
        public Dictionary<string, int> SoLuongTheoTrangThai { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
