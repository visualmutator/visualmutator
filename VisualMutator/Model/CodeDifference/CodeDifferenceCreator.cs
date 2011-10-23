namespace VisualMutator.Model
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    using CommonUtilityInfrastructure.Comparers;

    using DiffLib;

    using ICSharpCode.Decompiler;
    using ICSharpCode.ILSpy;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;

    #endregion

    public interface ICodeDifferenceCreator
    {
        CodeWithDifference CreateCodeDifference(Mutant mutant, IList<AssemblyDefinition> originalAssemblies);
    }

    public class CodeDifferenceCreator : ICodeDifferenceCreator
    {
        private readonly IAssembliesManager _assembliesManager;

        public CodeDifferenceCreator(IAssembliesManager assembliesManager)
        {
            _assembliesManager = assembliesManager;
        }

        public CodeWithDifference CreateCodeDifference(Mutant mutant, IList<AssemblyDefinition> originalAssemblies)
        {
            var originalMethod = mutant.MutationTarget.GetMethod(originalAssemblies);

            var mutatedAssemblies = _assembliesManager.Load(mutant.StoredAssemblies);
            var mutatedMethod = mutant.MutationTarget.GetMethod(mutatedAssemblies);

            var cs = new CSharpLanguage();

            var mutatedOutput = new PlainTextOutput();
            var originalOutput = new PlainTextOutput();
            var decompilationOptions = new DecompilationOptions();
            decompilationOptions.DecompilerSettings.ShowXmlDocumentation = false;

            cs.DecompileMethod(mutatedMethod, mutatedOutput, decompilationOptions);
            cs.DecompileMethod(originalMethod, originalOutput, decompilationOptions);

            string originalString = originalOutput.ToString().Replace("\t", "   ");
            string mutatedString = mutatedOutput.ToString().Replace("\t", "   ");

            return GetDiff(originalString, mutatedString);
        }

        public CodeWithDifference GetDiff(string input1, string input2)
        {
            var diff = new StringBuilder();
            var lineChanges = CreateDiff(input1, input2, diff);
            return new CodeWithDifference
            {
                Code = diff.ToString(),
                LineChanges = lineChanges
            };
        }

        private LineChange NewLineChange(LineChangeType type, StringBuilder diff, int startIndex, int endIndex)
        {
            string text = diff.ToString().Substring(startIndex, endIndex - startIndex - 2);

            return new LineChange(type, text);
        }

        private List<LineChange> CreateDiff(string input1, string input2, StringBuilder diff)
        {
            var differ = new AlignedDiff<string>(
                NormalizeAndSplitCode(input1),
                NormalizeAndSplitCode(input2),
                new CodeLineEqualityComparer(),
                new StringSimilarityComparer(),
                new StringAlignmentFilter());

            int line1 = 0, line2 = 0;

            var list = new List<LineChange>();
            foreach (var change in differ.Generate())
            {
                int startIndex = 0;
                switch (change.Change)
                {
                    case ChangeType.Same:
                        diff.AppendFormat("{0,4} {1,4} ", ++line1, ++line2);
                        diff.AppendFormat("  ");
                        diff.AppendLine(change.Element1);
                        break;
                    case ChangeType.Added:
                        startIndex = diff.Length;
                        diff.AppendFormat("     {1,4}  +  ", line1, ++line2);

                        diff.AppendLine(change.Element2);
                        list.Add(NewLineChange(LineChangeType.Add, diff, startIndex, diff.Length));
                        break;
                    case ChangeType.Deleted:
                        startIndex = diff.Length;
                        diff.AppendFormat("{0,4}       -  ", ++line1, line2);
                        diff.AppendLine(change.Element1);
                        list.Add(NewLineChange(LineChangeType.Remove, diff, startIndex, diff.Length));
                        break;
                    case ChangeType.Changed:
                        startIndex = diff.Length;
                        diff.AppendFormat("{0,4}      ", ++line1, line2);
                        diff.AppendFormat("(-) ");
                        diff.AppendLine(change.Element1);
                        list.Add(NewLineChange(LineChangeType.Remove, diff, startIndex, diff.Length));
                        startIndex = diff.Length;
                        diff.AppendFormat("     {1,4} ", line1, ++line2);
                        diff.AppendFormat("(+) ");
                        diff.AppendLine(change.Element2);
                        list.Add(NewLineChange(LineChangeType.Add, diff, startIndex, diff.Length));
                        break;
                }
            }

            return list;
        }

        private IEnumerable<string> NormalizeAndSplitCode(string input)
        {
            return input.Split(new[] { "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);
        }

    }
}