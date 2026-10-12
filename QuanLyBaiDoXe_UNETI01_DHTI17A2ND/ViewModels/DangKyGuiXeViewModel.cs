// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - ViewModel cho đăng ký gửi xe
// =================================================================
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
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
        public DateTime ThoiGianDuKienVao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến ra")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian dự kiến ra")]
        public DateTime ThoiGianDuKienRa { get; set; } = DateTime.Now.AddHours(2);

        // Danh sách nạp dữ liệu hiển thị trên Form
        public List<PhuongTien> DanhSachPhuongTien { get; set; } = new();
        public List<ViTriDoXe> DanhSachViTri { get; set; } = new();
    }

}
