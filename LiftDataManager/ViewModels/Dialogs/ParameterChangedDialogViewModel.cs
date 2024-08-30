namespace LiftDataManager.ViewModels.Dialogs;
public partial class ParameterChangedDialogViewModel : ObservableObject
{
    public ParameterChangedDialogViewModel()
    {

    }
    [RelayCommand]
    public async Task GoToParameterAsync(ParameterChangedDialog sender)
    {
        var selectedParameter = sender.SelectedParameter;

        if (selectedParameter is InfoCenterEntry)
        {
            sender.Hide();
            var navigationService = App.GetService<IJsonNavigationViewService>();
            navigationService.NavigateTo(typeof(DatenansichtDetailPage), selectedParameter.Value.ParameterName);
        }
        await Task.CompletedTask;
    }
}

