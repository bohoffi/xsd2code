using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System.Globalization;

namespace xsd2code.v1.Utils
{
    public class Util
    {
        public static AdhocWorkspace CreateWorkspace()
        {
            return new AdhocWorkspace();
        }

        public static SyntaxGenerator CreateSyntaxGenerator()
        {
            return SyntaxGenerator.GetGenerator(CreateWorkspace(), LanguageNames.CSharp);
        }

        public static string ToTitleCase(string input)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);
        }
    }
}
