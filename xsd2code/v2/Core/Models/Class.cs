using System.Collections.Generic;
using System.Linq;
using xsd2code.v2.Core.Enums;

namespace xsd2code.v2.Core.Models
{
    public class Class
    {
        public string Name { get; }
        public string Code { get; private set; }
        public List<XmlAttribute> XmlAttributes { get; private set; }
        public List<Property> Properties { get; private set; }
        public Modifier Modifier { get; private set; }

        private Class(string name)
        {
            Name = name;
        }

        public class Builder
        {
            private readonly string _name;
            private readonly List<XmlAttribute> _xmlAttributes;
            private readonly List<Property> _properties;
            private Modifier _modifier;

            public Builder(string name)
            {
                _name = name;

                _xmlAttributes = new List<XmlAttribute>();
                _properties = new List<Property>();
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

            public Builder WithProperty(Property property)
            {
                if (_properties.Any(p => p.Name == property.Name)) return this;
                _properties.Add(property);
                return this;
            }

            public Builder WithProperties(IEnumerable<Property> properties)
            {
                _properties.AddRange(properties.Except(_properties));
                return this;
            }

            public Builder WIthModifier(Modifier modifier)
            {
                _modifier = modifier;
                return this;
            }

            public Class Build()
            {
                return new Class(_name)
                {
                    XmlAttributes = _xmlAttributes,
                    Properties = _properties
                };
            }
        }
    }
}