using LiftDataManager.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;

namespace LiftDataManager.Views.Dialogs;

public sealed partial class ImportLiftDataDialog : ContentDialog
{
    public string FullPathXml { get; set; }
    public string SpezifikationName { get; set; }
    public SpezifikationTyp CurrentSpezifikationTyp { get; set; }

    public string? ImportSpezifikationName { get; set; }

    public IEnumerable<TransferData>? ImportPamameter { get; set; }

    public ImportLiftDataDialogViewModel ViewModel
    {
        get;
    }

    public ImportLiftDataDialog(string fullPathXml, string spezifikationName, SpezifikationTyp spezifikationTyp)
    {
        ViewModel = App.GetService<ImportLiftDataDialogViewModel>();
        FullPathXml = fullPathXml;
        SpezifikationName = spezifikationName;
        CurrentSpezifikationTyp = spezifikationTyp;
        InitializeComponent();
    }

    private void Border_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
        e.DragUIOverride.Caption = "Anfrageformular";
        e.DragUIOverride.SetContentFromBitmapImage(
            new BitmapImage(new Uri("ms-appx:///Images/PdfTransparent.png", UriKind.RelativeOrAbsolute)));
        e.DragUIOverride.IsCaptionVisible = true;
        e.DragUIOverride.IsContentVisible = true; 
        e.DragUIOverride.IsGlyphVisible = false;
    }
}
