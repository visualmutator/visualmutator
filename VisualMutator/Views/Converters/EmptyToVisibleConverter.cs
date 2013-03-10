namespace VisualMutator.Views.Converters
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(IEnumerable), typeof(Visibility))]
    public class EmptyToVisibleConverter : Converter<EmptyToVisibleConverter, IEnumerable, Visibility>
    {

        public override Visibility Convert(IEnumerable value)
        {
            return value == null || !value.GetEnumerator().MoveNext() ? Visibility.Visible : Visibility.Hidden;
        }

        public override IEnumerable ConvertBack(Visibility value)
        {
            throw new NotSupportedException();
        }
    }
}