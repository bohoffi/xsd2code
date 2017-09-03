using System.Collections.Generic;

namespace xsd2code.v2.Core.Models
{
    public class XmlAttribute
    {
        public XmlAttribute()
        {
            Values = new Dictionary<string, object>();
        }

        public string Name { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }
}