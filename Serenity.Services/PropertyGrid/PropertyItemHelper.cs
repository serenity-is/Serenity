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

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if (SkipMember(member))
                    continue;

                var source = new MemberPropertySource(member, basedOnRow);

                PropertyItem pi = new PropertyItem();
                pi.Name = member.Name;

                foreach (var processor in processors)
                    processor.Process(source, pi);

                list.Add(pi);
            }

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

        private static bool SkipMember(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Property &&
                member.MemberType != MemberTypes.Field)
                return true;

            if (member.GetCustomAttribute<IgnoreAttribute>(false) != null)
                return true;

            return false;
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