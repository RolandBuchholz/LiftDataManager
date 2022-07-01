using System;
using System.Threading.Tasks;
using LiftDataManager.Contracts.Services;
using LiftDataManager.ViewModels;
using Microsoft.UI.Xaml;

namespace LiftDataManager.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(HomeViewModel).FullName, args.Arguments);

        await Task.CompletedTask;
    }
}
