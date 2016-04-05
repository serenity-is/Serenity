using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Reflection;
using Serenity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Serenity.CodeGeneration
{
    public class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        public ServerTypingsGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }

        protected override bool IsTS()
        {
            return true;
        }

        protected override void GenerateAll()
        {
            base.GenerateAll();
            GenerateSSDeclarations();
        }

        protected override void HandleMemberType(Type memberType, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            if (memberType == typeof(String))
            {
                sb.Append("string");
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(memberType);
            if (nullableType != null)
                memberType = nullableType;

            if (memberType == typeof(Int16) ||
                memberType == typeof(Int32) ||
                memberType == typeof(Int64) ||
                memberType == typeof(UInt16) ||
                memberType == typeof(UInt32) ||
                memberType == typeof(UInt64) ||
                memberType == typeof(Single) ||
                memberType == typeof(Double) ||
                memberType == typeof(Decimal))
            {
                sb.Append("number");
                return;
            }

            if (memberType == typeof(Boolean))
            {
                sb.Append("boolean");
                return;
            }

            if (memberType == typeof(TimeSpan))
            {
                sb.Append("string");
                return;
            }

            if (memberType == typeof(DateTime?) || memberType == typeof(DateTime) ||
                memberType == typeof(TimeSpan) || memberType == typeof(TimeSpan?))
            {
                sb.Append("string"); // for now transfer datetime as string, as its ISO formatted
                return;
            }

            if (memberType == typeof(SortBy[]))
            {
                sb.Append("string[]");
                return;
            }

            if (memberType == typeof(Stream))
            {
                sb.Append("number[]");
                return;
            }

            if (memberType == typeof(Object))
            {
                sb.Append("any");
                return;
            }

            if (memberType.IsArray)
            {
                HandleMemberType(memberType.GetElementType(), codeNamespace, sb);
                sb.Append("[]");
                return;
            }

            if (memberType.IsGenericType &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) ||
                memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append("[]");
                return;
            }

            if (memberType.IsGenericType &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                sb.Append("{ [key: ");
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append("]: ");
                HandleMemberType(memberType.GenericTypeArguments[1], codeNamespace, sb);
                sb.Append(" }");
                return;
            }

            EnqueueType(memberType);

            MakeFriendlyReference(memberType, codeNamespace);
        }
        protected string ShortenFullName(ExternalType type, string codeNamespace)
        {
            if (type.FullName == "Serenity.Widget")
                return "Serenity.Widget<any>";

            var ns = ShortenNamespace(type, codeNamespace);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + type.Name;
            else
                return type.Name;
        }

        private void GenerateEnum(Type enumType)
        {
            var codeNamespace = GetNamespace(enumType);
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            cw.Indented("export enum ");
            var generatedName = MakeFriendlyName(enumType, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

            cw.InBrace(delegate
            {
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                int i = 0;
                foreach (var name in names)
                {
                    if (i > 0)
                        sb.AppendLine(",");

                    cw.Indented(name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt32(((IList)values)[i]));
                    i++;
                }

                sb.AppendLine();
            });

            cw.Indented("Serenity.Decorators.addAttribute(");
            sb.Append(enumType.Name);
            sb.Append(", new Serenity.EnumKeyAttribute('");
            sb.Append(enumKey);
            sb.AppendLine("'));");
        }

        private void GenerateRowMetadata(Type rowType)
        {
            Row row = (Row)rowType.GetInstance();

            var idRow = row as IIdRow;
            var isActiveRow = row as IIsActiveRow;
            var nameRow = row as INameRow;
            var lookupAttr = rowType.GetCustomAttribute<LookupScriptAttribute>();
            if (lookupAttr == null)
            {
                var script = lookupScripts.FirstOrDefault(x =>
                    x.BaseType != null &&
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericArguments().Any(z => z == rowType));

                if (script != null)
                    lookupAttr = script.GetCustomAttribute<LookupScriptAttribute>();
            }

            sb.AppendLine();
            cw.Indented("export namespace ");
            sb.Append(rowType.Name);

            cw.InBrace(delegate
            {
                bool anyMetadata = false;

                if (idRow != null)
                {
                    cw.Indented("export const idProperty = '");
                    var field = ((Field)idRow.IdField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (isActiveRow != null)
                {
                    cw.Indented("export const isActiveProperty = '");
                    var field = (isActiveRow.IsActiveField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (nameRow != null)
                {
                    cw.Indented("export const nameProperty = '");
                    var field = (nameRow.NameField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                var localTextPrefix = row.GetFields().LocalTextPrefix;
                if (!string.IsNullOrEmpty(localTextPrefix))
                {
                    cw.Indented("export const localTextPrefix = '");
                    sb.Append(localTextPrefix);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (lookupAttr != null)
                {
                    cw.Indented("export const lookupKey = '");
                    sb.Append(lookupAttr.Key);
                    sb.AppendLine("';");

                    sb.AppendLine();
                    cw.Indented("export function lookup()");
                    cw.InBrace(delegate
                    {
                        cw.Indented("return Q.getLookup('");
                        sb.Append(lookupAttr.Key);
                        sb.AppendLine("');");
                    });

                    anyMetadata = true;

                }

                if (anyMetadata)
                    sb.AppendLine();

                cw.Indented("export namespace ");
                sb.Append("Fields");

                cw.InBrace(delegate
                {
                    foreach (var field in row.GetFields())
                    {
                        cw.Indented("export declare const ");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.Append(": '");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.AppendLine("';");
                    }
                });

                sb.AppendLine();
                cw.Indented("[");
                int i = 0;
                foreach (var field in row.GetFields())
                {
                    if (i++ > 0)
                        sb.Append(", ");
                    sb.Append("'");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.Append("'");
                }
                sb.AppendLine("].forEach(x => (<any>Fields)[x] = x);");
            });
        }

        private void GenerateRowMembers(Type rowType)
        {
            var codeNamespace = GetNamespace(rowType);

            Row row = (Row)rowType.GetInstance();

            foreach (var field in row.GetFields())
            {
                cw.Indented(field.PropertyName ?? field.Name);
                sb.Append("?: ");
                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType, codeNamespace);
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType, codeNamespace);
                }

                sb.AppendLine(";");
            }
        }
        protected override void GenerateCodeFor(Type type)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("namespace ");
            sb.Append(codeNamespace);

            cw.InBrace(delegate
            {
                if (type.IsEnum)
                {
                    GenerateEnum(type);
                    return;
                }

                if (type.IsSubclassOf(typeof(Controller)))
                {
                    GenerateService(type);
                    return;
                }

                var formScriptAttr = type.GetCustomAttribute<FormScriptAttribute>();
                if (formScriptAttr != null)
                {
                    GenerateForm(type, formScriptAttr);
                    return;
                }

                if (type.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                {
                    //GenerateColumns(type);
                    return;
                }

                cw.Indented("export interface ");

                var generatedName = MakeFriendlyName(type, codeNamespace);
                generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

                var baseClass = GetBaseClass(type);
                if (baseClass != null)
                {
                    sb.Append(" extends ");
                    MakeFriendlyReference(baseClass, GetNamespace(type));
                }

                cw.InBrace(delegate
                {
                    if (type.IsSubclassOf(typeof(Row)))
                        GenerateRowMembers(type);
                    else
                    {
                        foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (member.GetCustomAttribute<JsonIgnoreAttribute>(false) != null)
                                continue;

                            if (baseClass != null && member.DeclaringType.IsAssignableFrom(baseClass))
                                continue;

                            var pi = member as PropertyInfo;
                            var fi = member as FieldInfo;
                            if (pi == null && fi == null)
                                continue;

                            var memberType = pi != null ? pi.PropertyType : fi.FieldType;

                            if (!CanHandleType(memberType))
                                continue;

                            var memberName = pi != null ? pi.Name : fi.Name;

                            var jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>(false);
                            if (jsonProperty != null && !jsonProperty.PropertyName.IsEmptyOrNull())
                                memberName = jsonProperty.PropertyName;

                            cw.Indented(memberName);
                            sb.Append("?: ");
                            HandleMemberType(memberType, codeNamespace);
                            sb.AppendLine();
                        }
                    }
                });

                if (type.IsSubclassOf(typeof(Row)))
                    GenerateRowMetadata(type);
            });
        }

        private void GenerateService(Type type)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export namespace ");
            var identifier = GetControllerIdentifier(type);
            sb.Append(identifier);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);

            cw.InBrace(delegate
            {
                var serviceUrl = GetServiceUrlFromRoute(type);
                if (serviceUrl == null)
                    serviceUrl = GetNamespace(type).Replace(".", "/");

                cw.Indented("export const baseUrl = '");
                sb.Append(serviceUrl);
                sb.AppendLine("';");
                sb.AppendLine();

                Type responseType;
                Type requestType;
                string requestParam;

                var methodNames = new List<string>();
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (methodNames.Contains(method.Name))
                        continue;

                    if (!IsPublicServiceMethod(method, out requestType, out responseType, out requestParam))
                        continue;

                    methodNames.Add(method.Name);

                    cw.Indented("export declare function ");
                    sb.Append(method.Name);

                    sb.Append("(request: ");
                    MakeFriendlyReference(requestType, codeNamespace);

                    sb.Append(", onSuccess?: (response: ");
                    MakeFriendlyReference(responseType, codeNamespace);
                    sb.AppendLine(") => void, opt?: Serenity.ServiceOptions<any>): JQueryXHR;");
                }

                sb.AppendLine();
                cw.Indented("export namespace ");
                sb.Append("Methods");
                cw.InBrace(delegate
                {
                    foreach (var methodName in methodNames)
                    {
                        cw.Indented("export declare const ");
                        sb.Append(methodName);
                        sb.Append(": '");
                        sb.Append(serviceUrl);
                        sb.Append("/");
                        sb.Append(methodName);
                        sb.AppendLine("';");
                    }
                });

                sb.AppendLine();
                cw.Indented("[");
                int i = 0;
                foreach (var methodName in methodNames)
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    sb.Append("'");
                    sb.Append(methodName);
                    sb.Append("'");
                }
                sb.AppendLine("].forEach(x => {");
                cw.Block(delegate () {
                    cw.Indented("(<any>");
                    sb.Append(identifier);
                    sb.AppendLine(")[x] = function (r, s, o) { return Q.serviceRequest(baseUrl + '/' + x, r, s, o); };");
                    cw.IndentedLine("(<any>Methods)[x] = baseUrl + '/' + x;");
                });
                cw.IndentedLine("});");
            });
        }

        private void GenerateForm(Type type, FormScriptAttribute formScriptAttribute)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export class ");
            var generatedName = MakeFriendlyName(type, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

            sb.Append(" extends Serenity.PrefixedContext");
            cw.InBrace(delegate
            {
                cw.Indented("static formKey = '");
                sb.Append(formScriptAttribute.Key);
                sb.AppendLine("';");
                sb.AppendLine();
            });

            sb.AppendLine();

            cw.Indented("export interface ");
            MakeFriendlyName(type, codeNamespace);
            sb.Append(" extends Serenity.PrefixedContext");

            StringBuilder initializer = new StringBuilder("[");

            cw.InBrace(delegate
            {
                int j = 0;
                foreach (var item in Serenity.PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type))
                {
                    var editorType = item.EditorType ?? "String";

                    ExternalType scriptType = null;

                    foreach (var rootNamespace in RootNamespaces)
                    {
                        string wn = rootNamespace + "." + editorType;
                        if ((scriptType = (GetScriptType(wn) ?? GetScriptType(wn + "Editor"))) != null)
                            break;
                    }

                    if (scriptType == null &&
                        (scriptType = (GetScriptType(editorType) ?? GetScriptType(editorType + "Editor"))) == null)
                        continue;

                    var fullName = ShortenFullName(scriptType, codeNamespace);

                    if (j++ > 0)
                        initializer.Append(", ");

                    initializer.Append("['");
                    initializer.Append(item.Name);
                    initializer.Append("', ");
                    initializer.Append(fullName);
                    initializer.Append("]");

                    cw.Indented(item.Name);
                    sb.Append("(): ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }
            });

            initializer.Append("].forEach(x => ");
            MakeFriendlyName(type, codeNamespace, initializer);
            initializer.Append(".prototype[<string>x[0]] = function() { return this.w(x[0], x[1]); });");

            sb.AppendLine();
            cw.IndentedLine(initializer.ToString());
        }

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
            else if (typeName.StartsWith("PropertyDialog`1<"))
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
                ssTypes.Values.Where(x =>
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
                        bool isClass = !type.IsInterface && !isStatic;

                        if (type.IsInterface)
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

                            bool isSerializable = type.IsSerializable ||
                                type.Attributes.Any(x =>
                                    x.Type == "System.SerializableAttribute");

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

            AddFile("SSDeclarations.ts");
        }
    }
}