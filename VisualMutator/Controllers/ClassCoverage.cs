using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualMutator.Model.Mutations.Types;
using VisualMutator.Model.Tests.TestsTree;



namespace VisualMutator.Controllers
{
    public static class ClassCoverage
    {
        public static void UnmarkNotCovered(ReadOnlyCollection<AssemblyNode> assemblies, ReadOnlyCollection<TestNodeAssembly> testAssemblies)
        {
            if (assemblies != null)
            {
                foreach (AssemblyNode a in assemblies)
                {
                    if (testAssemblies.Any(b => b.Name == a.Name))
                    {
                        a.IsIncluded = false;
                    }
                    else
                    {
                        foreach (UsefulTools.CheckboxedTree.CheckedNode pr in a.Children)
                        {
                            foreach (UsefulTools.CheckboxedTree.CheckedNode cl in pr.Children)
                            {
                                if (checkClassCoverage(cl, testAssemblies))
                                {
                                    if (cl.Children == null)
                                    {
                                        cl.IsIncluded = true;
                                    }
                                    else
                                    {
                                        foreach (UsefulTools.CheckboxedTree.CheckedNode met in cl.Children)
                                        {
                                            met.IsIncluded = checkMethodCoverage(cl.Name, met, testAssemblies);
                                        }
                                    }
                                }
                                else
                                {
                                    cl.IsIncluded = false;
                                }
                            }
                            if (pr.Children.All(c => c.IsIncluded == false))
                            {
                                pr.IsIncluded = false;
                            }
                        }

                        if (a.Children.All(d => d.IsIncluded == false))
                        {
                            a.IsIncluded = false;
                        }
                    }
                }
            }
        }
        public static bool checkClassCoverage(UsefulTools.CheckboxedTree.CheckedNode cl, ReadOnlyCollection<TestNodeAssembly> testAssemblies)
        {
            string searchedClass = cl.Name;
            bool result = false;

            foreach (TestNodeAssembly test in testAssemblies)
            {
                    if (File.ReadAllText(test.AssemblyPath).Contains(searchedClass))
                    {
                        result = true;
                    }
            }       
            return result; 
        }

        public static bool checkMethodCoverage(string clName,UsefulTools.CheckboxedTree.CheckedNode met, ReadOnlyCollection<TestNodeAssembly> testAssemblies)
        {
            string searchedMethod = met.Name;
            bool result=false;

            foreach (TestNodeAssembly test in testAssemblies)
            {
                if (File.ReadAllText(test.AssemblyPath).Contains(searchedMethod) && File.ReadAllText(test.AssemblyPath).Contains(clName))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
