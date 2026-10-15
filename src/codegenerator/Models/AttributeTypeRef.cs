namespace Serenity.CodeGenerator;

public class AttributeTypeRef(string typeName, params object?[] arguments)
{
    public string TypeName { get; } = typeName ?? throw new ArgumentNullException(nameof(typeName));
    public object?[] Arguments { get; } = arguments;

    public string ToString(CodeWriter cw)
    {
        var s = cw.ShortTypeRef(TypeName);
        if (Arguments == null || Arguments.Length == 0)
            return s;

        s += "(";
        for (var i = 0; i < Arguments.Length; i++)
        {
            var value = Arguments[i];
            if (i > 0)
                s += ", ";

            s += FormatArgument(value, cw);
        }

        return s + ")";
    }

    private static string FormatArgument(object? value, CodeWriter cw)
    {
        if (value is TypeOfRef tr)
            return tr.ToString(cw);
        else if (value is NameOfRef nr)
            return nr.ToString(cw);
        else if (value is NamedArg na)
            return na.PropertyName + " = " + FormatArgument(na.Value, cw);
        else if (value is string str)
            return StringHelper.ToDoubleQuoted(str);
        else if (value is bool b)
            return b ? "true" : "false";
        else if (value is IFormattable fmt)
            return fmt.ToString(null, CultureInfo.InvariantCulture);
        else
            return value?.ToString() ?? "null";
    }
}