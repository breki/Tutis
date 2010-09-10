using System.Collections.Generic;

namespace App
{
    public class MinimumWidthConstraint : IRelationship
    {
        public MinimumWidthConstraint(ILayoutElement target, int minimumWidth)
        {
            targets.Add(target);
            this.minimumWidth = minimumWidth;
        }

        public IList<ILayoutElement> Sources
        {
            get { return new ILayoutElement[0]; }
        }

        public IList<ILayoutElement> Targets
        {
            get { return targets; }
        }

        public void Apply()
        {
            if (targets[0].Width < minimumWidth)
                targets[0].Width = minimumWidth;
        }

        public void Check()
        {
            if (targets[0].Width < minimumWidth)
                throw new ConstraintViolationException(
                    "Element '{0}' has a width of {1} but it should be at least {2}",
                    targets[0].Name,
                    targets[0].Width,
                    minimumWidth);
        }

        private readonly int minimumWidth;
        private List<ILayoutElement> targets = new List<ILayoutElement>();
    }
}