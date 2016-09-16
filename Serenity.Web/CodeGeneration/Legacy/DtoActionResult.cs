#if !COREFX
using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Serenity.CodeGeneration;

namespace Serenity.Services
{
    public class DtoActionResult : ActionResult
    {
        public string Namespace { get; private set; }
        public string Language { get; private set; }
        public Type[] ExtraTypes { get; private set; }

        public DtoActionResult(string language, string anamespace, params Type[] extraTypes)
        {
            this.Namespace = anamespace;
            this.Language = language;
            this.ExtraTypes = extraTypes;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var types = new HashSet<Type>();

            var sb = new StringBuilder();
            var cw = new CodeWriter(sb, 4);
            cw.Indented("namespace ");
            sb.AppendLine(this.Namespace);
            cw.InBrace(delegate
            {
                foreach (var ns in
                    new string[] {
                        "System",
                        "System.Collections.Generic"
                    })
                {
                    cw.Indented("using ");
                    sb.Append(ns);
                    sb.AppendLine(";");
                }

                sb.AppendLine();

                cw.Indented("public interface I");
                var className = context.Controller.GetType().Name;
                if (className.EndsWith("Controller"))
                    className = className.Substring(0, className.Length - 10);
                sb.Append(className);
                sb.AppendLine("Service");

                cw.InBrace(delegate
                {
                    foreach (var method in context.Controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (method.GetCustomAttribute<NonActionAttribute>() != null)
                            continue;

                        if (typeof(Controller).IsSubclassOf(method.DeclaringType))
                            continue;

                        if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
                            continue;

                        // belki burada daha sonra metod listesini de verebiliriz (ayrı bir namespace de?)
                        var parameters = method.GetParameters();
                        if (parameters.Length != 1)
                        {
                            // tek parametreli olmalı
                            continue;
                        }

                        var returnType = method.ReturnType;
                        if (returnType.IsAssignableFrom(typeof(DtoActionResult)))
                            continue;

                        var paramType = parameters[0].ParameterType;
                        if (paramType.IsPrimitive || !DtoGenerator.CanHandleType(paramType))
                            continue;

                        types.Add(parameters[0].ParameterType);

                        Type responseType = returnType;
                        if (returnType != null &&
                            returnType.IsGenericType &&
                            returnType.GetGenericTypeDefinition() == typeof(Result<>))
                        {
                            responseType = returnType.GenericTypeArguments[0];
                            types.Add(responseType);
                        }

                        cw.Indent();
                        DtoGenerator.HandleMemberType(sb, responseType, enqueueType: null);
                        sb.Append(' ');
                        sb.Append(method.Name);
                        
                        sb.Append("(");
                        DtoGenerator.HandleMemberType(sb, paramType, enqueueType: null);
                        sb.Append(' ');
                        sb.Append(parameters[0].Name);
                        sb.AppendLine(");");
                    }
                });
            });

            if (ExtraTypes != null &&
                ExtraTypes.Length > 0)
                types.AddRange(ExtraTypes);

            Type[] fromTypes = new Type[types.Count];
            types.CopyTo(fromTypes, 0);

            sb.AppendLine();
            sb.AppendLine();
            var code = sb.ToString() + new DtoGenerator().GenerateCode(Namespace, fromTypes);

            new ContentResult 
            { 
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/plain",
                Content = code
            }.ExecuteResult(context);
        }
    }
}
#endif