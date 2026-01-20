using DynamicForm.Mobile.ViewModels;

namespace DynamicForm.Mobile;

public partial class FormsPage : ContentPage
{
    private readonly FormsListViewModel _vm;

    public FormsPage(FormsListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_vm.Forms.Any())
        {
            await _vm.LoadFormsAsync();
        }
    }
}
