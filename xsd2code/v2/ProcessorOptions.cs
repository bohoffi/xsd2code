using System.Collections.Generic;

namespace xsd2code.v2
{
    public class ProcessorOptions
    {
        public List<string> Files { get; set; }
        public string TargetPath { get; set; }
        public string Namespace { get; set; }
        public bool SpecifiedPattern { get; set; }
    }
}
