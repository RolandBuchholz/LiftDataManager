using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SpeziInspector.Controls
{
    public sealed partial class FooterControl : UserControl
    {
        public string XmlPath
        {
            get { return (string)GetValue(XmlPathProperty); }
            set { SetValue(XmlPathProperty, value); }
        }

        public static readonly DependencyProperty XmlPathProperty =
            DependencyProperty.Register("XmlPath", typeof(string), typeof(FooterControl), new PropertyMetadata(string.Empty));

        public int ParameterFound
        {
            get { return (int)GetValue(ParameterfoundProperty); }
            set { SetValue(ParameterfoundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterfoundProperty =
            DependencyProperty.Register("ParameterFound", typeof(int), typeof(FooterControl), new PropertyMetadata(0));



        public FooterControl()
        {
            this.InitializeComponent();
        }
    }
}
