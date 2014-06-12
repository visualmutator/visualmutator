namespace VisualMutator.Model.Mutations.MutantsTree
{
    public enum MutantResultState
    {

        //Not yet tested
        Untested,

        Creating,

        //During testing
        Tested,


        //Any test failed or was inconclusive
        Killed,

        //All tests passed
        Live,

       

        //Error occurred while testing
        Error,

        Writing
    }
}