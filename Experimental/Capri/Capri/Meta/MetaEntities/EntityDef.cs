using System.Collections.Generic;

namespace Capri.Meta.MetaEntities
{
    public class EntityDef : MetaDef
    {
        public EntityDef(string id, string name) : base(id, name)
        {
        }

        public void AddProperty(EntityPropertyDef property)
        {
            properties.Add(property);
        }

        public void AddRelationship(EntityRelationshipDef relationship)
        {
            relationships.Add(relationship);
        }

        private List<EntityPropertyDef> properties = new List<EntityPropertyDef>();
        private List<EntityRelationshipDef> relationships = new List<EntityRelationshipDef>();
    }
}