using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    ///   A static class with helper functions to update display orders of all records or
    ///   groups of records in a table.</summary>
    public static class DisplayOrderHelper
    {
        /// <summary>
        ///   Gets the next display order value for a table or a group of records.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="tableName">
        ///   Table name (required).</param>
        /// <param name="orderField">
        ///   Display order field meta (required).</param>
        /// <param name="filter">
        ///   Filter for records (can be null).</param>
        /// <returns>
        ///   One more of maximum display order values of records in the group. 
        ///   If none, 1.</returns>
        public static int GetNextDisplayOrderValue(IDbConnection connection, string tableName, 
            Field orderField, BaseCriteria filter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");
            if (orderField == null)
                throw new ArgumentNullException("orderField");

            using (IDataReader reader = SqlHelper.ExecuteReader(connection,
                new SqlQuery().Select(
                    Sql.Max(orderField.Name))
                .FromAs(
                    tableName, Alias.T0)
                .Where(
                    filter)))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                    return Convert.ToInt32(reader.GetValue(0)) + 1;
                else
                    return 1;
            }
        }

        /// <summary>
        ///   Gets the next display order value for a table or a group of records.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="row">
        ///   Row with a display order field (required).</param>
        /// <param name="filter">
        ///   Filter for records (can be null).</param>
        /// <returns>
        ///   One more of maximum display order values of records in the group. 
        ///   If none, 1.</returns>
        public static int GetNextDisplayOrderValue(IDbConnection connection, IDisplayOrderRow row, BaseCriteria filter)
        {
            return GetNextDisplayOrderValue(connection, ((Row)row).Table, row.DisplayOrderField, filter);
        }

        /// <summary>
        ///   Gets the next display order value for a table or a group of records.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="row">
        ///   Row with a display order field (required).</param>
        /// <returns>
        ///   One more of maximum display order values of records in the table. 
        ///   If none, 1.</returns>
        public static int GetNextDisplayOrderValue(IDbConnection connection, IDisplayOrderRow row)
        {
            return GetNextDisplayOrderValue(connection, ((Row)row).Table, row.DisplayOrderField, null);
        }


        /// <summary>
        ///   Gets the stored display order value for a record.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="tableName">
        ///   Table name (required).</param>
        /// <param name="keyField">
        ///   ID meta field that will be used to locate the record (required).</param>
        /// <param name="orderField">
        ///   Display order field meta (required).</param>
        /// <param name="recordID">
        ///   Key value of the field.</param>
        /// <returns>
        ///   Display order field value of the record. If not found, <see cref="Null.Int64"/></returns>
        public static Int32? GetDisplayOrderValue(IDbConnection connection, string tableName,
            Field keyField, Field orderField, Int64 recordID)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");
            if (orderField == null)
                throw new ArgumentNullException("orderField");

            using (IDataReader reader = SqlHelper.ExecuteReader(connection,
                new SqlQuery()
                    .Select(Alias.T0[orderField.Name])
                    .FromAs(tableName, Alias.T0)
                    .Where(new Criteria(keyField) == recordID)))
            {
                if (reader.Read())
                    return Convert.ToInt32(reader.GetValue(0));
                else
                    return null;
            }
        }

        /// <summary>
        ///   Gets the actual display order of a record in a table or group of records.
        ///   The value in the display order field, may not match the actual display order, because of
        ///   deletions, changes etc. By scanning all the records in the group, this method returns
        ///   the actual display order for a record.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="tableName">
        ///   Tablename (required).</param>
        /// <param name="keyField">
        ///   ID field that will be used to locate the record (required).</param>
        /// <param name="baseKeyField">
        ///   Staging base ID field (optional).</param>
        /// <param name="orderField">
        ///   Display order field meta (required).</param>
        /// <param name="filter">
        ///   Filter that will determine the record group (can be null).</param>
        /// <param name="recordID">
        ///   ID value of the field.</param>
        /// <param name="descendingKeyOrder">
        ///   Will records with same display order values be sorted in ascending or descending ID order?
        ///   For example, if records with ID's 1, 2, 3 has display order value of "0", their actual display
        ///   orders are 1, 2 and 3. If this parameter is set to true (descending), their display orders will
        ///   become 3, 2, 1. This parameter controls if records that are added recently and has no display
        ///   order value assigned (or 0) be shown at start or at the end.</param>
        /// <returns>
        ///   Records actual display order value. If not found, <see cref="Null.Int32"/></returns>
        public static Int32? GetTrueDisplayOrder(IDbConnection connection, string tableName,
            Field keyField, Field baseKeyField, Field orderField, 
            Criteria filter, Int64 recordID, bool descendingKeyOrder)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");
            if (keyField == null)
                throw new ArgumentNullException("keyField");
            if (orderField == null)
                throw new ArgumentNullException("orderField");

            int order = 0;

            SqlQuery query = new SqlQuery()
                .Select(Alias.T0[keyField])
                .FromAs(tableName, Alias.T0)
                .Where(filter)
                .OrderBy(
                    orderField);

            if (baseKeyField != null)
                query.Where(new Criteria(baseKeyField).IsNull());

            if (descendingKeyOrder)
                query.OrderByDescending(keyField.Name);
            else
                query.OrderBy(keyField);

            using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
            {
                while (reader.Read())
                {
                    order++;
                    if (Convert.ToInt32(reader.GetValue(0)) == recordID)
                        return order;
                }
            }

            return null;
        }

        /// <summary>
        ///   Sets a records display order to to requested value, and also renumbers other records
        ///   in the group as required.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="tableName">
        ///   Tablename (required).</param>
        /// <param name="keyField">
        ///   ID field meta that will be used to locate the record (required).</param>
        /// <param name="baseKeyField">
        ///   Staging base ID field (optional), whose display order will also be updated.</param>
        /// <param name="orderField">
        ///   Display order field meta.</param>
        /// <param name="filter">
        ///   Filter that will determine the record group (can be null).</param>
        /// <param name="recordID">
        ///   ID value of the record.</param>
        /// <param name="newDisplayOrder">
        ///   New display order of the record.</param>
        /// <param name="descendingKeyOrder">
        ///   Will records with same display order values be sorted in ascending or descending ID order?
        ///   For example, if records with ID's 1, 2, 3 has display order value of "0", their actual display
        ///   orders are 1, 2 and 3. If this parameter is set to true (descending), their display orders will
        ///   become 3, 2, 1. This parameter controls if records that are added recently and has no display
        ///   order value assigned (or 0) be shown at start or at the end.</param>
        /// <returns>
        ///   If any of the display order values is changed true.</returns>
        public static bool FixRecordOrdering(IDbConnection connection, string tableName,
            Field keyField, Field baseKeyField, Field orderField, 
            BaseCriteria filter, Int64 recordID, int newDisplayOrder, bool descendingKeyOrder,
            bool hasUniqueConstraint = false)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (tableName == null || tableName.Length == 0)
                throw new ArgumentNullException("tableName");
            if (keyField == null)
                throw new ArgumentNullException("keyField");
            if (orderField == null)
                throw new ArgumentNullException("orderField");

            // last assigned display order value
            int order = 0;

            // a list that will contain an element for each record, and hold old and new display 
            // order values of records
            List<OrderRecord> orderRecords = new List<OrderRecord>();

            // link to the order entry for record whose display order value is asked to be changed
            OrderRecord changing = null;

            // query to fetch id and display order values of the records in the group
            SqlQuery query = new SqlQuery()
                .Select(
                    keyField,
                    orderField)
                .FromAs(
                    tableName, Alias.T0)
                .Where(
                    filter)
                .OrderBy(
                    orderField);

            if (baseKeyField != null)
                query.Where(new Criteria(baseKeyField).IsNull());

            // determine display order for records with same display order values 
            // based on ID ordering set
            if (descendingKeyOrder)
                query.OrderByDescending(keyField.Name);
            else
                query.OrderBy(keyField.Name);

            // read all existing records
            using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
            {
                while (reader.Read())
                {
                    // each records actual display order value is one more than previous one
                    order++;
                    // create an entry to hold current and new display order value of the record
                    OrderRecord r = new OrderRecord();
                    // record ID
                    r.recordID = Convert.ToInt64(reader.GetValue(0));
                    // old display order field value (not the actual display order!)
                    r.oldOrder = Convert.ToInt32(reader.GetValue(1));
                    // new display order value (actual one to be set)
                    r.newOrder = order;
                    
                    orderRecords.Add(r);

                    // if this is the one that is requested to be changed, hold a link to its entry
                    if (recordID == r.recordID)
                        changing = r;
                }
            }

            // ensure that the new display order is within limits
            // if its lower than 1 or bigger than record count, fix it
            if (newDisplayOrder <= 0)
                newDisplayOrder = 1;
            else if (newDisplayOrder > order)
                newDisplayOrder = order;

            // if the record whose display order is to be changed can be found, and its display order value is different
            // than the one in database
            if (changing != null && changing.newOrder != newDisplayOrder)
            {
                // let's say record had a display order value of 6and now it will become 10, the records with actual
                // display orders of 7, 8, 9, 10 will become 6, 7, 8, 9 orders.
                //
                // WARNING: notice that array is 0 based, so record with actual display order of 7 is in the
                // 6th index in the array)
                for (int i = changing.newOrder; i < newDisplayOrder; i++)
                    orderRecords[i].newOrder = i;

                // if the records display order is to be changed from 9 to 5, the records with actual orders of 5, 6, 7, 8 
                // is going to be 6, 7, 8, 9 ordered.
                for (int i = newDisplayOrder - 1; i < changing.newOrder - 1; i++)
                    orderRecords[i].newOrder = i + 2;

                // as the records that will be changing are assigned new orders, we may assign new display order
                // directly.
                changing.newOrder = newDisplayOrder;
            }

            // from now on, apply the new display order values by preparing queries

            // StringBuilder that will contain query(s)
            StringBuilder queries = new StringBuilder();

            if (SqlSettings.CurrentDialect.NeedsExecuteBlockStatement())
            {
                queries.AppendLine("EXECUTE BLOCK AS");
                queries.AppendLine("BEGIN");
            }

            int updateCount = 0;

            Action<long, long> appendSingleUpdate = delegate(long id, long newOrder)
            {
                queries.AppendLine(String.Format(
                    "UPDATE {0} SET {1} = {2} WHERE {3} = {4};", tableName,
                    orderField.Name, newOrder, keyField.Name, id));
                updateCount++;
            };

            if (hasUniqueConstraint)
            {
                var byCurrentOrder = new Dictionary<Int64, OrderRecord>();
                foreach (var rec in orderRecords)
                    byCurrentOrder[rec.oldOrder] = rec;

                var list = new List<OrderRecord>();
                list.AddRange(orderRecords);
                list.Sort((x, y) => (x.newOrder - y.newOrder));

                foreach (var rec in list)
                {
                    if (rec.oldOrder != rec.newOrder)
                    {
                        byCurrentOrder.Remove(rec.oldOrder);

                        OrderRecord congestion;
                        if (byCurrentOrder.TryGetValue(rec.newOrder, out congestion))
                        {
                            var empty = list.Count * 2;
                            while (byCurrentOrder.ContainsKey(empty))
                                empty++;

                            congestion.oldOrder = empty;
                            appendSingleUpdate(congestion.recordID, empty);
                            byCurrentOrder[empty] = congestion;
                        }

                        appendSingleUpdate(rec.recordID, rec.newOrder);
                        byCurrentOrder[rec.newOrder] = rec;
                    }
                }
            }
            else
            {
                // StringBuilder that will contain IN(...) part of the latest query
                StringBuilder sb = new StringBuilder();

                // scan all display order changing records
                int start = 0;
                while (start < orderRecords.Count)
                {
                    OrderRecord rs = orderRecords[start];

                    // if this records display order is not changed, skip it
                    if (rs.oldOrder == rs.newOrder)
                    {
                        start++;
                        continue;
                    }

                    // find the difference between old and new display orders
                    int difference = rs.oldOrder - rs.newOrder;

                    // clear the IN(...) list
                    sb.Length = 0;

                    // add this records ID to the IN (...) part
                    sb.Append(rs.recordID);

                    // now we'll find all following records whose display orders are changed same amount 
                    // (difference between old and new is same), so we will update them with just one query
                    // like UPDATE ORDER = ORDER + 1 WHERE ID IN (X, Y, Z....).
                    int finish = start;

                    while (finish + 1 < orderRecords.Count)
                    {
                        // if we found more than 100 records whose display orders changed same amount, to 
                        // limit IN(...) part to overgrow, break searching and run the query. Collect the
                        // rest in another query. If query is too complex, might result in performance
                        // degration in SQL server
                        if (finish - start >= 100)
                            break;

                        OrderRecord rf = orderRecords[finish + 1];

                        // is this records display order value changed same amount
                        if (rf.oldOrder - rf.newOrder != difference)
                            break;

                        sb.Append(',');
                        sb.Append(rf.recordID);

                        finish++;
                    }

                    // if only one record in batch, no need to use IN clause or the filter.
                    if (start == finish)
                    {
                        queries.AppendLine(String.Format(
                            "UPDATE {0} SET {1} = {2} WHERE {3} = {4};", tableName,
                            orderField.Name, rs.newOrder, keyField.Name, rs.recordID));
                        updateCount++;
                    }
                    else
                    {
                        // batch update, use IN (...) and if exists group filter to be on safe side.
                        OrderRecord rf = orderRecords[finish];

                        queries.AppendLine(String.Format(
                            "UPDATE {0} SET {1} = {1} - ({2}) WHERE ({3}) AND ({4} IN ({5}));",
                            tableName,
                            orderField.Name,
                            rs.oldOrder - rs.newOrder,
                            Object.ReferenceEquals(filter, null) ? "1 = 1" : filter.ToString(), // FIX!!
                            keyField.Name,
                            sb.ToString()));
                        updateCount++;
                    }

                    start = finish + 1;
                }
            }

            if (queries.Length > 0 && updateCount > 0)
            {
                if (baseKeyField != null)
                {
                    queries.AppendLine(
                        new SqlUpdate(tableName).SetTo(orderField.Name, String.Format(
                            "(SELECT {1} FROM {0} T2 WHERE T2.{2} = {0}.{3})", 
                                tableName, orderField.Name, keyField.Name, baseKeyField.Name))
                        .Where(
                            filter)
                        .Where(
                            new Criteria(baseKeyField).IsNotNull())
                        .ToString());
                }

                if (SqlSettings.CurrentDialect.NeedsExecuteBlockStatement())
                    queries.AppendLine("END;");

                ParameterizedQuery parameterized = null;
                if (!Object.ReferenceEquals(filter, null))
                {
                    parameterized = new ParameterizedQuery();
                    filter.ToString(parameterized);
                }

                SqlHelper.ExecuteNonQuery(connection, queries.ToString(), Object.ReferenceEquals(filter, null) ? null : parameterized.Params);
                // one ore more records has changed display order values

                return true;
            }
            else
            {
                // nothing changed, all display orders are same
                return false;
            }
        }

        /// <summary>
        ///   Sets a records display order to to requested value, and also renumbers other records
        ///   in the group as required.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="row">
        ///   Row with a display order and ID field (should implement IDbIdRow interface).</param>
        /// <param name="filter">
        ///   Filter that will determine the record group (can be null).</param>
        /// <param name="recordID">
        ///   ID value of the record.</param>
        /// <param name="newDisplayOrder">
        ///   New display order of the record.</param>
        /// <param name="descendingKeyOrder">
        ///   Will records with same display order values be sorted in ascending or descending ID order?
        ///   For example, if records with ID's 1, 2, 3 has display order value of "0", their actual display
        ///   orders are 1, 2 and 3. If this parameter is set to true (descending), their display orders will
        ///   become 3, 2, 1. This parameter controls if records that are added recently and has no display
        ///   order value assigned (or 0) be shown at start or at the end.</param>
        /// <returns>
        ///   If any of the display order values is changed true.</returns>
        public static bool FixRecordOrdering(IDbConnection connection, IDisplayOrderRow row,
            BaseCriteria filter, Int64 recordID, int newDisplayOrder, bool descendingKeyOrder, bool hasUniqueConstraint = false)
        {
            return FixRecordOrdering(connection, ((Row)row).Table, (Field)((IIdRow)row).IdField, (row is IStagingRow) ?
                ((IStagingRow)row).StagingBaseIdField : null, row.DisplayOrderField, filter, recordID, 
                newDisplayOrder, descendingKeyOrder, hasUniqueConstraint);
        }

        /// <summary>
        ///   Sets a records display order to to requested value, and also renumbers other records
        ///   in the group as required.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="row">
        ///   Row with a display order and ID field (should implement IDbIdRow interface).</param>
        /// <param name="recordID">
        ///   ID value of the record.</param>
        /// <param name="newDisplayOrder">
        ///   New display order of the record.</param>
        /// <returns>
        ///   If any of the display order values is changed true.</returns>
        public static bool FixRecordOrdering(IDbConnection connection, IDisplayOrderRow row, Int64 recordID, int newDisplayOrder)
        {
            return FixRecordOrdering(connection, ((Row)row).Table, (Field)((IIdRow)row).IdField, (row is IStagingRow) ?
                ((IStagingRow)row).StagingBaseIdField : null, row.DisplayOrderField, null, recordID, 
                newDisplayOrder, false);
        }

        /// <summary>
        ///   An internal class that is used FixRecordOrdering to store old and new display orders
        ///   for records to be sorted.</summary>
        private class OrderRecord
        {
            public Int64 recordID;
            public int oldOrder;
            public int newOrder;
        }
    }
}