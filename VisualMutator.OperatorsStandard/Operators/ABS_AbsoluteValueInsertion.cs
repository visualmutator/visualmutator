namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;

    public class ABS_AbsoluteValueInsertion : IMutationOperator
    { 
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ABS", "Absolute Value Insertion", "");
            }
        }
       

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new ABSVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new ABSRewriter();
        }
        public class ABSVisitor : OperatorCodeVisitor
        {


            private void ProcessOperation(IExpression operation)
            {
                //TODO:other types
                if (operation.Type.TypeCode == PrimitiveTypeCode.Int32)
                {
                    List<string> passes = new List<string>();
                    var con = operation as CompileTimeConstant;
                    if(con != null && con.Value != null)
                    {
                        int value = (int) con.Value;
                        if (value == 0)
                        {
                            passes.Add("FailOnZero");
                        }
                        else if (value < 0)
                        {
                            passes.Add("Abs");
                        }
                        else if (value > 0)
                        {
                            passes.Add("NegAbs");
                        }
                    }
                    if (passes.Count > 0)
                    {
                        MarkMutationTarget(operation, passes);
                    }
                }

            }
            public override void Visit(IExpression operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IRootUnitNamespace ns)
            {
                MarkCommon(ns);
            }

        }
    
        public class ABSRewriter : OperatorCodeRewriter
        {
            public override void Initialize()
            {
                CoreAssembly = Host.LoadAssembly(Host.CoreAssemblySymbolicIdentity);

                var host = new PeReader.DefaultHost();
                IModule module = OperatorUtils.CompileModuleFromCode(
@"using System;
namespace VisualMutatorGeneratedNamespace
{
    public class VisualMutatorGeneratedClass
    {
        public static int FailOnZero(int x)
        {
            if(x == 0) throw new InvalidOperationException(""FailOnZero: x"");
            return x;
        }                                                                                                                                                                                                                 
    }
}", host);
                GeneratedType = (NamespaceTypeDefinition)module.GetAllTypes().Single(t => t.Name.Value == "VisualMutatorGeneratedClass");
                var methodBody = TypeHelper.GetMethod(GeneratedType,
                    host.NameTable.GetNameFor("FailOnZero"), host.PlatformType.SystemInt32).Body;
                var generatedBody = (SourceMethodBody)methodBody;
                GeneratedBlock = generatedBody.Block;
                host.Dispose();
            }

            protected IBlockStatement GeneratedBlock { get; set; }

            protected NamespaceTypeDefinition GeneratedType { get; set; }

            protected IAssembly CoreAssembly { get; set; }

            private IExpression ReplaceOperation<T>(T operation) where T : IExpression
            {
                var mcall = operation as MethodCall;
                if (mcall != null)
                {
                    if (mcall.MethodToCall.Name.Value == "Abs")
                    {
                        return operation;
                    }
                }
              
            
                if (MutationTarget.PassInfo.IsIn("Abs", "NegAbs"))
                {
                    INamedTypeDefinition systemConsole = UnitHelper.FindType(NameTable, CoreAssembly, "System.Math");
                    IMethodDefinition abs = TypeHelper.GetMethod(systemConsole, NameTable.GetNameFor("Abs"), operation.Type);
                    
                    var call = new MethodCall
                        {
                            IsStaticCall = true,
                            MethodToCall = abs,
                            Type = abs.Type
                        };
                    call.Arguments.Add(operation);

                    IExpression result = call;

                    if (MutationTarget.PassInfo == "NegAbs")
                    {
                        result = new UnaryNegation
                            {
                                CheckOverflow = false,
                                Operand = call,
                                Type = operation.Type,
                            };
                    }
                    return result;
                }
                else
                {
                    INamedTypeDefinition systemConsole = UnitHelper.FindType(NameTable, Module, "VisualMutatorGeneratedClass");
                    IMethodDefinition failOnZero = (IMethodDefinition) systemConsole.GetMembersNamed(NameTable.GetNameFor("FailOnZero"), false).Single();
                    var call = new MethodCall
                    {
                        IsStaticCall = true,
                        MethodToCall = failOnZero,
                        Type = failOnZero.Type
                    };
                    call.Arguments.Add(operation);
                    return call;
                }
            }

            public override IExpression Rewrite(IExpression operation)
            {
                return ReplaceOperation(operation);
            }
            public override IRootUnitNamespace Rewrite(IRootUnitNamespace root)
            {
                var testClass = new NamespaceTypeDefinition
                {
                    BaseClasses = new List<ITypeReference>(1) { Host.PlatformType.SystemObject },
                    ContainingUnitNamespace = root,
                    InternFactory = Host.InternFactory,
                    IsClass = true,
                    IsPublic = true,
                    Methods = new List<IMethodDefinition>(1),
                    Name = NameTable.GetNameFor("VisualMutatorGeneratedClass"),
                };
                ((RootUnitNamespace)root).Members.Add(testClass);
                ((Assembly)Module).AllTypes.Add(testClass);


                var mainMethod = new MethodDefinition
                {
                    ContainingTypeDefinition = testClass,
                    InternFactory = Host.InternFactory,
                    IsCil = true,
                    IsStatic = true,
                    Name = NameTable.GetNameFor("FailOnZero"),
                    Type = Host.PlatformType.SystemInt32,
                    Visibility = TypeMemberVisibility.Public,
                };
                mainMethod.Parameters = new List<IParameterDefinition>()
                    {
                        new ParameterDefinition()
                            {
                                Type = Host.PlatformType.SystemInt32,
                                Name = NameTable.GetNameFor("x"),
                            }
                    };
                testClass.Methods.Add(mainMethod);
                
                var body = new SourceMethodBody(Host)
                {
                    MethodDefinition = mainMethod,
                    LocalsAreZeroed = true
                };
                mainMethod.Body = body;
               
               
                body.Block = GeneratedBlock;
                return root;
            }
      
        }

    

    }
}