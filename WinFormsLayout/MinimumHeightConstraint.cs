namespace App
{
    public class MinimumHeightConstraint : IConstraint
    {
        public MinimumHeightConstraint(ILayoutElement element, int minimumHeight)
        {
            this.element = element;
            this.minimumHeight = minimumHeight;
        }

        public void Apply()
        {
            if (element.Height < minimumHeight)
                element.Height = minimumHeight;
        }

        public void Check()
        {
            if (element.Height < minimumHeight)
                throw new ConstraintViolationException(
                    "Element '{0}' has a height of {1} but it should be at least {2}",
                    element.Name,
                    element.Height,
                    minimumHeight);
        }

        private readonly ILayoutElement element;
        private readonly int minimumHeight;
    }
}