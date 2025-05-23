﻿namespace LiftDataManager.Views;

public sealed partial class GeneralSettingPage : Page
{
    public GeneralSettingViewModel ViewModel
    {
        get;
    }

    public GeneralSettingPage()
    {
        ViewModel = App.GetService<GeneralSettingViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
