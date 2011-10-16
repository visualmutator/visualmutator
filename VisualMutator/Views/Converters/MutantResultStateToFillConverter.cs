namespace VisualMutator.Views.Converters
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.TestsTree;

    #endregion

    [ValueConversion(typeof(TestNodeState), typeof(Brush))]
    public class MutantResultStateToFillConverter : Converter<MutantResultStateToFillConverter, MutantResultState, Brush>
    {
        public override Brush Convert(MutantResultState st)
        {

            Brush result = st == MutantResultState.NoState ? Brushes.Gainsboro
                    : st == MutantResultState.Killed ? Brushes.Gray 
                    : st == MutantResultState.Live ? Brushes.Orange
                    : st == MutantResultState.Tested ? Brushes.Blue 
                    : null;
            Debug.Assert(result != null);
            return result;
        }

        public override MutantResultState ConvertBack(Brush br)
        {
            throw new InvalidOperationException();
        }
    }
}