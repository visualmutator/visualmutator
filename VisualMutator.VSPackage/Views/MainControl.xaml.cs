namespace PiotrTrzpil.VisualMutator_VSPackage.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MainControl : UserControl, IMainControl, IDisposable
    {
        public MainControl()
        {
            InitializeComponent();
            this.ClipToBounds = true;
        
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "VisualMutator");

        }

        public void Dispose()
        {
            
        }


    }
}