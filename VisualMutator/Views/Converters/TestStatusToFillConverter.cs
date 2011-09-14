namespace PiotrTrzpil.VisualMutator_VSPackage.Views.Converters
{
    #region Usings

    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;

    #endregion

    [ValueConversion(typeof(TestNodeState), typeof(Brush))]
    public class TestStatusToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var st = (TestNodeState)value;

            return st == TestNodeState.Inactive  ? Brushes.Gainsboro
                    : st == TestNodeState.Failure ? Brushes.Red 
                    : st == TestNodeState.Inconclusive ? Brushes.Orange
                    : st == TestNodeState.Success ? Brushes.Green
                    : st == TestNodeState.Running ? Brushes.Blue 
                    : Binding.DoNothing;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}