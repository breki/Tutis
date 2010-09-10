using System.Collections.Generic;

namespace App
{
    public class MaximumHeightConstraint : IConstraint
    {
        public MaximumHeightConstraint(ILayoutElement element)
        {
            this.element = element;
        }

        public MaximumHeightConstraint(int maximumHeight)
        {
            this.maximumHeight = maximumHeight;
        }

        public void Apply()
        {
            element.Height = maximumHeight;
        }

        public void Check()
        {
            if (element.Height > maximumHeight)
                throw new ConstraintViolationException(
                    "Element '{0}' has a height of {1} but it should be no more than {2}",
                    element.Name,
                    element.Height,
                    maximumHeight);
        }

        private readonly int maximumHeight;
        private readonly ILayoutElement element;
    }
}