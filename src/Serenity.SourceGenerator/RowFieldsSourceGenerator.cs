using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.SourceGenerator
{
    [Generator]
    public class RowFieldsSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) 
        {
            context.RegisterForSyntaxNotifications(() => new RowPropertiesSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not RowPropertiesSyntaxReceiver receiver)
                return;

            foreach (var group in receiver.Properties.GroupBy<IPropertySymbol, INamedTypeSymbol>(
                x => x.ContainingType,
                SymbolEqualityComparer.Default))
            {
                ProcessRow(group.Key, group, context);
            }

        }

        const string RowFieldsClass = "RowFields";

        private void ProcessRow(INamedTypeSymbol rowSymbol, 
            IEnumerable<IPropertySymbol> properties, GeneratorExecutionContext context)
        {
            var ns = rowSymbol.ContainingNamespace.ToDisplayString();
            var fullName = rowSymbol.GetFullName();
            var fieldsClass = context.Compilation.GetTypeByMetadataName(fullName + "+" + RowFieldsClass);
            var hasFieldsType = fieldsClass != null;
            var fieldsClassHasCtor = fieldsClass != null && fieldsClass.GetMembers().Any(x => x is
                IMethodSymbol m && m.Name == fieldsClass.Name);

            var cw = new CodeWriter();
            var cwFieldsClass = new StringBuilder();
            var cwFieldsInit = new StringBuilder();
            cw.InNamespace(ns, () =>
            {
                cw.IndentedLine($"partial class {rowSymbol.Name} : Row<{rowSymbol.Name}.{RowFieldsClass}>");
                cw.InBrace(() =>
                {
                    string valueTypeRef;
                    string fieldTypeRef;
                    var fieldIndex = 0;

                    foreach (var property in properties)
                    {
                        if (property.Type is INamedTypeSymbol namedType &&
                            namedType.IsGenericType &&
                            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                        {
                            var valueTypeSymbol = namedType.TypeArguments[0];
                            switch (valueTypeSymbol.SpecialType)
                            {
                                case SpecialType.System_Boolean:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "bool?";
                                    fieldTypeRef = "BooleanField";
                                    break;

                                case SpecialType.System_DateTime:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "DateTime?";
                                    fieldTypeRef = "DateTimeField";
                                    break;

                                case SpecialType.System_Decimal:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "decimal?";
                                    fieldTypeRef = "DecimalField";
                                    break;

                                case SpecialType.System_Double:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "double?";
                                    fieldTypeRef = "DoubleField";
                                    break;

                                case SpecialType.System_Int16:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "short?";
                                    fieldTypeRef = "Int16Field";
                                    break;

                                case SpecialType.System_Int32:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "int?";
                                    fieldTypeRef = "Int32Field";
                                    break;

                                case SpecialType.System_Int64:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "long?";
                                    fieldTypeRef = "Int64Field";
                                    break;

                                default:
                                    if (valueTypeSymbol.TypeKind == TypeKind.Enum)
                                    {
                                        cw.Using("Serenity.Data");
                                        valueTypeRef = valueTypeSymbol.ToTypeRef(cw) + "?";
                                        fieldTypeRef = "EnumField<" + valueTypeRef + ">";
                                        break;
                                    }

                                    var ns = namedType.GetNamespace();

                                    if (ns == typeof(Guid).Namespace &&
                                        namedType.Name == nameof(Guid))
                                    {
                                        cw.Using("Serenity.Data");
                                        valueTypeRef = "Guid?";
                                        fieldTypeRef = "GuidField";
                                        break;
                                    }

                                    if (ns == typeof(TimeSpan).Namespace &&
                                        namedType.Name == nameof(TimeSpan))
                                    {
                                        cw.Using("Serenity.Data");
                                        valueTypeRef = "TimeSpan?";
                                        fieldTypeRef = "TimeSpanField";
                                        break;
                                    }

                                    if (ns == typeof(System.IO.Stream).Namespace &&
                                        namedType.Name == nameof(System.IO.Stream))
                                    {
                                        cw.Using("Serenity.Data");
                                        valueTypeRef = "Stream";
                                        fieldTypeRef = "StreamField";
                                        break;
                                    }

                                    continue;
                            }
                        }
                        else
                        {
                            switch (property.Type.SpecialType)
                            {
                                case SpecialType.System_String:
                                    cw.Using("Serenity.Data");
                                    valueTypeRef = "string";
                                    fieldTypeRef = "StringField";
                                    break;

                                default:
                                    continue;
                            }
                        }

                        var fieldName = property.Name;
                        if (!char.IsLetter(fieldName[0]) ||
                            char.IsLower(fieldName[0]))
                            fieldName = "_" + fieldName;
                        else
                            fieldName = char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);

                        cw.IndentedLine($"private {valueTypeRef} {fieldName};");

                        cwFieldsClass.AppendLine($"public {fieldTypeRef} {property.Name};");
                        
                        if (fieldIndex > 0)
                            cwFieldsInit.AppendLine();

                        cwFieldsInit.AppendLine($"{property.Name} = new {fieldTypeRef}(this, \"{property.Name}\", null, 0, FieldFlags.Default,");
                        cwFieldsInit.AppendLine($"{cw.Tab}getValue: row => (({rowSymbol.Name})row).{fieldName},");
                        cwFieldsInit.AppendLine($"{cw.Tab}setValue: (row, value) => (({rowSymbol.Name})row).{fieldName} = value);");

                        fieldIndex++;
                    }

                    if (fieldIndex > 0)
                        cw.AppendLine();

                    if (hasFieldsType)
                        cw.IndentedLine($"partial class {RowFieldsClass}");
                    else
                    {
                        cw.Using("Serenity.Data");
                        cw.IndentedLine($"public partial class {RowFieldsClass} : RowFieldsBase");
                    }

                    cw.InBrace(() =>
                    {
                        cw.IndentedMultiLine(cwFieldsClass.ToString().TrimEnd());

                        if (!fieldsClassHasCtor)
                        {
                            cw.AppendLine();
                            cw.IndentedLine($"protected override void CreateGeneratedFields()");
                            cw.InBrace(() =>
                            {
                                cw.IndentedMultiLine(cwFieldsInit.ToString().TrimEnd());
                            });
                        }
                    });
                });
            });

            context.AddSource(fullName + ".generated", SourceText.From(cw.ToString(), Encoding.UTF8));
        }
    }
}