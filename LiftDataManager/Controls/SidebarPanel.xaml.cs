namespace LiftDataManager.Controls;

public sealed partial class SidebarPanel : UserControl
{
    public QuickLinksViewModel ViewModel
    {
        get;
    }

    public SidebarPanel()
    {
        ViewModel = App.GetService<QuickLinksViewModel>();
        InitializeComponent();

        //  StackedButtonsExample_Loaded();
    }

    public string InfoText
    {
        get => (string)GetValue(InfoTextProperty);
        set
        {
            SetValue(InfoTextProperty, value);
            InfoTextScroller.UpdateLayout();
            var scrollViewerHeight = InfoTextScroller.ScrollableHeight;
            InfoTextScroller.ChangeView(null, scrollViewerHeight, null);
            CanTextClear = InfoText.Length > 45;
        }
    }

    public static readonly DependencyProperty InfoTextProperty =
        DependencyProperty.Register("InfoText", typeof(string), typeof(SidebarPanel), new PropertyMetadata(string.Empty));

    public bool CanTextClear
    {
        get => (bool)GetValue(CanTextClearProperty);
        set => SetValue(CanTextClearProperty, value);
    }

    public static readonly DependencyProperty CanTextClearProperty =
        DependencyProperty.Register("CanTextClear", typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    public bool ShowQuickLinks
    {
        get
        {
            ViewModel.CheckCanOpenFiles();
            return (bool)GetValue(ShowQuickLinksProperty);
        }

        set => SetValue(ShowQuickLinksProperty, value);
    }

    public static readonly DependencyProperty ShowQuickLinksProperty =
        DependencyProperty.Register("ShowQuickLinks", typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    private void Btn_ClearInfoText_Click(object sender, RoutedEventArgs e)
    {
        InfoText = "Info Sidebar Panel Text gelöscht\n----------\n";
        CanTextClear = false;
    }

    //private SpringVector3NaturalMotionAnimation _springAnimation;
    //Compositor _compositor = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositorForCurrentThread();

    //private void StackedButtonsExample_Loaded()
    //{
    //    // Animate the translation of each button relative to the scale and translation of the button above.
    //    var anim = _compositor.CreateExpressionAnimation();
    //    anim.Expression = "(above.Scale.Y - 1) * 50 + above.Translation.Y % (50 * index)";
    //    anim.Target = "Translation.Y";

    //    // Animate the second button relative to the first.
    //    anim.SetExpressionReferenceParameter("above", ExpressionButton1);
    //    anim.SetScalarParameter("index", 1);
    //    ExpressionButton2.StartAnimation(anim);

    //    // Animate the third button relative to the second.
    //    anim.SetExpressionReferenceParameter("above", ExpressionButton2);
    //    anim.SetScalarParameter("index", 2);
    //    ExpressionButton3.StartAnimation(anim);

    //    // Animate the fourth button relative to the third.
    //    anim.SetExpressionReferenceParameter("above", ExpressionButton3);
    //    anim.SetScalarParameter("index", 3);
    //    ExpressionButton4.StartAnimation(anim);
    //}

    //private void StartAnimationIfAPIPresent(UIElement sender, Microsoft.UI.Composition.CompositionAnimation animation)
    //{
    //    (sender as UIElement).StartAnimation(animation);
    //}

    //private void element_PointerEntered(object sender, PointerRoutedEventArgs e)
    //{
    //    UpdateSpringAnimation(1.5f);

    //    StartAnimationIfAPIPresent((sender as UIElement), _springAnimation);
    //}

    //private void element_PointerExited(object sender, PointerRoutedEventArgs e)
    //{
    //    UpdateSpringAnimation(1f);

    //    StartAnimationIfAPIPresent((sender as UIElement), _springAnimation);
    //}

    //private void UpdateSpringAnimation(float finalValue)
    //{
    //    if (_springAnimation == null)
    //    {
    //        _springAnimation = _compositor.CreateSpringVector3Animation();
    //        _springAnimation.Target = "Scale";
    //    }

    //    _springAnimation.FinalValue = new Vector3(finalValue);
    //    _springAnimation.DampingRatio = 0.6f;
    //    _springAnimation.Period = TimeSpan.FromMilliseconds(50);
    //}
}
