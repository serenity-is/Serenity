using System;
using System.Collections.Generic;
using System.Data;
using Serenity.Data;

namespace Serenity
{
    /// <summary>
    ///   Contains helper functions for bound tables.</summary>
    public static class BoundTableHelper
    {
        /// <summary>
        ///   Gets a dictionary of bound ID's (e.g. hobby_id) related to a key (e.g. profile_id).</summary>
        /// <param name="connection">
        ///   Connection to be used (required).</param>
        /// <param name="tableName">
        ///   Name of bound table (e.g. tb_profile_hobby).</param>
        /// <param name="mKeyID">
        ///   Field field of key column (e.g. profile_id)</param>
        /// <param name="mBoundID">
        ///   Field field of bound column (e.g. hobby_id).</param>
        /// <param name="keyID">
        ///   Value of key column (actual integer value of profile ID).</param>
        /// <returns>
        ///   A dictionary with bound ID's as keys.</returns>
        public static Dictionary<Int64, bool> ListBoundID(IDbConnection connection, string tableName,
            Field mKeyID, Field mBoundID, Int64 keyID)
        {
            Dictionary<Int64, bool> result = new Dictionary<Int64, bool>();

            using (IDataReader reader = SqlHelper.ExecuteReader(connection, new SqlSelect().Select(
                mBoundID)
            .FromAs(
                tableName, 0)
            .Where(
                new Criteria(mKeyID) == keyID)))
            {
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                        result[Convert.ToInt64(reader.GetValue(0))] = true;
            };

            return result;
        }

        /// <summary>
        ///   Updates a bound table with new values for a key.</summary>
        /// <param name="connection">
        ///   Connection to be used (required).</param>
        /// <param name="tableName">
        ///   Name of bound table (e.g. tb_profile_hobby).</param>
        /// <param name="mID">
        ///   Field field of ID column (e.g. profile_hobby_id)</param>
        /// <param name="mKeyID">
        ///   Field field of key column (e.g. profile_id)</param>
        /// <param name="mBoundID">
        ///   Field field of bound column (e.g. hobby_id).</param>
        /// <param name="keyID">
        ///   Value of key column (actual integer value of profile ID).</param>
        /// <param name="listBoundID">
        ///   List of bound ID values (e.g. list of checked hobby ID values).</param>
        public static void UpdateBoundTable(IDbConnection connection, string tableName,
            Field mID, Field mKeyID, Field mBoundID, Int64 keyID, IEnumerable<Int64> listBoundID)
        {
            if (mID == null)
                throw new ArgumentNullException("mID");
            if (mKeyID == null)
                throw new ArgumentNullException("mKeyID");
            if (mBoundID == null)
                throw new ArgumentNullException("mBoundID");
            if (listBoundID == null)
                throw new ArgumentNullException("listBoundID");

            SqlSelect query = new SqlSelect().Select(
                mID,
                mBoundID)
            .FromAs(
                tableName, 0)
            .Where(
                new Criteria(mKeyID) == keyID);

            Dictionary<Int64, Int64> existing = new Dictionary<Int64, Int64>();
            List<Int64> free = new List<Int64>();

            using (IDbTransaction transaction = SqlTransactions.BeginTransactionIf(connection))
            {
                using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
                {
                    while (reader.Read())
                    {
                        var myID = Convert.ToInt64(reader.GetValue(0));
                        existing[reader.ToInt64(1).Value] = myID;
                        free.Add(myID);
                    }
                }

                if (free.Count > 0)
                {
                    Int64 id;
                    foreach (var boundID in listBoundID)
                        if (existing.TryGetValue(boundID, out id))
                            free.Remove(id);
                }

                string insertQuery = null;

                foreach (Int64 boundID in listBoundID)
                {
                    if (!existing.ContainsKey(boundID))
                    {
                        if (free.Count > 0)
                        {
                            new SqlUpdate(tableName)
                                .Set(mKeyID, keyID)
                                .Set(mBoundID, boundID)
                                .WhereEqual(mID, free[0])
                                .Execute(connection);
                            free.RemoveAt(0);
                        }
                        else
                        {
                            if (insertQuery == null)
                            {
                                insertQuery = String.Format(sqlInsert,
                                    tableName, mKeyID.Name, mBoundID.Name);
                            }

                            SqlHelper.ExecuteNonQuery(connection, String.Format(insertQuery),
                                new Dictionary<string, object>() {
                                    { "@keyID", keyID },
                                    { "@boundID", (object)boundID }
                                });
                        }
                    }
                }

                foreach (Int64 id in free)
                {
                    new SqlDelete(tableName)
                        .WhereEqual(mID, id)
                        .Execute(connection);
                }

                transaction.Commit();
            }
        }


        /*
        private const string sqlGetNextIdentVal =
            "DECLARE @minidentval int;\n" +
            "DECLARE @nextidentval int;\n" +
            "SELECT @minidentval = MIN(IDENTITYCOL) FROM {0};\n" +
            "IF @minidentval = IDENT_SEED('{0}')\n" +
            "  SELECT @nextidentval = MIN(IDENTITYCOL) + 1\n" +
            "  FROM {0} t1\n" +
            "  WHERE NOT EXISTS (SELECT IDENTITYCOL FROM {0} t2\n" +
            "     WHERE t2.IDENTITYCOL = t1.IDENTITYCOL + 1)\n" +
            "ELSE\n" +
            "  SELECT @nextidentval = IDENT_SEED('{0}');\n";

        private const string sqlInsertWithIdentity =
            sqlGetNextIdentVal +
            "SET IDENTITY_INSERT {0} ON;\n" +
            "INSERT INTO {0} ({1}, {2}, {3}) VALUES (@nextidentval, {{0}}, {{1}});" +
            "SET IDENTITY_INSERT {0} OFF;\n";*/

        private const string sqlInsert =
            "INSERT INTO {0} ({1}, {2}) VALUES (@keyID, @boundID);";
    }
}
