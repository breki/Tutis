using System;

namespace Freude.Parsing
{
    [Flags]
    public enum WikiTextTokenScopes
    {
        None = 0,
        LineStart = 1 << 0,
        InnerText = 1 << 1,
        HeaderText = 1 << 2,
        HeaderSuffix = 1 << 3,
        LinkInternals = 1 << 4,
    }
}