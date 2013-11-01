namespace VisualMutator.Views.Converters
{
    #region

    using System;
    using System.Windows;
    using System.Windows.Data;

    #endregion

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : Converter<NullToVisibilityConverter, object, Visibility>
    {
        
        public override Visibility Convert(object value)
        {
            return value == null ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(Visibility value)
        {
            throw new NotSupportedException();
        }
    }
}