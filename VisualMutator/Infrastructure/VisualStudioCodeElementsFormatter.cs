namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model;

    public class VisualStudioCodeElementsFormatter
    {//what abou tnonymous?
        public MethodIdentifier CreateIdentifier(string methodFullName, IList<string> parameters)
        {
            string converted = ConvertConstructorName(methodFullName);
            var result = new MethodIdentifier(AppendMethodParameters(converted, parameters));

            Trace.WriteLine(methodFullName + " converted to " + result);
            return result;
        }

        private string ConvertConstructorName(string methodFullName)
        {
            var id = new MethodIdentifier(methodFullName);
            if(id.ClassSimpleName == id.MethodNameWithoutParams)
            {
                int dotIndex = methodFullName.LastIndexOf('.');
                return methodFullName.Substring(0, dotIndex + 1) + ".ctor";
            }
            return methodFullName;
        }

        private string AppendMethodParameters(string methodName, IList<string> parameters)
        {
            if (parameters.Count == 0)
            {
                return methodName + "()";
            }
            return methodName + ('(' + parameters.Aggregate((a, b) => a + ", " + b) + ')');
        }

        private string ConvertGenericNotation(string fullName)
        {
            string result = fullName;
            var reg = new Regex(@"<(\w+,? *)+>");
            var matches = reg.Matches(fullName);
            foreach (Match match in matches.Cast<Match>())
            {
                var capturesCount = match.Groups[1].Captures.Cast<Capture>().Count();
                result = result.Replace(match.Value, "`" + capturesCount);
            }
            return result;
        }

    }
}