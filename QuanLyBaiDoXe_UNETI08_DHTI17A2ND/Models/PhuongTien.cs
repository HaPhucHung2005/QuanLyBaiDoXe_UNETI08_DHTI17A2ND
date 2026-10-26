// =================================================================
// Họ và tên: Trần Ngọc Minh
// Mã sinh viên: 23203100006
// Nội dung thực hiện: Module 3 - Entity PhuongTien và Data Annotations
// =================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class PhuongTien
    {
        [Key]
        [Display(Name = "Mã phương tiện")]
        public int MaPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chủ phương tiện")]
        [Display(Name = "Chủ phương tiện")]
        public int MaChuPhuongTien { get; set; }

        [ForeignKey("MaChuPhuongTien")]
        public virtual ChuPhuongTien? ChuPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại phương tiện")]
        [Display(Name = "Loại phương tiện")]
        public int MaLoaiPhuongTien { get; set; }

        [ForeignKey("MaLoaiPhuongTien")]
        public virtual LoaiPhuongTien? LoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Biển số xe không được để trống")]
        [StringLength(20, ErrorMessage = "Biển số xe không quá 20 ký tự")]
        [Display(Name = "Biển số xe")]
        public string BienSoXe { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Nhãn hiệu không quá 50 ký tự")]
        [Display(Name = "Nhãn hiệu")]
        public string? NhanHieu { get; set; }

        [StringLength(30, ErrorMessage = "Màu sắc không quá 30 ký tự")]
        [Display(Name = "Màu sắc")]
        public string? MauSac { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;

        // Navigation Property - Quan hệ 1-N với PhieuGuiXe (CHỈ GIỮ LẠI 1 THUỘC TÍNH NÀY)
        public virtual ICollection<PhieuGuiXe> PhieuGuiXes { get; set; } = new List<PhieuGuiXe>();
    }
}