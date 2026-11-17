using ItemMonitor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItemMonitor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ItemStockService stockService;

        public IndexModel(ItemStockService stockService)
        {
            this.stockService = stockService;
        }

        public StockCounts Counts { get; private set; } = new();
        public ItemsPage ItemsPage { get; private set; } = new();

        [BindProperty(SupportsGet = true, Name = "q")]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true, Name = "status")]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true, Name = "sort")]
        public string Sort { get; set; } = "quantity";

        [BindProperty(SupportsGet = true, Name = "dir")]
        public string Dir { get; set; } = "asc";

        [BindProperty(SupportsGet = true, Name = "page")]
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync()
        {
            StockStatus? statusFilter = Enum.TryParse<StockStatus>(Status, out var parsed) ? parsed : null;
            var query = new ItemsQuery
            {
                Search = Search,
                Status = statusFilter,
                SortBy = Sort,
                SortDescending = string.Equals(Dir, "desc", StringComparison.OrdinalIgnoreCase),
                Page = PageNumber,
                PageSize = 50,
            };

            Counts = await stockService.GetCountsAsync();
            ItemsPage = await stockService.GetPageAsync(query);
        }
    }
}
