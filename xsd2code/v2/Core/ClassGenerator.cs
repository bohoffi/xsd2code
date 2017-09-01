using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using xsd2code.v2.Utils;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace xsd2code.v2.Core
{
    public class ClassGenerator
    {
        private AttributeGenerator _attrGen = new AttributeGenerator();
        private CompilationUnitSyntax _compilationUnit;

        internal ClassGenerator(Builder builder)
        {

            _compilationUnit = SF.CompilationUnit()
                .WithUsings(GenerateUsings(builder._usings))
                .WithMembers(SF.SingletonList<MemberDeclarationSyntax>(SF.NamespaceDeclaration(GenerateNamespaceSyntax(builder._namespace))
                    .WithMembers(
                        SF.SingletonList<MemberDeclarationSyntax>(
                            SF.ClassDeclaration(builder._className)
                            .WithModifiers(
                                SF.TokenList(
                                    SF.Token(SyntaxKind.PublicKeyword)))
                            .WithAttributeLists(
                                SF.SingletonList(
                                    SF.AttributeList(
                                        GenerateAttributes(builder._attributes))))
                            .WithMembers(GenerateProperties(builder._properties))))))
                .NormalizeWhitespace();
        }

        private SyntaxList<UsingDirectiveSyntax> GenerateUsings(List<string> usings)
        {
            return SF.List(usings.Select(u => SF.UsingDirective(SF.IdentifierName(u))).ToList());
        }

        private NameSyntax GenerateNamespaceSyntax(string namespaceName)
        {
            var parts = namespaceName.Split('.').ToList();

            if (parts.Count == 1)
            {
                return SF.IdentifierName(parts[0]);
            }

            return parts.Reduce<NameSyntax, string>((part, syntax) =>
            {
                if (syntax == null)
                {
                    return SF.IdentifierName(part);
                }
                else if (syntax is IdentifierNameSyntax)
                {
                    return SF.QualifiedName(syntax, SF.IdentifierName(part));
                }
                else if (syntax is QualifiedNameSyntax)
                {
                    return SF.QualifiedName(syntax, SF.IdentifierName(part));
                }

                return null;
            },
            null);
        }

        private SeparatedSyntaxList<AttributeSyntax> GenerateAttributes(List<Attribute> attributes)
        {
            return SF.SeparatedList(attributes.Select(attr =>
            {
                return _attrGen.GenerateAttribute(attr.Name, attr.Values);
            }).ToList());
        }

        private SyntaxList<MemberDeclarationSyntax> GenerateProperties(List<ClassProperty> props)
        {
            return SF.List(props.Select(p =>
            {
                //return (MemberDeclarationSyntax)SF.PropertyDeclaration(
                //            SF.PredefinedType(
                //                SF.Token(SyntaxKind.StringKeyword)),
                //            SF.Identifier(p.CleanName))
                //       .WithAttributeLists(
                //                SF.SingletonList(
                //                    SF.AttributeList(
                //                        GenerateAttributes(p.Attributes))))
                //       .WithModifiers(
                //            SF.TokenList(
                //                SF.Token(SyntaxKind.PublicKeyword)))
                //       .WithAccessorList(
                //            SF.AccessorList(
                //                SF.List(
                //                    new AccessorDeclarationSyntax[]{
                //                        SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                //                            .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)),
                //                        SF.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                //                            .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))
                //                    })));
                return (MemberDeclarationSyntax)p.ToDeclarationSyntax()
                       .WithAttributeLists(
                                SF.SingletonList(
                                    SF.AttributeList(
                                        GenerateAttributes(p.Attributes))))
                       .WithModifiers(
                            SF.TokenList(
                                SF.Token(SyntaxKind.PublicKeyword)))
                       .WithAccessorList(
                            SF.AccessorList(
                                SF.List(
                                    new AccessorDeclarationSyntax[]{
                                        SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                            .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken)),
                                        SF.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                            .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))
                                    })));
            }).ToList());
        }

        public SyntaxNode GenerateClassCode()
        {
            return _compilationUnit;
        }

        public IObservable<SyntaxNode> GenerateClassNode()
        {
            return Observable.Create<SyntaxNode>(observer => 
            {
                observer.OnNext(GenerateClassCode());
                observer.OnCompleted();

                return Disposable.Empty;
            });
        }

        public class Builder
        {
            internal List<string> _usings;
            internal string _namespace;
            internal string _className;
            //internal Accessibility? _classAccessibility = null;
            internal List<Attribute> _attributes;
            internal List<ClassProperty> _properties;

            public Builder()
            {
                _usings = new List<string>();
                _properties = new List<ClassProperty>();
                _attributes = new List<Attribute>();
            }

            public Builder WithUsing(string usingName)
            {
                if (!_usings.Contains(usingName))
                {
                    _usings.Add(usingName);
                }
                return this;
            }

            public Builder WithUsings(ICollection<string> usingNames)
            {
                _usings = _usings.Union(usingNames).ToList();
                return this;
            }

            public Builder WithNameSpace(string namespaceName)
            {
                _namespace = namespaceName;
                return this;
            }

            public Builder WithClassName(string className)
            {
                _className = className;
                return this;
            }

            //public Builder WithClassAccessibility(Accessibility accessibility)
            //{
            //    _classAccessibility = accessibility;
            //    return this;
            //}

            public Builder WithAttribute(Attribute attribute)
            {
                _attributes.Add(attribute);
                return this;
            }

            //public Builder WithProperties(List<string> names)
            //{
            //    _properties = _properties.Union(names.Select(n =>
            //    {
            //        return new ClassProperty
            //        {
            //            Name = n,
            //            //Accessibility = Accessibility.Public,
            //            IsAutoProperty = false
            //        };
            //    })).ToList();
            //    return this;
            //}

            public Builder WithProperties(List<ClassProperty> props)
            {
                _properties = _properties.Union(props).ToList();
                return this;
            }

            public ClassGenerator Build()
            {
                Validate();
                return new ClassGenerator(this);
            }

            private void Validate()
            {
                if (string.IsNullOrWhiteSpace(_namespace))
                {
                    throw new ArgumentNullException("namespace");
                }
                if (string.IsNullOrWhiteSpace(_className))
                {
                    throw new ArgumentNullException("class name");
                }
            }
        }

        public class ClassProperty
        {
            public ClassProperty(string typeString)
            {
                Attributes = new List<Attribute>();

                TypeString = typeString;
            }

            public string Name { get; set; }
            //public Accessibility Accessibility { get; set; }
            public string TypeString { get; set; }
            public bool IsAutoProperty { get; set; }
            public List<Attribute> Attributes { get; set; }

            public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;

            public PropertyDeclarationSyntax ToDeclarationSyntax()
            {
                var convertedType = TypeConverter.Type(TypeString);
                if (convertedType.HasValue)
                {
                    return new PropertyGenerator().GenerateProperty(CleanName, convertedType.Value);
                }
                else
                {
                    return new PropertyGenerator().GenerateProperty(CleanName, TypeString);
                }
            }
        }

        public class Attribute
        {
            public Attribute()
            {
                Values = new Dictionary<string, object>();
            }

            public string Name { get; set; }
            public Dictionary<string, object> Values { get; set; }
        }
    }
}
