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

namespace PiotrTrzpil.VisualMutator_VSPackage.Views
{
    using System.Waf.Applications;

    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public interface IILMutationsView : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.IView
    {
        Visibility Visibility
        {
            get;
            set;
        }
    }
    public partial class ILMutationsView : UserControl, IILMutationsView
    {
        public ILMutationsView()
        {
            InitializeComponent();
       
        }

       
    }
}
