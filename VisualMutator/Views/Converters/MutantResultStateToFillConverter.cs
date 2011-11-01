namespace VisualMutator.Views.Converters
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using CommonUtilityInfrastructure;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.TestsTree;

    #endregion

    [ValueConversion(typeof(TestNodeState), typeof(Brush))]
    public class MutantResultStateToFillConverter : Converter<MutantResultStateToFillConverter, MutantResultState, Brush>
    {
        public override Brush Convert(MutantResultState state)
        {

            return Functional.ValuedSwitch<MutantResultState, Brush>(state)
                .Case(MutantResultState.Untested, Brushes.Gainsboro)
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