namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;

    public class AstFormatter
    {
        private readonly FormattingVisitor _visitor;

        private class FormattingVisitor : CodeVisitor
        {
            private readonly AstFormatter _astFormatter;
            private string _formattedValue;

            public FormattingVisitor(AstFormatter astFormatter)
            {
                _astFormatter = astFormatter;
               
            }
            private string Format(object obj)
            {
                string ret = _astFormatter.Format(obj);
                return string.IsNullOrWhiteSpace(ret) ? obj.GetType().Name : ret;

            }
            private string Format<T>(IEnumerable<T> enumerable)
            {
                string ret = _astFormatter.Format(enumerable);
                return string.IsNullOrWhiteSpace(ret) ? enumerable.GetType().Name : ret;
            }
            public string FormattedValue
            {
                get
                {
                    string tmp = _formattedValue;
                    _formattedValue = null;
                    return tmp;
                }
            }

            public override void Visit(IThisReference addition)
            {
                _formattedValue = "this";
            }
            public override void Visit(IMethodReference method)
            {
                _formattedValue = method.Name.Value + Format(method.Parameters);
            }
            public override void Visit(IMethodCall methodCall)
            {
                _formattedValue = string.Format("IMethodCall: {0}.{1} Arguments: {2}", Format(methodCall.ThisArgument),
                                     Format(methodCall.MethodToCall), Format(methodCall.Arguments));
            }
            public override void Visit(ICompileTimeConstant compileTimeConstant)
            {
                _formattedValue = compileTimeConstant.Value == null ? "null" : compileTimeConstant.Value.ToString();
            }
            public override void Visit(ITypeReference type)
            {
                var namedTypeReference = type as INamedTypeReference;
                if (namedTypeReference != null)
                {
                    _formattedValue = namedTypeReference.Name.Value;
                }
                else
                {
                    _formattedValue = "<anonymous>";
                }
            }
            public override void Visit(IBinaryOperation binary)
            {
                _formattedValue = Format(binary.LeftOperand) + " # " + Format(binary.RightOperand);
            }
            public override void Visit(IBoundExpression expression)
            {
                _formattedValue = expression.Definition.GetType().Name;
            }

        }

        public AstFormatter()
        {
            _visitor = new FormattingVisitor(this);
        }

        public string Format<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return "null";
            }
            var b = new StringBuilder();
            b.Append("(");
            foreach (var elem in enumerable)
            {
                b.Append(Format(elem));
                b.Append(",");
            }
            if (enumerable.Any())
            {
                b.Remove(b.Length - 1, 1); //remove ,
            }
           
            b.Append(")");
            string ret = b.ToString();
            return string.IsNullOrWhiteSpace(ret) ? enumerable.GetType().Name : ret;
            
        }

        public string Format(object obj)
        {
            string ret;
            if (obj == null)
            {
                return "null";
            }
            var reference = obj as IReference;
            if (reference != null)
            {
                reference.Dispatch(_visitor);
                ret = _visitor.FormattedValue;
                return Return(ret, obj);
            }
            var expression = obj as IExpression;
            if (expression != null)
            {
                expression.Dispatch(_visitor);
                ret = _visitor.FormattedValue;
                return Return(ret, obj);
            }
            var statement = obj as IStatement;
            if (statement != null)
            {
                statement.Dispatch(_visitor);
                ret = _visitor.FormattedValue;
                return Return(ret, obj);
            }
            
            string value = _visitor.FormattedValue;
            ret = value;
            if (value.NullOrEmpty())
            {
                ret = "[" + obj.GetType().Name + "]";
            }
            return Return(ret, obj);
        }

        private string Return(string ret, object obj)
        {
            return string.IsNullOrWhiteSpace(ret) ? obj.GetType().Name : ret;
        }
    }
}