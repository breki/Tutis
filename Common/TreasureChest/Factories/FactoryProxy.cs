using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace TreasureChest.Factories
{
    public class FactoryProxy : RealProxy
    {
        public FactoryProxy(Chest chest, Type typeToProxy)
            : base(typeToProxy)
        {
            this.chest = chest;
            this.typeToProxy = typeToProxy;
        }

        public override IMessage Invoke(IMessage msg)
        {
            ReturnMessage responseMessage;
            object response = null;
            Exception caughtException = null;

            try
            {
                string methodName = (string)msg.Properties["__MethodName"];
                object[] args = (object[])msg.Properties["__Args"];

                IMethodMessage methodMessage = msg as IMethodMessage;
                MethodInfo method = (MethodInfo)methodMessage.MethodBase;
                Type[] genericArgs = method.GetGenericArguments();
                
                response = ExecuteMethod(method, methodName, genericArgs, args);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            IMethodCallMessage message = msg as IMethodCallMessage;

            // Check if there is an exception
            if (caughtException == null)
            {
                // Return the response from the service
                responseMessage = new ReturnMessage(response, null, 0, null, message);
            }
            else
            {
                // Return the exception thrown by the service
                responseMessage = new ReturnMessage(caughtException, message);
            }

            // Return the response message
            return responseMessage;
        }

        protected object ExecuteMethod(
            MethodInfo method,
            string methodName,
            Type[] genericArgs,
            object[] args)
        {
            switch (methodName)
            {
                case "Equals":
                    return ReferenceEquals(GetTransparentProxy(), args[0]);
                case "GetHashCode":
                    return GetHashCode();
                case "GetType":
                    return GetType();
            }

            if (method.ReturnType == typeof(void))
                return HandleReleaseMethodCall(method, args);

            Type actualServiceType;
            bool excludeFirstParameter = false;

            if (method.IsGenericMethod)
                actualServiceType = HandleGenericMethod(method, genericArgs, ref excludeFirstParameter);
            else
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Type))
                {
                    actualServiceType = (Type)args[0];
                    if (!method.ReturnType.IsAssignableFrom(actualServiceType))
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            "The factory {0} method {1} provides type parameter {3} which is incompatible with the return type {2}.",
                            typeToProxy.FullName,
                            methodName,
                            method.ReturnType.FullName,
                            actualServiceType.FullName);
                        throw new ChestException(message);
                    }

                    excludeFirstParameter = true;
                }
                else
                    actualServiceType = method.ReturnType;
            }

            if (!chest.HasService(actualServiceType))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The factory {0} method {1} returns type {2} which is not registered in the chest.",
                    typeToProxy.FullName,
                    methodName,
                    actualServiceType.FullName);
                throw new ChestException(message);
            }

            IDictionary<string, object> argsWithValues = CreateArgsDictionary(
                method, 
                args,
                excludeFirstParameter);

            return chest.Fetch(actualServiceType, argsWithValues).Instance;
        }

        private object HandleReleaseMethodCall(MethodInfo method, object[] args)
        {
            if (args.Length != 1)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The factory {0} release method {1} must have a single parameter.",
                    typeToProxy.FullName,
                    method.Name);
                throw new ChestException(message);                
            }

            chest.Return(args[0]);
            return null;
        }

        private static IDictionary<string, object> CreateArgsDictionary(
            MethodInfo method,
            object[] args, 
            bool excludeFirstParameter)
        {
            Dictionary<string, object> argsWithValues = new Dictionary<string, object>();

            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (excludeFirstParameter && i == 0)
                    continue;

                argsWithValues.Add(parameters[i].Name, args[i]);
            }

            return argsWithValues;
        }

        private Type HandleGenericMethod(
            MethodInfo method,
            Type[] genericArgs,
            ref bool excludeFirstParameter)
        {
            if (genericArgs.Length > 1)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The factory {0} method {1} contains more than one generic parameter, which is not allowed.",
                    typeToProxy.FullName,
                    method.Name);
                throw new ChestException(message);
            }

            Type serviceType = genericArgs[0];

            if (!method.ReturnType.IsGenericParameter)
            {
                // check that the return type is compatible with the service type
                if (!method.ReturnType.IsAssignableFrom(serviceType))
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The factory {0} method {1} call uses a parameter type {2}, but the return type {3} is not compatible with it.",
                        typeToProxy.FullName,
                        method.Name,
                        serviceType.FullName,
                        method.ReturnType.FullName);
                    throw new ChestException(message);
                }
            }

            return serviceType;
        }

        private readonly Chest chest;
        private readonly Type typeToProxy;
    }
}