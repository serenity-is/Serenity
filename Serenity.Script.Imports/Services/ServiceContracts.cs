using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, PreserveMemberCase]
    public class DeleteRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class DeleteResponse : ServiceResponse
    {
    }

    [Imported, Serializable, PreserveMemberCase]
    public class UndeleteRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class UndeleteResponse : ServiceResponse
    {
    }

    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class RetrieveLocalizationRequest : ServiceRequest
    {
        public Int64 EntityId { get; set; }
        public Int32 CultureId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class RetrieveResponse<TEntity> : ServiceResponse
        where TEntity: new()
    {
        public TEntity Entity { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class SaveRequest<TEntity> : ServiceRequest
        where TEntity : new()
    {
        public TEntity Entity { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class SaveRequestWithAttachment<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
        public object[] Attachments { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class CreateRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class UpdateRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
    }

    [Imported, Serializable, PreserveMemberCase, IncludeGenericArguments(false)]
    public class UpdateLocalizationRequest<TEntity> : SaveRequest<TEntity>
        where TEntity : new()
    {
        public Int32 CultureId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class CreateResponse
    {
        public Int64? EntityId { get; set; }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class UpdateResponse
    {
    }
}