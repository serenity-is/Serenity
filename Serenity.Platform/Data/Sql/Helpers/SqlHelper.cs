namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    public enum ExpectedRows
    {
        One = 0,
        ZeroOrOne = 1,
        Ignore = 2,
    }

    public static class SqlHelper
    {
        public static bool IsDatabaseException(Exception e)
        {
            return e != null && e is SqlException;
        }

        /// <summary>
        ///   <see cref="SqlInsert"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
        ///   istenirse eklenen kaydın IDENTITY alanının değerini döndürür.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection, true) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlInsert"/> nesnesi.</param>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <returns>
        ///   <paramref name="returnIdentity"/> true ise eklenen kaydın IDENTITY değeri, değilse 1.</returns>
        public static Int64? ExecuteAndGetID(this SqlInsert query, IDbConnection connection)
        {
            string queryText = query.ToString();
            if (((IQueryWithParams)query).Dialect.UseReturningIdentity())
            {
                string identityColumn = query.IdentityColumn();
                if (identityColumn == null)
                    throw new ArgumentNullException("query.IdentityColumn");
                queryText += " RETURNING " + identityColumn;

                using (var command = NewCommand(connection, queryText, query.Params))
                {
                    var param = command.CreateParameter();
                    param.Direction = ParameterDirection.Output;
                    param.ParameterName = identityColumn;
                    param.DbType = DbType.Int64;
                    command.Parameters.Add(param);
                    ExecuteNonQuery(command);
                    return Convert.ToInt64(param.Value);
                }
            }
            
            if (((IQueryWithParams)query).Dialect.UseScopeIdentity())
            {
                var scopeIdentityExpression = ((IQueryWithParams) query).Dialect.ScopeIdentityExpression();

                queryText += ";\nSELECT " + scopeIdentityExpression + " AS IDCOLUMNVALUE";

                using (IDataReader reader = ExecuteReader(connection, queryText, query.Params))
                {
                    if (reader.Read() &&
                        !reader.IsDBNull(0))
                        return Convert.ToInt64(reader.GetValue(0));
                    return null;
                }
            }
            
            throw new NotImplementedException();
        }

        /// <summary>
        ///   <see cref="SqlInsert"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlInsert"/> nesnesi.</param>
        public static void Execute(this SqlInsert query, IDbConnection connection)
        {
            ExecuteNonQuery(connection, query.ToString(), query.Params);
        }

        /// <summary>
        ///   <see cref="SqlInsert"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlInsert"/> nesnesi.</param>
        public static void Execute(this SqlInsert query, IDbConnection connection, Dictionary param)
        {
            ExecuteNonQuery(connection, query.ToString(), param);
        }

        /// <summary>
        ///   <see cref="SqlUpdate"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlUpdate"/> nesnesi.</param>
        /// <returns>
        ///   Etkilenen kayıt sayısı.</returns>
        public static int Execute(this SqlUpdate query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), query.Params));
        }

        private static int CheckExpectedRows(ExpectedRows expectedRows, int affectedRows)
        {
            if (expectedRows == ExpectedRows.Ignore)
                return affectedRows;

            if (expectedRows == ExpectedRows.One && affectedRows != 1)
                throw new InvalidOperationException(String.Format("Query affected {0} rows while 1 expected!", affectedRows));

            if (expectedRows == ExpectedRows.ZeroOrOne && affectedRows > 1)
                throw new InvalidOperationException("Query affected {0} rows while 1 expected!");

            return affectedRows;
        }

        /// <summary>
        ///   <see cref="SqlDelete"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlDelete"/> nesnesi.</param>
        /// <returns>
        ///   Etkilenen kayıt sayısı.</returns>
        public static int Execute(this SqlDelete query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), query.Params));
        }

        /// <summary>
        ///   <see cref="SqlDelete"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlDelete"/> nesnesi.</param>
        /// <returns>
        ///   Etkilenen kayıt sayısı.</returns>
        public static int Execute(this SqlDelete query, IDbConnection connection, Dictionary param, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), param));
        }

        /// <summary>
        ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p>
        ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
        ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
        /// <returns>
        ///   Sorgu sonuçlarına erişim sağlayan <see cref="IDataReader"/> nesnesi.</returns>
        public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection)
        {
            return ExecuteReader(connection, query.ToString(), query.Params);
        }

        /// <summary>
        ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk query.Execute(connection) şeklinde de 
        ///   çalıştırılabilir.</p>
        ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
        ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
        /// <returns>
        ///   Sorgu sonuçlarına erişim sağlayan <see cref="IDataReader"/> nesnesi.</returns>
        public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection, Dictionary param)
        {
            return ExecuteReader(connection, query.ToString(), param);
        }

        /// <summary>
        ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
        ///   sorgunun döndürdüğü her bir kayıt için parametresiz bir callback fonksiyonunu çağırır.</summary>
        /// <remarks>
        ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForEach(connection, delegate() {...})</c> 
        ///   şeklinde de çalıştırılabilir.</p>
        ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
        ///   önce çalıştırılır.</p>
        ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
        ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
        /// <param name="connection">
        ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
        /// <param name="query">
        ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
        /// <param name="callBack">
        ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
        /// <returns>
        ///   query.CountRecords true ise toplam kayıt sayısı, değilse 0.</returns>
        public static int ForEach(this SqlQuery query, IDbConnection connection,
            Action callBack)
        {
            int count = 0;

            if (query.Dialect().MultipleResultsets())
            {
                using (IDataReader reader = ExecuteReader(connection, query))
                {
                    while (reader.Read())
                    {
                        query.GetFromReader(reader);
                        callBack();
                    }

                    if (query.CountRecords && reader.NextResult() && reader.Read())
                        return Convert.ToInt32(reader.GetValue(0));
                }
            }
            else
            {
                string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (queries.Length > 1)
                    count = Convert.ToInt32(ExecuteScalar(connection, queries[1], query.Params));

                using (IDataReader reader = ExecuteReader(connection, queries[0], query.Params))
                {
                    while (reader.Read())
                    {
                        query.GetFromReader(reader);
                        callBack();
                    }
                }
            }

            return count;
        }

        /// <summary>
        ///   <see cref="IDataReader"/> parametresi alan ve sonuç döndürmeyen bir delegate tipi</summary>
        /// <param name="reader">
        ///   Callback fonksiyonuna geçirilen <see cref="IDataReader"/> tipinde nesne.</param>
        public delegate void ReaderCallBack(IDataReader reader);

       /// <summary>
       ///   İstenen bağlantıya bağlı ve verilen komutu içeren yeni bir IDbCommand nesnesi oluşturur.</summary>
       /// <param name="connection">
       ///   IDbCommand nesnesinin oluşturulacağı bağlantı.</param>
       /// <param name="commandText">
       ///   IDbCommand nesnesinin içereceği komut metni. <c>null</c> olabilir.</param>
       /// <returns>
       ///   Yeni IDbCommand nesnesi.</returns>
       public static IDbCommand NewCommand(IDbConnection connection, string commandText)
       {
           if (connection == null)
               throw new ArgumentNullException("connection");

           IDbCommand command = connection.CreateCommand();
           command.CommandText = commandText;
           return command;
       }


       /// <summary>
       ///   İstenen bağlantıya bağlı ve verilen komutu içeren yeni bir IDbCommand nesnesi oluşturur.</summary>
       /// <param name="connection">
       ///   IDbCommand nesnesinin oluşturulacağı bağlantı.</param>
       /// <param name="commandText">
       ///   IDbCommand nesnesinin içereceği komut metni. <c>null</c> olabilir.</param>
       /// <param name="param">
       ///   Parameters.</param>
       /// <returns>
       ///   Yeni IDbCommand nesnesi.</returns>
       public static IDbCommand NewCommand(IDbConnection connection, string commandText, IDictionary<string, object> param)
       {
           var command = (DbCommand)(NewCommand(connection, commandText));

           if (param == null || param.Count == 0)
               return command;

           try
           {
               foreach (var p in param)
                   AddParamWithValue(command, p.Key, p.Value);
               return command;
           }
           catch
           {
               command.Dispose();
               throw;
           }
       }

       /// <summary>
       ///   <see cref="DbCommand"/> nesnesine belirtilen isim ve değere sahip yeni bir parametre ekler.</summary>
       /// <param name="command">
       ///   Parametrenin ekleneceği <see cref="DbCommand"/> nesnesi</param>
       /// <param name="name">
       ///   Parametre ismi.</param>
       /// <param name="value">
       ///   Parametre değeri.</param>
       /// <returns>
       ///   Yeni oluşturulan <see cref="DbParameter"/> nesnesi.</returns>
       public static DbParameter AddParamWithValue(this DbCommand command, string name, object value)
       {
           DbParameter param = command.CreateParameter();

           if (value != null &&
               value is Stream)
           {
               if (value is MemoryStream)
                   value = ((MemoryStream)value).ToArray();
               else
               {
                   using (var ms = new MemoryStream())
                   {
                       ((Stream)value).CopyTo(ms);
                       value = ms.ToArray();
                   }
               }
           }

           param.Value = value ?? DBNull.Value;
           param.ParameterName = name;
           command.Parameters.Add(param);
           return param;
       }

       /// <summary>
       ///   Verilen Sql exception'ının numarasının, bilinen connection pool hatalarından biri olmasını 
       ///   denetler ve gerekirse bağlantıyı tekrar açıp kapatır.</summary>
       /// <param name="connection">
       ///   Hatanın oluştuğu bağlantı.</param>
       /// <param name="exception">
       ///   Numarası kontrol edilecek hata.</param>
       /// <returns>
       ///   Hata numarası 10054 ise true.</returns>
       private static bool CheckConnectionPoolException(IDbConnection connection, Exception exception)
       {
           var ex = exception as System.Data.SqlClient.SqlException;

           if (ex != null && ex.Number == 10054)
           {
               System.Data.SqlClient.SqlConnection.ClearAllPools();
               connection.Close();
               connection.Open();
               return true;
           }
           else
               return false;
       }

       /// <summary>
       ///   Bağlantı üzerinde sonuç döndürmeyen (INSERT, UPDATE, DELETE gibi) bir sorguyu çalıştırır.</summary>
       /// <param name="command">
       ///   Çalıştırılacak komut.</param>
       /// <returns>
       ///   Etkilenen satır sayısı (veritabanının desteklemesine bağlı).</returns>
       public static int ExecuteNonQuery(IDbCommand command)
       {
           if (command == null)
               throw new ArgumentNullException("command");

           if (command.Connection == null)
               throw new ArgumentNullException("command");

           try
           {
               command.Connection.EnsureOpen();
               try
               {
                   return command.ExecuteNonQuery();
               }
               catch (System.Data.SqlClient.SqlException ex)
               {
                   if (CheckConnectionPoolException(command.Connection, ex))
                       return command.ExecuteNonQuery();
                   else
                       throw;
               }
           }
           catch (Exception ex)
           {
               ex.SetData("sql_command_text", command.CommandText);
               throw;
           }
       }

       /// <summary>
       ///   Bağlantı üzerinde sonuç döndürmeyen (INSERT, UPDATE, DELETE gibi) bir sorguyu çalıştırır.</summary>
       /// <param name="connection">
       ///   Komutun çalıştırılacağı bağlantı.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak komut.</param>
       /// <param name="param">
       ///   Parameters (optional).</param>
       /// <returns>
       ///   Etkilenen satır sayısı (veritabanının desteklemesine bağlı).</returns>
       public static int ExecuteNonQuery(IDbConnection connection, string commandText,
           IDictionary<string, object> param)
       {
           using (IDbCommand command = NewCommand(connection, commandText, param))
           {
               if (Log.DebugLevel)
                   LogCommand("ExecuteNonQuery", command);

               var result = ExecuteNonQuery(command);

               if (Log.DebugLevel)
                   Log.Debug("END - ExecuteNonQuery");

               return result;
           }
       }

       /// <summary>
       ///   Bağlantı üzerinde sonuç döndürmeyen (INSERT, UPDATE, DELETE gibi) bir sorguyu çalıştırır.</summary>
       /// <param name="connection">
       ///   Komutun çalıştırılacağı bağlantı.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak komut.</param>
       /// <returns>
       ///   Etkilenen satır sayısı (veritabanının desteklemesine bağlı).</returns>
       public static int ExecuteNonQuery(IDbConnection connection, string commandText)
       {
           using (IDbCommand command = NewCommand(connection, commandText))
           {
               if (Log.DebugLevel)
                   LogCommand("ExecuteNonQuery", command);

               var result = ExecuteNonQuery(command);

               if (Log.DebugLevel)
                   Log.Debug("END - ExecuteNonQuery");

               return result;
           }
       }

       /// <summary>
       ///   Bağlantı üzerinde tek değer döndüren bir sorguyu çalıştırır.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak sorgu.</param>
       /// <param name="param">
       ///   Parameters (optional).</param>
       /// <returns>
       ///   Sorgunun döndürdüğü skalar değer.</returns>
       public static object ExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param)
       {
           if (connection == null)
               throw new ArgumentNullException("connection");

           connection.EnsureOpen();

           using (IDbCommand command = NewCommand(connection, commandText, param))
           {
               try
               {
                   try
                   {
                       if (Log.DebugLevel)
                           LogCommand("ExecuteScalar", command);

                       var result = command.ExecuteScalar();

                       if (Log.DebugLevel)
                           Log.Debug("END - ExecuteScalar");

                       return result;
                   }
                   catch (System.Data.SqlClient.SqlException ex)
                   {
                       if (CheckConnectionPoolException(connection, ex))
                           return command.ExecuteScalar();
                       else
                           throw;
                   }
               }
               catch (Exception ex)
               {
                   ex.SetData("sql_command_text", commandText);
                   throw;
               }
           }
       }

       /// <summary>
       ///   Bağlantı üzerinde tek değer döndüren bir sorguyu çalıştırır.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak sorgu.</param>
       /// <returns>
       ///   Sorgunun döndürdüğü skalar değer.</returns>
       public static object ExecuteScalar(IDbConnection connection, string commandText)
       {
           return ExecuteScalar(connection, commandText, null);
       }

       /// <summary>
       ///   Bağlantı üzerinde tek değer döndüren bir <see cref="SqlQuery"/> sorgusunu çalıştırır.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı.</param>
       /// <param name="selectQuery">
       ///   Çalıştırılacak sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Sorgunun döndürdüğü skalar değer.</returns>
       public static object ExecuteScalar(IDbConnection connection, SqlQuery selectQuery)
       {
           if (selectQuery == null)
               throw new ArgumentNullException("selectQuery");

           return ExecuteScalar(connection, selectQuery.ToString(), selectQuery.Params);
       }

       /// <summary>
       ///   Bağlantı üzerinde tek değer döndüren bir <see cref="SqlQuery"/> sorgusunu çalıştırır.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı.</param>
       /// <param name="selectQuery">
       ///   Çalıştırılacak sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Sorgunun döndürdüğü skalar değer.</returns>
       public static object ExecuteScalar(IDbConnection connection, SqlQuery selectQuery, Dictionary param)
       {
           if (selectQuery == null)
               throw new ArgumentNullException("selectQuery");

           return ExecuteScalar(connection, selectQuery.ToString(), param);
       }

       internal static void LogCommand(string type, IDbCommand command)
       {
           try
           {
               var sqlCmd = command as SqlCommand;
               if (sqlCmd != null)
               {
                   Log.Debug(type + "\r\n" + SqlCommandDumper.GetCommandText(sqlCmd));
                   return;
               }

               StringBuilder sb = new StringBuilder((command.CommandText ?? "").Length + 1000);
               sb.Append(type);
               sb.Append("\r\n");
               sb.Append(command.CommandText);
               if (command.Parameters != null && command.Parameters.Count > 0)
               {
                   sb.Append(" --- PARAMS --- ");
                   foreach (DbParameter p in command.Parameters)
                   {
                       sb.Append(p.ParameterName);
                       sb.Append("=");
                       if (p.Value == null || p.Value == DBNull.Value)
                           sb.Append("<NULL>");
                       else
                           sb.Append(p.Value.ToString());
                       sb.Append(" ");
                   }
               }

               Log.Debug(sb.ToString());
           }
           catch (Exception ex)
           {
               Log.Debug("Error logging command: " + ex.ToString());
           }
       }

       /// <summary>
       ///   Sorguyu belirtilen bağlantı üzerinde çalıştırır ve bir IDataReader nesnesi döndürür.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Açık değilse otomatik olarak açılır.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak SQL sorgusu.</param>
       /// <param name="param">
       ///   Parameters (optional).</param>
       /// <returns>
       ///   Sorgunun çalıştırılması sonucu elde edilen IDataReader nesnesi.</returns>
       public static IDataReader ExecuteReader(IDbConnection connection, string commandText,
           IDictionary<string, object> param)
       {
           if (connection == null)
               throw new ArgumentNullException("connection");

           connection.EnsureOpen();

           try
           {
               //using (new Tracer(commandText))
               {
                   IDbCommand command = NewCommand(connection, commandText, param);
                   try
                   {
                       if (Log.DebugLevel)
                           LogCommand("ExecuteReader", command);

                       var result = command.ExecuteReader();

                       if (Log.DebugLevel)
                           Log.Debug("END - ExecuteReader");

                       return result;
                   }
                   catch (System.Data.SqlClient.SqlException ex)
                   {
                       if (CheckConnectionPoolException(connection, ex))
                           return command.ExecuteReader();
                       else
                           throw;
                   }
               }
           }
           catch (Exception ex)
           {
               ex.SetData("sql_command_text", commandText);
               throw;
           }
       }

       /// <summary>
       ///   Sorguyu belirtilen bağlantı üzerinde çalıştırır ve bir IDataReader nesnesi döndürür.</summary>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Açık değilse otomatik olarak açılır.</param>
       /// <param name="commandText">
       ///   Çalıştırılacak SQL sorgusu.</param>
       /// <returns>
       ///   Sorgunun çalıştırılması sonucu elde edilen IDataReader nesnesi.</returns>
       public static IDataReader ExecuteReader(IDbConnection connection, string commandText)
       {
           return ExecuteReader(connection, commandText, null);
       }

       /// <summary>
       ///   Belli bir bağlantı string'i ve sorgu metni için Cache içinde anahtar olarak kullanılabilecek
       ///   bir string üretir.</summary>
       /// <remarks>
       ///   Bu fonksiyon ExecuteReader(IDbConnection, string, TimeSpan, params) tarafından çalıştırılan
       ///   sorguların uygulama Cache'i içerisinde önbelleklenmesi için gerekli olan anahtar string'in 
       ///   üretilmesinde kullanılır.</remarks>
       /// <param name="connectionString">
       ///   Sorgunun çalıştırılacağı bağlantı string'i</param>
       /// <param name="commandText">
       ///   Sorgu metni, <c>null</c> olabilir.</param>
       /// <returns>
       ///   Bağlantı string'i ve sorgu metnine göre unique bir anahtar.</returns>
       private static string GetReaderCacheKey(string connectionString, string commandText)
       {
           commandText = commandText ?? String.Empty;

           const string queryCacheKey = "SQL_SELECT_QUERY";
           StringBuilder sb = new StringBuilder(queryCacheKey,
               commandText.Length + connectionString.Length + queryCacheKey.Length + 50);
           sb.AppendLine(connectionString);
           sb.AppendLine(commandText);

           return sb.ToString();
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
       /// <remarks>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>       
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Sorgu sonuçlarına erişim sağlayan <see cref="IDataReader"/> nesnesi.</returns>
       public static IDataReader ExecuteReader(IDbConnection connection, SqlQuery query)
       {
           return ExecuteReader(connection, query.ToString(), query.Params);
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır.</summary>
       /// <remarks>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>       
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Sorgu sonuçlarına erişim sağlayan <see cref="IDataReader"/> nesnesi.</returns>
       public static IDataReader ExecuteReader(IDbConnection connection, SqlQuery query, Dictionary param)
       {
           return ExecuteReader(connection, query.ToString(), param);
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   sorgunun döndürdüğü her bir kayıt için <see cref="IDataReader"/> parametresi alan bir 
       ///   callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForEach(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="callBack">
       ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   query.CountRecords true ise toplam kayıt sayısı, değilse 0.</returns>
       public static int ForEach(this SqlQuery query, IDbConnection connection,
           ReaderCallBack callBack)
       {
           int count = 0;

           if (query.Dialect().MultipleResultsets())
           {
               using (IDataReader reader = ExecuteReader(connection, query))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack(reader);
                   }

                   if (query.CountRecords && reader.NextResult() && reader.Read())
                       return Convert.ToInt32(reader.GetValue(0));
               }
           }
           else 
           {
               string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
               if (queries.Length > 1)
                   count = Convert.ToInt32(ExecuteScalar(connection, queries[1], query.Params));

               using (IDataReader reader = ExecuteReader(connection, queries[0], query.Params))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack(reader);
                   }
               }
           }

           return count;
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetFirst(this SqlQuery query, IDbConnection connection)
       {
           using (IDataReader reader = ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetFirst(this SqlQuery query, IDbConnection connection, Row row, Dictionary param)
       {
           using (IDataReader reader = ExecuteReader(connection, query, param))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader, new Row[] { row });
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetSingle(this SqlQuery query, IDbConnection connection)
       {
           using (IDataReader reader = ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   
                   if (reader.Read())
                       throw new InvalidOperationException("Query returned more than one result!");

                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetSingle(this SqlQuery query, IDbConnection connection, Row row, Dictionary param)
       {
           using (IDataReader reader = ExecuteReader(connection, query, param))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader, new Row[] { row });

                   if (reader.Read())
                       throw new InvalidOperationException("Query returned more than one result!");

                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kayıt için parametresiz bir callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="callBack">
       ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool ForFirst(this SqlQuery query, IDbConnection connection,
           Action callBack)
       {
           using (IDataReader reader = ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   callBack();
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kayıt için <see cref="IDataReader"/> parametresi alan bir 
       ///   callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="callBack">
       ///   İlk kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool ForFirst(this SqlQuery query, IDbConnection connection,
           ReaderCallBack callBack)
       {
           using (IDataReader reader = ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   callBack(reader);
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   en azından 1 sonuç göndermesini kontrol eder.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.Exists(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool Exists(this SqlQuery query, IDbConnection connection)
       {
           using (IDataReader reader = ExecuteReader(connection, query))
               return reader.Read();
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   sonuçları <typeparamref name="TRow"/> tipinde row'lardan oluşan bir liste halinde döndürür.
       /// </summary>
       ///   <typeparamref name="TRow"/><see cref="Row"/>'dan türemiş bir row sınıfı.
       /// <param name="query">
       ///   Çalıştırılacak sorguyu içeren <see cref="SqlQuery"/> nesnesi</param>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="loaderRow">
       ///   <paramref name="SqlQuery"/> sorgusunun döndürdüğü sonuçların GetFromReader ile içine yazılacağı
       ///   <typeparamref name="TRow"/> tipindeki nesne. Her satırın alan değerleri öncelikle bu row'a yüklenir ve 
       ///   kopyası çıkarılıp sonuç listesine eklenir.</param>
       /// <returns>
       ///   Sorgudan dönen kayıtların <typeparamref name="TRow"/> tipinde bir listesi</returns>
       public static List<TRow> List<TRow>(this SqlQuery query,
           IDbConnection connection, TRow loaderRow = null) where TRow : Row
       {
           var list = new List<TRow>();
           ForEach(query, connection, delegate()
           {
               list.Add(loaderRow.Clone());
           });
           return list;
       }

       public static void GetFromReader(this SqlQuery query, IDataReader reader)
       {
           GetFromReader(query, reader, query.IntoRows);
       }

       public static void GetFromReader(this SqlQuery query, IDataReader reader, IList<IEntity> into)
       {
           int index = 0;
           foreach (var info in query.GetColumns())
           {
               if (info.IntoField as Field != null && info.IntoRowIndex != -1)
               {
                   var row = into[info.IntoRowIndex];
                   ((Field)info.IntoField).GetFromReader(reader, index, (Row)row);
               }
               else if (info.IntoRowIndex != -1)
               {
                   var row = (Row)(into[info.IntoRowIndex]);
                   var name = reader.GetName(index);
                   var field = row.FindField(name) ?? row.FindFieldByPropertyName(name);
                   if (field != null)
                   {
                       //info.IntoField = field;
                       field.GetFromReader(reader, index, row);
                   }
                   else
                   {
                       if (reader.IsDBNull(index))
                           row.SetDictionaryData(name, null);
                       else
                       {
                           var value = reader.GetValue(index);
                           row.SetDictionaryData(name, value);
                       }
                   }
               }

               index++;
           }
       }
   }
}