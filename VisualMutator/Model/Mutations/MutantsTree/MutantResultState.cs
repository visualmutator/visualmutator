namespace VisualMutator.Model.Mutations.MutantsTree
{
    public enum MutantResultState
    {

        //Not yet tested
        Untested,

        //Any test failed or was inconclusive
        Killed,

        //All tests passed
        Live,

        //During testing
        Tested,

        //Error occurred while testing
        Error,
    }
}