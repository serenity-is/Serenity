namespace Serenity.Services;

/// <summary>
/// Base class for types implementing <see cref="IUndeleteBehaviorAsync"/>
/// </summary>
public abstract class BaseUndeleteBehaviorAsync : IUndeleteBehaviorAsync
{
    /// <inheritdoc/>
    public virtual Task OnPrepareQueryAsync(IUndeleteRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnValidateRequestAsync(IUndeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnBeforeUndeleteAsync(IUndeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAfterUndeleteAsync(IUndeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAuditAsync(IUndeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IUndeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
