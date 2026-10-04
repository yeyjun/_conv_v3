using MauiRemoteController.Pages;

namespace MauiRemoteController;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Pushed by GoToAsync, not present as ShellContent in the flyout - only the root
		// terminal list is.
		Routing.RegisterRoute(Routes.TerminalDetail, typeof(TerminalDetailPage));
		Routing.RegisterRoute(Routes.AddEditTerminal, typeof(AddEditTerminalPage));
	}
}
