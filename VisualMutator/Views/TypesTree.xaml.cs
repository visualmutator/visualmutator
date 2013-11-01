namespace VisualMutator.Views
{
    #region

    using System.Windows.Controls;
    using UsefulTools.Core;

    #endregion

    public interface ITypesTreeView : IView
    {
         
    }
    public partial class TypesTree : UserControl, ITypesTreeView
    {
        public TypesTree()
        {
            InitializeComponent();
        }
    }
}
