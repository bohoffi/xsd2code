using System.Xml.Linq;

namespace xsd2code.v2.Xsd
{
    public class XsdSimpleType
    {
        protected XElement _xElement;

        public XsdSimpleType(string targetNamespace, XElement element)
        {
            TargetNamespace = targetNamespace;
            _xElement = element;

            Name = _xElement.Attribute("name").Value;
        }

        public string TargetNamespace { get; private set; }
        public string Name { get; private set; }

        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Name.Replace(".", string.Empty) : string.Empty;
    }
}
