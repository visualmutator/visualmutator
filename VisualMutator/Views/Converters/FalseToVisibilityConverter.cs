namespace PiotrTrzpil.VisualMutator_VSPackage.Views.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class FalseToVisibilityConverter : Converter<FalseToVisibilityConverter, bool, Visibility>
    {
        public override Visibility Convert(bool value)
        {
            return !value ? Visibility.Visible : Visibility.Hidden;
        }

        public override bool ConvertBack(Visibility value)
        {
            throw new NotSupportedException();
        }
    }
}