using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace xsd2code.v2.Core
{
    public class AttributeGenerator
    {
        public AttributeSyntax GenerateAttribute(string attributeName)
        {
            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeName));
        }

        public AttributeSyntax GenerateAttribute(string attributeName, object attributeValue)
        {
            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeName))
                .AddArgumentListArguments(SyntaxFactory.AttributeArgument(CreateExpressionSyntax(attributeValue)));
        }

        public AttributeSyntax GenerateAttribute(string attributeName, object attributeValue, Dictionary<string, object> attributeValues)
        {
            var valueNodes = new SeparatedSyntaxList<AttributeArgumentSyntax>();

            valueNodes.Add(SyntaxFactory.AttributeArgument(CreateExpressionSyntax(attributeValue)));

            foreach (var entry in attributeValues)
            {
                valueNodes.Add(
                    SyntaxFactory.AttributeArgument(CreateExpressionSyntax(entry.Value))
                    .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(entry.Key))));
            }

            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeName))
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(valueNodes));
        }

        public AttributeSyntax GenerateAttribute(string attributeName, Dictionary<string, object> attributeValues)
        {
            var valueNodes = new SeparatedSyntaxList<AttributeArgumentSyntax>();

            foreach (var entry in attributeValues)
            {
                valueNodes = valueNodes.Add(
                    SyntaxFactory.AttributeArgument(CreateExpressionSyntax(entry.Value))
                    .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(entry.Key))));
            }

            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeName))
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(valueNodes));
        }

        private LiteralExpressionSyntax CreateExpressionSyntax(object value)
        {
            if (value is ulong)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((ulong)value));
            }
            else if (value is float)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((float)value));
            }
            else if (value is double)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((double)value));
            }
            else if (value is long)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((long)value));
            }
            else if (value is int)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((int)value));
            }
            else if (value is char)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.CharacterLiteralExpression,
                    SyntaxFactory.Literal((char)value));
            }
            else if (value is string)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal((string)value));
            }
            else if (value is uint)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((uint)value));
            }
            else if (value is decimal)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((decimal)value));
            }
            else
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal((string)value));
            }
        }
    }
}
