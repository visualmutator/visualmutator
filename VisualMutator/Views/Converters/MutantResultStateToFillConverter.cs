namespace VisualMutator.Views.Converters
{
    #region

    using System;
    using System.Windows.Data;
    using System.Windows.Media;
    using Model.Mutations.MutantsTree;
    using Model.Tests.TestsTree;
    using UsefulTools.Switches;

    #endregion

    [ValueConversion(typeof(TestNodeState), typeof(Brush))]
    public class MutantResultStateToFillConverter : Converter<MutantResultStateToFillConverter, MutantResultState, Brush>
    {
        public override Brush Convert(MutantResultState state)
        {

            return FunctionalExt.ValuedSwitch<MutantResultState, Brush>(state)
                .Case(MutantResultState.Untested, Brushes.Gainsboro)
                .Case(MutantResultState.Killed, Brushes.Gray)
                .Case(MutantResultState.Live, Brushes.Orange)
                .Case(MutantResultState.Tested, Brushes.Blue)
                .Case(MutantResultState.Creating, Brushes.Green)
                .Case(MutantResultState.Writing, Brushes.Green)
                .Case(MutantResultState.Error, Brushes.Red)
                .GetResult();
        }

        public override MutantResultState ConvertBack(Brush br)
        {
            throw new InvalidOperationException();
        }
    }
}