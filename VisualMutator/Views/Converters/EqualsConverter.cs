namespace VisualMutator.Views.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(bool))]
    public class EqualsConverter : ParameterConverter<EqualsConverter, object, bool, object>
    {
        protected override bool Convert(object value, object parameter)
        {
            return value.Equals(parameter);
        }

        protected override object ConvertBack(bool value, object parameter)
        {
            throw new NotSupportedException();
        }
    }
}