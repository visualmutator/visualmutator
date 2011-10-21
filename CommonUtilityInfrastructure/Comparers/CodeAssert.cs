namespace CommonUtilityInfrastructure.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using DiffLib;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public enum LineChangeType
    {
        Add, Remove
    }
    public class LineChange
    {
        public LineChange(LineChangeType changeType, string text)
        {
            ChangeType = changeType;
            Text = text;
        }

        public LineChangeType ChangeType { get; set; }

        public string Text { get; set; }
    }

    public class CodeWithDifference
    {
        public string Code { get; set; }
        public List<LineChange> LineChanges
        {
            get;
            set;
        }
    }


    public class CodeAssert
    {
        
        public static CodeWithDifference GetDiff(string input1, string input2)
        {
            
            var diff = new StringBuilder();
            var lineChanges = CreateDiff(input1, input2, diff);
            return new CodeWithDifference
            {
                Code = diff.ToString(),
                LineChanges = lineChanges
            };
                
        }

        static LineChange NewLineChange(LineChangeType type, StringBuilder diff, int startIndex, int endIndex)
        {
            string text = diff.ToString().Substring(startIndex, endIndex - startIndex - 2);
            
            return new LineChange(type, text);
        }
        static List<LineChange> CreateDiff(string input1, string input2, StringBuilder diff)
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
                int startIndex=0;
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

        class CodeLineEqualityComparer : IEqualityComparer<string>
        {
            private IEqualityComparer<string> baseComparer = EqualityComparer<string>.Default;

            public bool Equals(string x, string y)
            {
                return baseComparer.Equals(
                    NormalizeLine(x),
                    NormalizeLine(y)
                );
            }

            public int GetHashCode(string obj)
            {
                return baseComparer.GetHashCode(NormalizeLine(obj));
            }
        }

        private static string NormalizeLine(string line)
        {
            line = line.Trim();
            var index = line.IndexOf("//");
            if (index >= 0)
            {
                return line.Substring(0, index);
            }
            else if (line.StartsWith("#"))
            {
                return string.Empty;
            }
            else
            {
                return line;
            }
        }

        private static bool ShouldIgnoreChange(string line)
        {
            // for the result, we should ignore blank lines and added comments
            return NormalizeLine(line) == string.Empty;
        }

        private static IEnumerable<string> NormalizeAndSplitCode(string input)
        {
            return input.Split(new[] { "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);
        }
    }
}
