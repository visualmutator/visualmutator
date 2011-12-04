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

namespace VisualMutator.Views
{
    using CommonUtilityInfrastructure.WpfUtils;

    public interface IResultsSavingView : IDialogView
    {
      

    }

    public partial class ResultsSavingView : Window, IResultsSavingView
    {
        private IOwnerWindowProvider _windowProvider;

        public ResultsSavingView(IOwnerWindowProvider windowProvider)
        {
            _windowProvider = windowProvider;
            InitializeComponent();
        }
        public bool? SetOwnerAndShowDialog()
        {
            _windowProvider.SetOwner(this);
            return ShowDialog();
        }
    }
}
