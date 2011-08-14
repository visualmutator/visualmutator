namespace PiotrTrzpil.VisualMutator_VSPackage.Views.Converters
{
    #region Usings

    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests;

    #endregion

    [ValueConversion(typeof(TestStatus), typeof(Brush))]
    public class TestStatusToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var st = (TestStatus)value;

            return
                st == TestStatus.Inconclusive
                    ? Brushes.Gainsboro
                    : st == TestStatus.Failure
                          ? Brushes.Red
                          : st == TestStatus.Success
                                ? Brushes.Green
                                : st == TestStatus.Running ? Brushes.Blue : Binding.DoNothing;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}