namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    public interface IOperatorsPackage
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