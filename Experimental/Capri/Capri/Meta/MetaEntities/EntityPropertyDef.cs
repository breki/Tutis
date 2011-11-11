namespace Capri.Meta.MetaEntities
{
    public class EntityPropertyDef : MetaDef
    {
        public EntityPropertyDef(
            EntityDef ownerEntity, 
            PropertyType propertyType,
            string id, 
            string name) : base(id, name)
        {
            this.ownerEntity = ownerEntity;
            this.propertyType = propertyType;
        }

        private readonly EntityDef ownerEntity;
        private readonly PropertyType propertyType;
    }
}