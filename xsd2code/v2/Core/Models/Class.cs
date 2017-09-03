using System.Collections.Generic;
using System.Linq;

namespace xsd2code.v2.Core.Models
{
    public class Class
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<XmlAttribute> XmlAttributes { get; set; }
        public List<Property> Properties { get; set; }

        private Class(string name)
        {
            Name = name;
        }

        public class Builder
        {
            private readonly string _name;
            private readonly List<XmlAttribute> _xmlAttributes;
            private readonly List<Property> _properties;

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

            public Class Build()
            {
                return new Class(_name);
            }
        }
    }
}