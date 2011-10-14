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
            AssemblyName = method.DeclaringType.Module.Assembly.Name.Name;
            TypeFullName = method.DeclaringType.FullName;
            MethodFullName = method.FullName;
        }

        public string AssemblyName
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
            //TODO: Problem with assembly fullname - different versions
            var aa = assemblies.Single(a => a.Name.Name == AssemblyName).MainModule;
            var b = aa.Types.Single(t => t.FullName == TypeFullName);
               var c = b.Methods.Single(m => m.FullName == MethodFullName);
            return c;
        }
    }

    public class InstructionMutationTarget : MutationTarget
    {
        public InstructionMutationTarget(MethodDefinition method, int instr)
            : base(method)
        {
            InstructionOffset = instr;
        }

        public int InstructionOffset
        {
            get;
            set;
        }

    
    }
       

}