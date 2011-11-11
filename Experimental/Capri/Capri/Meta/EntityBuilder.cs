using System;
using Capri.Meta.MetaEntities;

namespace Capri.Meta
{
    public class EntityBuilder
    {
        public EntityBuilder Entity(string entityName)
        {
            currentEntity = new EntityDef(entityName, entityName);
            metaModel.AddEntity(currentEntity);
            return this;
        }

        public EntityBuilder BelongsTo(string entityId)
        {
            EntityDef parentEntity = metaModel.GetEntity(entityId);

            EntityRelationshipDef relationship = new EntityRelationshipDef(
                currentEntity.Id, parentEntity, currentEntity, EntityRelationshipType.Composition);
            parentEntity.AddRelationship(relationship);
            currentEntity.AddRelationship(relationship);

            return this;
        }

        public EntityBuilder Date(string propertyName)
        {
            EntityPropertyDef propertyDef = new EntityPropertyDef(currentEntity, PropertyType.Date, propertyName, propertyName);
            currentEntity.AddProperty(propertyDef);
            return this;
        }

        public EntityBuilder Has(string entityId)
        {
            EntityDef parentEntity = metaModel.GetEntity(entityId);

            EntityRelationshipDef relationship = new EntityRelationshipDef(
                parentEntity.Id, parentEntity, currentEntity, EntityRelationshipType.Association);
            parentEntity.AddRelationship(relationship);
            currentEntity.AddRelationship(relationship);

            return this;
        }

        public EntityBuilder Has(string entityId, string relationshipName)
        {
            EntityDef parentEntity = metaModel.GetEntity(entityId);

            EntityRelationshipDef relationship = new EntityRelationshipDef(
                relationshipName, parentEntity, currentEntity, EntityRelationshipType.Association);
            parentEntity.AddRelationship(relationship);
            currentEntity.AddRelationship(relationship);

            return this;
        }

        public EntityBuilder Integer(string propertyName)
        {
            EntityPropertyDef propertyDef = new EntityPropertyDef(currentEntity, PropertyType.Integer, propertyName, propertyName);
            currentEntity.AddProperty(propertyDef);
            return this;
        }

        public EntityBuilder IsPartOf(string entityId)
        {
            EntityDef parentEntity = metaModel.GetEntity(entityId);

            EntityRelationshipDef relationship = new EntityRelationshipDef(
                currentEntity.Id, parentEntity, currentEntity, EntityRelationshipType.Aggregation);
            parentEntity.AddRelationship(relationship);
            currentEntity.AddRelationship(relationship);

            return this;
        }

        public EntityBuilder IsPartOf(string entityId, string relationshipName)
        {
            EntityDef parentEntity = metaModel.GetEntity(entityId);

            EntityRelationshipDef relationship = new EntityRelationshipDef(
                relationshipName, parentEntity, currentEntity, EntityRelationshipType.Aggregation);
            parentEntity.AddRelationship(relationship);
            currentEntity.AddRelationship(relationship);

            return this;
        }

        public EntityBuilder Text(string propertyName)
        {
            EntityPropertyDef propertyDef = new EntityPropertyDef(currentEntity, PropertyType.Text, propertyName, propertyName);
            currentEntity.AddProperty(propertyDef);
            return this;
        }

        private EntityDef currentEntity;
        private MetaModel metaModel = new MetaModel();
    }
}