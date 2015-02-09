namespace VisualMutator.Model.Decompilation.CodeDifference
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using DiffLib;
    using log4net;
    using Microsoft.Cci;
    using Mutations;
    using Mutations.MutantsTree;
    using StoringMutants;
    using UsefulTools.Switches;

    #endregion

    public interface ICodeDifferenceCreator
    {

        CodeWithDifference GetDiff(CodeLanguage language, string originalCode, string mutatedCode);
    }

    public class CodeDifferenceCreator : ICodeDifferenceCreator
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public CodeDifferenceCreator()
        {
        }

        public CodeWithDifference GetDiff(CodeLanguage language, string input1, string input2)
        {
            var diff = new StringBuilder();
            var lineChanges = CreateDiff(language, input1, input2, diff);
            return new CodeWithDifference
            {
                Code = diff.ToString(),
                LineChanges = lineChanges
            };
        }

     

        private LineChange NewLineChange(LineChangeType type, 
            StringBuilder diff, int startIndex, int endIndex)
        {
            string text = diff.ToString().Substring(startIndex, endIndex - startIndex - 2);
            return new LineChange(type, text);
        }

        private List<LineChange> CreateDiff(CodeLanguage language, string input1, string input2, StringBuilder diff)
        {
            IEqualityComparer<string> eq = FunctionalExt.ValuedSwitch<CodeLanguage, IEqualityComparer<string>>(language)
                .Case(CodeLanguage.CSharp, () => new CSharpCodeLineEqualityComparer())
                .Case(CodeLanguage.IL, () => new ILCodeLineEqualityComparer())
                .GetResult();
            var differ = new AlignedDiff<string>(
                NormalizeAndSplitCode(input1),
                NormalizeAndSplitCode(input2),
                eq,
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