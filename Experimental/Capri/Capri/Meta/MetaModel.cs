using System.Collections.Generic;
using Capri.Meta.MetaEntities;

namespace Capri.Meta
{
    public class MetaModel
    {
        public void AddEntity(EntityDef entity)
        {
            entities.Add(entity.Id, entity);
        }

        public EntityDef GetEntity(string entityId)
        {
            return entities[entityId];
        }

        private Dictionary<string, EntityDef> entities = new Dictionary<string, EntityDef>();
    }
}