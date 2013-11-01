namespace VisualMutator.Views
{
    #region

    using System.Windows.Controls;
    using UsefulTools.Core;

    #endregion

    public interface IMutantsCreationOptionsView : IView
    {

    }
    public partial class MutantsCreationOptionsView : UserControl, IMutantsCreationOptionsView
    {
        public MutantsCreationOptionsView()
        {
            InitializeComponent();
        }
    }
}
