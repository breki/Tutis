using System;
using System.Collections.Generic;
using log4net;

namespace App
{
    public class RightDockConstraint : IRelationship
    {
        public RightDockConstraint(ILayoutElement source, ILayoutElement target)
        {
            sources.Add(source);
            targets.Add(target);
        }

        public RightDockConstraint(IEnumerable<ILayoutElement> sources, IEnumerable<ILayoutElement> targets)
        {
            this.sources.AddRange(sources);
            this.targets.AddRange(targets);
        }

        public IList<ILayoutElement> Sources
        {
            get { return sources; }
        }

        public IList<ILayoutElement> Targets
        {
            get { return targets; }
        }

        public void AddSourceElement(ILayoutElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            sources.Add(element);
        }

        public void Apply()
        {
            int anchorPosition = GetRightAnchorPosition();
            foreach (ILayoutElement target in Targets)
            {
                try
                {
                    target.Width = anchorPosition - target.X;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new ConstraintViolationException();
                }
            }
        }

        public void Check()
        {
            int anchorPosition = GetRightAnchorPosition();

            foreach (ILayoutElement target in Targets)
            {
                int elementRight = target.X + target.Width;

                if (elementRight != anchorPosition)
                    throw new ConstraintViolationException(
                        "Element {0} should be right-docked to {1} but it is right position is at {2} instead",
                        target.Name,
                        anchorPosition,
                        elementRight);
            }
        }

        private int GetRightAnchorPosition()
        {
            int min = int.MaxValue;
            foreach (ILayoutElement element in Sources)
            {
                if (element.X < min)
                    min = element.X;
            }

            return min;
        }

        private List<ILayoutElement> sources = new List<ILayoutElement>();
        private List<ILayoutElement> targets = new List<ILayoutElement>();
        private static readonly ILog log = LogManager.GetLogger(typeof(RightDockConstraint));
    }
}