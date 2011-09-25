namespace PiotrTrzpil.VisualMutator_VSPackage.Views.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NotNullToVisibilityConverter : Converter<NotNullToVisibilityConverter, object, Visibility>
    {
        
        public override Visibility Convert(object value)
        {
            return value != null ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(Visibility value)
        {
            throw new NotSupportedException();
        }
    }
}