using System;
using System.Collections.Generic;
using log4net;

namespace App
{
    public class RightNoFlyZoneConstraint : IRelationship
    {
        public RightNoFlyZoneConstraint(ILayoutElement source, ILayoutElement target)
        {
            sources.Add(source);
            targets.Add(target);
        }

        public RightNoFlyZoneConstraint(IEnumerable<ILayoutElement> sources, IEnumerable<ILayoutElement> targets)
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
            int noFlyZone = GetNoFlyZone();

            foreach (ILayoutElement target in Targets)
            {
                try
                {
                    if (target.X + target.Width > noFlyZone)
                        target.Width = noFlyZone - target.X;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new ConstraintViolationException();
                }
            }
        }

        public void Check()
        {
            int noFlyZone = GetNoFlyZone();

            foreach (ILayoutElement target in Targets)
            {
                int elementRight = target.X + target.Width;

                if (elementRight > noFlyZone)
                    throw new ConstraintViolationException(
                        "Element {0} should not cross the right margin of {1} but it is on {2}",
                        target.Name,
                        noFlyZone,
                        elementRight);
            }
        }

        private int GetNoFlyZone()
        {
            int min = int.MaxValue;
            foreach (ILayoutElement element in sources)
            {
                if (element.X < min)
                    min = element.X;
            }

            return min;
        }

        private List<ILayoutElement> sources = new List<ILayoutElement>();
        private List<ILayoutElement> targets = new List<ILayoutElement>();
        private static readonly ILog log = LogManager.GetLogger(typeof(RightNoFlyZoneConstraint));
    }
}