namespace VisualMutator.Model.CodeDifference
{
    using System.Collections.Generic;

    public class CodeWithDifference
    {
        public string Code
        {
            get;
            set;
        }
        public List<LineChange> LineChanges
        {
            get;
            set;
        }
    }
}