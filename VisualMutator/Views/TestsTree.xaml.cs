namespace PiotrTrzpil.VisualMutator_VSPackage.Views
{
    #region Usings

    using System.Windows.Controls;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    #endregion

    /// <summary>
    ///   Interaction logic for UnitTestsView.xaml
    /// </summary>
    public partial class TestsTree : UserControl, IUnitTestsView
    {
        public TestsTree()
        {
            InitializeComponent();
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                //here you would probably want to include code that is called by your
                //mouse down event handler.
                e.Handled = true;
            }
        }
    }
}