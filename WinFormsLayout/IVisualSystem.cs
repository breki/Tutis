using System.Collections.Generic;

namespace App
{
    public interface IVisualSystem
    {
        IList<ILayoutElement> Elements { get; }

        void AddElement(ILayoutElement element);
    }

    public class VisualSystem : IVisualSystem
    {
        public IList<ILayoutElement> Elements
        {
            get { return elements; }
        }

        public void AddElement(ILayoutElement element)
        {
            elements.Add(element);
        }

        private List<ILayoutElement> elements = new List<ILayoutElement>();
    }
}