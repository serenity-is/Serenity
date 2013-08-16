using System;
using System.Data;
using Serenity.Data;

namespace Serenity.Services
{
    public static class ServiceHelper
    {
        public static void ValidateRequestSupport(this ListRequest request, ListRequestSupport support)
        {
            DataValidation.CheckNotNull(request);

            if (request.Take < 0 ||
                request.Skip < 0)
                throw new ValidationError("Liste isteklerinde Take ve Skip parametreleri 0'dan büyük olmalıdır!");

            if ((support & ListRequestSupport.Paging) != ListRequestSupport.Paging &&
                (request.Take > 0 || request.Skip > 0))
            {
                throw new ValidationError("Bu liste servisi sayfalamayı desteklemiyor!");
            }

            if ((support & ListRequestSupport.Sorting) != ListRequestSupport.Sorting &&
                !request.Sort.IsEmptyOrNull())
            {
                throw new ValidationError("Bu liste servisi sıralamayı desteklemiyor!");
            }

            if ((support & ListRequestSupport.ContainsText) != ListRequestSupport.ContainsText &&
                !request.ContainsText.IsEmptyOrNull())
            {
                throw new ValidationError("Bu liste servisi metin aramasını desteklemiyor!");
            }

            if ((support & ListRequestSupport.IncludeDeleted) != ListRequestSupport.IncludeDeleted &&
                request.IncludeDeleted)
            {
                throw new ValidationError("Bu liste servisi silinmiş kayıtları göstermeyi desteklemiyor!");
            }

            if ((support & ListRequestSupport.FilterLines) != ListRequestSupport.FilterLines &&
                !request.FilterLines.IsEmptyOrNull())
            {
                throw new ValidationError("Bu liste servisi filtre satırlarını desteklemiyor!");
            }

            if ((support & ListRequestSupport.Filter) != ListRequestSupport.Filter &&
                request.Filter != null)
            {
                throw new ValidationError("Bu liste servisi filtre desteklemiyor!");
            }
        }

        public static void CheckRelatedOnDelete(IDbConnection connection, string tableName, Action<SqlSelect> filter)
        {
            var query = new SqlSelect().Select("1").From(tableName);
            filter(query);
            if (query.Take(1).Exists(connection))
                throw DataValidation.RelatedRecordExist(tableName);
        }

        public static void CheckParentNotDeleted(IDbConnection connection, string tableName, Action<SqlSelect> filter)
        {
            var query = new SqlSelect().Select("1").From(tableName);
            filter(query);
            if (query.Take(1).Exists(connection))
                throw DataValidation.ParentRecordDeleted(tableName);
        }

        public static void SetSkipTakeTotal<T>(this ListResponse<T> response, SqlSelect query)
        {
            response.Skip = query.Skip();
            response.Take = query.Take();
            if (response.Take == 0)
                response.TotalCount = response.Entities.Count + response.Skip;
        }

        public static bool IsUniqueIndexException(IDbConnection connection,
            Exception exception, string indexName,
            Row oldRow, Row newRow, params Field[] indexFields)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (exception == null)
                throw new ArgumentNullException("exception");

            if (!SqlHelper.IsDatabaseException(exception))
                return false;

            if (indexFields == null ||
                indexFields.Length == 0)
                throw new ArgumentNullException("indexField");

            if (indexName != null &&
                !exception.Message.Contains(indexName))
                return false;

            if (oldRow != null)
            {
                bool anyDifferent = false;
                foreach (var field in indexFields)
                    if (field.IndexCompare(oldRow, newRow) != 0)
                    {
                        anyDifferent = true;
                        break;
                    }

                if (!anyDifferent)
                    return false;
            }

            var row = newRow.CreateNew();
            var newId = ((IIdRow)newRow).IdField[newRow];

            var query = new SqlSelect().FromAs(row, 0).Select((Field)(((IIdRow)newRow).IdField));
            foreach (var field in indexFields)
                query.WhereEqual(field, field.AsObject(newRow));

            if (!query.GetFirst(connection))
                return false;

            var id = ((IIdRow)row).IdField[row];
            if (id != null && id == newId)
                return false;

            return true;
        }

        public static void HandleUniqueKodException(IDbConnection connection,
            Exception exception, string indexName,
            Row oldRow, Row newRow, params Field[] indexFields)
        {
            if (IsUniqueIndexException(connection, exception, indexName, oldRow, newRow, indexFields))
            {
                var fieldNames = indexFields[0].Name;
                var fieldTitles = indexFields[0].Title;
                for (var i = 1; i < indexFields.Length; i++)
                {
                    fieldNames += ", " + indexFields[i].Name;
                    fieldTitles += ", " + indexFields[i].Title;
                }

                throw new ValidationError("UniqueViolation", fieldNames,
                    String.Format("Aynı \"{0}\" değerine sahip başka bir kayıt daha var!",
                        fieldTitles));
            }
        }
    }
}
