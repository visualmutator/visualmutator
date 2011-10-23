namespace VisualMutator.Model.CodeDifference
{
    public class LineChange
    {
        public LineChange(LineChangeType changeType, string text)
        {
            ChangeType = changeType;
            Text = text;
        }

        public LineChangeType ChangeType
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }
    }
}