using Microsoft.UI.Xaml.Media.Imaging;
using System.Windows.Input;

namespace LiftDataManager.Controls;
public sealed partial class MarkdownTextBlockControl : UserControl
{
    private readonly Uri _baseUri = new("ms-appx:///");

    private readonly Dictionary<string, string> _imageMapping = new() 
    {
        {"image1","Erstellen von Aufträgen ohne Bestehendes Angebot" },
        {"image2","Parameterhistory" },
        {"image3","Schacht Detaileingabe" },
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
        ProcessHelpers.StartProgram(e.Link, string.Empty);
    }
}
