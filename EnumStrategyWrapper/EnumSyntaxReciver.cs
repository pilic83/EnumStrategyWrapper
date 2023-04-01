using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp;

namespace EnumStrategyWrapper
{
    internal class EnumSyntaxReceiver : ISyntaxReceiver
    {
        public List<EnumDeclarationSyntax> CandidateEnums { get; } = new List<EnumDeclarationSyntax>();
        public List<Tuple<string, List<string>>> AtributeParameters = new List<Tuple<string, List<string>>>();
        public List<MethodDeclarationSyntax> CandidateMethodes { get; } = new List<MethodDeclarationSyntax>();
        public Dictionary<Tuple<string, string, string>, string> MethodeParameters { get; } = new Dictionary<Tuple<string, string, string>, string>();
        public List<PropertyDeclarationSyntax> CandidateProperties { get; } = new List<PropertyDeclarationSyntax>();
        public Dictionary<Tuple<string, string, string>, string> PropertyParameters { get; } = new Dictionary<Tuple<string, string, string>, string>();
        public List<UsingDirectiveSyntax> UsingDirectives { get; } = new List<UsingDirectiveSyntax>();
        public Dictionary<string, List<string>> Binds { get; } = new Dictionary<string, List<string>>();
        private string EnumAttributName => "WrapEnum";
        private string MethodeAttributName => "MemberExecutions";
        private string PropertyAttributName => "MemberProperties";


        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is EnumDeclarationSyntax enumDeclaration)
            {
                Binds[enumDeclaration.Identifier.ValueText] = new List<string>();
                AddUsingsDirectives(syntaxNode);
                AddCandidatesEnum(enumDeclaration);
            }
            if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                AddUsingsDirectives(syntaxNode);
                AddCandidatesMethods(methodDeclaration);
            }
            if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                AddUsingsDirectives(syntaxNode);
                AddCandidatesProperties(propertyDeclaration);
            }
        }
        private void AddCandidatesEnum(EnumDeclarationSyntax enumDeclaration)
        {
            var attributeLists = enumDeclaration.AttributeLists;
            bool hasAttribute = ContainAttribute(attributeLists, EnumAttributName);
            if (hasAttribute)
            {
                var arguments = GetArguments(attributeLists, EnumAttributName);
                if (!(arguments is null))
                {
                    foreach (var argument in arguments)
                    {
                        string argumentName = argument.NameEquals?.Name.ToString();

                        ExpressionSyntax argumentValue = argument.Expression;
                        string arg = argumentValue.ToString();
                        string argProcessing = string.Empty;
                        arg.SkipWhile(ch => ch != '{').Skip(1).
                            TakeWhile(ch => ch != '}').
                            ToList().ForEach(ch => argProcessing += ch.ToString());
                        string[] values = argProcessing.Split(',');
                        List<string> ValuesCleaned = new List<string>();
                        values.ToList().ForEach(value =>
                        {
                            string param = string.Empty;
                            value.ToList().ForEach(ch =>
                            {
                                if (ch != '"' && ch != ' ')
                                    param += ch.ToString();
                            }
                            );
                            ValuesCleaned.Add(param);
                            if (!Binds[enumDeclaration.Identifier.ValueText].Contains(param))
                                Binds[enumDeclaration.Identifier.ValueText].Add(param);
                        }
                        );
                        AtributeParameters.Add(new Tuple<string, List<string>>(argumentName, ValuesCleaned));
                    }
                }
                CandidateEnums.Add(enumDeclaration);
            }
        }
        private void AddCandidatesMethods(MethodDeclarationSyntax methodDeclaration)
        {
            var attributeLists = methodDeclaration.AttributeLists;
            bool hasAttribute = ContainAttribute(attributeLists, MethodeAttributName);
            if (hasAttribute)
            {
                var arguments = GetArguments(attributeLists, MethodeAttributName);
                if (!(arguments is null))
                {
                    List<string> paramList = new List<string>();
                    foreach (var argument in arguments)
                    {
                        ExpressionSyntax argumentValue = argument.Expression;
                        string arg = argumentValue.ToString();
                        string argProcessing = arg.Substring(1, arg.Length - 2);
                        paramList.Add(argProcessing);
                    }
                    Tuple<string, string, string> key = null;
                    if (paramList.Count > 2)
                        key = new Tuple<string, string, string>(paramList[0], paramList[1], paramList[2]);
                    string body = methodDeclaration.Body.ToString();
                    MethodeParameters[key] = body.Substring(1, body.Length - 2);
                }
                CandidateMethodes.Add(methodDeclaration);
            }
        }
        private void AddCandidatesProperties(PropertyDeclarationSyntax propertyDeclaration)
        {
            var attributeLists = propertyDeclaration.AttributeLists;
            bool hasAttribute = ContainAttribute(attributeLists, PropertyAttributName);
            if (hasAttribute)
            {
                var arguments = GetArguments(attributeLists, PropertyAttributName);
                if (!(arguments is null))
                {
                    List<string> paramList = new List<string>();
                    foreach (var argument in arguments)
                    {
                        ExpressionSyntax argumentValue = argument.Expression;
                        string arg = argumentValue.ToString();
                        string argProcessing = arg.Substring(1, arg.Length - 2);
                        paramList.Add(argProcessing);
                    }
                    Tuple<string, string, string> key = null;
                    if (paramList.Count > 1)
                        key = new Tuple<string, string, string>(paramList[0], paramList[1], paramList[2]);
                    var accessorList = propertyDeclaration.AccessorList;
                    var getAccessor = accessorList.Accessors.FirstOrDefault(accessor => accessor.Kind() == SyntaxKind.GetAccessorDeclaration);
                    PropertyParameters[key] = $"{getAccessor.ToString()}";
                }
                CandidateProperties.Add(propertyDeclaration);
            }
        }
        private void AddUsingsDirectives(SyntaxNode syntaxNode)
        {
            var usings = syntaxNode.SyntaxTree.GetCompilationUnitRoot().Usings;
            foreach (var usingDirectiveSintax in usings)
            {
                if (UsingDirectives.Find(x => x.ToString().Equals(usingDirectiveSintax.ToString())) is null)
                    UsingDirectives.Add(usingDirectiveSintax);
            }
        }

        private bool ContainAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributName)
        {
            bool hasAttribute = attributeLists.Any(
                                        attributeList => attributeList.Attributes.Any(
                                            attribute => attribute.Name.ToString().Equals(attributName)));
            return hasAttribute;
        }

        private SeparatedSyntaxList<AttributeArgumentSyntax>? GetArguments(SyntaxList<AttributeListSyntax> attributeLists, string attributName)
        {
            AttributeSyntax attributeSyntax = null;
            attributeLists.ToList()
                .ForEach(attributeList =>
                {
                    var att = attributeList.Attributes.FirstOrDefault(attribute => attribute.Name.ToString().Equals(attributName));
                    if (!(att is null))
                        attributeSyntax = att;
                });
            return attributeSyntax.ArgumentList?.Arguments;
        }
    }
}
