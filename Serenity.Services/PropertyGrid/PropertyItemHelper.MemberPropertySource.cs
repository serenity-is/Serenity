using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public partial class PropertyItemHelper
    {
        internal class MemberPropertySource : IPropertySource
        {
            public MemberPropertySource(MemberInfo member, Row basedOnRow)
            {
                if (member == null)
                    throw new ArgumentNullException("member");

                Member = member;
                BasedOnRow = basedOnRow;

                if (basedOnRow != null)
                    BasedOnField = basedOnRow.FindField(member.Name) ??
                        basedOnRow.FindFieldByPropertyName(member.Name);

                var memberType = member.MemberType == MemberTypes.Property ?
                    ((PropertyInfo)member).PropertyType :
                    ((FieldInfo)member).FieldType;

                var nullableType = Nullable.GetUnderlyingType(memberType);
                ValueType = nullableType ?? memberType;

                if (ValueType.IsEnum)
                    EnumType = ValueType;
                else if (
                    !ReferenceEquals(null, BasedOnField)
                    && BasedOnField is IEnumTypeField)
                {
                    EnumType = (BasedOnField as IEnumTypeField).EnumType;
                    if (EnumType != null && !EnumType.IsEnum)
                        EnumType = null;
                }
            }

            public TAttribute GetAttribute<TAttribute>()
                where TAttribute : Attribute
            {
                return Member.GetCustomAttribute<TAttribute>() ??
                    BasedOnField.GetAttribute<TAttribute>();
            }

            public IEnumerable<TAttribute> GetAttributes<TAttribute>()
                where TAttribute : Attribute
            {
                var attrList = new List<TAttribute>();
                attrList.AddRange(Member.GetCustomAttributes<TAttribute>());

                if (!ReferenceEquals(null, BasedOnField) &&
                    BasedOnField.CustomAttributes != null)
                {
                    foreach (var a in BasedOnField.CustomAttributes)
                        if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                            attrList.Add((TAttribute)a);
                }

                return attrList;
            }

            public MemberInfo Member { get; private set; }
            public Type ValueType { get; private set; }
            public Type EnumType { get; private set; }
            public Row BasedOnRow { get; private set; }
            public Field BasedOnField { get; private set; }
        }
    }
}