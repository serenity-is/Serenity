using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Serenity.Web
{
    public class DynamicScriptRegistration
    {
        public static void Initialize(IEnumerable<Assembly> assemblies)
        {
            Check.NotNull(assemblies, "assemblies");

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || 
                        type.IsInterface || 
                        type.IsGenericTypeDefinition ||
                        !type.IsPublic ||
                        !type.IsSubclassOf(typeof(Controller)))
                        continue;

                    foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var attr = method.GetCustomAttribute<DynamicScriptAttribute>();
                        if (attr == null)
                            continue;

                        if (string.IsNullOrEmpty(attr.Key))
                            throw new ArgumentNullException("scripKey", String.Format(
                                "DynamicScript attribute on method {1} of type {0} has empty key!",
                                type.Name, method.Name));

                        var parameters = method.GetParameters();
                        if (parameters.Length > 2 ||
                            parameters.Any(x => x.ParameterType != typeof(ServiceRequest) &&
                                x.ParameterType != typeof(IDbConnection)))
                        {
                            throw new ArgumentOutOfRangeException("parameters", String.Format(
                                "DynamicScript actions shouldn't have any parameters other " + 
                                "than an a base ServiceRequest and optional IDbConnection. Method {0} of type {1} has {2} arguments",
                                type.Name, method.Name));
                        }

                        string connectionKey = null;
                        
                        if (parameters.Any(x => x.ParameterType == typeof(IDbConnection)))
                        {
                            var connectionKeyAttr = type.GetCustomAttribute<ConnectionKeyAttribute>();
                            if (connectionKeyAttr == null || connectionKeyAttr.Value.IsEmptyOrNull())
                                throw new ArgumentOutOfRangeException("connectionKey", String.Format(
                                    "Cannot determine connection key for DynamicScript defined on type {1}, method {2}",
                                    type.Name, method.Name));

                            connectionKey = connectionKeyAttr.Value;
                        }

                        var returnType = method.ReturnType;

                        if (returnType == typeof(void))
                            throw new ArgumentOutOfRangeException("returnType", String.Format(
                                "DynamicScript defined on method {2} of type {1} has void return type",
                                type.Name, method.Name));

                        var isResult = returnType.IsGenericType && 
                            returnType.GetGenericTypeDefinition() == typeof(Result<>);
                        if (!isResult && typeof(ActionResult).IsAssignableFrom(returnType))
                            throw new ArgumentOutOfRangeException("returnType", String.Format(
                                "DynamicScript defined on method {2} of type {1} has ActionResult descendant return type. " +
                                "It must be Result<T> or a regular class!",
                                type.Name, method.Name));

                        var dataScript = new DataScript(attr.Key, delegate()
                        {
                            var controller = Activator.CreateInstance(type);

                            object result;

                            object[] prm;
                            if (parameters.Length == 0)
                                prm = null;
                            else
                                prm = new object[parameters.Length];

                            IDbConnection connection = null;
                            try
                            {
                                for (var i = 0; i < parameters.Length; i++)
                                {
                                    if (parameters[i].ParameterType == typeof(ServiceRequest))
                                    {
                                        prm[i] = new ServiceRequest();
                                    }
                                    else
                                    {
                                        if (connection == null)
                                            connection = SqlConnections.NewByKey(connectionKey);

                                        prm[i] = connection;
                                    }
                                }

                                result = method.Invoke(controller, prm);
                            }
                            finally
                            {
                                if (connection != null)
                                    connection.Dispose();
                            }

                            if (isResult)
                                result = ((dynamic)result).Data;

                            return result;
                        });

                        dataScript.Expiration = TimeSpan.FromSeconds(attr.CacheDuration);
                        dataScript.GroupKey = attr.CacheGroupKey;
                        
                        var serviceAuthorize = method.GetCustomAttribute<ServiceAuthorizeAttribute>() ??
                            type.GetCustomAttribute<ServiceAuthorizeAttribute>();

                        if (serviceAuthorize != null)
                            dataScript.Permission = serviceAuthorize.Permission ?? "?";
                        else
                        {
                            var authorize = method.GetCustomAttribute<AuthorizeAttribute>() ??
                                type.GetCustomAttribute<AuthorizeAttribute>();

                            if (authorize != null)
                                dataScript.Permission = "?";
                        }

                        DynamicScriptManager.Register(dataScript.ScriptName, dataScript);
                    }
                }
            }
        }
    }
}
