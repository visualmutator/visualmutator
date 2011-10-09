namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MutationTarget
    {
        public MutationTarget(MethodDefinition method)
        {
            AssemblyFullName = method.DeclaringType.Module.Assembly.FullName;
            TypeFullName = method.DeclaringType.FullName;
            MethodFullName = method.FullName;
        }

        public string AssemblyFullName
        {
            get;
            set;
        }

        public string TypeFullName
        {
            get;
            set;
        }
        public string MethodFullName
        {
            get;
            set;
        }

        public MethodDefinition GetMethod(IEnumerable<AssemblyDefinition> assemblies)
        {
            return assemblies.Single(a=>a.FullName == AssemblyFullName).MainModule
                .Types.Single(t => t.FullName == TypeFullName)
                .Methods.Single(m => m.FullName == MethodFullName);
        }
    }

    public class InstructionMutationTarget : MutationTarget
    {
        public InstructionMutationTarget(MethodDefinition method, Instruction instr)
            : base(method)
        {
            InstructionOffset = instr.Offset;
        }

        public int InstructionOffset
        {
            get;
            set;
        }

    
    }
       

}