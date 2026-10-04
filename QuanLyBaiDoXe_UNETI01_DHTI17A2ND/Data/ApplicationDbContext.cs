// =================================================================
// Họ và tên: Hà Phúc Hưng
// Mã sinh viên: 23203100001
// Nội dung thực hiện: Khai báo DbContext, DbSet và Seed Data cho Module
// =================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Module 2: Vị trí đỗ xe
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.ViTriDoXe> ViTriDoXe { get; set; } = null!;

        // Module 1: Tài khoản & Loại phương tiện
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.LoaiPhuongTien> LoaiPhuongTien { get; set; } = null!;
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.TaiKhoan> TaiKhoan { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Quan hệ giữa các Entity theo Đề bài (Mục 9)
            modelBuilder.Entity<ViTriDoXe>()
                .HasOne(v => v.LoaiPhuongTien)
                .WithMany(l => l.DanhSachViTriDoXe)
                .HasForeignKey(v => v.MaLoaiPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Seed Data cho LoaiPhuongTien (Tối thiểu 3 loại phương tiện)
            modelBuilder.Entity<LoaiPhuongTien>().HasData(
                new LoaiPhuongTien { MaLoaiPhuongTien = 1, TenLoaiPhuongTien = "Xe máy", DonGiaTheoGio = 5000, TrangThai = true, MoTa = "Khu vực đỗ xe máy" },
                new LoaiPhuongTien { MaLoaiPhuongTien = 2, TenLoaiPhuongTien = "Ô tô 4 chỗ", DonGiaTheoGio = 20000, TrangThai = true, MoTa = "Khu vực đỗ ô tô con 4-5 chỗ" },
                new LoaiPhuongTien { MaLoaiPhuongTien = 3, TenLoaiPhuongTien = "Ô tô 7 chỗ", DonGiaTheoGio = 30000, TrangThai = true, MoTa = "Khu vực đỗ ô tô 7 chỗ trở lên" }
            );

            // 3. Seed Data cho TaiKhoan (Tài khoản Admin và Khách hàng thử nghiệm)
            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản Trị Viên", Email = "admin@uneti.edu.vn", VaiTro = "Admin", TrangThai = true },
                new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "khachhang1", MatKhau = "123456", HoTen = "Hà Phúc Hưng", Email = "hung@gmail.com", VaiTro = "Khách hàng", TrangThai = true }
            );

            // 4. Seed Data cho ViTriDoXe (Tạo nhiều bản ghi để thử nghiệm phân trang Skip & Take)
            modelBuilder.Entity<ViTriDoXe>().HasData(
                new ViTriDoXe { MaViTri = 1, TenViTri = "A01", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = "Còn trống", MoTa = "Vị trí gần cửa ra vào" },
                new ViTriDoXe { MaViTri = 2, TenViTri = "A02", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = "Đang sử dụng", MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 3, TenViTri = "A03", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = "Còn trống", MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 4, TenViTri = "B01", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 1, TrangThai = "Còn trống", MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 5, TenViTri = "B02", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 1, TrangThai = "Đang sử dụng", MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 6, TenViTri = "B03", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 2, TrangThai = "Tạm ngừng", MoTa = "Đang bảo trì" },
                new ViTriDoXe { MaViTri = 7, TenViTri = "C01", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 2, TrangThai = "Còn trống", MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 8, TenViTri = "C02", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 2, TrangThai = "Còn trống", MoTa = "Vị trí đỗ ô tô 7 chỗ" }
            );
        }
    }
}
