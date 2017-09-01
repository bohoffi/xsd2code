using System.Collections.Generic;
using System.Xml.Linq;

namespace xsd2code.v1.Xsd
{
    public class ComplexType : SimpleType
    {
        public ComplexType(string targetNamespace, XElement element) : base(targetNamespace, element)
        {
            SequenceElements = new List<string>();

            var upperSequence = _element.Element(XsdFile.DEFAULT_NAMESPACE + "sequence");
            if (upperSequence != null)
            {
                var seqElements = upperSequence.Elements(XsdFile.DEFAULT_NAMESPACE + "element");

                foreach (var seqElem in seqElements)
                {
                    SequenceElements.Add(seqElem.Attribute("name").Value);
                }
            }
        }

        public List<string> SequenceElements { get; set; }
    }
}
