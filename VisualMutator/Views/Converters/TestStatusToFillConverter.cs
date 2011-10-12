namespace VisualMutator.Views.Converters
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using VisualMutator.Model.Tests.TestsTree;

    #endregion

    [ValueConversion(typeof(TestNodeState), typeof(Brush))]
    public class TestStatusToFillConverter : Converter<TestStatusToFillConverter, TestNodeState, Brush>
    {
        public override Brush Convert(TestNodeState st)
        {
        
            Brush result = st == TestNodeState.Inactive  ? Brushes.Gainsboro
                    : st == TestNodeState.Failure ? Brushes.Red 
                    : st == TestNodeState.Inconclusive ? Brushes.Orange
                    : st == TestNodeState.Success ? Brushes.Green
                    : st == TestNodeState.Running ? Brushes.Blue 
                    : null;
            Debug.Assert(result != null);
            return result;
        }

        public override TestNodeState ConvertBack( Brush br  )
        {
            throw new InvalidOperationException();
        }
    }
}