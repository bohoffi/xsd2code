using System.Xml.Linq;
using xsd2code.v1.Utils;

namespace xsd2code.v1.Xsd
{
    public class SimpleType
    {
        protected XElement _element;

        public SimpleType(string targetNamespace, XElement element)
        {
            TargetNamespace = targetNamespace;
            _element = element;

            Name = _element.Attribute("name").Value;
        }

        public string TargetNamespace { get; set; }
        public string Name { get; set; }

        public string CleanName => !string.IsNullOrWhiteSpace(Name) ? Util.ToTitleCase(Name.Replace(".", string.Empty)) : string.Empty;
    }
}
