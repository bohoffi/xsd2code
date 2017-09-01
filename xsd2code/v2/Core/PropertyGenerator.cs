using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace xsd2code.v2.Core
{
    public class PropertyGenerator
    {
        public PropertyDeclarationSyntax GenerateProperty(string propertyName, PrimitiveType type)
        {
            return SF.PropertyDeclaration(SF.PredefinedType(SF.Token(FromPrimitive(type))), SF.Identifier(propertyName));
        }

        public PropertyDeclarationSyntax GenerateProperty(string propertyName, string typeName)
        {
            return SF.PropertyDeclaration(SF.IdentifierName(typeName), SF.Identifier(propertyName));
        }

        public PropertyDeclarationSyntax GenerateArrayProperty(string propertyName, PrimitiveType type)
        {
            return SF.PropertyDeclaration(SF.ArrayType(SF.PredefinedType(SF.Token(FromPrimitive(type)))), SF.Identifier(propertyName));
        }

        public PropertyDeclarationSyntax GenerateArrayProperty(string propertyName, string typeName)
        {
            return SF.PropertyDeclaration(SF.ArrayType(SF.IdentifierName(typeName)), SF.Identifier(propertyName));
        }

        public PropertyDeclarationSyntax GenerateListProperty(string propertyName, PrimitiveType type)
        {
            return SF.PropertyDeclaration(
                SF.GenericName(SF.Identifier("List"))
                .WithTypeArgumentList(
                    SF.TypeArgumentList(
                        SF.SingletonSeparatedList<TypeSyntax>(
                            SF.PredefinedType(SF.Token(FromPrimitive(type)))))), 
                SF.Identifier(propertyName));
        }

        public PropertyDeclarationSyntax GenerateListProperty(string propertyName, string typeName)
        {
            return SF.PropertyDeclaration(
                SF.GenericName(SF.Identifier("List"))
                .WithTypeArgumentList(
                    SF.TypeArgumentList(
                        SF.SingletonSeparatedList<TypeSyntax>(
                            SF.IdentifierName(typeName)))),
                SF.Identifier(propertyName));
        }

        private SyntaxKind FromPrimitive(PrimitiveType type)
        {
            var ushortValue = (ushort)type;
            try
            {
                return (SyntaxKind)ushortValue;
            }
            catch (Exception exception)
            {
                // TODO log
                Debug.Fail(exception.Message);
                return SyntaxKind.StringKeyword;
            }
        }
    }
}
