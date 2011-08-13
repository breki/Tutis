namespace NewDock
{
    public struct DockPaneHitTestResult
    {
        public DockPaneHitTestArea HitArea;
        public int Index;

        public DockPaneHitTestResult(DockPaneHitTestArea hitTestArea, int index)
        {
            HitArea = hitTestArea;
            Index = index;
        }
    }
}