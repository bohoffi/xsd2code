using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace xsd2code.v1.Utils
{
    public class TypeConverter
    {
        private Dictionary<string, SpecialType> _typings = new Dictionary<string, SpecialType>
        {
            { "anyUri", SpecialType.System_String },
            // need to be byte[]
            {"base64Binary", SpecialType.System_Array },
            {"boolean", SpecialType.System_Boolean },
            {"byte", SpecialType.System_SByte },
            {"Date", SpecialType.System_DateTime },
            {"dateTime", SpecialType.System_DateTime },
            {"decial", SpecialType.System_Decimal },
            {"double", SpecialType.System_Double },
            {"ENTITY", SpecialType.System_String },
            {"ENTITIES", SpecialType.System_String },
            {"float", SpecialType.System_Single },
            {"gDay", SpecialType.System_String },
            {"gMonth", SpecialType.System_String },
            {"gMonthDay", SpecialType.System_String },
            {"gYear", SpecialType.System_String },
            {"gYearMonth", SpecialType.System_String },
            // need to be byte[]
            {"hexBinary", SpecialType.System_Array },
            {"ID", SpecialType.System_String },
            {"IDREF", SpecialType.System_String },
            {"IDREFS", SpecialType.System_String },
            {"int", SpecialType.System_Int32 },
            {"integer", SpecialType.System_String },
            {"language", SpecialType.System_String },
            {"long", SpecialType.System_Int64 },
            {"Name", SpecialType.System_String },
            {"NCName", SpecialType.System_String },
            {"negativeInteger", SpecialType.System_String },
            {"NMTOKEN", SpecialType.System_String },
            {"NMTOKENS", SpecialType.System_String },
            {"normalizedString", SpecialType.System_String },
            {"nonNegativeInteger", SpecialType.System_String },
            {"nonPositiveInteger", SpecialType.System_String },
            {"NOTATION", SpecialType.System_String },
            {"positiveInteger", SpecialType.System_String },
            // need to be XmlQualifiedName
            {"QName", SpecialType.System_String },
            {"duration", SpecialType.System_String },
            {"string", SpecialType.System_String },
            {"short", SpecialType.System_Int16 },
            {"time", SpecialType.System_DateTime },
            {"token", SpecialType.System_String },
            {"unsignedByte", SpecialType.System_Byte },
            {"unsignedInt", SpecialType.System_UInt32 },
            {"unsignedLong", SpecialType.System_UInt64 },
            {"unsignedShort", SpecialType.System_UInt16 }
        };
    }
}
