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

        void MutationLog(string text);

        void ClearMutationLog();
    }

    public partial class ILMutationsView : UserControl, IILMutationsView
    {
        public ILMutationsView()
        {
            InitializeComponent();
        }

        public void MutationLog(string text)
        {
            this.MutationLogBox.AppendText(text+"\n");
        }

        public void ClearMutationLog()
        {
            MutationLogBox.Text = "";
        }
    }
}