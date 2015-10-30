using NUnit.Framework;
using Syborg.Razor;

namespace Freude.Tests.TemplatingTests
{
    public class GenerateFromTemplateTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), Test]
        public void Test()
        {   
        }

        [SetUp]
        public void Setup()
        {
            razorCompiler = new InMemoryRazorCompiler();
        }

        private IRazorCompiler razorCompiler;
    }
}