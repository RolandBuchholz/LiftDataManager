﻿namespace LiftDataManager.Views;

public sealed partial class SonstigesPage : Page
{
    public SonstigesViewModel ViewModel
    {
        get;
    }

    public SonstigesPage()
    {
        ViewModel = App.GetService<SonstigesViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}
