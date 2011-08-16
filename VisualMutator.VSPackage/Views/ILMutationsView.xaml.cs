namespace PiotrTrzpil.VisualMutator_VSPackage.Views
{
    #region Usings

    using System.Windows;
    using System.Windows.Controls;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    #endregion

    public interface IILMutationsView : IView
    {
        Visibility Visibility { get; set; }
    }

    public partial class ILMutationsView : UserControl, IILMutationsView
    {
        public ILMutationsView()
        {
            InitializeComponent();
        }
    }
}