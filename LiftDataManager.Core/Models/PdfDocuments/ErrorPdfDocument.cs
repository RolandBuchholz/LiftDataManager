using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Windows.ApplicationModel;

namespace LiftDataManager.Core.Models.PdfDocuments;

public class ErrorPdfDocument : IDocument
{
    private readonly string _user;
    private readonly string _LDMVersion;
    private Exception _pdfEx;
    private string? _pdfModel;
    public ErrorPdfDocument(Exception ex, string? pdfModel)
    {
        _user = GetUserName();
        _LDMVersion = GetVersion();
        _pdfEx = ex;
        _pdfModel = pdfModel;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        ImageQuality = 101,
        RasterDpi = 72,
        Title = $"ErrorPdfDocument - LDM Version {_LDMVersion}",
        Author = _user,
        Subject = "ErrorPdfDocument",
        Keywords = "#LiftDataManager#Berechnung",
        ModifiedDate = DateTime.Now,
        CreationDate = DateTime.Now,
    };

    private static string GetUserName()
    {
        string? user;
        try
        {
            user = string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name)
                ? "no user detected"
                : System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("PPS\\", "");
        }
        catch
        {
            user = "Useridentification failed";
        }
        return user;
    }

    private static string GetVersion()
    {
        var version = Package.Current.Id.Version;
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(0, Unit.Millimetre);
            page.MarginBottom(0, Unit.Millimetre);
            page.MarginLeft(0, Unit.Millimetre);
            page.MarginRight(0, Unit.Millimetre);
            page.Header()
                .Background(Colors.Red.Lighten1)
                .Height(75)
                .AlignCenter()
                .AlignMiddle()
                .Text("Error Pdf-Document")
                .Bold()
                .FontSize(36);
            page.Content()
                .Background(Colors.Grey.Lighten2)
                .Padding(15, Unit.Millimetre)
                .AlignLeft()
                .AlignTop()
                .Column(x =>
                {
                    x.Item().Text($"{_pdfModel}")
                            .Bold()
                            .FontColor(Colors.Red.Accent1)
                            .FontSize(24);
                    x.Item().Text($"Exception:")
                            .Bold()
                            .FontSize(16);
                    x.Item().Text($"{_pdfEx}");
                });
            page.Footer()
                .Background(Colors.Red.Lighten1)
                .Height(40)
                .AlignCenter()
                .AlignMiddle()
                .Text($"UserName: {_user} - LDMVersion:{_LDMVersion}")
                .Bold()
                .FontSize(16);
        });
    }
}
