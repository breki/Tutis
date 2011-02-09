using System.Collections.Generic;

namespace NewDock2
{
    public interface ILayoutContext
    {
        T Get<T>(string name);
        void Set(string name, object value);
    }

    public class LayoutContext : ILayoutContext
    {
        public T Get<T>(string name)
        {
            return (T) contextValues[name];
        }

        public void Set(string name, object value)
        {
            contextValues[name] = value;
        }

        private Dictionary<string, object> contextValues = new Dictionary<string, object>();
    }
}