namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    public interface IOperatorsPack
    {
        IEnumerable<IMutationOperator> Operators { get;  }
        string Name
        {
            get;
        }

        string Description
        {
            get;
        }
    }
}