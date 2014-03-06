namespace RankWatch.Razor
{
    public interface IReportRenderingEngine
    {
        string RenderView<TModel> (TModel viewModel);
    }
}