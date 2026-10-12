// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Module 3 - Entity ChuPhuongTien và Data Annotations
// =================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class ChuPhuongTien : IValidatableObject
    {
        [Key]
        public int MaChuPhuongTien { get; set; }

        // Quan hệ 1-1 với TaiKhoan (cấu hình trong Module34Configuration)
        [Display(Name = "Tài khoản")]
        public int MaTaiKhoan { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam"; // Nam / Nữ / Khác

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ (vd: 0912345678)")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200, ErrorMessage = "Địa chỉ không quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; } = null!;

        // Do hệ thống gán, người dùng không nhập
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Display(Name = "Hoạt động")]
        public bool TrangThai { get; set; } = true;

        public virtual ICollection<PhuongTien>? DanhSachPhuongTien { get; set; }

        // Ràng buộc ngày sinh hợp lệ (không phù hợp để dùng Data Annotation đơn thuần)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgaySinh.Date >= DateTime.Today)
                yield return new ValidationResult("Ngày sinh phải nhỏ hơn ngày hiện tại", new[] { nameof(NgaySinh) });
            else if (NgaySinh.Year < 1900)
                yield return new ValidationResult("Ngày sinh không hợp lệ", new[] { nameof(NgaySinh) });
        }
    }
}
