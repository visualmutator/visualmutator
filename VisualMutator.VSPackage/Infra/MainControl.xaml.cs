namespace PiotrTrzpil.VisualMutator_VSPackage.Views
{
    #region Usings

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    using VisualMutator.Views.Abstract;

    #endregion

    /// <summary>
    ///   Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MainControl : UserControl, IMainControl, IDisposable
    {
        public MainControl()
        {
            InitializeComponent();
            ClipToBounds = true;
        }

        public void Dispose()
        {
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(
                    CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", ToString()),
                "VisualMutator");
        }
    }
}