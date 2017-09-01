using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xsd2code.v2.Xsd
{
    /// <summary>
    /// XsdFile -> 0-n XsdSimpleType
    /// XsdFile -> 0-n XsdComplexType
    /// </summary>
    public class XsdFile
    {
        public static XNamespace DEFAULT_NAMESPACE = XNamespace.Get(@"http://www.w3.org/2001/XMLSchema");

        private XElement _rootElement;

        public XsdFile(string filePath)
        {
            _rootElement = XElement.Load(filePath);
            TargetNamespace = _rootElement.Attribute("targetNamespace") != null ? _rootElement.Attribute("targetNamespace").Value : string.Empty;

            SimpleTypes.AddRange(_rootElement.Elements(DEFAULT_NAMESPACE + "simpleType").ToList().Select(simple => new XsdSimpleType(TargetNamespace, simple)));
            ComplexTypes.AddRange(_rootElement.Elements(DEFAULT_NAMESPACE + "complexType").ToList().Select(complex => new XsdComplexType(TargetNamespace, complex)));
        }

        public string TargetNamespace { get; private set; }
        public List<XsdSimpleType> SimpleTypes { get; private set; } = new List<XsdSimpleType>();
        public List<XsdComplexType> ComplexTypes { get; private set; } = new List<XsdComplexType>();
    }
}
