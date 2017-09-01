using System;
using System.Xml.Linq;

namespace xsd2code.v2.Xsd
{
    // <summary>
    /// @see https://www.w3schools.com/xml/schema_simple_attributes.asp
    /// </summary>
    public class XsdAttribute
    {
        private XElement _xElement;

        public XsdAttribute(XElement element)
        {
            _xElement = element;

            Name = _xElement.Attribute("name").Value;
            var typeAttribute = _xElement.Attribute("type");
            if (typeAttribute != null)
            {
                Type = typeAttribute.Value;
            }
        }

        public string Name { get; private set; }
        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;
        public string Type { get; private set; }
        public string CleanType => !string.IsNullOrWhiteSpace(Type) ? Type.Split(':').Length == 2 ? Type.Split(':')[1] : Type : string.Empty;
        public string Default { get; private set; }
        public string Fixed { get; private set; }
        public XsdAttributeUse Use { get; private set; } = XsdAttributeUse.optional;
    }

    public enum XsdAttributeUse
    {
        [EnumStringValue("optional")]
        optional,
        [EnumStringValue("required")]
        required
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumStringValueAttribute : Attribute
    {
        public EnumStringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
