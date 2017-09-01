using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using xsd2code.v1.Utils;

namespace xsd2code.v1.Core
{
    public class ClassGenerator
    {
        private SyntaxGenerator _generator;

        private AttributeGenerator _attributeGenerator;

        private List<SyntaxNode> _usingNodes;
        private SyntaxNode _namespaceNode;
        private SyntaxNode _classNode;

        internal ClassGenerator(Builder builder)
        {
            _generator = Util.CreateSyntaxGenerator();
            _attributeGenerator = new AttributeGenerator(_generator);

            // Generate 'using' nodes
            _usingNodes = builder._usings.Select(u => _generator.NamespaceImportDeclaration(u)).ToList();

            // Generate properties
            var _classProperties = builder._properties
                .Select(p =>
                {
                    var property = _generator.PropertyDeclaration(p.CleanName,
                        _generator.TypeExpression(SpecialType.System_String),
                        p.Accessibility,
                        getAccessorStatements: null,
                        setAccessorStatements: null);
                    var xmlElementAttributeNode = _attributeGenerator.GenerateAttribute("XmlElement", p.Name);
                    return _generator.AddAttributes(property, xmlElementAttributeNode);
                });

            // Generate 'class' node
            _classNode = _generator.ClassDeclaration(builder._className,
                typeParameters: null,
                accessibility: GetAccessibility(builder),
                modifiers: DeclarationModifiers.None,
                baseType: null,
                members: _classProperties);

            // Generate attributes
            var attributeNodes = builder._attributes.Select(a => 
            {
                return _attributeGenerator.GenerateAttribute(a.Name, a.Values);
            }).ToList();
            _classNode = _generator.AddAttributes(_classNode, attributeNodes);

            // Generate 'namespace' node with encapsulated class node
            _namespaceNode = _generator.NamespaceDeclaration(builder._namespace, _classNode);
        }

        private Accessibility GetAccessibility(Builder builder)
        {
            return builder._classAccessibility != null ? builder._classAccessibility.Value : Accessibility.NotApplicable;
        }

        public SyntaxNode GenerateClassNode()
        {
            var declaratrionNodes = new List<SyntaxNode>(_usingNodes)
            {
                _namespaceNode
            };

            return _generator
                .CompilationUnit(declaratrionNodes)
                .NormalizeWhitespace();
        }

        public class Builder
        {
            internal List<string> _usings;
            internal string _namespace;
            internal string _className;
            internal Accessibility? _classAccessibility = null;
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

            public Builder WithClassAccessibility(Accessibility accessibility)
            {
                _classAccessibility = accessibility;
                return this;
            }

            //public Builder WithXmlRootAttribute(string elementName, string namespaceName)
            //{
            //    _xmlRoot = new KeyValuePair<string, string>(elementName, namespaceName);
            //    return this;
            //}
            public Builder WithAttribute(Attribute attribute)
            {
                _attributes.Add(attribute);
                return this;
            }

            public Builder WithProperty(string name)
            {
                _properties.Add(new ClassProperty
                {
                    Name = name,
                    Accessibility = Accessibility.Public,
                    IsAutoProperty = false
                });
                return this;
            }

            public Builder WithProperties(List<string> names)
            {
                _properties = _properties.Union(names.Select(n =>
                {
                    return new ClassProperty
                    {
                        Name = n,
                        Accessibility = Accessibility.Public,
                        IsAutoProperty = false
                    };
                })).ToList();
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
                //if (_xmlRoot == null)
                //{
                //    throw new ArgumentNullException("XmlRoot");
                //}
            }
        }

        internal class ClassProperty
        {
            public string Name { get; set; }
            public Accessibility Accessibility { get; set; }
            public bool IsAutoProperty { get; set; }
            public List<Attribute> Attributes { get; set; }

            public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Util.ToTitleCase(Name.Replace(".", string.Empty)) : string.Empty;
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
