// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Entity PhuongTien cho Module 3

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class PhuongTien
    {
        [Key]
        [Display(Name = "Mã phương tiện")]
        public int MaPhuongTien { get; set; }

        [Required]
        [Display(Name = "Chủ phương tiện")]
        public int MaChuPhuongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        [Display(Name = "Loại phương tiện")]
        public int MaLoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Biển số xe không được để trống")]
        [StringLength(20, ErrorMessage = "Biển số xe không vượt quá 20 ký tự")]
        [Display(Name = "Biển số xe")]
        public string BienSoXe { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Nhãn hiệu không vượt quá 50 ký tự")]
        [Display(Name = "Nhãn hiệu")]
        public string? NhanHieu { get; set; }

        [StringLength(30, ErrorMessage = "Màu sắc không vượt quá 30 ký tự")]
        [Display(Name = "Màu sắc")]
        public string? MauSac { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation Properties - Khoa ngoai
        // Quan he lien quan
        [ForeignKey("MaChuPhuongTien")]
        public virtual ChuPhuongTien? ChuPhuongTien { get; set; }

        [ForeignKey("MaLoaiPhuongTien")]
        public virtual LoaiPhuongTien? LoaiPhuongTien { get; set; }

        public virtual ICollection<PhieuGuiXe> PhieuGuiXes { get; set; } = new List<PhieuGuiXe>();
    }
}
