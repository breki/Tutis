using System.Collections.Generic;

namespace App
{
    public interface IRelationship
    {
        IList<ILayoutElement> Sources { get; }
        IList<ILayoutElement> Targets { get; }

        void Apply();
        void Check();
    }
}