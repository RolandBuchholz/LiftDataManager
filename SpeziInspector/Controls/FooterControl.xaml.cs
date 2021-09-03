﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SpeziInspector.Controls
{
    public sealed partial class FooterControl : UserControl
    {
        public FooterControl()
        {
            InitializeComponent();
        }

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

        public static readonly DependencyProperty ParameterfoundProperty =
            DependencyProperty.Register("ParameterFound", typeof(int), typeof(FooterControl), new PropertyMetadata(0));

    }
}
