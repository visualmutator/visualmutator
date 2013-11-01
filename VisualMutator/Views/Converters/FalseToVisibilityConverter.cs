namespace VisualMutator.Views.Converters
{
    #region

    using System;
    using System.Windows;
    using System.Windows.Data;

    #endregion

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