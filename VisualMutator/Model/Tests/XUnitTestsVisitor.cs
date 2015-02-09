namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;
    using Mutations.Types;
    using TestsTree;
    using UsefulTools.ExtensionMethods;

    public class XUnitTestsVisitor : CodeVisitor
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HashSet<string> _foundTests;
        private TestNodeClass _currentClass;
        private readonly List<TestNodeClass> _classes;

        public List<TestNodeClass> Classes
        {
            get { return _classes; }
        }

        public HashSet<string> FoundTests
        {
            get { return _foundTests; }
        }

        public XUnitTestsVisitor()
        {
            _foundTests = new HashSet<string>();
            _classes = new List<TestNodeClass>();
        }

        public override void Visit(INamespaceTypeDefinition type)
        {
            string nsName = type.ContainingUnitNamespace.ResolvedUnitNamespace.ToString();
            _currentClass = new TestNodeClass(type.Name.Value)
                            {
                                Namespace = nsName

                            };
            _classes.Add(_currentClass);
        }

        public override void Visit(IMethodDefinition method)
        {
           
            var methodd = method.Attributes.Any(a =>
            {
                var attrType = a.Type as INamespaceTypeReference;
                return attrType != null && attrType.GetTypeFullName().IsIn("Xunit.FactAttribute");
            }) ? method : null;
            if(methodd != null && _currentClass != null) //TODO: nested types?
            {
               
                var unspecMethod = MemberHelper.UninstantiateAndUnspecialize(methodd);
                var name = unspecMethod.Name.Value;
                _currentClass.Children.Add(new TestNodeMethod(_currentClass, name));
            }
            
        }

    }
}