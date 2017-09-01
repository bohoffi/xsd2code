using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xsd2code.v2.Xsd
{
    public class XsdComplexType : XsdSimpleType
    {
        public XsdComplexType(string targetNamespace, XElement element) : base(targetNamespace, element)
        {
            var upperSequence = _xElement.Element(XsdFile.DEFAULT_NAMESPACE + "sequence");
            if (upperSequence != null)
            {
                var seqElements = upperSequence.Elements(XsdFile.DEFAULT_NAMESPACE + "element");
                Elements.AddRange(seqElements.Select(e => new XsdElement(e)).ToList());
            }

            var attributes = _xElement.Elements(XsdFile.DEFAULT_NAMESPACE + "attribute");
            Attributes.AddRange(attributes.Select(a => new XsdAttribute(a)).ToList());
        }

        public List<XsdElement> Elements { get; private set; } = new List<XsdElement>();
        public List<XsdAttribute> Attributes { get; private set; } = new List<XsdAttribute>();
    }
}
