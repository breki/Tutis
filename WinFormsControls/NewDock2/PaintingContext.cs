using System.Collections.Generic;

namespace NewDock2
{
    public class PaintingContext : IPaintingContext
    {
        public T Get<T>(string name)
        {
            return (T)contextValues[name];
        }

        public void Set(string name, object value)
        {
            contextValues[name] = value;
        }

        private Dictionary<string, object> contextValues = new Dictionary<string, object>();
    }
}