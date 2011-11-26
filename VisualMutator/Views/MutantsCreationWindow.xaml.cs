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

    public interface IMutantsCreationView : IDialogView
    {
        bool? SetOwnerAndShowDialog();
    }

    /// <summary>
    /// Interaction logic for MutantsCreationWindow.xaml
    /// </summary>
    public partial class MutantsCreationWindow : Window, IMutantsCreationView
    {
        private readonly IOwnerWindowProvider _windowProvider;

        public MutantsCreationWindow(IOwnerWindowProvider windowProvider)
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
