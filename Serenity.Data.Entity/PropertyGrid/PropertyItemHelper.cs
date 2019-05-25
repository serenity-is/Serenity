using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public partial class PropertyItemHelper
    {
        public static List<PropertyItem> GetPropertyItemsFor(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var list = new List<PropertyItem>();

            bool checkNames;
            var basedOnRow = GetBasedOnRow(type, out checkNames);
            var processors = ProcessorTypes.Select(x => (IPropertyProcessor)Activator.CreateInstance(x))
                .OrderBy(x => x.Priority).ToList();

            foreach (var processor in processors)
            {
                processor.Items = list;
                processor.Type = type;
                processor.BasedOnRow = basedOnRow;
                processor.Initialize();
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(x => x.MetadataToken))
            {
                if (property.GetCustomAttribute<IgnoreAttribute>(false) != null)
                    continue;

                var source = new PropertyInfoSource(property, basedOnRow);
                if (checkNames &&
                    property.GetCustomAttribute<NotMappedAttribute>() == null &&
                    property.GetCustomAttribute<IgnoreNameAttribute>() == null)
                {
                    if (ReferenceEquals(null, source.BasedOnField))
                    {
                        throw new Exception(String.Format(
                            "{0} has a [BasedOnRow(typeof({2}), CheckNames = true)] attribute but its '{1}' property " +
                            "doesn't have a matching field with same property / field name in the row.\n\n" +
                            "Please check if property is named correctly.\n\n" +
                            "To remove this validation you may set CheckNames to false on [BasedOnRow] attribute.\n\n" +
                            "To disable check for this specific property add a [NotMapped] attribute to the property itself.",
                            type.FullName, property.Name, basedOnRow.GetType().FullName));
                    }
                    else if (
                        (!source.BasedOnField.PropertyName.IsEmptyOrNull() &&
                         source.BasedOnField.PropertyName != property.Name) ||
                        (source.BasedOnField.PropertyName.IsEmptyOrNull() &&
                         source.BasedOnField.Name != property.Name))
                    {
                        throw new Exception(String.Format(
                                "{0} has a [BasedOnRow(typeof({3}), CheckNames = true)] attribute but its '{1}' property " +
                                "doesn't match the property/field name '{2}' in the row.\n\n" +
                                "Property names must match case sensitively. Please change property name to '{2}'.\n\n" +
                                "To remove this validation you may set CheckNames to false on [BasedOnRow] attribute.\n\n" +
                                "To disable check for this specific property add a [NotMapped] attribute to the property itself.",
                                type.FullName, property.Name, source.BasedOnField.PropertyName.TrimToNull() ?? 
                                    source.BasedOnField.Name, basedOnRow.GetType().FullName));
                    }
                }

                PropertyItem pi = new PropertyItem();
                pi.Name = property.Name;

                foreach (var processor in processors)
                    processor.Process(source, pi);

                list.Add(pi);
            }

            foreach (var processor in processors)
                processor.PostProcess();

            return list;
        }

        private static Row GetBasedOnRow(Type type, out bool checkPropertyNames)
        {
            checkPropertyNames = false;
            var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
            if (basedOnRowAttr == null)
                return null;

            var basedOnRowType = basedOnRowAttr.RowType;
            if (!basedOnRowType.IsSubclassOf(typeof(Row)))
                throw new InvalidOperationException(String.Format(
                    "BasedOnRowAttribute value ({0}) must be set to a subclass of {1}!", 
                        type.FullName, typeof(Row).FullName));

            checkPropertyNames = basedOnRowAttr.CheckNames;
            return (Row)Activator.CreateInstance(basedOnRowType);
        }

        private static Type[] processorTypes;

        private static Type[] ProcessorTypes
        {
            get
            {
                if (processorTypes != null)
                    return processorTypes;

                processorTypes = ExtensibilityHelper.GetTypesWithInterface(
                        typeof(IPropertyProcessor))
                    .Where(x => !x.IsAbstract).ToArray();

                return processorTypes;
            }
        }
    }
}