using Serenity.ComponentModel;
using Serenity.Data;
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

            var basedOnRow = GetBasedOnRow(type);
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

        private static Row GetBasedOnRow(Type type)
        {
            var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
            if (basedOnRowAttr == null)
                return null;

            var basedOnRowType = basedOnRowAttr.RowType;
            if (!basedOnRowType.IsSubclassOf(typeof(Row)))
                throw new InvalidOperationException(String.Format(
                    "BasedOnRowAttribute value ({0}) must be set to a subclass of {0}!", 
                        type.FullName, typeof(Row).FullName));

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