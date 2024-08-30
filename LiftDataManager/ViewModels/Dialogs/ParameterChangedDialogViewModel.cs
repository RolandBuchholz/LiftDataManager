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
            var parameterName = selectedParameter.Value.UniqueName;
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return;
            }
            var navigationService = App.GetService<IJsonNavigationViewService>();
            navigationService.NavigateTo(typeof(DatenansichtDetailPage), parameterName);
        }
        await Task.CompletedTask;
    }
}

