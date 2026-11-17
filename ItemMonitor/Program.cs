using System.Text.Json.Serialization;
using ItemMonitor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ItemStockService>();
// dashboard.js matches StockStatus by name ("Normal"/"Low"/"OutOfStock"), not by its
// underlying int value, so /api/items needs enums serialized as strings.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Polled by wwwroot/js/dashboard.js for search/filter/sort/paging and the "live" refresh,
// so the page can update without a full navigation. Returns the same shape the initial
// Razor render uses, just as JSON.
app.MapGet("/api/items", async (ItemStockService service, string? q, string? status, string? sort, string? dir, int page = 1, int pageSize = 50) =>
{
    StockStatus? statusFilter = Enum.TryParse<StockStatus>(status, out var parsed) ? parsed : null;
    var query = new ItemsQuery
    {
        Search = q,
        Status = statusFilter,
        SortBy = sort ?? "quantity",
        SortDescending = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase),
        Page = page,
        PageSize = pageSize,
    };

    var counts = await service.GetCountsAsync();
    var itemsPage = await service.GetPageAsync(query);
    return Results.Json(new { counts, page = itemsPage });
});

app.Run();
