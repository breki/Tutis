using Freude.DocModel;

namespace Freude.Parsing
{
    public interface IFreudeTextParser
    {
        DocumentDef ParseText(string text);
    }
}