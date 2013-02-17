namespace VisualMutator.OperatorsStandard
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

    public class AbsoluteValueInsertion : IMutationOperator
    {
        #region IMutationOperator Members

        public string Identificator
        {
            get { return "ABS"; }
        }

        public string Name
        {
            get
            {
                return "Absolute Value Insertion";
            }
        }

        public string Description
        {
            get { return ""; }
        }

        public OperatorCodeVisitor FindTargets()
        {
            return new AbsoluteValueInsertionVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new AbsoluteValueInsertionRewriter();
        }

        #endregion

        #region Nested type: AbsoluteValueInsertionRewriter

        public class AbsoluteValueInsertionRewriter : OperatorCodeRewriter
        {
            private IExpression ReplaceOperation<T>(T operation) where T : IExpression
            {
                if (MutationTarget.PassInfo.IsIn("Abs", "NegAbs"))
                {
                    INamedTypeDefinition systemConsole = UnitHelper.FindType(NameTable, Module, "System.Math");
                    IMethodDefinition abs = TypeHelper.GetMethod(systemConsole, NameTable.GetNameFor("Abs"), operation.Type);
                    var call = new MethodCall
                        {
                            IsStaticCall = true,
                            MethodToCall = abs,
                            Type = operation.Type
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
                    IMethodDefinition abs = TypeHelper.GetMethod(systemConsole, NameTable.GetNameFor("FailOnZero"), operation.Type);
                    var call = new MethodCall
                    {
                        IsStaticCall = true,
                        MethodToCall = abs,
                        Type = operation.Type
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
                var testClass = OperatorCodeVisitor.CastTo<AbsoluteValueInsertionVisitor>().GeneratedType;
                testClass.ContainingUnitNamespace = root;
                testClass.InternFactory = Host.InternFactory;
                ((RootUnitNamespace)root).Members.Add(testClass);
                Module.AllTypes.Add(testClass);
                /*
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
                


                var mainMethod = new MethodDefinition
                {
                    ContainingTypeDefinition = testClass,
                    InternFactory = Host.InternFactory,
                    IsCil = true,
                    IsStatic = true,
                    Name = NameTable.GetNameFor("FailOnZero"),
                    Type = Host.PlatformType.SystemVoid,
                    Visibility = TypeMemberVisibility.Public,
                };
          
                testClass.Methods.Add(mainMethod);

                var body = new SourceMethodBody(Host)
                {
                    MethodDefinition = mainMethod,
                    LocalsAreZeroed = true
                };
                mainMethod.Body = body;
                
                var block = new BlockStatement();
                body.Block = block;

                block.Statements.Add(new ThrowStatement());
                */
                return root;
            }
        /*    public override IExpression Rewrite(ISubtraction operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IMultiplication operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IDivision operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IModulus operation)
            {
                return ReplaceOperation(operation);
            }*/
        }

        #endregion

        #region Nested type: AbsoluteValueInsertionVisitor

        public class AbsoluteValueInsertionVisitor : OperatorCodeVisitor
        {
            public NamespaceTypeDefinition GeneratedType { get; set; }

            public override void Initialize()
            {//TODO:float itp
                var tree = SyntaxTree.ParseText(
                @"using System;
                namespace VisualMutatorGeneratedNamespace
                {
                    public class VisualMutatorGeneratedClass
                    {
                        public static void FailOnZero(int x)
                        {
                            if(x == 0) throw new InvalidOperationException(""FailOnZero: x"");
                        }
                    }
                }");

                var comp = Compilation.Create("MyCompilation", 
                    new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddSyntaxTrees(tree)
                    .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location));

                var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
                var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);

                var result = comp.Emit(ilStream);
                ilStream.Close();

                using (var host = new PeReader.DefaultHost())
                {
                    var module = host.LoadUnitFrom(outputFileName) as IModule;
                    if (module == null || module == Dummy.Module || module == Dummy.Assembly)
                    {
                        throw new InvalidOperationException(outputFileName + " is not a PE file containing a CLR module or assembly.");
                    }


                    Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, null);
                    GeneratedType = (NamespaceTypeDefinition) decompiledModule.AllTypes.Single(t => t.Name.Value == "VisualMutatorGeneratedClass");


                }
            }

            private void ProcessOperation(IExpression operation)
            {
                List<string> passes = new List<string>
                    {
                        "Abs",
                        "NegAbs",
                        "FailOnZero"
                    }.ToList();

                MarkMutationTarget(operation, passes);
            }
            public override void Visit(IExpression operation)
            {
                ProcessOperation(operation);
            }
            

            /*
            public override void Visit(IAddition operation)
            {
                ProcessOperation(operation);
            }

            public override void Visit(ISubtraction operation)
            {
                ProcessOperation(operation);
            }

            public override void Visit(IMultiplication operation)
            {
                ProcessOperation(operation);
            }

            public override void Visit(IDivision operation)
            {
                ProcessOperation(operation);
            }

            public override void Visit(IModulus operation)
            {
                ProcessOperation(operation);
            }*/
        }

        #endregion
    }
}