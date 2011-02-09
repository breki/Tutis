namespace NewDock
{
    public class VS2005DockPaneStripTab : DockPaneStripTabBase
    {
        public VS2005DockPaneStripTab(IDockContent content)
            : base(content)
        {
        }

        public bool Flag { get; set; }
        public int MaxWidth { get; set; }
        public int TabWidth { get; set; }
        public int TabX { get; set; }
    }
}