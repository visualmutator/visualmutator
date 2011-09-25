namespace VisualMutator.Views.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    public abstract class Converter<TConverter, TConvertValue, TConvertBackValue> : MarkupExtension, IValueConverter
  where TConverter : class, new()
    {
        // ReSharper disable StaticFieldInGenericType
        private static TConverter _converter;
        // ReSharper restore StaticFieldInGenericType

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new TConverter());
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TConvertValue)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TConvertBackValue)value);
        }

        public abstract TConvertBackValue Convert(TConvertValue value);
        public abstract TConvertValue ConvertBack(TConvertBackValue value);
    }

    public abstract class ParameterConverter<TConverter, TConvertValue, TConvertBackValue, TParameter> : MarkupExtension, IValueConverter
       where TConverter : class, new()
    {
        // ReSharper disable StaticFieldInGenericType
        private static TConverter _converter;
        // ReSharper restore StaticFieldInGenericType

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new TConverter());
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TConvertValue)value, (TParameter)parameter);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TConvertBackValue)value, (TParameter)parameter);
        }

        protected abstract TConvertBackValue Convert(TConvertValue value, TParameter parameter);
        protected abstract TConvertValue ConvertBack(TConvertBackValue value, TParameter parameter);
    }

  

}