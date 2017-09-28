using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

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

            var allPropertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            // Checks to see if there are any MetadataTypeAttributes. If so, override base class properties with MetaData class
            var metadataType = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (metadataType != null)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForType(null, metadataType.MetadataClassType);
                var metaDataProperties = metaData.ModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var metaDataProperty in metaDataProperties)
                {
                    //If it exists in the base generated model, we remove it. In favor of metaDataPropInfo
                    var match = allPropertyInfos.FirstOrDefault(x => x.Name == metaDataProperty.Name);
                    if (match != null)
                    {
                        allPropertyInfos.Remove(match);
                        allPropertyInfos.Add(metaDataProperty);
                    }
                }
            }

            foreach (var property in allPropertyInfos
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