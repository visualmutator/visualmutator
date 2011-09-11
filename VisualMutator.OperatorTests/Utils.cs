namespace VisualMutator.OperatorTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Utils
    {
        public const string TestAssembliesFolder = @"..\..\TestAssemblies";

        public static string NerdDinner3AssemblyPath 
            = Path.Combine(TestAssembliesFolder, @"NerdDinner3-Debug\NerdDinner.dll");


    }
}