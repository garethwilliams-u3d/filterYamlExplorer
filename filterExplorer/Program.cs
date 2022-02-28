using System.IO;
using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.DocAsCode.DataContracts.ManagedReference;
using Microsoft.DocAsCode.Metadata.ManagedReference;

using FilterDebugging;

namespace filterExplorer
{
    class Program
    {
        private static string GetCsFile()
        {
            var fixtureDat =
    @"
[System.AttributeUsage(
    System.AttributeTargets.Class | 
    System.AttributeTargets.Method | 
    System.AttributeTargets.Constructor
)]
public class IgnoreAttribute : System.Attribute  
{  
    public IgnoreAttribute(){}
    public IgnoreAttribute(int intParam){}
    
    public int namedIntParam {get; set;}
}
namespace CatLib 
{
    public class Cat 
    {
        [Ignore]
        public Cat(string name) {}
        [Ignore(666)]
        public void Meow() {}
    }
}
namespace DogLib
{
    public class Dog
    {
        [Ignore(666, namedIntParam=999)]
        public void Woof() {}
    }
}
";
            return fixtureDat;
        }

        private static string getFilterYaml()
        {
            var filterYaml =
    @"apiRules:
- exclude:
    uidRegex: ^CatLib\.
";
            return filterYaml;
        }

        private static string CreateTempFilterFile(string _from)
        {
            var filename = "filter.yml";
            File.WriteAllText(filename, _from);
            return filename;
        }

        private static CSharpCompilation CompileAssembly(string _from)
        {
            var tree = CSharpSyntaxTree.ParseText(_from);

            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] 
                { 
                    tree 
                }, 
                references: new[] 
                { 
                    Mscorlib 
                });

            return compilation;
        }

        static void Main(string[] args)
        {
            var filterYaml = getFilterYaml();
            if (args.Length > 0)
            {
                var pathToFilter = args[0];
                filterYaml = File.ReadAllText(pathToFilter);
            }
            var filterFile = CreateTempFilterFile(filterYaml);

            var csFile = GetCsFile();
            if (args.Length > 1)
            {
                var pathToClass = args[1];
                csFile = File.ReadAllText(pathToClass);
            }

            var _compilation = CompileAssembly(_from: csFile);

            var visitor = new SymbolVisitorAdapter(
                generator: new CSYamlModelGenerator() + new VBYamlModelGenerator(),
                language: SyntaxLanguage.CSharp,
                compilation: _compilation,
                options: new ExtractMetadataOptions() { FilterConfigFile = filterFile }
            );

            Console.WriteLine("all members in assembly:");
            foreach (var item in _compilation.Assembly.GlobalNamespace.GetMembers()) Console.WriteLine(item.ToDisplayString());
            Console.WriteLine();

            Console.WriteLine("members included in documentation:");
            var output = _compilation.Assembly.Accept(visitor);
            foreach (var item in output.Items) Console.WriteLine(item.ToString());
            Console.WriteLine();

            Console.WriteLine("rule applications:");
            var report = ReportGenerator.Instance.GetReport();
            Console.WriteLine(report);

            Console.ReadKey();
        }
    }
}
