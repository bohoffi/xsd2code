using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace xsd2code.v1.Xsd
{
    public class XsdFile
    {
        public static XNamespace DEFAULT_NAMESPACE = XNamespace.Get(@"http://www.w3.org/2001/XMLSchema");

        private XElement _root;

        public XsdFile(string filePath)
        {
            SimpleTypes = new List<SimpleType>();
            ComplexTypes = new List<ComplexType>();

            _root = XElement.Load(filePath);
            TargetNamespace = _root.Attribute("targetNamespace") != null ? _root.Attribute("targetNamespace").Value : string.Empty;

            SimpleTypes.AddRange(_root.Elements(DEFAULT_NAMESPACE + "simpleType").ToList().Select(simple => new SimpleType(TargetNamespace, simple)));
            ComplexTypes.AddRange(_root.Elements(DEFAULT_NAMESPACE + "complexType").ToList().Select(complex => new ComplexType(TargetNamespace, complex)));
        }

        public string TargetNamespace { get; set; }

        public List<SimpleType> SimpleTypes { get; set; }
        public List<ComplexType> ComplexTypes { get; set; }
    }
}
