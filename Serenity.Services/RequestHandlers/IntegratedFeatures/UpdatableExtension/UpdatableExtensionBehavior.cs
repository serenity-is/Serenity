using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class UpdatableExtensionBehavior : BaseSaveDeleteBehavior, IImplicitBehavior
    {
        private class RelationInfo
        {
            public UpdatableExtensionAttribute Attr;
            public Func<Row> RowFactory;
            public Func<IListRequestProcessor> ListHandlerFactory;
            public Func<ISaveRequestProcessor> SaveHandlerFactory;
            public Func<IDeleteRequestProcessor> DeleteHandlerFactory;
            public Func<ISaveRequest> SaveRequestFactory;
            public Field ThisKeyField;
            public Field OtherKeyField;
            public List<Tuple<Field, Field>> Mappings;
            public Field PresenceField;
            public object PresenceValue;
        }

        private List<RelationInfo> infoList;

        public bool ActivateFor(Row row)
        {
            var attrs = row.GetType().GetCustomAttributes<UpdatableExtensionAttribute>();

            if (attrs == null || !attrs.Any())
                return false;

            var sourceByExpression = row.GetFields().ToLookup(x => x.Expression);

            this.infoList = attrs.Select(attr =>
            {
                var info = new RelationInfo();
                info.Attr = attr;

                var rowType = attr.RowType;
                if (rowType.IsAbstract ||
                    !typeof(Row).IsAssignableFrom(rowType))
                {
                    throw new ArgumentException(String.Format(
                        "Row type '{1}' has an ExtensionRelation attribute " +
                        "but its specified extension row type '{0}' is not a valid row class!",
                            rowType.FullName,
                            row.GetType().FullName));
                }

                info.RowFactory = FastReflection.DelegateForConstructor<Row>(rowType);

                info.ListHandlerFactory = FastReflection.DelegateForConstructor<IListRequestProcessor>(
                    typeof(ListRequestHandler<>).MakeGenericType(rowType));

                info.SaveHandlerFactory = FastReflection.DelegateForConstructor<ISaveRequestProcessor>(
                    typeof(SaveRequestHandler<>).MakeGenericType(rowType));

                info.SaveRequestFactory = FastReflection.DelegateForConstructor<ISaveRequest>(
                    typeof(SaveRequest<>).MakeGenericType(rowType));

                info.DeleteHandlerFactory = FastReflection.DelegateForConstructor<IDeleteRequestProcessor>(
                    typeof(DeleteRequestHandler<>).MakeGenericType(rowType));

                var thisKey = attr.ThisKey;
                if (string.IsNullOrEmpty(thisKey))
                {
                    if (!(row is IIdRow))
                    {
                        throw new ArgumentException(String.Format(
                            "Row type '{0}' has an ExtensionRelation attribute " +
                            "but its ThisKey is not specified!",
                                row.GetType().FullName));
                    }

                    info.ThisKeyField = (Field)(((IIdRow)row).IdField);
                }
                else
                {
                    info.ThisKeyField = row.FindFieldByPropertyName(attr.ThisKey) ?? row.FindField(attr.ThisKey);
                    if (ReferenceEquals(info.ThisKeyField, null))
                        throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                            "This field is specified for an ExtensionRelation attribute",
                            attr.ThisKey,
                            row.GetType().FullName));
                }

                var ext = info.RowFactory();

                var otherKey = attr.OtherKey;
                if (string.IsNullOrEmpty(otherKey))
                {
                    info.OtherKeyField = ext.FindField(info.ThisKeyField.Name);

                    if (ReferenceEquals(info.OtherKeyField, null) && ext is IIdRow)
                        info.OtherKeyField = (Field)(((IIdRow)row).IdField);

                    if (ReferenceEquals(info.OtherKeyField, null))
                        throw new ArgumentException(String.Format(
                            "Row type '{1}' has an ExtensionRelation attribute " +
                            "but its OtherKey is not specified!",
                                row.GetType().FullName));
                }
                else
                {
                    info.OtherKeyField = ext.FindFieldByPropertyName(attr.OtherKey) ?? ext.FindField(attr.OtherKey);
                    if (ReferenceEquals(info.OtherKeyField, null))
                        throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                            "This field is specified for an ExtensionRelation attribute on '{2}'",
                            attr.OtherKey,
                            ext.GetType().FullName,
                            row.GetType().FullName));
                }

                if (!string.IsNullOrEmpty(attr.PresenceField))
                {
                    info.PresenceField = ext.FindFieldByPropertyName(attr.PresenceField) ?? ext.FindField(attr.PresenceField);
                    if (ReferenceEquals(info.PresenceField, null))
                        throw new ArgumentException(String.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                            "This field is specified as PresenceField as an ExtensionRelation attribute.",
                            attr.PresenceField,
                            row.GetType().FullName));

                    info.PresenceValue = attr.PresenceValue;
                }

                var extFields = ext.GetFields();
                var alias = attr.Alias;
                var aliasPrefix = attr.Alias + "_";

                var joinByKey = new HashSet<string>(extFields.Joins.Keys, StringComparer.OrdinalIgnoreCase);

                Func<string, string> mapAlias = x =>
                {
                    if (x == "t0" || x == "T0")
                        return alias;

                    if (!joinByKey.Contains(x))
                        return x;

                    return aliasPrefix + x;
                };

                Func<string, string> mapExpression = x =>
                {
                    if (x == null)
                        return null;

                    return JoinAliasLocator.ReplaceAliases(x, mapAlias);
                };

                info.Mappings = new List<Tuple<Field, Field>>();
                foreach (var field in extFields)
                {
                    if (string.IsNullOrEmpty(field.Expression))
                        continue;

                    if (ReferenceEquals(info.OtherKeyField, field))
                        continue;

                    var expression = mapExpression(field.Expression);
                    var match = sourceByExpression[expression].FirstOrDefault();
                    if (ReferenceEquals(null, match))
                        continue;

                    if (match.IsTableField())
                        continue;

                    if (ReferenceEquals(info.ThisKeyField, match))
                        continue;

                    if (field.GetType() != match.GetType())
                        throw new ArgumentException(String.Format(
                            "Row type '{0}' has an ExtensionRelation attribute to '{1}'." +
                            "Their '{2}' and '{3}' fields are matched but they have different types ({0} and {1})!",
                                row.GetType().FullName,
                                ext.GetType().FullName,
                                field.GetType().Name,
                                match.GetType().Name));

                    info.Mappings.Add(new Tuple<Field, Field>(match, field));
                }

                if (info.Mappings.Count == 0)
                    throw new ArgumentException(String.Format(
                        "Row type '{0}' has an ExtensionRelation attribute " +
                        "but no view fields could be matched to extension row '{1}'!",
                            row.GetType().FullName,
                            ext.GetType().FullName));

                return info;
            }).ToList();

            return true;
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            foreach (var info in infoList)
            {
                handler.StateBag["UpdatableExtensionBehavior_Assignments_" + info.Attr.Alias] =
                    info.Mappings.Where(x => handler.Row.IsAssigned(x.Item1)).ToList();
            }
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            foreach (var info in infoList)
            {
                object mappingsObj;
                if (!handler.StateBag.TryGetValue("UpdatableExtensionBehavior_Assignments_" + info.Attr.Alias, out mappingsObj))
                    continue;

                var mappings = (IEnumerable<Tuple<Field, Field>>)mappingsObj;
                if (mappings == null || !mappings.Any())
                    continue;

                var thisKey = info.ThisKeyField.AsObject(handler.Row);
                if (ReferenceEquals(null, thisKey))
                    continue;

                var existing = info.ListHandlerFactory().Process(handler.Connection, new ListRequest
                {
                    ColumnSelection = ColumnSelection.KeyOnly,
                    Criteria = new Criteria(info.OtherKeyField.PropertyName ?? info.OtherKeyField.Name) == new ValueCriteria(thisKey)
                }).Entities;

                object oldID;
                if (existing.Count == 0)
                {
                    if (!ReferenceEquals(null, info.PresenceField))
                    {
                        if (!(info.PresenceField is BooleanField) &&
                            info.PresenceValue is Boolean)
                        {
                            if (info.PresenceField.IsNull(handler.Row) == (bool)info.PresenceValue)
                                continue;
                        }
                        else
                        {
                            var newRow = handler.Row.CreateNew();
                            info.PresenceField.AsObject(newRow, info.PresenceField.ConvertValue(info.PresenceValue, CultureInfo.InvariantCulture));
                            if (info.PresenceField.IndexCompare(handler.Row, newRow) != 0)
                                continue;
                        }
                    }

                    oldID = null;
                }
                else if (existing.Count > 1)
                {
                    throw new Exception(String.Format("Found multiple extension rows for UpdatableExtension '{0}'", info.Attr.Alias));
                }
                else
                {
                    oldID = ((Field)((IIdRow)existing[0]).IdField).AsObject((Row)existing[0]);
                }

                var extension = info.RowFactory();

                if (oldID != null)
                    ((Field)((IIdRow)extension).IdField).AsObject(extension, oldID);

                info.OtherKeyField.AsObject(extension, thisKey);

                var request = info.SaveRequestFactory();
                request.Entity = extension;
                request.EntityId = oldID;

                foreach (var mapping in mappings)
                    mapping.Item2.AsObject(extension, mapping.Item1.AsObject(handler.Row));

                info.SaveHandlerFactory().Process(handler.UnitOfWork, request, oldID == null ? SaveRequestType.Create : SaveRequestType.Update);
            }
        }

        public override void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            foreach (var info in infoList)
            {
                if (!info.Attr.CascadeDelete)
                    continue;

                var thisKey = info.ThisKeyField.AsObject(handler.Row);
                if (ReferenceEquals(null, thisKey))
                    continue;

                var existing = info.ListHandlerFactory().Process(handler.Connection, new ListRequest
                {
                    ColumnSelection = ColumnSelection.KeyOnly,
                    Criteria = new Criteria(info.OtherKeyField.PropertyName ?? info.OtherKeyField.Name) == new ValueCriteria(thisKey)
                }).Entities;

                if (existing.Count == 0)
                    continue;

                if (existing.Count > 1)
                    throw new Exception(String.Format("Found multiple extension rows for UpdatableExtension '{0}'", info.Attr.Alias));

                var oldID = ((Field)((IIdRow)existing[0]).IdField).AsObject((Row)existing[0]);
                if (oldID == null)
                    continue;

                info.DeleteHandlerFactory().Process(handler.UnitOfWork, new Services.DeleteRequest
                {
                    EntityId = oldID
                });
            }
        }
    }
}