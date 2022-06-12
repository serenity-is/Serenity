using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Serenity.Reflection;

namespace Serenity.SourceGenerator
{
    [Generator]
    public class RowSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) 
        {
            context.RegisterForSyntaxNotifications(() => new PrivateRowFieldsSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not PrivateRowFieldsSyntaxReceiver receiver)
                return;

            foreach (var group in receiver.PrivateFields.GroupBy<IFieldSymbol, INamedTypeSymbol>(
                x => x.ContainingType,
                SymbolEqualityComparer.Default))
            {
                ProcessRow(group.Key, group, context);
            }

        }

        const string RowFieldsClass = "RowFields";

        private void ProcessRow(INamedTypeSymbol rowSymbol, 
            IEnumerable<IFieldSymbol> fields, GeneratorExecutionContext context)
        {
            var ns = rowSymbol.ContainingNamespace.ToDisplayString();
            var fullName = StringHelper.JoinNonEmpty(".", ns, rowSymbol.Name);
            var fieldsClass = context.Compilation.GetTypeByMetadataName(fullName + "+" + RowFieldsClass);
            var hasFieldsType = fieldsClass != null;
            var fieldsClassHasCtor = fieldsClass != null && fieldsClass.GetMembers().Any(x => x is
                IMethodSymbol m && m.Name == fieldsClass.Name);

            var cw = new CodeWriter();
            var cwFieldsClass = new StringBuilder();
            var cwFieldsCtor = new StringBuilder();
            cw.InNamespace(ns, () =>
            {
                cw.IndentedLine($"partial class {rowSymbol.Name} : Row<{rowSymbol.Name}.{RowFieldsClass}>");
                cw.InBrace(() =>
                {
                    string valueTypeRef;
                    string fieldTypeRef;
                    var fieldIndex = 0;

                    foreach (var field in fields)
                    {
                        if (field.Type is INamedTypeSymbol namedType &&
                            namedType.IsGenericType &&
                            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                        {
                            switch (namedType.TypeArguments[0].SpecialType)
                            {
                                case SpecialType.System_Int32:
                                    valueTypeRef = "int?";
                                    fieldTypeRef = nameof(Int32Field);
                                    cw.Using(typeof(Int32Field).Namespace);
                                    break;

                                default:
                                    continue;
                            }
                        }
                        else
                        {
                            switch (field.Type.SpecialType)
                            {
                                case SpecialType.System_String:
                                    valueTypeRef = "string";
                                    fieldTypeRef = nameof(StringField);
                                    cw.Using(typeof(StringField).Namespace);
                                    break;

                                default:
                                    continue;
                            }
                        }

                        var propertyName = field.Name;
                        if (char.IsLetter(propertyName[0]) &&
                            char.IsUpper(propertyName[0]))
                            continue;

                        if (propertyName.StartsWith('_'))
                        {
                            if (propertyName.Length == 1)
                                continue;

                            propertyName = propertyName[1..];
                        }

                        propertyName = char.ToUpper(propertyName[0]) + propertyName[1..];

                        if (fieldIndex > 0)
                            cw.AppendLine();

                        cw.IndentedLine($"public {valueTypeRef} {propertyName}");
                        cw.InBrace(() =>
                        {
                            cw.IndentedLine($"get => fields.{propertyName}[this];");
                            cw.IndentedLine($"set => fields.{propertyName}[this] = value;");
                        });

                        cwFieldsClass.AppendLine($"public readonly {fieldTypeRef} {propertyName};");
                        
                        if (fieldIndex > 0)
                            cwFieldsCtor.AppendLine();

                        cwFieldsCtor.AppendLine($"{propertyName} = new {fieldTypeRef}(this, name: \"{propertyName}\", caption: null, size: 0, flags: FieldFlags.Default,");
                        cwFieldsCtor.AppendLine($"{cw.Tab}getValue: row => (({rowSymbol.Name})row).{field.Name},");
                        cwFieldsCtor.AppendLine($"{cw.Tab}setValue: (row, value) => (({rowSymbol.Name})row).{field.Name} = value);");

                        fieldIndex++;
                    }

                    if (fieldIndex > 0)
                        cw.AppendLine();

                    if (hasFieldsType)
                        cw.IndentedLine($"partial class {RowFieldsClass}");
                    else
                    {
                        cw.Using(typeof(RowFieldsBase).Namespace);
                        cw.IndentedLine($"public partial class {RowFieldsClass} : {nameof(RowFieldsBase)}");
                    }

                    cw.InBrace(() =>
                    {
                        cw.IndentedMultiLine(cwFieldsClass.ToString().TrimEnd());

                        if (!fieldsClassHasCtor)
                        {
                            cw.AppendLine();
                            cw.IndentedLine($"public {RowFieldsClass}()");
                            cw.InBrace(() =>
                            {
                                cw.IndentedMultiLine(cwFieldsCtor.ToString().TrimEnd());
                            });
                        }
                    });
                });
            });

            context.AddSource(fullName + ".generated", SourceText.From(cw.ToString(), Encoding.UTF8));
        }
    }
}