using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Cecil;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;
    public class CustomCodeRewriter : CodeRewriter 
    {
        public CustomCodeRewriter(IMetadataHost host, bool copyAndRewriteImmutableReferences = false) 
            : base(host, copyAndRewriteImmutableReferences)
        {
            base.dispatchingVisitor
        }
    }
    public class CustomCodeVisitor : CodeVisitor
    {
        
    }
    public class ChangeAdditionIntoSubstractionNew : IMutationOperator
    {

        public class Visi : CustomCodeVisitor
        {
            public override void Visit(IAddition addition)
            {
                base.Visit(addition);
            }
        }


        public CustomCodeVisitor FindTargets(Module module)
        {
            return new Visi();
            // select new MutationTarget().Add("AddInstr", method, instruction);

        }
        public void Mutate(MutationContext context)
        {
            MethodAndInstruction methodAndInstruction = context.MethodAndInstruction("AddInstr");
            var ilProcessor = methodAndInstruction.Method.Body.GetILProcessor();
           // ilProcessor.Replace(methodAndInstruction.Instruction, Instruction.Create(OpCodes.Sub));
        }

        public string Identificator
        {
            get
            {
                return "CAIS";
            }
        }

        public string Name
        {
            get
            {
                return "Change Addition Into Substraction";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of addition with substaction.";
            }
        }

    }
}
