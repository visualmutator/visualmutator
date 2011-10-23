namespace VisualMutator.Views.Converters
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
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

            return Functional.ValuedSwitch<MutantResultState, Brush>(st)
                .Case(MutantResultState.NoState, Brushes.Gainsboro)
                .Case(MutantResultState.Killed, Brushes.Gray)
                .Case(MutantResultState.Live, Brushes.Orange)
                .Case(MutantResultState.Tested, Brushes.Blue)
                .GetResult();

        }

        public override MutantResultState ConvertBack(Brush br)
        {
            throw new InvalidOperationException();
        }
    }
}