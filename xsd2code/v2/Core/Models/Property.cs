using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using xsd2code.v2.Utils;

namespace xsd2code.v2.Core.Models
{
    public class Property
    {
//        public Property(string typeString)
//        {
//            XmlAttributes = new List<XmlAttribute>();
//
//            TypeString = typeString;
//        }

        private Property(string name, string typeString)
        {
            Name = name;
            TypeString = typeString;
            XmlAttributes = new List<XmlAttribute>();
        }

        public string Name { get; set; }

        //public Accessibility Accessibility { get; set; }
        public string TypeString { get; set; }

//        public bool IsAutoProperty { get; set; }
        public List<XmlAttribute> XmlAttributes { get; set; }

        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;

        public PropertyDeclarationSyntax ToDeclarationSyntax()
        {
            var propGen = new PropertyGenerator();
            var convertedType = TypeConverter.Type(TypeString);
            return convertedType.HasValue
                ? propGen.GenerateProperty(CleanName, convertedType.Value)
                : propGen.GenerateProperty(CleanName, TypeString);
        }

        public class Builder
        {
            private readonly string _name;
            private readonly string _typeString;
            private readonly List<XmlAttribute> _xmlAttributes;

            public Builder(string name, string typeString)
            {
                _name = name;
                _typeString = typeString;
                _xmlAttributes = new List<XmlAttribute>();
            }

            public Builder WithXmlAttribute(XmlAttribute attribute)
            {
                if (_xmlAttributes.Any(x => x.Name == attribute.Name)) return this;
                _xmlAttributes.Add(attribute);
                return this;
            }

            public Builder WithXmlAttributes(IEnumerable<XmlAttribute> attributes)
            {
                _xmlAttributes.AddRange(attributes.Except(_xmlAttributes));
                return this;
            }

            public Property Build()
            {
                return new Property(_name, _typeString)
                {
                    XmlAttributes = _xmlAttributes
                };
            }
        }
    }
}