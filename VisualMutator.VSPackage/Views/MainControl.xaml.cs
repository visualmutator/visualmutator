using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PiotrTrzpil.VisualMutator_VSPackage
{
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MainControl : UserControl, IMainControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "VisualMutator");

        }
    }
}