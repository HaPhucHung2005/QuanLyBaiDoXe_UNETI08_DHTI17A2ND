// =================================================================
// Dùng chung Module 3 và Module 4
// Họ và tên: Trần Ngọc Minh - Mã sinh viên: [điền MSSV]
// Nội dung thực hiện: Cấu hình quan hệ, khóa ngoại, chỉ mục duy nhất cho
//                     ChuPhuongTien, PhuongTien, PhieuGuiXe
// =================================================================
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data
{
    public static class Module34Configuration
    {
        public static void CauHinhModule34(this ModelBuilder modelBuilder)
        {
            // TaiKhoan (1) - (1) ChuPhuongTien
            modelBuilder.Entity<ChuPhuongTien>()
                .HasOne(c => c.TaiKhoan)
                .WithOne()
                .HasForeignKey<ChuPhuongTien>(c => c.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // ChuPhuongTien (1) - (n) PhuongTien
            modelBuilder.Entity<PhuongTien>()
                .HasOne(p => p.ChuPhuongTien)
                .WithMany(c => c.DanhSachPhuongTien)
                .HasForeignKey(p => p.MaChuPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // LoaiPhuongTien (1) - (n) PhuongTien
            modelBuilder.Entity<PhuongTien>()
                .HasOne(p => p.LoaiPhuongTien)
                .WithMany()
                .HasForeignKey(p => p.MaLoaiPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // Biển số xe không được trùng (ràng buộc ở cả tầng CSDL)
            modelBuilder.Entity<PhuongTien>()
                .HasIndex(p => p.BienSoXe)
                .IsUnique();

            // PhuongTien (1) - (n) PhieuGuiXe
            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.PhuongTien)
                .WithMany(x => x.DanhSachPhieuGuiXe)
                .HasForeignKey(p => p.MaPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // ViTriDoXe (1) - (n) PhieuGuiXe
            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.ViTriDoXe)
                .WithMany()
                .HasForeignKey(p => p.MaViTri)
                .OnDelete(DeleteBehavior.Restrict);

            // Hỗ trợ truy vấn kiểm tra trùng lịch / lượt gửi đang hoạt động
            modelBuilder.Entity<PhieuGuiXe>()
                .HasIndex(p => new { p.MaViTri, p.TrangThai });
            modelBuilder.Entity<PhieuGuiXe>()
                .HasIndex(p => new { p.MaPhuongTien, p.TrangThai });
        }
    }
}
