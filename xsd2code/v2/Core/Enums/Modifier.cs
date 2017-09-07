using System;

namespace xsd2code.v2.Core.Enums
{
    [Flags]
    public enum Modifier : ushort
    {
        Public = 8343,
        Private = 8344,
        Internal = 8345,
        Protected = 8346,
        Static = 8347
    }
}