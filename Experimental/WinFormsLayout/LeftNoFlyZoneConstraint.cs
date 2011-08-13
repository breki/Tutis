using System;
using System.Collections.Generic;
using log4net;

namespace App
{
    public class LeftNoFlyZoneConstraint : IRelationship
    {
        public LeftNoFlyZoneConstraint(ILayoutElement source, ILayoutElement target)
        {
            sources.Add(source);
            targets.Add(target);
        }

        public LeftNoFlyZoneConstraint(IEnumerable<ILayoutElement> sources)
        {
            this.sources.AddRange(sources);
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
                if (noFlyZone < 0)
                    throw new ConstraintViolationException();

                if (target.X < noFlyZone)
                    target.X = noFlyZone;
            }
        }

        public void Check()
        {
            int noFlyZone = GetNoFlyZone();

            foreach (ILayoutElement target in Targets)
            {
                int currentX = target.X;

                if (currentX < noFlyZone)
                    throw new ConstraintViolationException(
                        "Element {0} should not cross the left margin of {1} but it is on {2}",
                        target.Name,
                        noFlyZone,
                        target.X);
            }
        }

        private int GetNoFlyZone()
        {
            int max = int.MinValue;
            foreach (ILayoutElement element in sources)
            {
                int pos = element.X + element.Width;
                if (pos > max)
                    max = pos;
            }

            return max;
        }

        private List<ILayoutElement> sources = new List<ILayoutElement>();
        private List<ILayoutElement> targets = new List<ILayoutElement>();
        private static readonly ILog log = LogManager.GetLogger(typeof(LeftNoFlyZoneConstraint));
    }
}