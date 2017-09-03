using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using xsd2code.v2.Utils;

namespace xsd2code.v2.Core.Models
{
    public class Property
    {
        public Property(string typeString)
        {
            XmlAttributes = new List<XmlAttribute>();

            TypeString = typeString;
        }

        public string Name { get; set; }
        //public Accessibility Accessibility { get; set; }
        public string TypeString { get; set; }
        public bool IsAutoProperty { get; set; }
        public List<XmlAttribute> XmlAttributes { get; set; }

        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;

        public PropertyDeclarationSyntax ToDeclarationSyntax()
        {
            var propGen = new PropertyGenerator();
            var convertedType = TypeConverter.Type(TypeString);
            return convertedType.HasValue ? propGen.GenerateProperty(CleanName, convertedType.Value) : propGen.GenerateProperty(CleanName, TypeString);
        }
    }
}