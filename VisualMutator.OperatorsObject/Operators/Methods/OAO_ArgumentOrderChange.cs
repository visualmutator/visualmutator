namespace VisualMutator.OperatorsObject.Operators.Methods
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using LinqLib.Sequence;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;

    public class OAO_ArgumentOrderChange : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("OAO", "Argument order change", "");
            }
        }

        class Reposition
        {
            public int OldPos { get; set; }
            public int NewPos { get; set; }

            public Reposition(int oldPos, int newPos)
            {
                OldPos = oldPos;
                NewPos = newPos;
            }
        }
    
        public class OAOVisitor : OperatorCodeVisitor
        {
            public override void Visit(IMethodCall methodCall)
            {
                var thisMethod = methodCall.MethodToCall.ResolvedMethod;

                var types = thisMethod.Parameters.Select(p => p.Type.ResolvedType).ToList();
                
                if (types.Distinct().Count() < thisMethod.Parameters.Count())
                {
                    MarkMutationTarget(methodCall);
                }
            }

        }

       

        public class OAORewriter : OperatorCodeRewriter
        {

            public override IExpression Rewrite(IMethodCall methodCall)
            {
                var newSeq = new List<List<Reposition>>();
                var thisMethod = methodCall.MethodToCall.ResolvedMethod;
                var types = thisMethod.Parameters.Select(p => p.Type.ResolvedType).ToList();
                var groups = types.Select( (t, i) => new{t,i}).GroupBy(a => a.t, a=>a.i);

                newSeq.AddRange(from @group in groups 
                                select @group.RotateRight(1).Select((i, index) => 
                                    new Reposition(i, @group.ElementAt(index))).ToList());

                var newArgs = new IExpression[types.Count];
                foreach (var repos in newSeq.Flatten())
                {
                    newArgs[repos.NewPos] = methodCall.Arguments.ElementAt(repos.OldPos);

                }
                return new MethodCall(methodCall)
                    {
                        Arguments = newArgs.ToList()
                    };
            }


        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OAOVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OAORewriter();
        }
    }
}