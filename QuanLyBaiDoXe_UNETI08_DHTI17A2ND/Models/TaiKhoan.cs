// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Entity TaiKhoan và Data Annotations
using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không quá 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 - 100 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Vui lòng nhập họ tên không quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "Khách hàng"; //Khach hang hoac Admin

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; //true: Hoat Dong; false: Khoa
    }
}
