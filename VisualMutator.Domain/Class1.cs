namespace VisualMutator.Domain
{
    #region Usings

    using System.Collections.Generic;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public class Class1
    {
        public void CreateTree(IEnumerable<string> paths, IList<AssemblyNode> nodes)
        {

        //    var nodes = new List<AssemblyNode>();

            foreach (string path in paths)
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(path);

                var node = new AssemblyNode(ad);

                nodes.Add(node);

                foreach (TypeDefinition typ in ad.MainModule.Types)
                {
                    node.Types.Add(new TypeNode(typ));


                }
            }
        }
    }
}