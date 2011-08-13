using System;
using System.Globalization;
using Wintellect.PowerCollections;

namespace App
{
    public class ExperimentalLayoutMechanism : ILayoutMechanism
    {
        public void PerformLayout(IVisualSystem visualSystem)
        {
            this.visualSystem = visualSystem;
            visitedElements.Clear();
            discoveredElements.Clear();

            foreach (ILayoutElement element in visualSystem.Elements)
                ProcessElement(element);
        }

        private void ProcessElement(ILayoutElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (false == visualSystem.Elements.Contains(element))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Element {0} is not part of the system",
                    element.Name);
                throw new InvalidOperationException(message);
            }

            if (visitedElements.Contains(element)
                || discoveredElements.Contains(element))
                return;

            discoveredElements.Add(element);

            foreach (IRelationship relationship in element.Relationships)
                ProcessRelationship(element, relationship);

            foreach (IRelationship relationship in element.Relationships)
                relationship.Check();

            discoveredElements.Remove(element);
            visitedElements.Remove(element);
        }

        private void ProcessRelationship(ILayoutElement element, IRelationship relationship)
        {
            foreach (ILayoutElement constraintElement in relationship.Targets)
            {
                if (false == visitedElements.Contains(constraintElement))
                {
                    if (discoveredElements.Contains(constraintElement))
                        continue;
                        //throw new InvalidOperationException();

                    ProcessElement(constraintElement);
                }                
            }

            relationship.Apply();
        }

        private Set<ILayoutElement> visitedElements = new Set<ILayoutElement>();
        private Set<ILayoutElement> discoveredElements = new Set<ILayoutElement>();
        private IVisualSystem visualSystem;
    }
}