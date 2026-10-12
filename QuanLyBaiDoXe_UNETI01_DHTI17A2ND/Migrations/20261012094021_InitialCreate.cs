using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiPhuongTien",
                columns: table => new
                {
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiPhuongTien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonGiaTheoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhuongTien", x => x.MaLoaiPhuongTien);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "ViTriDoXe",
                columns: table => new
                {
                    MaViTri = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenViTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false),
                    KhuVuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriDoXe", x => x.MaViTri);
                    table.ForeignKey(
                        name: "FK_ViTriDoXe_LoaiPhuongTien_MaLoaiPhuongTien",
                        column: x => x.MaLoaiPhuongTien,
                        principalTable: "LoaiPhuongTien",
                        principalColumn: "MaLoaiPhuongTien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChuPhuongTien",
                columns: table => new
                {
                    MaChuPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuPhuongTien", x => x.MaChuPhuongTien);
                    table.ForeignKey(
                        name: "FK_ChuPhuongTien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTien",
                columns: table => new
                {
                    MaPhuongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChuPhuongTien = table.Column<int>(type: "int", nullable: false),
                    MaLoaiPhuongTien = table.Column<int>(type: "int", nullable: false),
                    BienSoXe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NhanHieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MauSac = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTien", x => x.MaPhuongTien);
                    table.ForeignKey(
                        name: "FK_PhuongTien_ChuPhuongTien_MaChuPhuongTien",
                        column: x => x.MaChuPhuongTien,
                        principalTable: "ChuPhuongTien",
                        principalColumn: "MaChuPhuongTien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhuongTien_LoaiPhuongTien_MaLoaiPhuongTien",
                        column: x => x.MaLoaiPhuongTien,
                        principalTable: "LoaiPhuongTien",
                        principalColumn: "MaLoaiPhuongTien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGuiXe",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhuongTien = table.Column<int>(type: "int", nullable: false),
                    MaViTri = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDuKienVao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDuKienRa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianVaoThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianRaThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonGiaTheoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGuiXe", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuGuiXe_PhuongTien_MaPhuongTien",
                        column: x => x.MaPhuongTien,
                        principalTable: "PhuongTien",
                        principalColumn: "MaPhuongTien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuGuiXe_ViTriDoXe_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriDoXe",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "LoaiPhuongTien",
                columns: new[] { "MaLoaiPhuongTien", "DonGiaTheoGio", "MoTa", "TenLoaiPhuongTien", "TrangThai" },
                values: new object[,]
                {
                    { 1, 5000m, "Khu vực đỗ xe máy", "Xe máy", true },
                    { 2, 20000m, "Khu vực đỗ ô tô con 4-5 chỗ", "Ô tô 4 chỗ", true },
                    { 3, 30000m, "Khu vực đỗ ô tô 7 chỗ trở lên", "Ô tô 7 chỗ", true }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "MaTaiKhoan", "Email", "HoTen", "MatKhau", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 1, "admin@uneti.edu.vn", "Quản Trị Viên", "123456", "admin", true, "Admin" },
                    { 2, "hung@gmail.com", "Hà Phúc Hưng", "123456", "khachhang1", true, "Khách hàng" },
                    { 3, "admin2@uneti.edu.vn", "Quản Trị Viên 2", "123456", "admin2", true, "Admin" },
                    { 4, "khachhang2@gmail.com", "Lương Chính Nguyên", "123456", "khachhang2", true, "Khách hàng" },
                    { 5, "khachhang3@gmail.com", "Trần Ngọc Minh", "123456", "khachhang3", true, "Khách hàng" },
                    { 6, "khachhang4@gmail.com", "Nguyễn Văn An", "123456", "khachhang4", true, "Khách hàng" },
                    { 7, "khachhang5@gmail.com", "Lê Thị Bích", "123456", "khachhang5", true, "Khách hàng" },
                    { 8, "khachhang6@gmail.com", "Phạm Quang Huy", "123456", "khachhang6", true, "Khách hàng" },
                    { 9, "khachhang7@gmail.com", "Đỗ Thị Lan", "123456", "khachhang7", true, "Khách hàng" },
                    { 10, "khachhang8@gmail.com", "Vũ Minh Tuấn", "123456", "khachhang8", true, "Khách hàng" },
                    { 11, "khachhang9@gmail.com", "Bùi Thu Hà", "123456", "khachhang9", true, "Khách hàng" },
                    { 12, "khachhang10@gmail.com", "Hoàng Đức Anh", "123456", "khachhang10", true, "Khách hàng" },
                    { 13, "khachhang11@gmail.com", "Ngô Thanh Tùng", "123456", "khachhang11", true, "Khách hàng" },
                    { 14, "khachhang12@gmail.com", "Đặng Mai Phương", "123456", "khachhang12", true, "Khách hàng" },
                    { 15, "khachhang13@gmail.com", "Phan Văn Long", "123456", "khachhang13", true, "Khách hàng" },
                    { 16, "khachhang14@gmail.com", "Trịnh Hải Yến", "123456", "khachhang14", true, "Khách hàng" },
                    { 17, "khachhang15@gmail.com", "Nguyễn Thị Thu", "123456", "khachhang15", true, "Khách hàng" }
                });

            migrationBuilder.InsertData(
                table: "ChuPhuongTien",
                columns: new[] { "MaChuPhuongTien", "DiaChi", "Email", "GioiTinh", "HoTen", "MaTaiKhoan", "NgayDangKy", "NgaySinh", "SoDienThoai", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Số 7, phường 1, Hà Nội", "hung@gmail.com", "Nam", "Hà Phúc Hưng", 2, new DateTime(2026, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1986, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345137", true },
                    { 2, "Số 14, phường 2, Hà Nội", "khachhang2@gmail.com", "Nữ", "Lương Chính Nguyên", 4, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1987, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345274", true },
                    { 3, "Số 21, phường 3, Hà Nội", "khachhang3@gmail.com", "Nam", "Trần Ngọc Minh", 5, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345411", true },
                    { 4, "Số 28, phường 4, Hà Nội", "khachhang4@gmail.com", "Nữ", "Nguyễn Văn An", 6, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1989, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345548", true },
                    { 5, "Số 35, phường 5, Hà Nội", "khachhang5@gmail.com", "Nam", "Lê Thị Bích", 7, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345685", true },
                    { 6, "Số 42, phường 6, Hà Nội", "khachhang6@gmail.com", "Nữ", "Phạm Quang Huy", 8, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345822", true },
                    { 7, "Số 49, phường 7, Hà Nội", "khachhang7@gmail.com", "Nam", "Đỗ Thị Lan", 9, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912345959", true },
                    { 8, "Số 56, phường 8, Hà Nội", "khachhang8@gmail.com", "Nữ", "Vũ Minh Tuấn", 10, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346096", true },
                    { 9, "Số 63, phường 9, Hà Nội", "khachhang9@gmail.com", "Nam", "Bùi Thu Hà", 11, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346233", true },
                    { 10, "Số 70, phường 10, Hà Nội", "khachhang10@gmail.com", "Nữ", "Hoàng Đức Anh", 12, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346370", true },
                    { 11, "Số 77, phường 11, Hà Nội", "khachhang11@gmail.com", "Nam", "Ngô Thanh Tùng", 13, new DateTime(2026, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1996, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346507", true },
                    { 12, "Số 84, phường 12, Hà Nội", "khachhang12@gmail.com", "Nữ", "Đặng Mai Phương", 14, new DateTime(2026, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1997, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346644", true },
                    { 13, "Số 91, phường 13, Hà Nội", "khachhang13@gmail.com", "Nam", "Phan Văn Long", 15, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1998, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346781", true },
                    { 14, "Số 98, phường 14, Hà Nội", "khachhang14@gmail.com", "Nữ", "Trịnh Hải Yến", 16, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1999, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912346918", true },
                    { 15, "Số 105, phường 15, Hà Nội", "khachhang15@gmail.com", "Nam", "Nguyễn Thị Thu", 17, new DateTime(2026, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "0912347055", false }
                });

            migrationBuilder.InsertData(
                table: "ViTriDoXe",
                columns: new[] { "MaViTri", "KhuVuc", "MaLoaiPhuongTien", "MoTa", "Tang", "TenViTri", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Khu A", 1, "Vị trí gần cửa ra vào", 1, "A01", "Còn trống" },
                    { 2, "Khu A", 1, "Vị trí đỗ xe máy khu A", 1, "A02", "Đang sử dụng" },
                    { 3, "Khu A", 1, "Vị trí đỗ xe máy khu A", 1, "A03", "Còn trống" },
                    { 4, "Khu B", 2, "Vị trí đỗ ô tô con", 1, "B01", "Còn trống" },
                    { 5, "Khu B", 2, "Vị trí đỗ ô tô con", 1, "B02", "Đang sử dụng" },
                    { 6, "Khu B", 2, "Đang bảo trì", 2, "B03", "Tạm ngừng" },
                    { 7, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 2, "C01", "Còn trống" },
                    { 8, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 2, "C02", "Còn trống" },
                    { 9, "Khu A", 1, "Vị trí đỗ xe máy khu A", 1, "A04", "Còn trống" },
                    { 10, "Khu A", 1, "Vị trí đỗ xe máy khu A", 1, "A05", "Còn trống" },
                    { 11, "Khu A", 1, "Vị trí đỗ xe máy tầng 2", 2, "A06", "Còn trống" },
                    { 12, "Khu A", 1, "Vị trí đỗ xe máy tầng 2", 2, "A07", "Còn trống" },
                    { 13, "Khu A", 1, "Vị trí đỗ xe máy tầng 2", 2, "A08", "Còn trống" },
                    { 14, "Khu B", 2, "Vị trí đỗ ô tô con", 1, "B04", "Còn trống" },
                    { 15, "Khu B", 2, "Vị trí đỗ ô tô con", 2, "B05", "Còn trống" },
                    { 16, "Khu B", 2, "Đang sửa chữa", 2, "B06", "Tạm ngừng" },
                    { 17, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 2, "C03", "Đang sử dụng" },
                    { 18, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 3, "C04", "Còn trống" },
                    { 19, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 3, "C05", "Còn trống" },
                    { 20, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 3, "C06", "Còn trống" }
                });

            migrationBuilder.InsertData(
                table: "PhuongTien",
                columns: new[] { "MaPhuongTien", "BienSoXe", "MaChuPhuongTien", "MaLoaiPhuongTien", "MauSac", "MoTa", "NhanHieu", "TrangThai" },
                values: new object[,]
                {
                    { 1, "29B1-12345", 1, 1, "Đỏ", null, "Honda Wave", true },
                    { 2, "30F2-23456", 2, 1, "Xanh", null, "Yamaha Exciter", true },
                    { 3, "29C1-34567", 3, 1, "Trắng", null, "Honda Vision", true },
                    { 4, "18H1-45678", 4, 1, "Đen", null, "Honda Lead", true },
                    { 5, "29D1-56789", 5, 1, "Xám", null, "Suzuki Raider", true },
                    { 6, "30E1-67890", 6, 1, "Đen", null, "Honda SH", true },
                    { 7, "29K1-78901", 7, 1, "Vàng", null, "Yamaha Janus", true },
                    { 8, "90B1-89012", 8, 1, "Bạc", null, "Honda Future", false },
                    { 9, "30A-11111", 9, 2, "Trắng", null, "Toyota Vios", true },
                    { 10, "29A-22222", 10, 2, "Bạc", null, "Hyundai Accent", true },
                    { 11, "30G-33333", 11, 2, "Đỏ", null, "Kia Morning", true },
                    { 12, "29H-44444", 12, 2, "Xanh", null, "Mazda 3", true },
                    { 13, "30K-55555", 13, 2, "Đen", null, "Honda City", true },
                    { 14, "29A-66666", 14, 2, "Cam", null, "VinFast Fadil", true },
                    { 15, "30E-77777", 15, 2, "Trắng", null, "Toyota Altis", true },
                    { 16, "29A-88888", 1, 3, "Bạc", null, "Toyota Innova", true },
                    { 17, "30A-99999", 2, 3, "Trắng", null, "Mitsubishi Xpander", true },
                    { 18, "29C-10101", 3, 3, "Đen", null, "Kia Carnival", true },
                    { 19, "30H-20202", 4, 3, "Xám", null, "Hyundai Santa Fe", true },
                    { 20, "29E-30303", 5, 3, "Xanh", null, "VinFast Lux SA", true }
                });

            migrationBuilder.InsertData(
                table: "PhieuGuiXe",
                columns: new[] { "MaPhieu", "DonGiaTheoGio", "MaPhuongTien", "MaViTri", "NgayDangKy", "ThanhTien", "ThoiGianDuKienRa", "ThoiGianDuKienVao", "ThoiGianRaThucTe", "ThoiGianVaoThucTe", "TrangThai" },
                values: new object[,]
                {
                    { 1, 5000m, 1, 1, new DateTime(2026, 8, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 20000m, new DateTime(2026, 8, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 2, 11, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 2, 8, 5, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 2, 5000m, 2, 3, new DateTime(2026, 8, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 40000m, new DateTime(2026, 8, 5, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 5, 16, 40, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 5, 9, 10, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 3, 20000m, 9, 4, new DateTime(2026, 8, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), 80000m, new DateTime(2026, 8, 8, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 8, 11, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 4, 20000m, 10, 14, new DateTime(2026, 8, 11, 13, 0, 0, 0, DateTimeKind.Unspecified), 100000m, new DateTime(2026, 8, 12, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 12, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 12, 17, 55, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 12, 13, 5, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 5, 30000m, 16, 7, new DateTime(2026, 8, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), 60000m, new DateTime(2026, 8, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 6, 5000m, 3, 9, new DateTime(2026, 8, 19, 7, 0, 0, 0, DateTimeKind.Unspecified), 25000m, new DateTime(2026, 8, 20, 12, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 7, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 12, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 7, 35, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 7, 20000m, 11, 15, new DateTime(2026, 8, 24, 14, 0, 0, 0, DateTimeKind.Unspecified), 120000m, new DateTime(2026, 8, 25, 20, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 25, 19, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 25, 14, 10, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 8, 30000m, 17, 8, new DateTime(2026, 8, 27, 8, 0, 0, 0, DateTimeKind.Unspecified), 270000m, new DateTime(2026, 8, 28, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 28, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 28, 16, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 28, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 9, 5000m, 4, 10, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 20000m, new DateTime(2026, 9, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 2, 11, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 10, 20000m, 12, 4, new DateTime(2026, 9, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 120000m, new DateTime(2026, 9, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 11, 30000m, 18, 17, new DateTime(2026, 9, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), 120000m, new DateTime(2026, 9, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 9, 8, 15, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 12, 5000m, 5, 1, new DateTime(2026, 9, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), 20000m, new DateTime(2026, 9, 12, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 12, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 13, 20000m, 13, 5, new DateTime(2026, 9, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), 220000m, new DateTime(2026, 9, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 15, 18, 40, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 14, 30000m, 19, 18, new DateTime(2026, 9, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), 120000m, new DateTime(2026, 9, 18, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 18, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 15, 5000m, 6, 11, new DateTime(2026, 9, 21, 7, 0, 0, 0, DateTimeKind.Unspecified), 65000m, new DateTime(2026, 9, 22, 19, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 19, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 7, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 16, 20000m, 14, 14, new DateTime(2026, 9, 25, 8, 0, 0, 0, DateTimeKind.Unspecified), 80000m, new DateTime(2026, 9, 26, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 26, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 26, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 26, 8, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 17, 5000m, 7, 12, new DateTime(2026, 10, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 10000m, new DateTime(2026, 10, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 2, 9, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 2, 8, 5, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 18, 30000m, 20, 19, new DateTime(2026, 10, 4, 13, 0, 0, 0, DateTimeKind.Unspecified), 120000m, new DateTime(2026, 10, 5, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 5, 16, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), "Đã lấy xe" },
                    { 19, 5000m, 2, 2, new DateTime(2026, 10, 8, 20, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 19, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 9, 7, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 7, 10, 0, 0, DateTimeKind.Unspecified), "Đang gửi" },
                    { 20, 20000m, 13, 5, new DateTime(2026, 10, 8, 21, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 8, 5, 0, 0, DateTimeKind.Unspecified), "Đang gửi" },
                    { 21, 30000m, 18, 17, new DateTime(2026, 10, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 9, 6, 30, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 9, 6, 40, 0, 0, DateTimeKind.Unspecified), "Đang gửi" },
                    { 22, 5000m, 4, 10, new DateTime(2026, 10, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Chờ xác nhận" },
                    { 23, 30000m, 19, 18, new DateTime(2026, 10, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 21, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 21, 9, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Chờ xác nhận" },
                    { 24, 20000m, 11, 4, new DateTime(2026, 10, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 22, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 22, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Chờ xác nhận" },
                    { 25, 5000m, 3, 1, new DateTime(2026, 10, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Chờ xác nhận" },
                    { 26, 20000m, 9, 15, new DateTime(2026, 10, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đã hủy" },
                    { 27, 5000m, 1, 3, new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đã hủy" },
                    { 28, 30000m, 16, 8, new DateTime(2026, 10, 7, 10, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 10, 13, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Đã hủy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChuPhuongTien_MaTaiKhoan",
                table: "ChuPhuongTien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaPhuongTien_TrangThai",
                table: "PhieuGuiXe",
                columns: new[] { "MaPhuongTien", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaViTri_TrangThai",
                table: "PhieuGuiXe",
                columns: new[] { "MaViTri", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_BienSoXe",
                table: "PhuongTien",
                column: "BienSoXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_MaChuPhuongTien",
                table: "PhuongTien",
                column: "MaChuPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_MaLoaiPhuongTien",
                table: "PhuongTien",
                column: "MaLoaiPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_ViTriDoXe_MaLoaiPhuongTien",
                table: "ViTriDoXe",
                column: "MaLoaiPhuongTien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuGuiXe");

            migrationBuilder.DropTable(
                name: "PhuongTien");

            migrationBuilder.DropTable(
                name: "ViTriDoXe");

            migrationBuilder.DropTable(
                name: "ChuPhuongTien");

            migrationBuilder.DropTable(
                name: "LoaiPhuongTien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
