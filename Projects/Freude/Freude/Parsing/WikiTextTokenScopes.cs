using System;

namespace Freude.Parsing
{
    [Flags]
    public enum WikiTextTokenScopes
    {
        None = 0,
        LineStart = 1 << 0,
        InnerText = 1 << 1,
        HeadingText = 1 << 2,
        HeadingSuffix = 1 << 3,
        InternalLinkInternals = 1 << 4,
        ExternalLinkUrl = 1 << 5,
        ExternalLinkText = 1 << 6,
    }
}