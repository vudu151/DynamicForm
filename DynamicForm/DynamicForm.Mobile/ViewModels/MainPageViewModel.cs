using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicForm.Mobile.Models;
using DynamicForm.Mobile.Services;

namespace DynamicForm.Mobile.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    private readonly ApiService _api;

    private string formCode = "PHIEU_KHAM";
    private string objectId = "PATIENT-001";
    private string objectType = "PHIEU_KHAM";

    public string FormCode
    {
        get => formCode;
        set => SetProperty(ref formCode, value);
    }

    public string ObjectId
    {
        get => objectId;
        set => SetProperty(ref objectId, value);
    }

    public string ObjectType
    {
        get => objectType;
        set => SetProperty(ref objectType, value);
    }

    public Guid CurrentFormVersionId { get; private set; }

    public ObservableCollection<FormFieldDto> Fields { get; } = new();

    public Dictionary<string, object?> Values { get; } = new();

    public ICommand LoadFormCommand { get; }
    public ICommand SubmitCommand { get; }

    public MainPageViewModel(ApiService api)
    {
        _api = api;
        Title = "DynamicForm Mobile";

        LoadFormCommand = new Command(async () => await LoadFormAsync());
        SubmitCommand = new Command(async () => await SubmitAsync(), () => Fields.Any());

        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(Fields))
            {
                ((Command)SubmitCommand).ChangeCanExecute();
            }
        };
    }

    public async Task LoadFormAsync()
    {
        if (string.IsNullOrWhiteSpace(FormCode))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập mã form (Code).", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            Fields.Clear();
            Values.Clear();

            var metadata = await _api.GetFormMetadataByCodeAsync(FormCode);
            if (metadata == null)
            {
                await Shell.Current.DisplayAlert("Lỗi", "Không tải được metadata.", "OK");
                return;
            }

            CurrentFormVersionId = metadata.Version.Id;

            var sortedFields = metadata.Fields
                .Where(f => f.IsVisible)
                .OrderBy(f => f.DisplayOrder)
                .ToList();

            foreach (var field in sortedFields)
            {
                Fields.Add(field);
                Values[field.FieldCode] = field.DefaultValue;
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

    public async Task<ValidationResultDto> ValidateAsync()
    {
        var dataDict = Values.ToDictionary(kv => kv.Key, kv => (object?)kv.Value ?? string.Empty);
        return await _api.ValidateFormDataAsync(CurrentFormVersionId, dataDict);
    }

    public async Task SubmitWithoutValidateAsync()
    {
        var dataDict = Values.ToDictionary(kv => kv.Key, kv => (object?)kv.Value ?? string.Empty);

        var request = new CreateFormDataRequest
        {
            FormVersionId = CurrentFormVersionId,
            ObjectId = ObjectId,
            ObjectType = ObjectType,
            Data = dataDict
        };

        await _api.CreateFormDataAsync(request);
    }

    public async Task SubmitAsync()
    {
        if (!Fields.Any())
        {
            await Shell.Current.DisplayAlert("Lỗi", "Chưa có field nào để submit.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var dataDict = Values.ToDictionary(kv => kv.Key, kv => (object?)kv.Value ?? string.Empty);

            var validation = await _api.ValidateFormDataAsync(CurrentFormVersionId, dataDict);
            if (!validation.IsValid)
            {
                var msg = string.Join("\n", validation.Errors.Select(e => $"{e.FieldCode}: {e.Message}"));
                await Shell.Current.DisplayAlert("Lỗi validate", msg, "OK");
                return;
            }

            var request = new CreateFormDataRequest
            {
                FormVersionId = CurrentFormVersionId,
                ObjectId = ObjectId,
                ObjectType = ObjectType,
                Data = dataDict
            };

            await _api.CreateFormDataAsync(request);
            await Shell.Current.DisplayAlert("Thành công", "Lưu FormData thành công.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi submit", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
