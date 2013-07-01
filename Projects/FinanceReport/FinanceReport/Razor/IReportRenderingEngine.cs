namespace FinanceReport.Razor
{
    public interface IReportRenderingEngine
    {
        string RenderView<TModel> (TModel viewModel);
    }
}