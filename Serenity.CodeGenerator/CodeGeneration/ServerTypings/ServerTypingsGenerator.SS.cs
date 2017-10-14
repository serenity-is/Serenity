using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        private void SSTypeNameToTS(string typeName, string codeNamespace, string defaultType = "any",
            string[] leaveAsIs = null)
        {
            const string nullable = "System.Nullable<";

            if (typeName.StartsWith(nullable) &&
                typeName.EndsWith(">"))
            {
                typeName = typeName.Substring(nullable.Length, typeName.Length - nullable.Length - 1);
            }

            switch (typeName)
            {
                case "System.Type":
                    sb.Append("Function");
                    return;

                case "System.String":
                    sb.Append("string");
                    return;

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    sb.Append("number");
                    return;

                case "System.Boolean":
                    sb.Append("boolean");
                    return;

                case "System.JsDate":
                    sb.Append("Date");
                    return;

                case "jQueryApi.jQueryObject":
                    sb.Append("JQuery");
                    return;

                case "System.Action":
                    sb.Append("() => void");
                    return;

                case "System.Guid":
                    sb.Append("string");
                    return;

                default:

                    typeName = FixupSSGenerics(typeName);

                    Action<string> handlePart = part =>
                    {
                        if (leaveAsIs != null &&
                            leaveAsIs.Contains(part))
                            sb.Append(part);
                        else
                            SSTypeNameToTS(part, codeNamespace, "any", leaveAsIs);
                    };

                    if (IsGenericTypeName(typeName))
                    {
                        var parts = SplitGenericArguments(ref typeName);

                        var scriptType = GetScriptType(typeName);
                        if (scriptType == null)
                        {
                            if (parts.Length == 1 &&
                                typeName == "System.Collections.Generic.List`1" ||
                                typeName == "System.Collections.Generic.IList`1")
                            {
                                handlePart(parts[0]);
                                sb.Append("[]");
                                return;
                            }
                            else if (parts.Length > 0 &&
                                typeName.StartsWith("System.Func`"))
                            {
                                int k = 0;
                                sb.Append("(");
                                foreach (var part in parts.Take(parts.Length - 1))
                                {
                                    if (k++ > 0)
                                        sb.Append(", ");

                                    sb.Append("p" + k);
                                    sb.Append(": ");
                                    handlePart(part);
                                }
                                sb.Append(") => ");
                                handlePart(parts.Last());
                                return;
                            }
                            else if (typeName.StartsWith("System.Action`"))
                            {
                                int k = 0;
                                sb.Append("(");
                                foreach (var part in parts)
                                {
                                    if (k++ > 0)
                                        sb.Append(", ");

                                    sb.Append("p" + k);
                                    sb.Append(": ");
                                    handlePart(part);
                                }
                                sb.Append(") => void");
                                return;
                            }
                            else
                            {
                                sb.Append(defaultType);
                                return;
                            }
                        }
                        else
                        {
                            var ns = ShortenNamespace(scriptType, codeNamespace);
                            if (!string.IsNullOrEmpty(ns))
                            {
                                sb.Append(ns);
                                sb.Append(".");
                            }

                            sb.Append(scriptType.Name.Split('`')[0]);
                        }

                        sb.Append("<");
                        int i = 0;
                        foreach (var part in parts)
                        {
                            if (i++ > 0)
                                sb.Append(", ");

                            if (leaveAsIs != null &&
                                leaveAsIs.Contains(part))
                                sb.Append(part);
                            else
                                SSTypeNameToTS(part, codeNamespace, "any", leaveAsIs);
                        }

                        sb.Append(">");
                    }
                    else
                    {
                        var scriptType = GetScriptType(typeName);
                        if (scriptType == null)
                            sb.Append(defaultType);
                        else
                            sb.Append(ShortenFullName(scriptType, codeNamespace));
                    }
                    break;
            }
        }

        private void SSMethodArguments(IEnumerable<ExternalArgument> arguments, string codeNamespace)
        {
            int k = 0;
            foreach (var arg in arguments)
            {
                if (k++ > 0)
                    sb.Append(", ");

                sb.Append(arg.Name);
                if (arg.IsOptional || arg.HasDefault)
                    sb.Append("?");

                sb.Append(": ");

                SSTypeNameToTS(arg.Type, codeNamespace);
            }
        }

        private void SSDeclarationConstructor(ExternalMethod ctor, string codeNamespace)
        {
            cw.Indented("constructor(");
            SSMethodArguments(ctor.Arguments, codeNamespace);
            sb.AppendLine(");");
        }

        private string GetMethodName(ExternalMethod method, bool preserveMemberCase)
        {
            string methodName = method.Name;

            var scriptNameAttr = method.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                methodName = scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !method.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                if (methodName == "ID")
                    methodName = "id";
                else methodName = methodName.Substring(0, 1).ToLowerInvariant()
                    + methodName.Substring(1);
            }

            return methodName;
        }

        private void SSDeclarationMethodInternal(ExternalMethod method, string codeNamespace,
            bool isStaticClass, bool preserveMemberCase)
        {
            if (method.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.InlineCodeAttribute"))
                return;

            var methodName = GetMethodName(method, preserveMemberCase);

            if (isStaticClass && method.IsStatic)
            {
                cw.Indented("function ");
                sb.Append(methodName);
            }
            else if (method.IsStatic)
            {
                cw.Indented("static ");
                sb.Append(methodName);
            }
            else
            {
                cw.Indented(methodName);
            }

            sb.Append("(");
            SSMethodArguments(method.Arguments, codeNamespace);
            sb.Append("): ");

            if (method.Type == null ||
                method.Type == "System.Void")
            {
                sb.Append("void");
            }
            else
            {
                SSTypeNameToTS(method.Type, codeNamespace);
            }
            sb.AppendLine(";");
        }

        private void SSDeclarationMethod(ExternalMethod method, string codeNamespace,
            bool isStaticClass, bool preserveMemberCase)
        {
            if (method.IsConstructor || method.IsOverride || method.IsGetter || method.IsSetter)
                return;

            SSDeclarationMethodInternal(method, codeNamespace, isStaticClass, preserveMemberCase);
        }

        private void SSDeclarationProperty(ExternalType type, ExternalProperty prop, string codeNamespace,
            bool isStaticClass, bool isSerializable, bool preserveMemberCase)
        {
            if (string.IsNullOrEmpty(prop.GetMethod) &&
                string.IsNullOrEmpty(prop.SetMethod))
                return;

            var propName = GetPropertyScriptName(prop, preserveMemberCase);

            if (isSerializable ||
                prop.Attributes.FirstOrDefault(x => 
                    x.Type == "System.Runtime.CompilerServices.IntrinsicPropertyAttribute") != null)
            {
                if (isStaticClass && prop.IsStatic)
                {
                    cw.Indented("let ");
                    sb.Append(propName);
                }
                else if (prop.IsStatic)
                {
                    cw.Indented("static ");
                    sb.Append(propName);
                }
                else
                {
                    cw.Indented(propName);
                }

                sb.Append(": ");
                SSTypeNameToTS(prop.Type, codeNamespace);
                sb.AppendLine(";");
            }
            else
            {
                var getMethod = type.Methods.FirstOrDefault(x => x.Name == prop.GetMethod);

                if (getMethod != null)
                {
                    getMethod.Name = "get_" + propName;
                    SSDeclarationMethodInternal(getMethod, codeNamespace, isStaticClass, preserveMemberCase);
                }

                var setMethod = type.Methods.FirstOrDefault(x => x.Name == prop.SetMethod);
                if (setMethod != null)
                {
                    setMethod.Name = "set_" + propName;
                    SSDeclarationMethodInternal(setMethod, codeNamespace, isStaticClass, preserveMemberCase);
                }
            }
        }

        private void SSDeclarationField(ExternalType type, ExternalMember field, string codeNamespace,
            bool isStaticClass, bool isSerializable, bool preserveMemberCase)
        {
            string fieldName = field.Name;

            var scriptNameAttr = field.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                fieldName = scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !field.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                if (fieldName == "ID")
                    fieldName = "id";
                else fieldName = fieldName.Substring(0, 1).ToLowerInvariant()
                    + fieldName.Substring(1);
            }

            if (isStaticClass && field.IsStatic)
            {
                cw.Indented("let ");
                sb.Append(fieldName);
            }
            else if (field.IsStatic)
            {
                cw.Indented("static ");
                sb.Append(fieldName);
            }
            else
            {
                cw.Indented(fieldName);
            }

            sb.Append(": ");
            SSTypeNameToTS(field.Type, codeNamespace);
            sb.AppendLine(";");
        }

        private string FixupSSGenerics(string typeName)
        {
            if (typeName == "Serenity.TemplatedDialog")
                typeName = "Serenity.TemplatedDialog`1<System.Object>";
            else if (typeName == "Serenity.EntityDialog")
                typeName = "Serenity.EntityDialog`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.EntityDialog`1<"))
            {
                typeName = typeName.Replace("Serenity.EntityDialog`1<", "Serenity.EntityDialog`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.PropertyDialog")
                typeName = "Serenity.PropertyDialog`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.PropertyDialog`1<"))
            {
                typeName = typeName.Replace("Serenity.PropertyDialog`1<", "Serenity.PropertyDialog`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.EntityGrid")
                typeName = "Serenity.EntityGrid`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.EntityGrid`1<"))
            {
                typeName = typeName.Replace("Serenity.EntityGrid`1<", "Serenity.EntityGrid`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.DataGrid")
                typeName = "Serenity.DataGrid`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.DataGrid`1<"))
            {
                typeName = typeName.Replace("Serenity.DataGrid`1<", "Serenity.DataGrid`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.PropertyPanel")
                typeName = "Serenity.PropertyPanel`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.PropertyPanel`1<"))
            {
                typeName = typeName.Replace("Serenity.PropertyPanel`1<", "Serenity.PropertyPanel`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.CheckTreeEditor")
                typeName = "Serenity.CheckTreeEditor`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.CheckTreeEditor`1<"))
            {
                typeName = typeName.Replace("Serenity.CheckTreeEditor`1<", "Serenity.CheckTreeEditor`2<System.Object, ");
            }
            else if (typeName == "Serenity.CheckTreeItem")
            {
                typeName = "Serenity.CheckTreeItem`1<System.Object>";
            }
            else if (typeName == "Serenity.LookupEditorBase")
                typeName = "Serenity.LookupEditorBase`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.LookupEditorBase`1<"))
            {
                typeName = typeName.Replace("Serenity.LookupEditorBase`1<", "Serenity.LookupEditorBase`2<System.Object, ");
            }
            else if (typeName == "Serenity.TemplatedWidget")
                typeName = "Serenity.TemplatedWidget`1<System.Object>";
            else if (typeName == "Serenity.ServiceCallOptions")
            {
                typeName = "Serenity.ServiceCallOptions`1<System.Object>";
            }
            else if (typeName == "System.Promise")
            {
                typeName = "System.Promise`1<System.Object>";
            }
            return typeName;
        }

        private void SSDeclarationBaseTypeReference(ExternalType type, string baseType, string codeNamespace)
        {
            if (string.IsNullOrEmpty(baseType))
                return;

            if (baseType == "System.Object")
                return;

            sb.Append(" extends ");

            SSTypeNameToTS(baseType, codeNamespace, "Object",
                type.GenericParameters.Select(x => x.Name).ToArray());
        }

        private void GenerateSSDeclarations()
        {
            var byNamespace = 
                ssByScriptName.Values.Where(x =>
                    !tsTypes.ContainsKey(x.FullName) &&
                    !generatedTypes.Contains(x.FullName))
                .Where(x => !x.AssemblyName.StartsWith("Serenity.Script"))
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name)
                .ToLookup(x => x.Namespace);

            int i = 0;
            foreach (var item in byNamespace)
            {
                if (i++ > 0)
                    sb.AppendLine();

                cw.Indented("declare namespace ");
                var codeNamespace = item.Key;
                sb.Append(codeNamespace);

                cw.InBrace(delegate
                {
                    int j = 0;
                    foreach (var type in item)
                    {
                        if (j++ > 0)
                            sb.AppendLine();

                        bool isStatic = type.IsAbstract && type.IsSealed;
                        bool isSerializable = type.IsSerializable ||
                            type.Attributes.Any(x =>
                                x.Type == "System.SerializableAttribute");
                        bool isClass = !type.IsInterface && !isStatic;

                        if (type.IsInterface || isSerializable)
                            cw.Indented("interface ");
                        else if (isStatic)
                            cw.Indented("namespace ");
                        else
                            cw.Indented("class ");

                        sb.Append(type.Name.Split('`')[0]);

                        if (isClass && type.GenericParameters.Count > 0)
                        {
                            sb.Append("<");

                            var k = 0;
                            foreach (var arg in type.GenericParameters)
                            {
                                if (k++ > 0)
                                    sb.Append(", ");

                                sb.Append(arg.Name);
                            }
                            sb.Append(">");
                        }

                        if (isClass)
                            SSDeclarationBaseTypeReference(type, type.BaseType, codeNamespace);

                        cw.InBrace(delegate
                        {

                            bool preserveMemberCase = type.Attributes.Any(x =>
                                x.Type == "System.Runtime.CompilerServices.PreserveMemberCaseAttribute");

                            foreach (var field in type.Fields)
                                SSDeclarationField(type, field, codeNamespace, isStatic, isSerializable, preserveMemberCase);

                            if (isClass)
                            {
                                var ctors = type.Methods.Where(x => x.IsConstructor)
                                    .OrderByDescending(x => x.Arguments.Count);

                                var ctor = ctors.FirstOrDefault();

                                if (ctor != null && ctor.Arguments.Count > 0)
                                    SSDeclarationConstructor(ctor, codeNamespace);
                            }

                            foreach (var method in type.Methods)
                            {
                                SSDeclarationMethod(method, codeNamespace, isStatic, preserveMemberCase);
                            }

                            foreach (var prop in type.Properties)
                            {
                                SSDeclarationProperty(type, prop, codeNamespace, isStatic, isSerializable, preserveMemberCase);
                            }

                        });
                    }
                });
            }

            if (sb.Length > 0)
                AddFile("SSDeclarations.ts");
        }
    }
}