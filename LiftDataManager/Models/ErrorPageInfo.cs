namespace LiftDataManager.Models;
public class ErrorPageInfo
{
    public ErrorPageInfo(string callerMember, object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs errorArgs)
    {
        CallerMember = callerMember;
        Sender = sender;
        ErrorArgs = errorArgs;
    }

    public string CallerMember { get; set; }
    public object Sender { get; set; }
    public Microsoft.UI.Xaml.UnhandledExceptionEventArgs ErrorArgs { get; set; }
}
