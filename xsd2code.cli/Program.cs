using System;
using xsd2code.cli.Core;

namespace xsd2code.cli
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessArguments(args);
            Console.ReadKey();
        }

        private static void ProcessArguments(string[] args)
        {
            // args should be a file path or a directory path
            if (args != null && args.Length == 1)
            {
                new Processor(args[0], ApiVersion.v2).Process();
            }
        }
    }
}
