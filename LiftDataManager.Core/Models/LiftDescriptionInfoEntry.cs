
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Core.Models;

public struct LiftDescriptionInfoEntry(InfoBarSeverity state, string entryName, string message)
{
    public InfoBarSeverity State { get; set; } = state;
    public string EntryName { get; set; } = entryName;
    public string Message { get; set; } = message;
}
