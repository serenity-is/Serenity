#if TODO
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
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
            public Func<IRow> RowFactory;
            public Field ThisKeyField;
            public Field OtherKeyField;
            public Field FilterField;
            public object FilterValue;
            public List<Tuple<Field, Field>> Mappings;
            public Field PresenceField;
            public object PresenceValue;
        }

        private List<RelationInfo> infoList;

        public bool ActivateFor(IRow row)
        {
            var attrs = row.GetType().GetCustomAttributes<UpdatableExtensionAttribute>();

            if (attrs == null || !attrs.Any())
                return false;

            var sourceByExpression = row.GetFields().ToLookup(x =>
                BracketLocator.ReplaceBrackets(x.Expression.TrimToEmpty(), BracketRemoverDialect.Instance));

            this.infoList = attrs.Select(attr =>
            {
                var info = new RelationInfo();
                info.Attr = attr;

                var rowType = attr.RowType;
                if (rowType.IsAbstract ||
                    !typeof(IRow).IsAssignableFrom(rowType))
                {
                    throw new ArgumentException(string.Format(
                        "Row type '{1}' has an ExtensionRelation attribute " +
                        "but its specified extension row type '{0}' is not a valid row class!",
                            rowType.FullName,
                            row.GetType().FullName));
                }

                info.RowFactory = FastReflection.DelegateForConstructor<IRow>(rowType);

                var thisKey = attr.ThisKey;
                if (string.IsNullOrEmpty(thisKey))
                {
                    if (!(row is IIdRow))
                    {
                        throw new ArgumentException(string.Format(
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
                        throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
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
                        throw new ArgumentException(string.Format(
                            "Row type '{1}' has an ExtensionRelation attribute " +
                            "but its OtherKey is not specified!",
                                row.GetType().FullName));
                }
                else
                {
                    info.OtherKeyField = ext.FindFieldByPropertyName(attr.OtherKey) ?? ext.FindField(attr.OtherKey);
                    if (ReferenceEquals(info.OtherKeyField, null))
                        throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                            "This field is specified for an ExtensionRelation attribute on '{2}'",
                            attr.OtherKey,
                            ext.GetType().FullName,
                            row.GetType().FullName));
                }

                if (!string.IsNullOrEmpty(attr.FilterField))
                {
                    info.FilterField = ext.FindFieldByPropertyName(attr.FilterField) ?? ext.FindField(attr.FilterField);
                    if (ReferenceEquals(info.FilterField, null))
                        throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
                            "This field is specified as FilterField for an ExtensionRelation attribute on '{2}'",
                            attr.OtherKey,
                            ext.GetType().FullName,
                            row.GetType().FullName));

                    info.FilterValue = info.FilterField.ConvertValue(attr.FilterValue, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(attr.PresenceField))
                {
                    info.PresenceField = row.FindFieldByPropertyName(attr.PresenceField) ?? row.FindField(attr.PresenceField);
                    if (ReferenceEquals(info.PresenceField, null))
                        throw new ArgumentException(string.Format("Field '{0}' doesn't exist in row of type '{1}'." +
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
                    if (ReferenceEquals(info.OtherKeyField, field))
                        continue;

                    if (ReferenceEquals(info.FilterField, field))
                        continue;

                    var expression = field.Expression.TrimToEmpty();

                    if (string.IsNullOrEmpty(expression))
                        continue;

                    expression = mapExpression(expression);
                    expression = BracketLocator.ReplaceBrackets(expression, 
                        BracketRemoverDialect.Instance);

                    var match = sourceByExpression[expression].FirstOrDefault();
                    if (ReferenceEquals(null, match))
                        continue;

                    if (match.IsTableField())
                        continue;

                    if (ReferenceEquals(info.ThisKeyField, match))
                        continue;

                    if (field.GetType() != match.GetType())
                        throw new ArgumentException(string.Format(
                            "Row type '{0}' has an ExtensionRelation attribute to '{1}'." +
                            "Their '{2}' and '{3}' fields are matched but they have different types ({4} and {5})!",
                                row.GetType().FullName,
                                ext.GetType().FullName,
                                field.PropertyName ?? field.Name,
                                match.PropertyName ?? match.Name,
                                field.GetType().Name,
                                match.GetType().Name));

                    info.Mappings.Add(new Tuple<Field, Field>(match, field));
                }

                if (info.Mappings.Count == 0)
                    throw new ArgumentException(string.Format(
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
                var mappings = info.Mappings.Where(x => handler.Row.IsAssigned(x.Item1)).ToList();

                if (!mappings.Any())
                    continue;

                handler.StateBag["UpdatableExtensionBehavior_Assignments_" + info.Attr.Alias] =
                    mappings;
            }
        }

        private object GetExistingID(IDbConnection connection, RelationInfo info,
            object thisKey)
        {
            var criteria = new Criteria(info.OtherKeyField.PropertyName ?? info.OtherKeyField.Name) ==
                new ValueCriteria(thisKey);

            if (!ReferenceEquals(null, info.FilterField))
            {
                var flt = new Criteria(info.FilterField.PropertyName ?? info.FilterField.Name);
                if (info.FilterValue == null)
                    criteria &= flt.IsNull();
                else
                    criteria &= flt == new ValueCriteria(info.FilterValue);
            }

            var listHandler = DefaultHandlerFactory.ListHandlerFor(info.Attr.RowType);
            var listRequest = DefaultHandlerFactory.ListRequestFor(info.Attr.RowType);
            listRequest.ColumnSelection = ColumnSelection.KeyOnly;
            listRequest.Criteria = criteria;

            var existing = listHandler.Process(connection, listRequest).Entities;

            if (existing.Count > 1)
                throw new Exception(string.Format("Found multiple extension rows for UpdatableExtension '{0}'", 
                    info.Attr.Alias));

            if (existing.Count == 0)
                return null;

            return ((Field)((IIdRow)existing[0]).IdField).AsObject((IRow)existing[0]);
        }

        private bool CheckPresenceValue(RelationInfo info, IRow row)
        {
            if (!ReferenceEquals(null, info.PresenceField))
            {
                if (!(info.PresenceField is BooleanField) &&
                    info.PresenceValue is Boolean)
                {
                    if (info.PresenceField.IsNull(row) == (bool)info.PresenceValue)
                        return false;
                }
                else
                {
                    var newRow = row.CreateNew();
                    info.PresenceField.AsObject(newRow, info.PresenceField.ConvertValue(
                        info.PresenceValue, CultureInfo.InvariantCulture));
                    if (info.PresenceField.IndexCompare(row, newRow) != 0)
                        return false;
                }
            }

            return true;
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

                object oldID = GetExistingID(handler.Connection, info, thisKey);
                if (oldID == null && !CheckPresenceValue(info, handler.Row))
                    continue;

                var extension = info.RowFactory();

                if (oldID != null)
                    ((Field)((IIdRow)extension).IdField).AsObject(extension, oldID);

                info.OtherKeyField.AsObject(extension, thisKey);
                if (!ReferenceEquals(null, info.FilterField))
                    info.FilterField.AsObject(extension, info.FilterValue);

                var request = DefaultHandlerFactory.SaveRequestFor(info.Attr.RowType);
                request.Entity = extension;
                request.EntityId = oldID;

                foreach (var mapping in mappings)
                    mapping.Item2.AsObject(extension, mapping.Item1.AsObject(handler.Row));

                DefaultHandlerFactory.SaveHandlerFor(info.Attr.RowType)
                    .Process(handler.UnitOfWork, request, oldID == null ? SaveRequestType.Create : SaveRequestType.Update);
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

                var oldID = GetExistingID(handler.Connection, info, thisKey);
                if (oldID == null)
                    continue;

                var deleteHandler = DefaultHandlerFactory.DeleteHandlerFor(info.Attr.RowType);
                var deleteRequest = DefaultHandlerFactory.DeleteRequestFor(info.Attr.RowType);
                deleteRequest.EntityId = oldID;
                deleteHandler.Process(handler.UnitOfWork, deleteRequest);
            }
        }
    }
}
#endif