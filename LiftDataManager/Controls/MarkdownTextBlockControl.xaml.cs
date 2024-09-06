using Microsoft.UI.Xaml.Media.Imaging;
using System.Windows.Input;

namespace LiftDataManager.Controls;
public sealed partial class MarkdownTextBlockControl : UserControl
{
    private readonly Uri _baseUri = new("ms-appx:///");
    private readonly List<Tuple<string, string, SolidColorBrush>> HashtagsList = [];
    private readonly List<Color> HashtagsColors = [];

    private readonly Dictionary<string, string> _imageMapping = new()
    {
        {"image1","Erstellen von Aufträgen ohne Bestehendes Angebot" },
        {"image2","Parameterhistory" },
        {"image3","Schacht Detaileingabe" },
        {"image143","Funktionsprinzip" },
        {"image144","Auftrag im Vault anlegen" },
        {"image145","Erstellen von Angeboten-Vorplanungen" },
        {"image146","Erstellen von Aufträgen ohne Bestehendes Angebot" },
        {"image147","Bestehendes Angebot in Auftrag konvertieren" },
        {"image148","Liftdaten im LiftDataManager öffnen" },
        {"image149","Liftdaten im LiftDataManager bearbeiten" },
        {"image150","Liftdaten hochladen" },
        {"image151","Home Ansicht" },
        {"image152","Allgemeine Aufzugsdaten" },
        {"image153","Berechnungen" },
        {"image154","Detail Eingaben" },
        {"image155","Listen-Raster-und Tabellenansicht" },
        {"image156","Datenbank editieren" },
        {"image157","Allgemeine Daten" },
        {"image158","Schacht" },
        {"image159","Kabine" },
        {"image160","Bausatz" },
        {"image161","Türen" },
        {"image162","Antrieb Steuerung Notruf" },
        {"image163","Signalisation" },
        {"image164","Wartung Montage TÜV" },
        {"image165","Sonstiges" },
        {"image166","Einreichunterlagen (work in process)" },
        {"image167","Kabinengewicht" },
        {"image168","Nutzlastberechnung" },
        {"image169","Kabinenlüftungsberechnung" },
        {"image170","Schacht Detaileingabe" },
        {"image171","Kabinen Detaileingabe" },
        {"image172","Bausatz Detaileingabe" },
        {"image173","Automatische Überschreibung der Daten" },
        {"image174","Fehlerstatus des Parameters ändern" },
        {"image175","Anpassen der Auswahllisten" },
        {"image176","Inventor" },
        {"image177","Vault" },
        {"image178","CarFrameProgram" },
        {"image179","ZA-Lift" },
        {"image180","Lilo" },
        {"image181","Software Architektur" },
        {"image182","Parameteraufbau" },
        {"image183","Datenbank" },
    };
    public MarkdownTextBlockControl()
    {
        InitializeComponent();
    }

    public ICommand SwitchContentCommand
    {
        get => (ICommand)GetValue(SwitchContentCommandProperty);
        set => SetValue(SwitchContentCommandProperty, value);
    }

    public static readonly DependencyProperty SwitchContentCommandProperty = DependencyProperty.Register(
        nameof(SwitchContentCommand),
        typeof(ICommand),
        typeof(MarkdownTextBlockControl),
        new PropertyMetadata(null));

    public string MarkdownText
    {
        get => (string)GetValue(MarkdownTextProperty);
        set => SetValue(MarkdownTextProperty, value);
    }

    public static readonly DependencyProperty MarkdownTextProperty = DependencyProperty.Register(
            nameof(MarkdownText),
            typeof(string),
            typeof(MarkdownTextBlockControl),
            new PropertyMetadata(default(string)));

    private void MarkdownTextBlock_ImageClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
    {
        var imageName = Path.GetFileNameWithoutExtension(e.Link);
        if (_imageMapping.TryGetValue(imageName, out var link))
        {

            SwitchContentCommand.Execute(link);
        }
    }

    private void MarkdownTextBlock_ImageResolving(object sender, CommunityToolkit.WinUI.UI.Controls.ImageResolvingEventArgs e)
    {
        var imageUrl = new Uri(e.Url[16..], UriKind.Relative);
        var fullImageUri = new Uri(_baseUri, imageUrl);
        if (fullImageUri != null)
        {
            e.Image = new BitmapImage(fullImageUri);
            e.Handled = true;
        }
    }

    private void MarkdownTextBlock_LinkClicked(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
    {
        var link = e.Link;
        if (string.IsNullOrWhiteSpace(link))
        {
            return;
        }
        if (link.StartsWith("https"))
        {
            ProcessHelpers.StartProgram(link, string.Empty);
        }
        else
        {
            SwitchContentCommand.Execute(link);
        }
    }

    private void MarkdownTextBlock_MarkdownRendered(object sender, CommunityToolkit.WinUI.UI.Controls.MarkdownRenderedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MarkdownText))
        {
            return;
        }

        var startTags = MarkdownText.IndexOf("[//]: # (Tags:");
        if (startTags == -1)
        {
            return;
        }
        Random rnd = new();
        HashtagsColors.Add(Colors.BlueViolet);
        HashtagsColors.Add(Colors.DarkOliveGreen);
        HashtagsColors.Add(Colors.DarkRed);
        HashtagsColors.Add(Colors.DarkOrange);
        HashtagsColors.Add(Colors.Goldenrod);
        HashtagsColors.Add(Colors.Brown);
        HashtagsColors.Add(Colors.CadetBlue);
        HashtagsColors.Add(Colors.DarkKhaki);
        HashtagsColors.Add(Colors.Crimson);
        HashtagsColors.Add(Colors.Chocolate);
        HashtagsColors.Add(Colors.BurlyWood);

        var tagString = MarkdownText[startTags..];
        var tagsArray = tagString[15..tagString.IndexOf(')')].Split('|');
        if (tagsArray.Length > 1)
        {
            var subject = tagsArray[0].Trim();
            for (global::System.Int32 i = 1; i < tagsArray.Length; i++)
            {
                HashtagsList.Add(new Tuple<string, string, SolidColorBrush>(subject, tagsArray[i].Trim(), new SolidColorBrush(HashtagsColors[rnd.Next(HashtagsColors.Count)])));
            }
        }
    }
}
