namespace VisualMutator.Tests.Mutations
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DiffMatchPatch;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MetadataReader;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using NUnit.Framework;
    using Operators;
    using OperatorsStandard.Operators;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using Util;
    using MethodIdentifier = Extensibility.MethodIdentifier;

    #endregion

    [TestFixture]
    public class MutantsContainerTests
    {
        [Test]
        public void Test00()
        {
            const string code =
    @"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            return a + b;
        }
    }
}";
            var cci = new CciModuleSource(TestProjects.DsaPath);
            var cci2 = new CciModuleSource(TestProjects.DsaPath);
            var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Deque") as NamedTypeDefinition;
            var method = type.Methods.Single(m => m.Name.Value == "EnqueueFront");

            //   var cci = MutationTestsHelper.CreateModuleFromCode(code);
            var choices = new MutationSessionChoices
            {
                Filter = new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  new MethodIdentifier(method).InList()),
                //Filter = MutationFilter.AllowAll(),
                SelectedOperators = new AOR_ArithmeticOperatorReplacement().InList<IMutationOperator>(),
                WhiteSource = cci.InList(),
            };

            //   var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Test") as NamedTypeDefinition;
            //  var method = type.Methods.Single(m => m.Name.Value == "Method1");

           
            var exec = new MutationExecutor(choices);
            var container = new MutantsContainer(choices, exec);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive(), cci.InList());

            var mut = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>().ElementAt(4);

            


            MutationResult executeMutation = exec.ExecuteMutation(mut, cci2).Result;

            var c = new CodeDeepCopier(cci.Host);
            MethodDefinition methodDefinition = c.Copy(mut.MutationTarget.MethodRaw);

            var vis = new CodeVisualizer(null, null);
            var s = vis.Visualize(CodeLanguage.CSharp, cci2);



            var v = new MutantsCache.Viss(cci2.Host, methodDefinition);
            var modClean = v.Rewrite(cci2.Modules.Single().Module);
            cci2.ReplaceWith(modClean);

            var debug = new DebugOperatorCodeVisitor();
            var debug2 = new DebugOperatorCodeVisitor();
            new DebugCodeTraverser(debug).Traverse(cci.Modules.Single().Module);
            new DebugCodeTraverser(debug2).Traverse(cci2.Modules.Single().Module);
            File.WriteAllText(@"C:\PLIKI\tree1.txt", debug.ToString());
            File.WriteAllText(@"C:\PLIKI\tree2.txt", debug2.ToString());
           //   Console.WriteLine(debug);
           //  Console.WriteLine(debug2);
            //  cci.ReplaceWith(executeMutation.MutatedModules.Modules.Single().Module);

        //    var s2 = vis.Visualize(CodeLanguage.CSharp, cci2);
          //  Console.WriteLine(s);
           // Console.WriteLine(s2);
            //       var viss = new Viss(cci2.Host, sourceMethod);
            //     IModule newMod = viss.Rewrite(executeMutation.MutatedModules.Modules.Single().Module);

            // cci2.ReplaceWith(executeMutation.MutatedModules.Modules.Single().Module);

          //  MutationResult executeMutation2 = exec.ExecuteMutation(mut, cci2).Result;
        }

        [Test]
        public void Test0()
        {

            var cci = new CciModuleSource(TestProjects.DsaPath);
            var cci2 = new CciModuleSource(TestProjects.DsaPath);
            var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Deque") as NamedTypeDefinition;
            var method = type.Methods.Single(m => m.Name.Value == "EnqueueFront");
            var choices = new MutationSessionChoices
                          {
                              Filter = new MutationFilter(
                                  new List<TypeIdentifier>(), 
                                  new MethodIdentifier(method).InList()),
                              SelectedOperators = new AOR_ArithmeticOperatorReplacement().InList<IMutationOperator>(),
                              WhiteSource = cci.InList(),
                          };

            var exec = new MutationExecutor(choices);
            var container = new MutantsContainer(choices, exec);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive(), cci.InList());

            var mut = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>().First();

            var sourceMethod = type.Methods.Single(m => m.Name.Value == "EnqueueFront");


            MutationResult executeMutation = exec.ExecuteMutation(mut, cci2).Result;


     //       var viss = new Viss(cci2.Host, sourceMethod);
       //     IModule newMod = viss.Rewrite(executeMutation.MutatedModules.Modules.Single().Module);

            cci2.ReplaceWith(executeMutation.MutatedModules.Modules.Single().Module);

            MutationResult executeMutation2 = exec.ExecuteMutation(mut, cci2).Result;
        }
        

        [Test]
        public void Test1()
        {
            var cci2 = new CciModuleSource(@"C:\PLIKI\VisualMutator\testprojects\MiscUtil\MiscUtil.UnitTests\bin\Debug\MiscUtil.dll");
            var copied21 = cci2.CreateCopier().Copy(cci2.Modules.Single().Module);
            var copied22 = cci2.CreateCopier().Copy(cci2.Modules.Single().Module);

            var debug1 = new DebugOperatorCodeVisitor();
            var debug2 = new DebugOperatorCodeVisitor();
            new DebugCodeTraverser(debug1).Traverse(copied21);
            new DebugCodeTraverser(debug2).Traverse(copied22);
           // File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\tree1" + ".txt", debug1.ToStringBasicVisit(), Encoding.ASCII);
           // File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\tree2" + ".txt", debug2.ToStringBasicVisit(), Encoding.ASCII);

            var dif = new diff_match_patch();
            var diff = dif.diff_main(debug1.ToStringBasicVisit(), debug2.ToStringBasicVisit(), true);

            var sb = new StringBuilder();
            foreach (var diff1 in diff)
            {
                sb.AppendLine(diff1.ToString());
            }
            File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\treediff", sb.ToString());
            //

            //            foreach (var


            //   trace(@"C:\PLIKI\VisualMutator\testprojects\MiscUtil\MiscUtil.UnitTests\bin\Debug\MiscUtil.dll");
            //            trace(@"C:\PLIKI\VisualMutator\Projekty do testów\MiscUtil\MiscUtil.UnitTests\bin\Debug\MiscUtil.UnitTests.dll");
            //            trace(@"C:\Users\Arego\AppData\Local\Microsoft\VisualStudio\11.0\Designer\ShadowCache\xr5pbts3.ul3\itvv4cbi.0l3\VisualMutator.dll");
            //            trace(@"C:\PLIKI\Programowanie\C#\CREAM\Cream\bin\x86\Debug\TestRunnerNunit.dll");
            //        trace(@"C:\PLIKI\Programowanie\C#\CREAM\Cream\bin\x86\Debug\MutationTools.dll");
        }
        public void trace(string file)
        {
           
            var cci = new CciModuleSource(file);
           // ModuleInfo mod = (ModuleInfo) cci.Modules.Single();
//            var copied = cci.CreateCopier().Copy(cci.Modules.Single().Module);
//            var copied2 = cci.CreateCopier().Copy(cci.Modules.Single().Module);
            var copied3 = cci.CreateCopier().Copy(cci.Modules.Single().Module);
            var white = cci.CloneWith(copied3);

    
            var type = white.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Deque") as NamedTypeDefinition;
            var method = type.Methods.Single(m => m.Name.Value == "EnqueueFront");
            var choices = new MutationSessionChoices
            {
                Filter = new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  new MethodIdentifier(method).InList()),
                SelectedOperators = new AOR_ArithmeticOperatorReplacement().InList<IMutationOperator>(),
                WhiteSource = white.InList(),
            };

            var exec = new MutationExecutor(choices);
            var container = new MutantsContainer(choices, exec);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive(), cci.InList());

            var mutants = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>().ToList();

            string name = Path.GetFileNameWithoutExtension(file);
            var debug = new DebugOperatorCodeVisitor();
            new DebugCodeTraverser(debug).Traverse(copied3);//.Modules.Single().Module);
            File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\" + name + "1.txt", debug.ToString(), Encoding.ASCII);

            var cci2 = new CciModuleSource(file);
            // ModuleInfo mod = (ModuleInfo) cci.Modules.Single();
            var copied21 = cci2.CreateCopier().Copy(cci2.Modules.Single().Module);
            var copied22 = cci2.CreateCopier().Copy(cci2.Modules.Single().Module);

            var debug1 = new DebugOperatorCodeVisitor();
            var debug2 = new DebugOperatorCodeVisitor();
            new DebugCodeTraverser(debug1).Traverse(copied21);
            new DebugCodeTraverser(debug2).Traverse(copied22);
            File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\tree1" + ".txt", debug1.ToString(), Encoding.ASCII);
            File.WriteAllText(@"C:\PLIKI\VisualMutator\trace\tree2" + ".txt", debug2.ToString(), Encoding.ASCII);

//
//            foreach (var mutant in mutants)
//            {
//                var copied23 = cci2.CreateCopier().Copy(cci.Modules.Single().Module);
//                var mutCci = cci2.CloneWith(copied23);
//                
//              //  debug2.ToString().ShouldEqual(debug.ToString());
//
//
//             //   MutationResult executeMutation = exec.ExecuteMutation(mutant, mutCci).Result;
//
//            }




            //            var copier2 = new CodeDeepCopier(new DefaultWindowsRuntimeHost());
            //            var copied2 = copier2.Copy(cci.Modules.Single().Module);
            //
            //            var a1 = new Assembly();
            //            var copier3 = new CodeDeepCopier(new PeReader.DefaultHost(), a1);
            //            var copied3 = copier3.Copy(cci.Modules.Single().Module);
            //
            //            var a2 = new Assembly();
            //            var copier4 = new CodeDeepCopier(new PeReader.DefaultHost(), a2);
            //            var copied4 = copier4.Copy(cci.Modules.Single().Module);


            //var copier5 = new CodeDeepCopier(cci.Host);
            //var copied5 = copier5.Copy(cci.Modules.Single().Module);

            //            var copied1 = cci.Copy();
            //            var copied2 = cci.Copy();
            //            var copied3 = cci.Copy();
            //            var copied4 = cci.Copy();

       
      
        }
    }
}