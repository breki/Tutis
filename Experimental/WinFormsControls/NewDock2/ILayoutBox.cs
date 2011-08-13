namespace NewDock2
{
    public interface ILayoutBox
    {
        LayoutPosition Position { get; }
        LayoutSize Size { get; }

        LayoutSize GetPreferredSize(ILayoutContext context);
        bool IsInside(LayoutPosition point);
        void Move(LayoutPosition newPosition);
        void Resize(LayoutSize newSize);
    }
}