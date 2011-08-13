using System;

namespace App
{
    public class HorizontalDimensionConstraint : IDimensionConstraint
    {
        public HorizontalDimensionConstraint(ILayoutElement element, int? minSize, int? maxSize, int? optimumSize)
        {
            this.element = element;
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.optimumSize = optimumSize;
        }

        public Dimension Dimension
        {
            get { return Dimension.Horizontal; }
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        private readonly ILayoutElement element;
        private readonly int? minSize;
        private readonly int? maxSize;
        private readonly int? optimumSize;
    }
}