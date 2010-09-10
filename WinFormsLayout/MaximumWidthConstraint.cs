using System.Collections.Generic;

namespace App
{
    public class MaximumWidthConstraint : IConstraint
    {
        public MaximumWidthConstraint(ILayoutElement element, int maximumWidth)
        {
            this.element = element;
            this.maximumWidth = maximumWidth;
        }

        public void Apply()
        {
            //if (onElement.Width > maximumWidth)
                element.Width = maximumWidth;
        }

        public void Check()
        {
            if (element.Width > maximumWidth)
                throw new ConstraintViolationException(
                    "Element '{0}' has a width of {1} but it should be no more than {2}",
                    element.Name,
                    element.Width,
                    maximumWidth);
        }

        private readonly ILayoutElement element;
        private readonly int maximumWidth;
    }
}