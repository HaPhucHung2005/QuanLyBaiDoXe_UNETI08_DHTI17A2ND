// =================================================================
// Họ và tên: Hà Phúc Hưng - Trần Ngọc Minh
// Mã sinh viên: 23203100001 - [MSSV]
// Nội dung thực hiện: Khai báo DbContext, DbSet, Fluent API Ràng buộc và Seed Data
// =================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Models;

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Module 3: Chủ phương tiện - Phương tiện - Phiếu gửi xe
        public DbSet<PhieuGuiXe> PhieuGuiXe { get; set; } = null!;
        public DbSet<PhuongTien> PhuongTien { get; set; } = null!;
        public DbSet<ChuPhuongTien> ChuPhuongTien { get; set; } = null!;

        // Module 2: Vị trí đỗ xe
        public DbSet<ViTriDoXe> ViTriDoXe { get; set; } = null!;

        // Module 1: Tài khoản & Loại phương tiện
        public DbSet<LoaiPhuongTien> LoaiPhuongTien { get; set; } = null!;
        public DbSet<TaiKhoan> TaiKhoan { get; set; } = null!;

        // Hàm tính phí gửi xe tĩnh dùng riêng cho Seed Data
        public static class QuyTacTinhPhi
        {
            public static int TinhSoGioTinhPhi(DateTime vao, DateTime ra)
            {
                var soGio = (ra - vao).TotalHours;
                return (int)Math.Max(1, Math.Ceiling(soGio));
            }

            public static decimal TinhThanhTien(DateTime vao, DateTime ra, decimal donGiaTheoGio)
                => TinhSoGioTinhPhi(vao, ra) * donGiaTheoGio;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =================================================================
            // 1. CẤU HÌNH QUAN HỆ VÀ RÀNG BUỘC (FLUENT API)
            // =================================================================

            // ViTriDoXe (n) - (1) LoaiPhuongTien
            modelBuilder.Entity<ViTriDoXe>()
                .HasOne(v => v.LoaiPhuongTien)
                .WithMany(l => l.DanhSachViTriDoXe)
                .HasForeignKey(v => v.MaLoaiPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // TaiKhoan (1) - (1) ChuPhuongTien
            modelBuilder.Entity<ChuPhuongTien>()
                .HasOne(c => c.TaiKhoan)
                .WithOne()
                .HasForeignKey<ChuPhuongTien>(c => c.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // ChuPhuongTien (1) - (n) PhuongTien
            modelBuilder.Entity<PhuongTien>()
                .HasOne(p => p.ChuPhuongTien)
                .WithMany(c => c.PhuongTiens) // Đã đồng bộ với ChuPhuongTien.cs
                .HasForeignKey(p => p.MaChuPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // LoaiPhuongTien (1) - (n) PhuongTien
            modelBuilder.Entity<PhuongTien>()
                .HasOne(p => p.LoaiPhuongTien)
                .WithMany()
                .HasForeignKey(p => p.MaLoaiPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // Biển số xe không trùng lặp
            modelBuilder.Entity<PhuongTien>()
                .HasIndex(p => p.BienSoXe)
                .IsUnique();

            // PhuongTien (1) - (n) PhieuGuiXe
            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.PhuongTien)
                .WithMany(x => x.PhieuGuiXes) // Đã đồng bộ với PhuongTien.cs
                .HasForeignKey(p => p.MaPhuongTien)
                .OnDelete(DeleteBehavior.Restrict);

            // ViTriDoXe (1) - (n) PhieuGuiXe
            modelBuilder.Entity<PhieuGuiXe>()
                .HasOne(p => p.ViTriDoXe)
                .WithMany()
                .HasForeignKey(p => p.MaViTri)
                .OnDelete(DeleteBehavior.Restrict);

            // Chỉ mục hỗ trợ tìm kiếm trùng lịch
            modelBuilder.Entity<PhieuGuiXe>()
                .HasIndex(p => new { p.MaViTri, p.TrangThai });
            modelBuilder.Entity<PhieuGuiXe>()
                .HasIndex(p => new { p.MaPhuongTien, p.TrangThai });

            // =================================================================
            // 2. SEED DATA TỐI ƯU
            // =================================================================

            // 2.1. LoaiPhuongTien
            modelBuilder.Entity<LoaiPhuongTien>().HasData(
                new LoaiPhuongTien { MaLoaiPhuongTien = 1, TenLoaiPhuongTien = "Xe máy", DonGiaTheoGio = 5000, TrangThai = true, MoTa = "Khu vực đỗ xe máy" },
                new LoaiPhuongTien { MaLoaiPhuongTien = 2, TenLoaiPhuongTien = "Ô tô 4 chỗ", DonGiaTheoGio = 20000, TrangThai = true, MoTa = "Khu vực đỗ ô tô con 4-5 chỗ" },
                new LoaiPhuongTien { MaLoaiPhuongTien = 3, TenLoaiPhuongTien = "Ô tô 7 chỗ", DonGiaTheoGio = 30000, TrangThai = true, MoTa = "Khu vực đỗ ô tô 7 chỗ trở lên" }
            );

            // 2.2. TaiKhoan (Khóa chính: 1..17)
            string[] dsTen =
            {
                "Hà Phúc Hưng", "Lương Chính Nguyên", "Trần Ngọc Minh", "Nguyễn Văn An", "Lê Thị Bích",
                "Phạm Quang Huy", "Đỗ Thị Lan", "Vũ Minh Tuấn", "Bùi Thu Hà", "Hoàng Đức Anh",
                "Ngô Thanh Tùng", "Đặng Mai Phương", "Phan Văn Long", "Trịnh Hải Yến", "Nguyễn Thị Thu"
            };

            var dsTaiKhoan = new List<TaiKhoan>
            {
                new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản Trị Viên", Email = "admin@uneti.edu.vn", VaiTro = "Admin", TrangThai = true },
                new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "khachhang1", MatKhau = "123456", HoTen = "Hà Phúc Hưng", Email = "hung@gmail.com", VaiTro = "Khách hàng", TrangThai = true },
                new TaiKhoan { MaTaiKhoan = 3, TenDangNhap = "admin2", MatKhau = "123456", HoTen = "Quản Trị Viên 2", Email = "admin2@uneti.edu.vn", VaiTro = "Admin", TrangThai = true }
            };

            for (int i = 2; i <= 15; i++)
            {
                dsTaiKhoan.Add(new TaiKhoan
                {
                    MaTaiKhoan = i + 2, // 4..17
                    TenDangNhap = $"khachhang{i}",
                    MatKhau = "123456",
                    HoTen = dsTen[i - 1],
                    Email = $"khachhang{i}@gmail.com",
                    VaiTro = "Khách hàng",
                    TrangThai = true
                });
            }
            modelBuilder.Entity<TaiKhoan>().HasData(dsTaiKhoan);

            // 2.3. ViTriDoXe (Khóa chính: 1..20)
            modelBuilder.Entity<ViTriDoXe>().HasData(
                new ViTriDoXe { MaViTri = 1, TenViTri = "A01", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí gần cửa ra vào" },
                new ViTriDoXe { MaViTri = 2, TenViTri = "A02", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = TrangThaiViTri.DangSuDung, MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 3, TenViTri = "A03", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 4, TenViTri = "B01", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 5, TenViTri = "B02", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 1, TrangThai = TrangThaiViTri.DangSuDung, MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 6, TenViTri = "B03", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 2, TrangThai = TrangThaiViTri.TamNgung, MoTa = "Đang bảo trì" },
                new ViTriDoXe { MaViTri = 7, TenViTri = "C01", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 8, TenViTri = "C02", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 9, TenViTri = "A04", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 10, TenViTri = "A05", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy khu A" },
                new ViTriDoXe { MaViTri = 11, TenViTri = "A06", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy tầng 2" },
                new ViTriDoXe { MaViTri = 12, TenViTri = "A07", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy tầng 2" },
                new ViTriDoXe { MaViTri = 13, TenViTri = "A08", MaLoaiPhuongTien = 1, KhuVuc = "Khu A", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ xe máy tầng 2" },
                new ViTriDoXe { MaViTri = 14, TenViTri = "B04", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 1, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 15, TenViTri = "B05", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 2, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô con" },
                new ViTriDoXe { MaViTri = 16, TenViTri = "B06", MaLoaiPhuongTien = 2, KhuVuc = "Khu B", Tang = 2, TrangThai = TrangThaiViTri.TamNgung, MoTa = "Đang sửa chữa" },
                new ViTriDoXe { MaViTri = 17, TenViTri = "C03", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 2, TrangThai = TrangThaiViTri.DangSuDung, MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 18, TenViTri = "C04", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 3, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 19, TenViTri = "C05", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 3, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô 7 chỗ" },
                new ViTriDoXe { MaViTri = 20, TenViTri = "C06", MaLoaiPhuongTien = 3, KhuVuc = "Khu C", Tang = 3, TrangThai = TrangThaiViTri.ConTrong, MoTa = "Vị trí đỗ ô tô 7 chỗ" }
            );

            // 2.4. ChuPhuongTien (Khóa chính: 1..15)
            var dsChu = new List<ChuPhuongTien>();
            for (int i = 1; i <= 15; i++)
            {
                dsChu.Add(new ChuPhuongTien
                {
                    MaChuPhuongTien = i,
                    MaTaiKhoan = i == 1 ? 2 : i + 2,
                    HoTen = dsTen[i - 1],
                    NgaySinh = new DateTime(1985 + i % 15, (i % 12) + 1, (i % 27) + 1),
                    GioiTinh = i % 2 == 0 ? "Nữ" : "Nam",
                    SoDienThoai = "0912" + (345000 + i * 137).ToString(),
                    Email = i == 1 ? "hung@gmail.com" : $"khachhang{i}@gmail.com",
                    DiaChi = $"Số {i * 7}, phường {i}, Hà Nội",
                    NgayDangKy = new DateTime(2026, 7, i + 1),
                    TrangThai = i != 15 // Chủ xe 15 tạm khóa
                });
            }
            modelBuilder.Entity<ChuPhuongTien>().HasData(dsChu);

            // 2.5. PhuongTien (Khóa chính: 1..20)
            var ptRaw = new (int id, int chu, int loai, string bien, string hieu, string mau, bool hd)[]
            {
                (1, 1, 1, "29B1-12345", "Honda Wave", "Đỏ", true),
                (2, 2, 1, "30F2-23456", "Yamaha Exciter", "Xanh", true),
                (3, 3, 1, "29C1-34567", "Honda Vision", "Trắng", true),
                (4, 4, 1, "18H1-45678", "Honda Lead", "Đen", true),
                (5, 5, 1, "29D1-56789", "Suzuki Raider", "Xám", true),
                (6, 6, 1, "30E1-67890", "Honda SH", "Đen", true),
                (7, 7, 1, "29K1-78901", "Yamaha Janus", "Vàng", true),
                (8, 8, 1, "90B1-89012", "Honda Future", "Bạc", false),
                (9, 9, 2, "30A-11111", "Toyota Vios", "Trắng", true),
                (10, 10, 2, "29A-22222", "Hyundai Accent", "Bạc", true),
                (11, 11, 2, "30G-33333", "Kia Morning", "Đỏ", true),
                (12, 12, 2, "29H-44444", "Mazda 3", "Xanh", true),
                (13, 13, 2, "30K-55555", "Honda City", "Đen", true),
                (14, 14, 2, "29A-66666", "VinFast Fadil", "Cam", true),
                (15, 15, 2, "30E-77777", "Toyota Altis", "Trắng", true),
                (16, 1, 3, "29A-88888", "Toyota Innova", "Bạc", true),
                (17, 2, 3, "30A-99999", "Mitsubishi Xpander", "Trắng", true),
                (18, 3, 3, "29C-10101", "Kia Carnival", "Đen", true),
                (19, 4, 3, "30H-20202", "Hyundai Santa Fe", "Xám", true),
                (20, 5, 3, "29E-30303", "VinFast Lux SA", "Xanh", true),
            };

            modelBuilder.Entity<PhuongTien>().HasData(ptRaw.Select(x => new PhuongTien
            {
                MaPhuongTien = x.id,
                MaChuPhuongTien = x.chu,
                MaLoaiPhuongTien = x.loai,
                BienSoXe = x.bien,
                NhanHieu = x.hieu,
                MauSac = x.mau,
                MoTa = null,
                TrangThai = x.hd
            }));

            // 2.6. PhieuGuiXe (Khóa chính: 1..28)
            var loaiCuaPt = ptRaw.ToDictionary(x => x.id, x => x.loai);
            var donGia = new Dictionary<int, decimal> { [1] = 5000m, [2] = 20000m, [3] = 30000m };

            DateTime D(int thang, int ngay, int gio, int phut = 0) => new DateTime(2026, thang, ngay, gio, phut, 0);

            PhieuGuiXe P(int id, int pt, int vt, DateTime dk, DateTime vao, DateTime ra, string tt, DateTime? vaoTT = null, DateTime? raTT = null)
            {
                var gia = donGia[loaiCuaPt[pt]];
                return new PhieuGuiXe
                {
                    MaPhieu = id,
                    MaPhuongTien = pt,
                    MaViTri = vt,
                    NgayDangKy = dk,
                    ThoiGianDuKienVao = vao,
                    ThoiGianDuKienRa = ra,
                    ThoiGianVaoThucTe = vaoTT,
                    ThoiGianRaThucTe = raTT,
                    DonGiaTheoGio = gia,
                    ThanhTien = tt == TrangThaiPhieu.DaLayXe && vaoTT.HasValue && raTT.HasValue
                        ? QuyTacTinhPhi.TinhThanhTien(vaoTT.Value, raTT.Value, gia)
                        : null,
                    TrangThai = tt
                };
            }

            var dsPhieu = new List<PhieuGuiXe>
            {
                // Đã lấy xe (1..18)
                P(1,  1, 1,  D(8,1,7),   D(8,2,8),   D(8,2,12),  TrangThaiPhieu.DaLayXe, D(8,2,8,5),   D(8,2,11,50)),
                P(2,  2, 3,  D(8,4,9),   D(8,5,9),   D(8,5,17),  TrangThaiPhieu.DaLayXe, D(8,5,9,10),  D(8,5,16,40)),
                P(3,  9, 4,  D(8,7,8),   D(8,8,8),   D(8,8,11),  TrangThaiPhieu.DaLayXe, D(8,8,8),     D(8,8,11,20)),
                P(4,  10,14, D(8,11,13), D(8,12,13), D(8,12,18), TrangThaiPhieu.DaLayXe, D(8,12,13,5), D(8,12,17,55)),
                P(5,  16,7,  D(8,14,8),  D(8,15,8),  D(8,15,10), TrangThaiPhieu.DaLayXe, D(8,15,8),    D(8,15,10)),
                P(6,  3, 9,  D(8,19,7),  D(8,20,7,30), D(8,20,12,30), TrangThaiPhieu.DaLayXe, D(8,20,7,35), D(8,20,12,20)),
                P(7,  11,15, D(8,24,14), D(8,25,14), D(8,25,20), TrangThaiPhieu.DaLayXe, D(8,25,14,10), D(8,25,19,50)),
                P(8,  17,8,  D(8,27,8),  D(8,28,8),  D(8,28,16), TrangThaiPhieu.DaLayXe, D(8,28,8),    D(8,28,16,30)),
                P(9,  4, 10, D(9,1,8),   D(9,2,8),   D(9,2,12),  TrangThaiPhieu.DaLayXe, D(9,2,8),     D(9,2,11,30)),
                P(10, 12,4,  D(9,4,9),   D(9,5,9),   D(9,5,15),  TrangThaiPhieu.DaLayXe, D(9,5,9),     D(9,5,15)),
                P(11, 18,17, D(9,8,8),   D(9,9,8),   D(9,9,12),  TrangThaiPhieu.DaLayXe, D(9,9,8,15),  D(9,9,12)),
                P(12, 5, 1,  D(9,11,10), D(9,12,10), D(9,12,14), TrangThaiPhieu.DaLayXe, D(9,12,10),   D(9,12,13,30)),
                P(13, 13,5,  D(9,14,8),  D(9,15,8),  D(9,15,18), TrangThaiPhieu.DaLayXe, D(9,15,8),    D(9,15,18,40)),
                P(14, 19,18, D(9,17,9),  D(9,18,9),  D(9,18,13), TrangThaiPhieu.DaLayXe, D(9,18,9),    D(9,18,13)),
                P(15, 6, 11, D(9,21,7),  D(9,22,7),  D(9,22,19), TrangThaiPhieu.DaLayXe, D(9,22,7),    D(9,22,19,10)),
                P(16, 14,14, D(9,25,8),  D(9,26,8),  D(9,26,12), TrangThaiPhieu.DaLayXe, D(9,26,8),    D(9,26,12)),
                P(17, 7, 12, D(10,1,8),  D(10,2,8),  D(10,2,10), TrangThaiPhieu.DaLayXe, D(10,2,8,5),  D(10,2,9,50)),
                P(18, 20,19, D(10,4,13), D(10,5,13), D(10,5,17), TrangThaiPhieu.DaLayXe, D(10,5,13),   D(10,5,16,30)),

                // Đang gửi (19..21)
                P(19, 2, 2,  D(10,8,20), D(10,9,7),  D(10,9,19), TrangThaiPhieu.DangGui, D(10,9,7,10)),
                P(20, 13,5,  D(10,8,21), D(10,9,8),  D(10,9,17), TrangThaiPhieu.DangGui, D(10,9,8,5)),
                P(21, 18,17, D(10,8,22), D(10,9,6,30), D(10,9,18), TrangThaiPhieu.DangGui, D(10,9,6,40)),

                // Chờ xác nhận (22..25)
                P(22, 4, 10, D(10,9,8),  D(10,20,8), D(10,20,12), TrangThaiPhieu.ChoXacNhan),
                P(23, 19,18, D(10,9,8),  D(10,21,9), D(10,21,17), TrangThaiPhieu.ChoXacNhan),
                P(24, 11,4,  D(10,9,9),  D(10,22,8), D(10,22,11), TrangThaiPhieu.ChoXacNhan),
                P(25, 3, 1,  D(10,9,9),  D(10,20,14), D(10,20,18), TrangThaiPhieu.ChoXacNhan),

                // Đã hủy (26..28)
                P(26, 9, 15, D(10,7,8),  D(10,10,8), D(10,10,12), TrangThaiPhieu.DaHuy),
                P(27, 1, 3,  D(10,7,9),  D(10,12,8), D(10,12,12), TrangThaiPhieu.DaHuy),
                P(28, 16,8,  D(10,7,10), D(10,13,8), D(10,13,16), TrangThaiPhieu.DaHuy)
            };

            modelBuilder.Entity<PhieuGuiXe>().HasData(dsPhieu);
        }
    }
}