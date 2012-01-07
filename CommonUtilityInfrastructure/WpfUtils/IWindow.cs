

namespace CommonUtilityInfrastructure.WpfUtils
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;

    public interface IWindow : IView
    {
        bool? ShowDialog();
        bool? ShowDialog(IWindow owner);
        bool? SetDefaultOwnerAndShowDialog();

        void Close();

      //  IView Owner { get; set; }

    
    }
}
