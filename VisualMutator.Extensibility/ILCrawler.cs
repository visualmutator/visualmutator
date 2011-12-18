namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil.Cil;

    public class ILCrawler
    {
        private readonly ILProcessor _proc;

        public ILCrawler(ILProcessor proc)
        {
            _proc = proc;
        }

        public Instruction GetParameterLoadInstruction(Instruction callInstruction, int parameterIndex)
        {
         //   if(callInstruction.OpCode == OpCodes.
            return null;
        }
    }
}