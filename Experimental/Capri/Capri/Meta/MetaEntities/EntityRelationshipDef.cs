namespace Capri.Meta.MetaEntities
{
    public class EntityRelationshipDef : MetaDef
    {
        public EntityRelationshipDef(string entityName, EntityDef sourceEntity, EntityDef destinationEntity, EntityRelationshipType relationshipType)
            : base(entityName, entityName)
        {
            this.sourceEntity = sourceEntity;
            this.destinationEntity = destinationEntity;
            this.relationshipType = relationshipType;
        }

        public EntityDef SourceEntity
        {
            get { return sourceEntity; }
        }

        public EntityDef DestinationEntity
        {
            get { return destinationEntity; }
        }

        public EntityRelationshipType RelationshipType
        {
            get { return relationshipType; }
        }

        private EntityDef sourceEntity;
        private EntityDef destinationEntity;
        private EntityRelationshipType relationshipType;
    }
}