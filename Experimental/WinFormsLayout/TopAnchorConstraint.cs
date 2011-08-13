using System;
using System.Collections.Generic;
using log4net;

namespace App
{
    public class TopAnchorConstraint : IRelationship
    {
        public TopAnchorConstraint(ILayoutElement source, ILayoutElement target)
        {
            sources.Add(source);
            targets.Add(target);
        }

        public TopAnchorConstraint(IEnumerable<ILayoutElement> sources, IEnumerable<ILayoutElement> targets)
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
            int anchorPosition = GetAnchorPosition();
            foreach (ILayoutElement target in Targets)
            {
                try
                {
                    target.Y = anchorPosition;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new ConstraintViolationException();
                }
            }
        }

        public void Check()
        {
            int noFlyZone = GetAnchorPosition();
            foreach (ILayoutElement target in Targets)
            {
                int currentY = target.Y;

                if (currentY != noFlyZone)
                    throw new ConstraintViolationException(
                        "Element {0} should be anchored to the top margin of {1} but it is on {2}",
                        target.Name,
                        noFlyZone,
                        target.Y);
            }
        }

        private int GetAnchorPosition()
        {
            int max = int.MinValue;
            foreach (ILayoutElement element in sources)
            {
                int pos = element.Y + element.Height;
                if (pos > max)
                    max = pos;
            }

            return max;
        }

        private List<ILayoutElement> sources = new List<ILayoutElement>();
        private List<ILayoutElement> targets = new List<ILayoutElement>();
        private static readonly ILog log = LogManager.GetLogger(typeof(RightNoFlyZoneConstraint));
    }
}