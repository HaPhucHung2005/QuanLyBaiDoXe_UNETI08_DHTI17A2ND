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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhuongTien_LoaiPhuongTien_MaLoaiPhuongTien",
                        column: x => x.MaLoaiPhuongTien,
                        principalTable: "LoaiPhuongTien",
                        principalColumn: "MaLoaiPhuongTien",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuGuiXe_ViTriDoXe_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriDoXe",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.Cascade);
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
                    { 2, "hung@gmail.com", "Hà Phúc Hưng", "123456", "khachhang1", true, "Khách hàng" }
                });

            migrationBuilder.InsertData(
                table: "ChuPhuongTien",
                columns: new[] { "MaChuPhuongTien", "DiaChi", "Email", "GioiTinh", "HoTen", "MaTaiKhoan", "NgayDangKy", "NgaySinh", "SoDienThoai", "TrangThai" },
                values: new object[] { 1, "Hà Nội", "hung@gmail.com", "Nam", "Hà Phúc Hưng", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2003, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0987654321", true });

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
                    { 8, "Khu C", 3, "Vị trí đỗ ô tô 7 chỗ", 2, "C02", "Còn trống" }
                });

            migrationBuilder.InsertData(
                table: "PhuongTien",
                columns: new[] { "MaPhuongTien", "BienSoXe", "MaChuPhuongTien", "MaLoaiPhuongTien", "MauSac", "MoTa", "NhanHieu", "TrangThai" },
                values: new object[,]
                {
                    { 1, "29A1-12345", 1, 1, "Đen", "Xe đi học hàng ngày", "Honda Wave", true },
                    { 2, "30H-99999", 1, 2, "Trắng", "Xe cá nhân", "Mazda 3", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChuPhuongTien_MaTaiKhoan",
                table: "ChuPhuongTien",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaPhuongTien",
                table: "PhieuGuiXe",
                column: "MaPhuongTien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGuiXe_MaViTri",
                table: "PhieuGuiXe",
                column: "MaViTri");

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
