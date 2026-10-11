// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Entity PhieuGuiXe phục vụ Đăng ký gửi xe & Quản lý phiếu

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class PhieuGuiXe
    {
        [Key]
        [Display(Name = "Mã phiếu")]
        public int MaPhieu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương tiện")]
        [Display(Name = "Phương tiện")]
        public int MaPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vị trí đỗ")]
        [Display(Name = "Vị trí đỗ xe")]
        public int MaViTri { get; set; }

        [Display(Name = "Ngày đăng ký")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến vào")]
        [Display(Name = "Thời gian dự kiến vào")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianDuKienVao { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian dự kiến ra")]
        [Display(Name = "Thời gian dự kiến ra")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianDuKienRa { get; set; }

        [Display(Name = "Thời gian vào thực tế")]
        [DataType(DataType.DateTime)]
        public DateTime? ThoiGianVaoThucTe { get; set; }

        [Display(Name = "Thời gian ra thực tế")]
        [DataType(DataType.DateTime)]
        public DateTime? ThoiGianRaThucTe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá theo giờ")]
        public decimal DonGiaTheoGio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal? ThanhTien { get; set; }

        [StringLength(30)]
        [Display(Name = "Trạng thái phiếu")]
        public string TrangThai { get; set; } = "Chờ xác nhận"; // Các trạng thái: Chờ xác nhận, Đang gửi, Đã lấy xe, Đã hủy

        // Navigation Properties
        [ForeignKey("MaPhuongTien")]
        public virtual PhuongTien? PhuongTien { get; set; }

        [ForeignKey("MaViTri")]
        public virtual ViTriDoXe? ViTriDoXe { get; set; }
    }
}
