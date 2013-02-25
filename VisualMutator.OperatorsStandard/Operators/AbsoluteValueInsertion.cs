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
    using Microsoft.Cci.Immutable;
    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

    public class AbsoluteValueInsertion : IMutationOperator
    {
        #region IMutationOperator Members
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ABS", "Absolute Value Insertion", "");
            }
        }
       

        public IOperatorCodeVisitor FindTargets()
        {
            return new AbsoluteValueInsertionVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new AbsoluteValueInsertionRewriter();
        }

        #endregion

        #region Nested type: AbsoluteValueInsertionRewriter

        public class AbsoluteValueInsertionRewriter : OperatorCodeRewriter
        {
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
              
                var coreAssembly = OperatorCodeVisitor.CastTo<AbsoluteValueInsertionVisitor>().CoreAssembly;
                if (MutationTarget.PassInfo.IsIn("Abs", "NegAbs"))
                {
                    INamedTypeDefinition systemConsole = UnitHelper.FindType(NameTable, coreAssembly, "System.Math");
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
                  //  IMethodDefinition abs = TypeHelper.GetMethod(systemConsole, NameTable.GetNameFor("FailOnZero"), operation.Type);
                    IMethodDefinition abs = (IMethodDefinition) systemConsole.GetMembersNamed(NameTable.GetNameFor("FailOnZero"), false).Single();
                    var call = new MethodCall
                    {
                        IsStaticCall = true,
                        MethodToCall = abs,
                        Type = abs.Type
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
                /*
                var block = new BlockStatement();
                body.Block = block;

                new ConditionalStatement()
                    {
                        Condition = new Equality()
                            {
                                LeftOperand = new BoundExpression()
                                    {
                                        Definition = null
                                    }
                            }
                    };


                /*  new Bra
                  var rr = new ReturnStatement();
                  new LabeledStatement()
                      {
                        
                      }
                  new GotoStatement()
                      {
                          TargetStatement = rr
                      }
              
                  block.Statements.Add(new ThrowStatement());

                   INamedTypeDefinition systemConsole = UnitHelper.FindType(nameTable, coreAssembly, "System.Console");
                  IMethodDefinition writeLine = TypeHelper.GetMethod(systemConsole, nameTable.GetNameFor("WriteLine"),
                                                                     host.PlatformType.SystemString);

                  var call = new MethodCall
                      {IsStaticCall = true, MethodToCall = writeLine, Type = host.PlatformType.SystemVoid};
                  call.Arguments.Add(new CompileTimeConstant {Type = host.PlatformType.SystemString, Value = "hello"});
                  block.Statements.Add(new ExpressionStatement {Expression = call});
                  block.Statements.Add(new ReturnStatement());

                   */

            //    var generatedClass = OperatorCodeVisitor.CastTo<AbsoluteValueInsertionVisitor>().GeneratedBlock;
              //  var methodBody = TypeHelper.GetMethod(generatedClass, 
              //      NameTable.GetNameFor("FailOnZero"), Host.PlatformType.SystemInt32).Body;
               // var generatedBody = (SourceMethodBody) methodBody;
                body.Block = OperatorCodeVisitor.CastTo<AbsoluteValueInsertionVisitor>().GeneratedBlock;
                

                /*
                var testClass = OperatorCodeVisitor.CastTo<AbsoluteValueInsertionVisitor>().GeneratedType;
                testClass.Name = NameTable.GetNameFor("VisualMutatorGeneratedClass");
                var methodDefinition = testClass.Methods.Single(m => m.Name.Value == "FailOnZero").CastTo<MethodDefinition>();
                methodDefinition.Name = NameTable.GetNameFor("FailOnZero");
                methodDefinition.ContainingTypeDefinition = testClass;
                var ss = new NestedUnitNamespace
                    {
                        ContainingUnitNamespace = root,
                        Name = NameTable.GetNameFor("VisualMutatorGeneratedNamespace"),
                        Members = new List<INamespaceMember>
                        {testClass
                        }
                    };
                testClass.ContainingUnitNamespace = ss;
                testClass.InternFactory = Host.InternFactory;
                ((RootUnitNamespace)root).Members.Add(ss);
                ((Assembly) Module).AllTypes.Add(testClass);
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
            public IBlockStatement GeneratedBlock
            {
                get;
                set;
            }
            public IAssembly CoreAssembly
            {
                get;
                set;
            }

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
            }

            


            private void ProcessOperation(IExpression operation)
            {
                //TODO:other types
                if (operation.Type.TypeCode == PrimitiveTypeCode.Int32)
                {
                    List<string> passes = new List<string>
                    {
                        "Abs",
                        "NegAbs",
                        "FailOnZero"
                    }.ToList();

                    MarkMutationTarget(operation, passes);
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

        #endregion
    }
}