namespace VisualMutator.Views
{
    #region

    using System.Windows.Controls;
    using UsefulTools.Core;

    #endregion

    public interface IMutantsTestingOptionsView : IView
    {
         
    }
    public partial class MutantsTestingOptionsView : UserControl, IMutantsTestingOptionsView
    {
        public MutantsTestingOptionsView()
        {
            InitializeComponent();
        }
    }
}
