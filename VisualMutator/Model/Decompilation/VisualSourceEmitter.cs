namespace VisualMutator.Model.Decompilation
{
    using System.Collections.Generic;
    using CSharpSourceEmitter;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;

    //-----------------------------------------------------------------------------
    //
    // Copyright (c) Microsoft. All rights reserved.
    // This code is licensed under the Microsoft Public License.
    // THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
    // ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
    // IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
    // PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
    //
    //-----------------------------------------------------------------------------

    namespace PeToText
    {
        #region

        using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

        #endregion

        public class VisualSourceEmitter : SourceEmitter
        {

            public VisualSourceEmitter(ISourceEmitterOutput sourceEmitterOutput, IMetadataHost host, PdbReader/*?*/ pdbReader, bool noIL, bool printCompilerGeneratedMembers)
                : base(sourceEmitterOutput)
            {
                this.host = host;
                this.pdbReader = pdbReader;
                this.noIL = noIL;
                this.printCompilerGeneratedMembers = printCompilerGeneratedMembers;
            }

            IMetadataHost host;
            PdbReader/*?*/ pdbReader;
            bool noIL;

            public override void Traverse(IMethodBody methodBody)
            {
                PrintToken(CSharpToken.LeftCurly);

                ISourceMethodBody/*?*/ sourceMethodBody = methodBody as ISourceMethodBody;
                if (sourceMethodBody == null)
                {
                    var options = DecompilerOptions.Loops;
                    if (!printCompilerGeneratedMembers)
                        options |= (DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators);
                    sourceMethodBody = new SourceMethodBody(methodBody, host, pdbReader, pdbReader, options);
                }
                if (noIL)
                    Traverse(sourceMethodBody.Block.Statements);
                else
                {
                   // this.Traverse(sourceMethodBody.Block);
                  //  PrintToken(CSharpToken.NewLine);

                    if (pdbReader != null)
                        PrintScopes(methodBody);
                    else
                        PrintLocals(methodBody.LocalVariables);

                    int currentIndex = -1; // a number no index matches
                    foreach (IOperation operation in methodBody.Operations)
                    {
                        if (pdbReader != null)
                        {
                            foreach (IPrimarySourceLocation psloc in pdbReader.GetPrimarySourceLocationsFor(operation.Location))
                            {
                                if (psloc.StartIndex != currentIndex)
                                {
                                    PrintSourceLocation(psloc);
                                    currentIndex = psloc.StartIndex;
                                }
                            }
                        }
                        PrintOperation(operation);
                    }
                }

                PrintToken(CSharpToken.RightCurly);
            }

            private void PrintScopes(IMethodBody methodBody)
            {
                foreach (ILocalScope scope in pdbReader.GetLocalScopes(methodBody))
                    PrintScopes(scope);
            }

            private void PrintScopes(ILocalScope scope)
            {
                sourceEmitterOutput.Write(string.Format("IL_{0} ... IL_{1} ", scope.Offset.ToString("x4"), (scope.Offset + scope.Length).ToString("x4")), true);
                sourceEmitterOutput.WriteLine("{");
                sourceEmitterOutput.IncreaseIndent();
                PrintConstants(pdbReader.GetConstantsInScope(scope));
                PrintLocals(pdbReader.GetVariablesInScope(scope));
                sourceEmitterOutput.DecreaseIndent();
                sourceEmitterOutput.WriteLine("}", true);
            }

            private void PrintConstants(IEnumerable<ILocalDefinition> locals)
            {
                foreach (ILocalDefinition local in locals)
                {
                    sourceEmitterOutput.Write("const ", true);
                    PrintTypeReference(local.Type);
                    sourceEmitterOutput.WriteLine(" " + GetLocalName(local));
                }
            }

            private void PrintLocals(IEnumerable<ILocalDefinition> locals)
            {
                foreach (ILocalDefinition local in locals)
                {
                    sourceEmitterOutput.Write("", true);
                    PrintTypeReference(local.Type);
                    sourceEmitterOutput.WriteLine(" " + GetLocalName(local));
                }
            }

            public override void PrintLocalName(ILocalDefinition local)
            {
                sourceEmitterOutput.Write(GetLocalName(local));
            }

            private void PrintOperation(IOperation operation)
            {
                sourceEmitterOutput.Write("IL_" + operation.Offset.ToString("x4") + ": ", true);
                sourceEmitterOutput.Write(operation.OperationCode.ToString());
                ILocalDefinition/*?*/ local = operation.Value as ILocalDefinition;
                if (local != null)
                    sourceEmitterOutput.Write(" " + GetLocalName(local));
                else if (operation.Value is string)
                    sourceEmitterOutput.Write(" \"" + operation.Value + "\"");
                else if (operation.Value != null)
                {
                    if (OperationCode.Br_S <= operation.OperationCode && operation.OperationCode <= OperationCode.Blt_Un)
                        sourceEmitterOutput.Write(" IL_" + ((uint)operation.Value).ToString("x4"));
                    else if (operation.OperationCode == OperationCode.Switch)
                    {
                        foreach (uint i in (uint[])operation.Value)
                            sourceEmitterOutput.Write(" IL_" + i.ToString("x4"));
                    }
                    else
                        sourceEmitterOutput.Write(" " + operation.Value);
                }
                sourceEmitterOutput.WriteLine("", false);
            }

            protected virtual string GetLocalName(ILocalDefinition local)
            {
                string localName = local.Name.Value;
                if (pdbReader != null)
                {
                    foreach (IPrimarySourceLocation psloc in pdbReader.GetPrimarySourceLocationsForDefinitionOf(local))
                    {
                        if (psloc.Source.Length > 0)
                        {
                            localName = psloc.Source;
                            break;
                        }
                    }
                }
                return localName;
            }

            private void PrintSourceLocation(IPrimarySourceLocation psloc)
            {
                sourceEmitterOutput.WriteLine("");
                sourceEmitterOutput.Write(psloc.Document.Name.Value + "(" + psloc.StartLine + ":" + psloc.StartColumn + ")-(" + psloc.EndLine + ":" + psloc.EndColumn + "): ", true);
                sourceEmitterOutput.WriteLine(psloc.Source);
            }
        }

    }

   
}