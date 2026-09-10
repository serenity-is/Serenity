namespace Microsoft.Data.SqlClient;

public class SqlParameter : System.Data.Common.DbParameter
{
    public object DbValue { get; set; }

    public override DbType DbType { get; set; }

    public SqlDbType SqlDbType { get; set; }

    public override ParameterDirection Direction { get; set; }

    public override bool IsNullable { get; set; }

    public override string ParameterName { get; set; }

    public override string SourceColumn { get; set; }

    public override bool SourceColumnNullMapping { get; set; }

    public override int Size { get; set; }

    public override object Value { get { return DbValue; } set { DbValue = value; } }

    public override void ResetDbType()
    {
    }
}
