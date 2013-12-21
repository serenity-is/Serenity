namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        public SqlQuery Join(Join join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            _cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref _from, " \n", join.GetKeyword());

            _from.Append(' ');
            _from.Append(join.ToTable);

            // joinAlias belirtilmişse ekle
            if (!join.Name.IsEmptyOrNull())
            {
                _from.Append(' ');
                _from.Append(join.Name);

                if (_joinAliases == null)
                    _joinAliases = new HashSet<string>();

                _joinAliases.Add(join.Name);
            }

            if (!Object.ReferenceEquals(null, join.OnCriteria) &&
                !join.OnCriteria.IsEmpty)
            {
                _from.Append(" ON (");
                _from.Append(join.OnCriteria.ToString(this));
                _from.Append(')');
            }

            return this;
        }

        //public SqlQuery LeftJoin(string joinTable, Alias joinAlias, string joinCondition)
        //{
        //    return LeftJoin(joinTable, joinAlias.Name, joinCondition);
        //}

        //public SqlQuery LeftJoin(string joinTable, string joinAlias, BaseCriteria joinCondition)
        //{
        //    if (Object.ReferenceEquals(joinCondition, null))
        //        throw new ArgumentNullException("joinCondition");

        //    LeftJoin(joinTable, joinAlias, joinCondition.ToString(this));

        //    return this;
        //}

        //public SqlQuery LeftJoin(string joinTable, Alias joinAlias, BaseCriteria joinCondition)
        //{
        //    if (Object.ReferenceEquals(joinCondition, null))
        //        throw new ArgumentNullException("joinCondition");

        //    LeftJoin(joinTable, joinAlias, joinCondition.ToString(this));

        //    return this;
        //}

        //public SqlQuery LeftJoinOn(string thisField, RowFieldsBase joinTable, Alias joinAlias, Field field)
        //{
        //    return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, field) == new Criteria(thisField));
        //}

        //public SqlQuery LeftJoinOn(Criteria thisField, RowFieldsBase joinTable, Alias joinAlias, Field field)
        //{
        //    return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, field) == thisField);
        //}

        //public SqlQuery LeftJoinId(string thisField, RowFieldsBase joinTable, Alias joinAlias)
        //{
        //    var idField = joinTable.FirstOrDefault(x => x.Flags.HasFlag(FieldFlags.PrimaryKey));
        //    if (idField == null)
        //        throw new ArgumentOutOfRangeException("joinTable", String.Format("Table {0} has no primary key!", joinTable.TableName));

        //    return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, idField) == new Criteria(thisField));
        //}

        //public SqlQuery LeftJoinId(RowFieldsBase joinTable, Alias joinAlias, Criteria thisField)
        //{
        //    var idField = joinTable.FirstOrDefault(x => x.Flags.HasFlag(FieldFlags.PrimaryKey));
        //    if (idField == null)
        //        throw new ArgumentOutOfRangeException("joinTable", String.Format("Table {0} has no primary key!", joinTable.TableName));

        //    return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, idField) == thisField);
        //}



        //public SqlQuery LeftJoin(string joinTable, int joinNumber, BaseCriteria joinCondition)
        //{
        //    if (Object.ReferenceEquals(joinCondition, null))
        //        throw new ArgumentNullException("joinCondition");

        //    LeftJoin(joinTable, joinNumber.TableAlias(), joinCondition.ToString(this));

        //    return this;
        //}

        //public SqlQuery Join(Join join)
        //{
        //    return LeftJoin(join.ToTable, join.Name, join.OnCriteria);
        //}

        //public SqlQuery OuterApply(string joinAlias, string expression)
        //{
        //    if (expression == null || expression.Length == 0)
        //        throw new ArgumentNullException("expression");

        //    if (joinAlias.IsEmptyOrNull())
        //        throw new ArgumentNullException("joinAlias");

        //    _cachedQuery = null;

        //     araya bir boşluk ve LEFT OUTER JOIN metnini ekle.
        //    AppendUtils.AppendWithSeparator(ref _from, " ", "OUTER APPLY (");
        //     tablo adını ekle
        //    _from.Append(expression);

        //    _from.Append(") ");
        //    _from.Append(joinAlias);

        //    if (_joinAliases == null)
        //        _joinAliases = new HashSet<string>();

        //    _joinAliases.Add(joinAlias);

        //    return this;
        //}

        //public SqlQuery OuterApply(Alias joinAlias, string expression)
        //{
        //    return OuterApply(joinAlias.Name, expression);
        //}
    }
}