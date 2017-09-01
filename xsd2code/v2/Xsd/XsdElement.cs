using System.Collections.Generic;
using System.Xml.Linq;

namespace xsd2code.v2.Xsd
{
    public class XsdElement
    {
        private XElement _xElement;

        public XsdElement(XElement element)
        {
            _xElement = element;

            Name = _xElement.Attribute("name").Value;

            var typeAttribute = _xElement.Attribute("type");
            if (typeAttribute == null)
            {
                IsComplex = true;
            }
            else
            {
                Type = typeAttribute.Value;
                System.Console.WriteLine($"{Name}.{Type}");
            }

            var minOccursAttribute = _xElement.Attribute("minOccurs");
            if (minOccursAttribute != null)
            {
                MinOccurs = int.Parse(minOccursAttribute.Value);
            }

            var maxOccursAttribute = _xElement.Attribute("maxOccurs");
            if (maxOccursAttribute != null && !maxOccursAttribute.Value.Equals("unbounded"))
            {
                MaxOccours = int.Parse(maxOccursAttribute.Value);
            }

            var defaultAttribute = _xElement.Attribute("default");
            if (defaultAttribute != null)
            {
                Default = defaultAttribute.Value;
            }

            var fixedAttribute = _xElement.Attribute("fixed");
            if (fixedAttribute != null)
            {
                Fixed = fixedAttribute.Value;
            }
        }

        public string Name { get; private set; }
        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;
        public string Type { get; private set; }
        public string CleanType => !string.IsNullOrWhiteSpace(Type) ? Type.Split(':').Length == 2 ? Type.Split(':')[1] : Type : string.Empty;
        public int? MinOccurs { get; private set; }
        public int? MaxOccours { get; private set; }
        public string Default { get; private set; }
        public string Fixed { get; private set; }
        public bool IsComplex { get; private set; }
        public List<XsdElement> Elements { get; private set; } = new List<XsdElement>();
        public List<XsdAttribute> Attributes { get; private set; } = new List<XsdAttribute>();
    }
}
