namespace VisualMutator.Views
{
    #region Usings

    using System.Windows.Controls;
    using System.Windows.Input;

    using CommonUtilityInfrastructure.WpfUtils;

 
    #endregion

   public interface ITestsTreeView : IView
   {
       
   }
   public partial class TestsTree : UserControl, ITestsTreeView
    {
        public TestsTree()
        {
            InitializeComponent();
        }



    }
}