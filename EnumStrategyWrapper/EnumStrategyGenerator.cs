using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Mail;

namespace EnumStrategyWrapper
{
    [Generator]
    public class MyEnumStrategyGenerator : ISourceGenerator
    {
        private string NameSpace;
        private List<Tuple<string, List<string>>> AtributeParameters;
        private Dictionary<Tuple<string, string, string>, string> MethodeParameters;
        private Dictionary<Tuple<string, string, string>, string> PropertyParameters;
        private List<UsingDirectiveSyntax> UsingDirectives;
        private Dictionary<string, List<string>> Binds;
        private StringBuilder stringBuilder;
        private readonly string tab = "    ";
        public void Initialize(GeneratorInitializationContext context)
        {
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch();
            //            }
            //#endif
            context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is EnumSyntaxReceiver receiver))
            {
                return;
            }
            string source;
            var compilation = context.Compilation;

            foreach (var enumDeclaration in receiver.CandidateEnums)
            {
                var model = compilation.GetSemanticModel(enumDeclaration.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(enumDeclaration);
                NameSpace = symbol.ContainingNamespace.Name;
                AtributeParameters = receiver.AtributeParameters;
                MethodeParameters = receiver.MethodeParameters;
                PropertyParameters = receiver.PropertyParameters;
                UsingDirectives = receiver.UsingDirectives;
                Binds = receiver.Binds;
                source = GenerateSourceForEnum((INamedTypeSymbol)symbol);

                context.AddSource($"{symbol.Name}Strategy.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
        private string GenerateSourceForEnum(INamedTypeSymbol enumSymbol)
        {
            stringBuilder = new StringBuilder();
            AddUsings();
            AddNamespace(enumSymbol);
            return stringBuilder.ToString();
        }

        private void AddUsings()
        {
            UsingDirectives.ForEach(us =>
            {
                string line = us.ToString();
                stringBuilder.AppendLine(line);
                if (line.Contains("EnumStrategyWrapper.Attribute;"))
                    line = line.Substring(0, line.Length - 10) + "Enumeration;";
                var usingDirective = UsingDirectives.Find(x => x.ToString().Equals(line));
                if (usingDirective is null)
                    stringBuilder.AppendLine(line);
            });
            stringBuilder.AppendLine();
        }
        private void AddNamespace(INamedTypeSymbol enumSymbol)
        {
            stringBuilder.AppendLine($"namespace {NameSpace}");
            stringBuilder.AppendLine("{");
            AddAbstractClass(enumSymbol);
            AddExtendedClasses(enumSymbol);
            stringBuilder.AppendLine("}");
        }
        private void AddAbstractClass(INamedTypeSymbol enumSymbol)
        {
            stringBuilder.AppendLine($"{tab}public abstract class {enumSymbol.Name}Strategy : Enumeration<{enumSymbol.Name}Strategy>");
            stringBuilder.AppendLine($"{tab}{{");
            foreach (var member in enumSymbol.GetMembers())
            {
                if (member.Kind != SymbolKind.Field)
                {
                    continue;
                }

                string fieldName = member.Name;
                string line = $"{tab}{tab}public static readonly {enumSymbol.Name}Strategy {fieldName} = new {fieldName}{enumSymbol.Name}();";
                stringBuilder.AppendLine(line);
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{tab}{tab}protected {enumSymbol.Name}Strategy(int value, string name) : base(value, name)");
            stringBuilder.AppendLine($"{tab}{tab}{{ }}");
            stringBuilder.AppendLine();
            AtributeParameters.ForEach(strategy => strategy.Item2.ForEach(item =>
            {
                if (Binds[enumSymbol.Name].Contains(item))
                {
                    string line = $"{tab}{tab}public abstract object? {item} {{ get; }}";
                    if (strategy.Item1.Equals("StrategyExecutions"))
                        line = $"{tab}{tab}public abstract object? {item}(params object?[]? args);";
                    stringBuilder.AppendLine(line);
                }
            }));
            stringBuilder.AppendLine($"{tab}}}");
        }
        private void AddExtendedClasses(INamedTypeSymbol enumSymbol)
        {
            int index = 1;
            foreach (var member in enumSymbol.GetMembers())
            {
                if (member.Kind != SymbolKind.Field)
                {
                    continue;
                }

                var fieldName = member.Name;
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"{tab}public class {fieldName}{enumSymbol.Name} : {enumSymbol.Name}Strategy");
                stringBuilder.AppendLine($"{tab}{{");
                stringBuilder.AppendLine($"{tab}{tab}public {fieldName}{enumSymbol.Name}() : base({index}, \"{fieldName}\")");
                stringBuilder.AppendLine($"{tab}{tab}{{ }}");
                AtributeParameters.ForEach(strategy => strategy.Item2.ForEach(item =>
                {
                    if (Binds[enumSymbol.Name].Contains(item))
                    {
                        stringBuilder.AppendLine();
                        string line = $"{tab}{tab}public override object? {item}";
                        if (strategy.Item1.Equals("StrategyExecutions"))
                            line = $"{tab}{tab}public override object? {item}(params object?[]? args)";
                        stringBuilder.AppendLine(line);
                        stringBuilder.AppendLine($"{tab}{tab}{{");
                        string returnLine = $"{tab}{tab}{tab}get => default!;";
                        if (PropertyParameters.ContainsKey(new Tuple<string, string, string>(enumSymbol.Name, fieldName, item)))
                            returnLine = $"{tab}{tab}{tab}{PropertyParameters[new Tuple<string, string, string>(enumSymbol.Name, fieldName, item)]}";
                        if (strategy.Item1.Equals("StrategyExecutions"))
                        {
                            returnLine = $"{tab}{tab}{tab}return default!;";
                            if (MethodeParameters.ContainsKey(new Tuple<string, string, string>(enumSymbol.Name, fieldName, item)))
                                returnLine = $"{tab}{tab}{tab}{MethodeParameters[new Tuple<string, string, string>(enumSymbol.Name, fieldName, item)]}";
                        }
                        stringBuilder.AppendLine(returnLine);
                        stringBuilder.AppendLine($"{tab}{tab}}}");
                    }
                }));

                stringBuilder.AppendLine($"{tab}}}");
                index++;
            }
        }
    }
}
