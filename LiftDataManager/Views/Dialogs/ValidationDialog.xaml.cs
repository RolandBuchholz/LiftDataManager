using Cogs.Collections;
using LiftDataManager.ViewModels.Dialogs;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class ValidationDialog : ContentDialog
{
    public ValidationDialogViewModel ViewModel {get;}
    public int ParamterCount { get; set; }
    public ObservableDictionary<string, List<ParameterStateInfo>> ParameterErrorDictionary { get; set; }
    public ValidationDialog(int parameterCount, ObservableDictionary<string, List<ParameterStateInfo>> parameterErrorDictionary)
    {
        ViewModel = App.GetService<ValidationDialogViewModel>();
        DataContext = ViewModel;
        ParamterCount = parameterCount;
        ParameterErrorDictionary = parameterErrorDictionary;
        InitializeComponent();
    }
}