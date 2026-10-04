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

            migrationBuilder.CreateIndex(
                name: "IX_ViTriDoXe_MaLoaiPhuongTien",
                table: "ViTriDoXe",
                column: "MaLoaiPhuongTien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "ViTriDoXe");

            migrationBuilder.DropTable(
                name: "LoaiPhuongTien");
        }
    }
}
