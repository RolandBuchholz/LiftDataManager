using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.AllgemeineDaten;
using LiftDataManager.core.Helpers;

namespace LiftDataManager.ViewModels;

public partial class EinreichunterlagenViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;

    public EinreichunterlagenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService, ParameterContext parametercontext) :
         base(parameterDataService, dialogService, navigationService)
    {
        _parametercontext = parametercontext;
        _parametercontext = parametercontext;
    }

    public string DateTimeNow => DateTime.Now.ToShortDateString();

    public string Manufacturer => """
                                    Berchtenbreiter GmbH
                                    Maschinenbau - Aufzugtechnik
                                    Mähderweg 1a
                                    86637 Rieblingen
                                    """;

    public string LiftType
    {
        get
        {
            string aufzugstyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Aufzugstyp");

            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == aufzugstyp);
            return cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
            _ = SetModelStateAsync();
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
