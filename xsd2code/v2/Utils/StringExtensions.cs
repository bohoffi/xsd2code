using System.Globalization;

namespace xsd2code.v2.Utils
{
    public static class StringExtensions
    {
        public static string Clean(this string source)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(source.Replace(".", string.Empty));
        }
    }
}
