namespace Serenity.CodeGenerator;

public class AttributeTypeRef(string typeName, params object[] arguments)
{
    public string TypeName { get; } = typeName ?? throw new ArgumentNullException(nameof(typeName));
    public object[] Arguments { get; } = arguments;

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

            if (value is TypeOfRef tr)
                s += tr.ToString(cw);
            else if (value is NameOfRef nr)
                s += nr.ToString(cw);
            else if (value is string str)
                s += StringHelper.ToDoubleQuoted(str);
            else if (value is bool b)
                s += b ? "true" : "false";
            else if (value is IFormattable fmt)
                s += fmt.ToString(null, CultureInfo.InvariantCulture);
            else
                s += value?.ToString() ?? "null";
        }

        return s + ")";
    }
}