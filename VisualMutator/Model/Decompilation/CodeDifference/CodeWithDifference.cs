namespace VisualMutator.Model.Decompilation.CodeDifference
{
    #region

    using System.Collections.Generic;

    #endregion

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