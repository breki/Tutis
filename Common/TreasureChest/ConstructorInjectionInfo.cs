using System.Collections.Generic;
using System.Reflection;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class ConstructorInjectionInfo
    {
        public ConstructorInjectionInfo(
            PolicyCollection chestPolicies, 
            ConstructorInfo constructor,
            IDictionary<string, object> args)
        {
            this.chestPolicies = chestPolicies;
            this.constructor = constructor;

            parametersByOrder = constructor.GetParameters();
            foreach (ParameterInfo info in parametersByOrder)
                parametersByNames.Add(info.Name, info);

            FindManuallyWiredParameters(args);
        }

        public ConstructorInfo Constructor
        {
            get { return constructor; }
        }

        public bool HasManualParameters
        {
            get { return manualParameters.Count > 0; }
        }

        public bool IsMatch
        {
            get { return isMatch; }
        }

        public IEnumerable<ParameterInfo> EnumerateParameters()
        {
            return parametersByOrder;
        }

        public object Invoke()
        {
            return constructor.Invoke(ListParameterValues());
        }

        public bool IsManualParameter(string parameterName)
        {
            return manualParameters.Contains(parameterName);
        }

        public object[] ListParameterValues()
        {
            object[] values = new object[parametersByOrder.Length];

            for (int i = 0; i < parametersByOrder.Length; i++)
            {
                ParameterInfo parameter = parametersByOrder[i];
                values[i] = parameterValues[parameter.Name];
            }

            return values;
        }

        public void SetParameterValue(string name, object value)
        {
            parameterValues[name] = value;
        }

        private void FindManuallyWiredParameters(IDictionary<string, object> args)
        {
            if (args.Count > parametersByOrder.Length)
                return;

            foreach (KeyValuePair<string, object> arg in args)
            {
                if (parametersByNames.ContainsKey(arg.Key)
                    && (arg.Value == null 
                    || parametersByNames[arg.Key].ParameterType.IsAssignableFrom(arg.Value.GetType())))
                {
                    manualParameters.Add(arg.Key);
                    SetParameterValue(arg.Key, arg.Value);
                    continue;
                }

                return;
            }

            IAutowiringConstructorArgumentTypesPolicy autowiringPolicy
                = chestPolicies.FindPolicyOf<IAutowiringConstructorArgumentTypesPolicy>();

            for (int i = 0; i < parametersByOrder.Length; i++)
            {
                ParameterInfo parameterInfo = parametersByOrder[i];
                if (!IsManualParameter(parameterInfo.Name))
                {
                    if (!autowiringPolicy.ShouldArgumentTypeBeAutowired(parameterInfo.ParameterType))
                        return;
                }
            }

            isMatch = true;
        }

        private readonly PolicyCollection chestPolicies;
        private readonly ConstructorInfo constructor;
        private bool isMatch;
        private HashSet<string> manualParameters = new HashSet<string>();
        private Dictionary<string, object> parameterValues = new Dictionary<string, object>();
        private Dictionary<string, ParameterInfo> parametersByNames = new Dictionary<string, ParameterInfo>();
        private ParameterInfo[] parametersByOrder;
    }
}