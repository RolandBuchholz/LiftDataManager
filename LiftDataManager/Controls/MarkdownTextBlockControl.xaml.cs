using Microsoft.UI.Xaml.Media.Imaging;

namespace LiftDataManager.Controls;
public sealed partial class MarkdownTextBlockControl : UserControl
{
    private readonly Uri _baseUri = new("ms-appx:///");
    public MarkdownTextBlockControl()
    {
        InitializeComponent();
    }

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

    }

    private void MarkdownTextBlock_ImageResolving(object sender, CommunityToolkit.WinUI.UI.Controls.ImageResolvingEventArgs e)
    {
        var imageUrl = new Uri(Path.Combine("Docs", e.Url[1..]), UriKind.Relative);
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
