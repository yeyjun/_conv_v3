using MauiRemoteController.Pages;
using MauiRemoteController.Services;
using Microsoft.Extensions.Logging;

namespace MauiRemoteController;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<TerminalStore>();
		builder.Services.AddTransient<TerminalListPage>();
		builder.Services.AddTransient<AddEditTerminalPage>();
		builder.Services.AddTransient<TerminalDetailPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
