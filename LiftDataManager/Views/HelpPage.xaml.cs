﻿using LiftDataManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views;

public sealed partial class HelpPage : Page
{
    public HelpViewModel ViewModel
    {
        get;
    }

    public HelpPage()
    {
        ViewModel = App.GetService<HelpViewModel>();
        InitializeComponent();
    }
}
