namespace VisualMutator.Views.Converters
{
    #region

    using System;
    using System.Windows.Data;

    #endregion

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