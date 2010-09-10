using System.Collections.Generic;

namespace App
{
    public interface ILayoutElement
    {
        string Name { get; }
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool Visible { get; }

        IList<IRelationship> Relationships { get; }

        void AddRelationship (IRelationship relationship);
    }
}