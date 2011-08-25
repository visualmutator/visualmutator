namespace ConsoleApplication2
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Mono.Cecil;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    #endregion

    internal class Program
    {
        private static void Main(string[] args)
        {
            string results = @"C:\results.xml";
            try
            {
          /*      var p = new Process();
                p.StartInfo = new ProcessStartInfo(
@"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\MSTest.exe");


                string filePath = @"C:\Users\SysOp\Documents\Visual Studio 2010\Projects\MusicRename\MusicRename.Tests\bin\Debug\MusicRename.Tests.dll";

                
                p.StartInfo.Arguments = @"/testcontainer:" + QuotePath(filePath) + @" -resultsfile:" + QuotePath(results);


                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;

                File.Delete(results);
                p.Start();
                StreamReader sr = p.StandardOutput;
                string r = sr.ReadToEnd();

         
                p.WaitForExit();

                Console.Out.WriteLine(r);

                string s = File.ReadAllText(results);
                
                Console.Out.WriteLine(s);

                Console.ReadLine();
             //   return;
                */
                XDocument doc = XDocument.Load(results);
               // var ss = doc.Root.Elements().ToList();//.Descendants().ToList();
                 //   Element("Results").Descendants("UnitTestResult");

             //   Utility.DescendantsAnyNs(doc.Root, "UnitTestResult");
                foreach (var testResult in doc.Root.DescendantsAnyNs("UnitTestResult"))
                {
                    string value = testResult.Attribute("testId").Value;
                    var unitTest = doc.Root.DescendantsAnyNs("UnitTest")
                        .Single(n => n.Attribute("id").Value == value);
                    var testMethod = unitTest.ElementAnyNS("TestMethod");

                    string methodName = testMethod.Attribute("name").Value;
                    string longClassName = testMethod.Attribute("className").Value;

                    string fullClassName = longClassName.Substring(0, longClassName.IndexOf(","));

                    int splitIndex = longClassName.LastIndexOf(".");


             //       Trace.WriteLine(fullClassName);
               //     Console.Out.WriteLine(methodName);
                }
                



            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
           
            }
            
            
        }


        public static string QuotePath(string path)
        {
            return @"""" + path + @"""";
        }
    }




}