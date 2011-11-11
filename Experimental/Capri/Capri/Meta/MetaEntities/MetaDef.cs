namespace Capri.Meta.MetaEntities
{
    public abstract class MetaDef
    {
        protected MetaDef(string id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        private string id;
        private string name;
    }
}