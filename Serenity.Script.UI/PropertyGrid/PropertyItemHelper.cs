using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Serenity.ComponentModel;

namespace Serenity
{
    public class PropertyItemHelper
    {
        public static List<PropertyItem> GetPropertyItemsFor(Type type)
        {
            if (type == null)
                throw new Exception("type");

            var list = new List<PropertyItem>();


            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyItem pi = new PropertyItem();

                if (member.MemberType != MemberTypes.Property &&
                    member.MemberType != MemberTypes.Field)
                    continue;

                var hiddenAttribute = member.GetCustomAttributes(typeof(HiddenAttribute), false);
                if (hiddenAttribute.Length > 0)
                    continue;

                var displayNameAttribute = member.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (displayNameAttribute.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla başlık belirlenmiş!", type.Name, pi.Name));

                var hintAttribute = member.GetCustomAttributes(typeof(HintAttribute), false);
                if (hintAttribute.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla ipucu belirlenmiş!", type.Name, pi.Name));

                var placeholderAttribute = member.GetCustomAttributes(typeof(PlaceholderAttribute), false);
                if (placeholderAttribute.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla placeholder belirlenmiş!", type.Name, pi.Name));

                Type memberType = member.MemberType == MemberTypes.Property ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;

                if (member.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)member;
                    if (p.ScriptFieldName == null) // şu an sadece serializable destekliyoruz!
                        continue;

                    pi.Name = p.ScriptFieldName;
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    var f = (FieldInfo)member;
                    pi.Name = f.ScriptName;
                }
                else
                    continue;

                var categoryAttribute = member.GetCustomAttributes(typeof(CategoryAttribute), false);
                if (categoryAttribute.Length == 1)
                    pi.Category = ((CategoryAttribute)categoryAttribute[0]).Category;
                else if (categoryAttribute.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla kategori belirlenmiş!", type.Name, pi.Name));

                var cssClassAttr = member.GetCustomAttributes(typeof(CssClassAttribute), false);
                if (cssClassAttr.Length == 1)
                    pi.CssClass = ((CssClassAttribute)cssClassAttr[0]).CssClass;
                else if (cssClassAttr.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla css class belirlenmiş!", type.Name, pi.Name));

                if (member.GetCustomAttributes(typeof(OneWayAttribute), false).Length > 0)
                    pi.OneWay = true;

                if (member.GetCustomAttributes(typeof(ReadOnlyAttribute), false).Length > 0)
                    pi.ReadOnly = true;

                if (displayNameAttribute.Length > 0)
                    pi.Title = ((DisplayNameAttribute)displayNameAttribute[0]).DisplayName;

                if (hintAttribute.Length > 0)
                    pi.Hint = ((HintAttribute)hintAttribute[0]).Hint;

                if (placeholderAttribute.Length > 0)
                    pi.Placeholder = ((PlaceholderAttribute)placeholderAttribute[0]).Value;

                if (pi.Title == null)
                    pi.Title = pi.Name;

                var defaultValueAttribute = member.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (defaultValueAttribute.Length == 1)
                    pi.DefaultValue = ((DefaultValueAttribute)defaultValueAttribute[0]).Value;

                var insertableAttribute = member.GetCustomAttributes(typeof(InsertableAttribute), false);
                if (insertableAttribute.Length > 0)
                    pi.Insertable = insertableAttribute[0].As<InsertableAttribute>().Value;
                else
                    pi.Insertable = true;

                var updatableAttribute = member.GetCustomAttributes(typeof(UpdatableAttribute), false);
                if (updatableAttribute.Length > 0)
                    pi.Updatable = updatableAttribute[0].As<UpdatableAttribute>().Value;
                else
                    pi.Updatable = true;

                var typeAttrArray = member.GetCustomAttributes(typeof(EditorTypeAttribute), false);
                if (typeAttrArray.Length > 1)
                    throw new Exception(String.Format("{0}.{1} için birden fazla editör tipi belirlenmiş!", type.Name, pi.Name));

                Type nullableType = memberType;//Nullable.GetUnderlyingType(memberType);
                Type enumType = null;
                if (memberType.IsEnum)
                    enumType = memberType;
                else if (nullableType != null && nullableType.IsEnum)
                    enumType = nullableType;

                if (typeAttrArray.Length == 0)
                {
                    if (enumType != null)
                        pi.EditorType = "Select";
                    else if (memberType == typeof(JsDate))
                        pi.EditorType = "Date";
                    else if (memberType == typeof(Boolean))
                        pi.EditorType = "Boolean";
                    else if (memberType == typeof(Decimal) || memberType == typeof(Double))
                        pi.EditorType = "Decimal";
                    else if (memberType == typeof(Int32))
                        pi.EditorType = "Integer";
                    else
                        pi.EditorType = "String";
                }
                else
                {
                    var et = ((EditorTypeAttribute)typeAttrArray[0]);
                    pi.EditorType = et.EditorType;
                    et.SetParams(pi.EditorParams);
                }

                var reqAttr = member.GetCustomAttributes(typeof(RequiredAttribute), true);
                if (reqAttr.Length > 0)
                    pi.Required = reqAttr[0].As<RequiredAttribute>().IsRequired;

                var maxLengthAttr = member.GetCustomAttributes(typeof(MaxLengthAttribute), false);
                if (maxLengthAttr.Length > 0)
                {
                    pi.MaxLength = maxLengthAttr.As<MaxLengthAttribute>().MaxLength;
                    pi.EditorParams["maxLength"] = pi.MaxLength;
                }

                foreach (EditorOptionAttribute param in member.GetCustomAttributes(typeof(EditorOptionAttribute), false))
                {
                    var key = param.Key;
                    if (key != null &&
                        key.Length >= 1)
                        key = key.Substring(0, 1).ToLower() + key.Substring(1);

                    pi.EditorParams[key] = param.Value;
                }

                list.Add(pi);
            }

            return list;
        }
    }

    [Obsolete("Use PropertyItemHelper")]
    public class PropertyEditorHelper : PropertyItemHelper
    {
    }
}