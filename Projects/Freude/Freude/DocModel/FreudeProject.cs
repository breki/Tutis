using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Syborg.Razor;

namespace Freude.DocModel
{
    public class FreudeProject
    {
        public ICompiledRazorTemplate GetTemplate(string templateId)
        {
            Contract.Requires(templateId != null);
            Contract.Ensures(Contract.Result<ICompiledRazorTemplate>() != null);

            return templates[templateId];
        }

        public void RegisterTemplate(string templateId, ICompiledRazorTemplate template)
        {
            Contract.Requires(templateId != null);
            Contract.Requires(template != null);
            templates.Add(templateId, template);
        }

        private readonly Dictionary<string, ICompiledRazorTemplate> templates = new Dictionary<string, ICompiledRazorTemplate>();
    }
}