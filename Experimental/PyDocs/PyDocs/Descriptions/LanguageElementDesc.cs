namespace PyDocs.Descriptions
{
    public abstract class LanguageElementDesc
    {
        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        protected LanguageElementDesc(string name)
        {
            this.name = name;
        }

        private string name;
        private string description;
    }
}