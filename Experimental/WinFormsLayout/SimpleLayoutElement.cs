using System;
using System.Collections.Generic;

namespace App
{
    public class SimpleLayoutElement : ILayoutElement
    {
        public SimpleLayoutElement(string name)
        {
            this.name = name;
        }

        public SimpleLayoutElement(string name, int width, int height)
            : this(name)
        {
            this.width = width;
            this.height = height;
        }

        public string Name
        {
            get { return name; }
        }

        public int X
        {
            get { return x; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                x = value;
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                y = value;
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                width = value;
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                height = value;
            }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public IList<IRelationship> Relationships
        {
            get { return relationships; }
        }

        public void AddRelationship (IRelationship relationship)
        {
            relationships.Add(relationship);
        }

        public SimpleLayoutElement MaximumWidth(int maximumWidth)
        {
            relationships.Add(new MaximumWidthConstraint(maximumWidth));
            return this;
        }

        public SimpleLayoutElement MinimumWidth(int minimumWidth)
        {
            relationships.Add(new MinimumWidthConstraint(minimumWidth));
            return this;
        }

        public SimpleLayoutElement MaximumHeight(int maximumHeight)
        {
            relationships.Add(new MaximumHeightConstraint(maximumHeight));
            return this;
        }

        public SimpleLayoutElement MinimumHeight(int minimumHeight)
        {
            relationships.Add(new MinimumHeightConstraint(minimumHeight));
            return this;
        }

        public SimpleLayoutElement LeftNoFlyZone(params ILayoutElement[] elements)
        {
            relationships.Add(new LeftNoFlyZoneConstraint(elements));
            return this;
        }

        public SimpleLayoutElement RightNoFlyZone(params ILayoutElement[] elements)
        {
            relationships.Add(new RightNoFlyZoneConstraint(elements));
            return this;
        }

        public SimpleLayoutElement RightAnchor(params ILayoutElement[] elements)
        {
            relationships.Add(new RightAnchorConstraint(elements));
            return this;
        }

        public SimpleLayoutElement RightDock(params ILayoutElement[] elements)
        {
            relationships.Add(new RightDockConstraint(elements));
            return this;
        }

        public SimpleLayoutElement TopAnchor(params ILayoutElement[] elements)
        {
            relationships.Add(new TopAnchorConstraint(elements));
            return this;
        }

        public SimpleLayoutElement TopNoFlyZone(params ILayoutElement[] elements)
        {
            relationships.Add(new TopNoFlyZoneConstraint(elements));
            return this;
        }

        private readonly string name;
        private int x;
        private int y;
        private int width;
        private int height;
        private List<IRelationship> relationships = new List<IRelationship>();
        private bool visible = true;
    }
}