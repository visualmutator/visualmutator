namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;

    public interface IMessageBoxService
    {
        void Show(string str);
    }

    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxService()
        {
            
        }
        public void Show(string str)
        {
            MessageBox.Show(str);
        }
    }
}