using LiftDataManager.Core.DataAccessLayer.Models;
using LiftDataManager.Core.DataAccessLayer;
using Windows.ApplicationModel;

namespace LiftDataManager.ViewModels;

public partial class AboutSettingViewModel : ObservableRecipient, INavigationAwareEx
{
    private readonly ParameterContext _parametercontext;
    public AboutSettingViewModel(ParameterContext parametercontext)
    {
        _parametercontext = parametercontext;
    }
    [ObservableProperty]
    private string? versionDescription;

    public List<LiftDataManagerVersion> VersionsHistory => _parametercontext.Set<LiftDataManagerVersion>().OrderByDescending(x => x.VersionsNumber).ToList();
    
    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();
        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = GetVersionDescription();
    }
    public void OnNavigatedFrom()
    {

    }
}
