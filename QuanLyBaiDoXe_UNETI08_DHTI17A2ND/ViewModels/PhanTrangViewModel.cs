// Dùng chung Module 3 và 4 - Lương Chính Nguyên
// Trần Ngọc Minh (MSSV: 23203100006)
// Nội dung thực hiện: ViewModel cho partial phân trang, giữ nguyên điều kiện tìm kiếm/lọc/sắp xếp khi chuyển trang
namespace QuanLyBaiDoXe_UNETI01_DHTI17A2ND.ViewModels
{
    public class PhanTrangViewModel
    {
        public string Action { get; set; } = "Index";
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public Dictionary<string, string?> RouteValues { get; set; } = new();

        public Dictionary<string, string> Route(int page)
        {
            var d = RouteValues
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value!);
            d["pageIndex"] = Math.Max(1, Math.Min(page, Math.Max(TotalPages, 1))).ToString();
            return d;
        }
    }
}
