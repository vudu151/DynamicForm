namespace DynamicForm.Mobile;

public partial class App : Application
{
	private readonly FormsPage _formsPage;

	public App(FormsPage formsPage)
	{
		InitializeComponent();
		_formsPage = formsPage;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new NavigationPage(_formsPage));
	}
}