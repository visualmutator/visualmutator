namespace PiotrTrzpil.VisualMutator_VSPackage.Views.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Markup;

  

    public class DebugConverter : MarkupExtension, IValueConverter
    {
        private static DebugConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new DebugConverter());
        }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            Debugger.Break();
            return value; 
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
   
    }
}