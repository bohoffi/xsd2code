using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Generic;
using xsd2code.v1.Utils;

namespace xsd2code.v1.Core
{
    public class AttributeGenerator
    {
        private SyntaxGenerator _generator;

        public AttributeGenerator()
        {
            _generator = Util.CreateSyntaxGenerator();
        }

        public AttributeGenerator(SyntaxGenerator generator)
        {
            _generator = generator;
        }

        public SyntaxNode GenerateAttribute(string attributeName)
        {
            var attributeNode = _generator.Attribute(attributeName);
            return attributeNode;
        }

        public SyntaxNode GenerateAttribute(string attributeName, object attributeValue)
        {
            var attributeNode = _generator.Attribute(attributeName);
            var valueExpressionNode = _generator.LiteralExpression(attributeValue);
            var valueNode = _generator.AttributeArgument(valueExpressionNode);
            return _generator
                .AddAttributeArguments(attributeNode, new List<SyntaxNode>
                {
                    valueNode
                });
        }

        public SyntaxNode GenerateAttribute(string attributeName, object attributeValue, Dictionary<string, object> attributeValues)
        {
            var attributeNode = _generator.Attribute(attributeName);
            var valueNodes = new List<SyntaxNode>(attributeValues.Count);

            var valueExpressionNode = _generator.LiteralExpression(attributeValue);
            valueNodes.Add(_generator.AttributeArgument(valueExpressionNode));

            foreach (var entry in attributeValues)
            {
                var valueStatementNode = _generator.AssignmentStatement(
                    _generator.IdentifierName(entry.Key),
                    _generator.LiteralExpression(entry.Value));
                var valueNode = _generator.AttributeArgument(valueStatementNode);
                valueNodes.Add(valueNode);
            }

            return _generator.AddAttributeArguments(attributeNode, valueNodes);
        }

        public SyntaxNode GenerateAttribute(string attributeName, Dictionary<string, object> attributeValues)
        {
            var attributeNode = _generator.Attribute(attributeName);
            var valueNodes = new List<SyntaxNode>(attributeValues.Count);

            foreach (var entry in attributeValues)
            {
                var valueStatementNode = _generator.AssignmentStatement(
                    _generator.IdentifierName(entry.Key),
                    _generator.LiteralExpression(entry.Value));
                var valueNode = _generator.AttributeArgument(valueStatementNode);
                valueNodes.Add(valueNode);
            }

            return _generator.AddAttributeArguments(attributeNode, valueNodes);
        }
    }
}
