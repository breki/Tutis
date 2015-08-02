using Freude.DocModel;

namespace Freude.HtmlGenerating
{
    public interface IHtmlGenerator
    {
        string GenerateHtml(DocumentDef doc);
    }
}