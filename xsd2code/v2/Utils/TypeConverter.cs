using System.Collections.Generic;
using xsd2code.v2.Core;

namespace xsd2code.v2.Utils
{
    public class TypeConverter
    {
        private static Dictionary<string, PrimitiveType> _typings = new Dictionary<string, PrimitiveType>
        {
            { "anyUri", PrimitiveType.String_Type },
            // need to be byte[]
            //{"base64Binary", new KeyValuePair<string, PrimitiveType>("array", PrimitiveType.Byte_Type) },
            {"boolean", PrimitiveType.Bool_Type },
            {"byte", PrimitiveType.SByte_Type },
            //{"Date", "DateTime" },
            //{"dateTime", "DateTime" },
            {"decial", PrimitiveType.Decimal_Type },
            {"double", PrimitiveType.Double_Type },
            {"ENTITY", PrimitiveType.String_Type },
            {"ENTITIES", PrimitiveType.String_Type },
            {"float", PrimitiveType.Float_Type },
            {"gDay", PrimitiveType.String_Type },
            {"gMonth", PrimitiveType.String_Type },
            {"gMonthDay", PrimitiveType.String_Type },
            {"gYear", PrimitiveType.String_Type },
            {"gYearMonth", PrimitiveType.String_Type },
            // need to be byte[]
            //{"hexBinary", new KeyValuePair<string, PrimitiveType>("array", PrimitiveType.Byte_Type) },
            {"ID", PrimitiveType.String_Type },
            {"IDREF", PrimitiveType.String_Type },
            {"IDREFS", PrimitiveType.String_Type },
            {"int", PrimitiveType.Int_Type },
            {"integer", PrimitiveType.String_Type },
            {"language", PrimitiveType.String_Type },
            {"long", PrimitiveType.Int_Type },
            {"Name", PrimitiveType.String_Type },
            {"NCName", PrimitiveType.String_Type },
            {"negativeInteger", PrimitiveType.String_Type },
            {"NMTOKEN", PrimitiveType.String_Type },
            {"NMTOKENS", PrimitiveType.String_Type },
            {"normalizedString", PrimitiveType.String_Type },
            {"nonNegativeInteger", PrimitiveType.String_Type },
            {"nonPositiveInteger", PrimitiveType.String_Type },
            {"NOTATION", PrimitiveType.String_Type },
            {"positiveInteger", PrimitiveType.String_Type },
            // need to be XmlQualifiedName
            //{"QName", "XmlQualifiedName" },
            {"duration", PrimitiveType.String_Type },
            {"string", PrimitiveType.String_Type },
            {"short", PrimitiveType.Short_Type },
            //{"time", "DateTime" },
            {"token", PrimitiveType.String_Type },
            {"unsignedByte", PrimitiveType.Byte_Type },
            {"unsignedInt", PrimitiveType.UInt_Type },
            {"unsignedLong", PrimitiveType.ULong_Type },
            {"unsignedShort", PrimitiveType.UShort_Type }
        };

        public static PrimitiveType? Type(string typeString)
        {
            if (_typings.ContainsKey(typeString))
            {
                return _typings[typeString];
            }
            return null;
        }
    }
}
