﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.ViewModels;

public partial class KabineDetailViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{

    public KabineDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, 
                                 ISettingService settingService, ILogger<DataViewModelBase> baseLogger) :
     base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
    }

    [ObservableProperty]
    public partial bool ShowAutoCeilingWarning { get; set; }

    private void CheckAutoCeilingWarning()
    {
        var ruleActivationDate = new DateTime(2024, 01, 11);
        var creationDate = DateTime.MinValue;

        if (DateTime.TryParse(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_ErstelltAm"), out DateTime parsedDate))
            creationDate = parsedDate;

        ShowAutoCeilingWarning = !string.IsNullOrWhiteSpace(ParameterDictionary["var_KD"].Value) && ruleActivationDate.CompareTo(creationDate) > 0;
    }

    [RelayCommand]
    private static void GoToKabine()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(KabinePage));
    }

    [ObservableProperty]
    public partial PivotItem? SelectedPivotItem { get; set; }
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {

                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        CheckAutoCeilingWarning();
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}