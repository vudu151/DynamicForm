using Microsoft.Extensions.Logging;
using DynamicForm.Mobile.Services;
using DynamicForm.Mobile.ViewModels;

namespace DynamicForm.Mobile;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Đăng ký DI
		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddSingleton<IServiceProvider>(sp => sp);
		builder.Services.AddTransient<MainPageViewModel>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<FormsListViewModel>();
		builder.Services.AddTransient<FormsPage>();

		return builder.Build();
	}
}
