using LiftDataManager.Core.Messenger;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class NumberInputDialogViewModel : ObservableObject
{
    public NumberInputDialogViewModel()
    {

    }

    [RelayCommand]
    public async Task NumberInputDialogLoadedAsync(NumberInputDialog sender)
    {
        Message = sender.Message;
        TextBoxName = sender.TextBoxName;
        MaxLength = sender.MaxLength;
        MinLength = sender.MinLength;
        await Task.CompletedTask;
    }

    [ObservableProperty]
    public partial int MaxLength { get; set; }

    [ObservableProperty]
    public partial int MinLength { get; set; }

    [ObservableProperty]
    public partial string? Message { get; set; }

    [ObservableProperty]
    public partial string? TextBoxName { get; set; }

    [ObservableProperty]
    public partial bool NumberIsValid { get; set; }

    [ObservableProperty]
    public partial int? InputNumber { get; set; }
    partial void OnInputNumberChanged(int? value) 
    {
        NumberIsValid = CheckNumberInput(value);
    }

    private bool CheckNumberInput(int? value)
    {
        return value is not null && value.ToString()?.Length >= MinLength;  
    }

    [RelayCommand]
    public async Task PrimaryButtonClicked(NumberInputDialog sender)
    {
        sender.InputNumber = (int)(InputNumber is not null ? InputNumber : 0);
        await Task.CompletedTask;
    }
}