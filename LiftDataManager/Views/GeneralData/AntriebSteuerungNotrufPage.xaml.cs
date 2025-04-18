﻿namespace LiftDataManager.Views;

public sealed partial class AntriebSteuerungNotrufPage : Page
{
    public AntriebSteuerungNotrufViewModel ViewModel
    {
        get;
    }

    public AntriebSteuerungNotrufPage()
    {
        ViewModel = App.GetService<AntriebSteuerungNotrufViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
