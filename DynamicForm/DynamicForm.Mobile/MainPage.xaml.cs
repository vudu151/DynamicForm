using DynamicForm.Mobile.Models;
using DynamicForm.Mobile.ViewModels;

namespace DynamicForm.Mobile;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _vm;

    // map FieldCode -> Label lỗi
    private readonly Dictionary<string, Label> _errorLabels = new();

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;

        // Subscribe to Fields collection changes
        _vm.Fields.CollectionChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() => BuildDynamicFormUI());
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildDynamicFormUI();
    }

    private void BuildDynamicFormUI()
    {
        FormLayout.Children.Clear();
        _errorLabels.Clear();

        foreach (var field in _vm.Fields)
        {
            if (!field.IsVisible)
                continue;

            // Label
            var label = new Label
            {
                Text = field.Label + (field.IsRequired ? " *" : string.Empty),
                FontAttributes = FontAttributes.Bold
            };
            FormLayout.Children.Add(label);

            // Input control
            View input;

            switch (field.FieldType)
            {
                case 1: // Text
                    input = CreateTextEntry(field);
                    break;
                case 2: // Number
                    input = CreateNumberEntry(field);
                    break;
                case 3: // Date
                    input = CreateDatePicker(field);
                    break;
                case 6: // Select
                    input = CreatePicker(field);
                    break;
                case 10: // TextArea
                    input = CreateEditor(field);
                    break;
                default:
                    input = CreateTextEntry(field);
                    break;
            }

            FormLayout.Children.Add(input);

            // Label hiển thị lỗi cho field này
            var errorLabel = new Label
            {
                TextColor = Colors.Red,
                FontSize = 12,
                IsVisible = false
            };
            _errorLabels[field.FieldCode] = errorLabel;
            FormLayout.Children.Add(errorLabel);

            if (!string.IsNullOrWhiteSpace(field.HelpText))
            {
                FormLayout.Children.Add(new Label
                {
                    Text = field.HelpText,
                    FontSize = 12,
                    TextColor = Colors.Gray
                });
            }
        }
    }

    private View CreateTextEntry(FormFieldDto field)
    {
        var entry = new Entry
        {
            Placeholder = field.Placeholder,
            Text = field.DefaultValue
        };

        entry.TextChanged += (_, e) =>
        {
            _vm.Values[field.FieldCode] = e.NewTextValue;
        };

        return entry;
    }

    private View CreateNumberEntry(FormFieldDto field)
    {
        var entry = new Entry
        {
            Placeholder = field.Placeholder,
            Keyboard = Keyboard.Numeric,
            Text = field.DefaultValue
        };

        entry.TextChanged += (_, e) =>
        {
            if (double.TryParse(e.NewTextValue, out var number))
                _vm.Values[field.FieldCode] = number;
            else
                _vm.Values[field.FieldCode] = e.NewTextValue;
        };

        return entry;
    }

    private View CreateDatePicker(FormFieldDto field)
    {
        var picker = new DatePicker();

        picker.DateSelected += (_, e) =>
        {
            _vm.Values[field.FieldCode] = e.NewDate;
        };

        // Nếu có DefaultValue là string date thì parse
        if (DateTime.TryParse(field.DefaultValue, out var dt))
        {
            picker.Date = dt;
            _vm.Values[field.FieldCode] = dt;
        }

        return picker;
    }

    private View CreatePicker(FormFieldDto field)
    {
        var picker = new Picker
        {
            Title = field.Placeholder ?? field.Label
        };

        foreach (var opt in field.Options.OrderBy(o => o.DisplayOrder))
        {
            picker.Items.Add(opt.Label);
        }

        picker.SelectedIndexChanged += (_, _) =>
        {
            if (picker.SelectedIndex < 0)
            {
                _vm.Values[field.FieldCode] = null;
                return;
            }

            var selected = field.Options.OrderBy(o => o.DisplayOrder).ElementAt(picker.SelectedIndex);
            _vm.Values[field.FieldCode] = selected.Value;
        };

        return picker;
    }

    private View CreateEditor(FormFieldDto field)
    {
        var editor = new Editor
        {
            Placeholder = field.Placeholder,
            AutoSize = EditorAutoSizeOption.TextChanges,
            Text = field.DefaultValue
        };

        editor.TextChanged += (_, e) =>
        {
            _vm.Values[field.FieldCode] = e.NewTextValue;
        };

        return editor;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (!_vm.Fields.Any())
        {
            await DisplayAlert("Lỗi", "Chưa có field nào để submit.", "OK");
            return;
        }

        try
        {
            _vm.IsBusy = true;

            // clear lỗi cũ
            foreach (var lbl in _errorLabels.Values)
            {
                lbl.Text = string.Empty;
                lbl.IsVisible = false;
            }

            // 1) Validate qua API
            var result = await _vm.ValidateAsync();

            if (!result.IsValid)
            {
                // map lỗi vào từng field
                foreach (var error in result.Errors)
                {
                    if (_errorLabels.TryGetValue(error.FieldCode, out var lbl))
                    {
                        lbl.Text = error.Message;
                        lbl.IsVisible = true;
                    }
                }

                // thêm popup tổng quan
                var msg = string.Join("\n", result.Errors.Select(e2 => $"{e2.FieldCode}: {e2.Message}"));
                await DisplayAlert("Lỗi validate", msg, "OK");
                return;
            }

            // 2) Nếu hợp lệ -> submit
            await _vm.SubmitWithoutValidateAsync();
            await DisplayAlert("Thành công", "Lưu FormData thành công.", "OK");

            // có thể reset form nếu muốn
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi submit", ex.Message, "OK");
        }
        finally
        {
            _vm.IsBusy = false;
        }
    }
}
