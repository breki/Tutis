using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Brejc.Common.FileSystem;
using log4net;

namespace RankWatch.Razor
{
    public class RazorReportRenderingEngine : IReportRenderingEngine
    {
        public RazorReportRenderingEngine (IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string RenderView<TModel> (
            TModel viewModel)
        {
            string modelTypeName = typeof (TModel).Name;
            string viewName = modelTypeName.Substring (0, modelTypeName.IndexOf ("ReportModel", StringComparison.Ordinal));

            string fullViewFilePath = Path.Combine ("Reporting", Path.Combine ("Reports", viewName)) + ".cshtml";

            string viewContents = fileSystem.ReadFileAsString (fullViewFilePath);
            dynamic viewBag = null;
            InMemoryRazorEngine.ExecutionResult executionResult = InMemoryRazorEngine.Execute (
                viewName,
                viewContents,
                viewModel,
                viewBag,
                typeof (RazorReportRenderingEngine).Assembly,
                typeof (IFileSystem).Assembly);

            if (executionResult.CompilerResult.Errors.HasErrors)
            {
                StringBuilder b = new StringBuilder();
                foreach (CompilerError error in executionResult.CompilerResult.Errors)
                    b.AppendLine(error.ToString());

                throw new InvalidOperationException(b.ToString());
            }
            if (executionResult.CompilerResult.Errors.HasWarnings)
                throw new InvalidOperationException ("executionResult.CompilerResult.Errors.HasWarnings");
            if (!executionResult.GeneratorResult.Success)
                throw new InvalidOperationException ("executionResult.GeneratorResult.Success == false");
            
            return executionResult.RuntimeResult;
        }

        private readonly IFileSystem fileSystem;
        private static readonly ILog log = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);
    }
}