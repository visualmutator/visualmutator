namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    public class InstructionWithIndex
    {
        private readonly Instruction _instruction;

        private readonly int _index;

        public InstructionWithIndex(Instruction instruction, int index)
        {
            _instruction = instruction;
            _index = index;
        }

        public Instruction Instruction
        {
            get
            {
                return _instruction;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }


        public InstructionWithIndex Previous
        {
            get
            {
                Throw.If(_instruction.Previous == null);
                return new InstructionWithIndex(_instruction.Previous, Index - 1);
            }
        }
        public InstructionWithIndex Next
        {
            get
            {
                Throw.If(_instruction.Next == null);
                return new InstructionWithIndex(_instruction.Next, Index + 1);
            }
        }

        public InstructionWithIndex GoBackBy(int number)
        {
            int index = _index;
            Instruction instr = _instruction;
            for (int i = 0; i < number; i++)
            {
                Throw.If(instr == null);
                instr = instr.Previous;
                index   --;
            }
            return new InstructionWithIndex(instr, index);
            
        }
        public InstructionWithIndex GoForwardBy(int number)
        {
            int index = _index;
            Instruction instr = _instruction;
            for (int i = 0; i < number; i++)
            {
                Throw.If(instr == null);
                instr = instr.Next;
                index++;
            }
            return new InstructionWithIndex(instr, index);

        }

    }
    public static class CecilExtensions
    {
        public static bool IsOfType(this TypeDefinition type, string fullName)
        {
            TypeDefinition currentType = type;

            for (int i = 0; i < 10000; i++)
            {
                if (currentType.FullName == "<Module>" || !currentType.IsClass || currentType.FullName == "System.Object")
                {
                    return false;
                }
                if (currentType.FullName == fullName)
                {
                    return true;
                }

                try
                {
                    currentType = currentType.BaseType.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    return false;
                }

            }
            throw new InvalidOperationException();
        }

        public static IEnumerable<InstructionWithIndex> SelectWithIndexes(this Collection<Instruction> instructions)
        {

            return instructions.Select((instr, index) => new InstructionWithIndex(instr, index));
        }

        public static Instruction GetInstructionAtOffset(this MethodBody body, int offset)
        {
            
            return body.Instructions.Single(i => i.Offset == offset);
        }

        public static ModuleDefinition GetAspNetMvcModule(ModuleDefinition currentModule)
        {
            var mvcModules = currentModule.AssemblyReferences.Where(x => x.Name == "System.Web.Mvc").ToList();
            AssemblyNameReference refer =
                mvcModules.FirstOrDefault(x => x.Version == Version.Parse("3.0.0.0"))
             ?? mvcModules.FirstOrDefault(x => x.Version == Version.Parse("2.0.0.0"));
            if (refer == null)
            {
                throw new ReferencedAssemblyNotFoundException("Valid ASP.NET MVC assembly is not referenced by the project. Only versions 2 and 3 are supported.");
            }
       

            return currentModule.AssemblyResolver.Resolve(refer).MainModule;

        }
    }
}