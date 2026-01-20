using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicForm.Mobile.Models;
using DynamicForm.Mobile.Services;

namespace DynamicForm.Mobile.ViewModels;

public class FormsListViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly IServiceProvider _serviceProvider;

    public ObservableCollection<FormDto> Forms { get; } = new();

    private FormDto? selectedForm;
    public FormDto? SelectedForm
    {
        get => selectedForm;
        set
        {
            if (SetProperty(ref selectedForm, value) && value != null)
            {
                FormSelectedCommand.Execute(value);
            }
        }
    }

    public ICommand LoadFormsCommand { get; }
    public ICommand FormSelectedCommand { get; }

    public FormsListViewModel(ApiService api, IServiceProvider serviceProvider)
    {
        _api = api;
        _serviceProvider = serviceProvider;
        Title = "Danh sách Form";

        LoadFormsCommand = new Command(async () => await LoadFormsAsync());
        FormSelectedCommand = new Command<FormDto>(async form => await OnFormSelected(form));
    }

    public async Task LoadFormsAsync()
    {
        try
        {
            IsBusy = true;
            Forms.Clear();

            var list = await _api.GetFormsAsync();
            foreach (var f in list.OrderBy(x => x.Code))
            {
                Forms.Add(f);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnFormSelected(FormDto form)
    {
        var vm = _serviceProvider.GetRequiredService<MainPageViewModel>();
        vm.FormCode = form.Code;
        vm.ObjectType = form.Code;
        
        var mainPage = _serviceProvider.GetRequiredService<MainPage>();
        await Shell.Current.Navigation.PushAsync(mainPage);
    }
}
