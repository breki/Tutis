using NUnit.Framework;
using Syborg.Razor;

namespace Freude.Tests.TemplatingTests
{
    public class GenerateFromTemplateTests
    {
        [Test]
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