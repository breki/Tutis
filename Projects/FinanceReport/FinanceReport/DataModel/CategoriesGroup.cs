using System.Linq;

namespace FinanceReport.DataModel
{
    public class CategoriesGroup
    {
        public CategoriesGroup(string name, params int[] categories)
        {
            this.name = name;
            this.categories = categories;
        }

        public string Name
        {
            get { return name; }
        }

        public int[] Categories
        {
            get { return categories; }
        }

        public bool HasCategory(int category)
        {
            return categories.AsQueryable().Any(x => x == category);
        }

        private readonly string name;
        private readonly int[] categories;
    }
}