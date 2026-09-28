// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Module 2 - Entity ViTriDoXe và Data Annotations

using System.ComponentModel.DataAnnotations;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models
{
    public class ViTriDoXe
    {
        [Key]
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Tên vị trí không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        [Display(Name = "Loại phương tiện")]
        public int MaLoaiPhuongTien { get; set; }

        // Lập khóa ngoại
        [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
        [Display(Name = "Loại phương tiện")]
        public virtual LoaiPhuongTien? LoaiPhuongTien { get; set; }

        [Required(ErrorMessage = "Khu vực không được để trống")]
        [StringLength(50)]
        [Display(Name = "Khu vực")]
        public string KhuVuc { get; set; } = null!;

        [Required(ErrorMessage = "Tầng không được để trống")]
        [Range(1, 100, ErrorMessage = "Tầng phải từ 1 trở lên")]
        [Display(Name = "Tầng")]
        public int Tang { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Còn trống"; // "Còn trống", "Đang sử dụng", "Tạm ngừng"

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }
    }
}
