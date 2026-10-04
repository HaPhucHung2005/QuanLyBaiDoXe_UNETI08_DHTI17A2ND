using Microsoft.EntityFrameworkCore;
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.ViTriDoXe> ViTriDoXe { get; set; } = default!;
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.LoaiPhuongTien> LoaiPhuongTien { get; set; } = default!;
        public DbSet<QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models.TaiKhoan> TaiKhoan { get; set; } = default!;
    }
}
