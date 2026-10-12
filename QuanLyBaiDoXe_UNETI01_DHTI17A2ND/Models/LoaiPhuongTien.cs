// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 1 - Entity LoaiPhuongTien và Data Annotations

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class LoaiPhuongTien
    {
        [Key]
        public int MaLoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Tên loại phương tiện không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên loại phương tiện")]
        public string TenLoaiPhuongTien { get; set; } = null!;

        [Required(ErrorMessage = "Đơn giá theo giờ không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá theo giờ phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá theo giờ (VNĐ)")]
        public decimal DonGiaTheoGio { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        //Quan he 1-n Loai Phuong Tien voi Vi Tri Do Xe
        public virtual ICollection<ViTriDoXe> DanhSachViTriDoXe { get; set; } = new List<ViTriDoXe>();
    }
}