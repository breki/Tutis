using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TreasureChest
{
    public class ResolvingContext
    {
        public ResolvingContext(
            ObjectDependencyGraph dependencyGraph, 
            Type rootServiceType)
        {
            this.dependencyGraph = dependencyGraph;
            PushServiceType(rootServiceType);
        }

        public ResolvingContext(
            ObjectDependencyGraph dependencyGraph, 
            Type rootResolvingType,
            IDictionary<string, object> args)
            : this(dependencyGraph, rootResolvingType)
        {
            this.args = args;
        }

        public ObjectDependencyGraph DependencyGraph
        {
            get { return dependencyGraph; }
        }

        public IDictionary<string, object> Args
        {
            get { return args; }
        }

        public void MarkCallingArgsAsConsumed()
        {
            args.Clear();
        }

        public void PushServiceType(Type type)
        {
            resolvingStack.Push(type);
        }

        public void PushImplementationType(Type type)
        {
            if (resolvingStack.Contains(type) && resolvingStack.Peek() != type)
            {
                // push it to be displayed in the exception message
                resolvingStack.Push(type);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Looks like there is a cycle in the dependency tree. Service {0} has been visited for the second time: {1}.",
                    type.FullName,
                    ResolvingStackToString());
                throw new ChestException(message);
            }

            resolvingStack.Push(type);
        }

        public void PopType()
        {
            resolvingStack.Pop();
        }

        public string ResolvingStackToString()
        {
            StringBuilder msg = new StringBuilder();
            bool first = true;

            Type lastType = null;
            foreach (Type type in EnumerateResolvingStack())
            {
                if (type == lastType)
                    continue;

                if (!first)
                    msg.Append(" -> ");
                msg.Append(type.FullName);
                first = false;

                lastType = type;
            }

            return msg.ToString();
        }

        private IEnumerable<Type> EnumerateResolvingStack()
        {
            return resolvingStack.AsQueryable().Reverse();
        }

        private IDictionary<string, object> args = new Dictionary<string, object>();
        private Stack<Type> resolvingStack = new Stack<Type>();
        private ObjectDependencyGraph dependencyGraph;
    }
}