using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace LiftDataManager;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        var isRedirect = DecideRedirection();
        if (!isRedirect)
        {
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new DispatcherQueueSynchronizationContext(
                    DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new App();
            });
        }
    }

    private static void OnActivated(object sender, AppActivationArguments args)
    {
        if (args != null)
        {
            _ = args.Kind;
        }
    }

    private static bool DecideRedirection()
    {
        var isRedirect = false;

        var args = AppInstance.GetCurrent().GetActivatedEventArgs();
        var kind = args.Kind;

        try
        {
            var keyInstance = AppInstance.FindOrRegisterForKey("randomKey");

            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += OnActivated;
            }
            else
            {
                isRedirect = true;
                RedirectActivationTo(args, keyInstance);
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return isRedirect;
    }

    public static void RedirectActivationTo(
        AppActivationArguments args, AppInstance keyInstance)
    {
        var redirectSemaphore = new Semaphore(0, 1);
        Task.Run(() =>
        {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            redirectSemaphore.Release();
        });
        redirectSemaphore.WaitOne();
    }
}
