namespace Serenity.Data;

/// <summary>
///   Contains static extension methods for DbField and Meta objects.</summary>
public static class ServiceRequestExtensions
{
    /// <summary>
    /// Adds the field to the request.IncludeColumns
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="request">Request</param>
    /// <param name="field">Field</param>
    public static TRequest IncludeField<TRequest>(this TRequest request, Field field)
        where TRequest : ServiceRequest, IIncludeExcludeColumns
    {
        request.IncludeColumns ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        request.IncludeColumns.Add(field.PropertyName ?? field.Name);
        return request;
    }
}