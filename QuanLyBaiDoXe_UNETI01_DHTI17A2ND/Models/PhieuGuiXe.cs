// =================================================================
// Họ và tên: Trần Ngọc Minh - Mã sinh viên: [điền MSSV]
//   Nội dung: Module 3 - Entity PhieuGuiXe (thông tin đăng ký)
// Họ và tên: Trần Ngọc Minh - Mã sinh viên: [điền MSSV]
//   Nội dung: Module 4 - Các trường thực tế vào/ra, đơn giá, thành tiền, trạng thái xử lý
// =================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class PhieuGuiXe
    {
        [Key]
        public int MaPhieu { get; set; }

        [Display(Name = "Phương tiện")]
        public int MaPhuongTien { get; set; }

        [ForeignKey("MaPhuongTien")]
        public virtual PhuongTien? PhuongTien { get; set; }

        [Display(Name = "Vị trí đỗ")]
        public int MaViTri { get; set; }

        [ForeignKey("MaViTri")]
        public virtual ViTriDoXe? ViTriDoXe { get; set; }

        // Do hệ thống xác định, không cho người dùng nhập
        [Display(Name = "Ngày đăng ký")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Dự kiến vào")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ThoiGianDuKienVao { get; set; }

        [Display(Name = "Dự kiến ra")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ThoiGianDuKienRa { get; set; }

        [Display(Name = "Vào thực tế")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ThoiGianVaoThucTe { get; set; }

        [Display(Name = "Ra thực tế")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ThoiGianRaThucTe { get; set; }

        // Lấy từ LoaiPhuongTien tại thời điểm đăng ký (lưu lại để đổi giá sau này không ảnh hưởng phiếu cũ)
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá / giờ")]
        public decimal DonGiaTheoGio { get; set; }

        // Do hệ thống tính khi xác nhận xe ra; null nghĩa là chưa tính
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal? ThanhTien { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiPhieu.ChoXacNhan;
    }
}
