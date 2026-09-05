namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="IRetrieveBehaviorAsync"/>
/// </summary>
public abstract class BaseRetrieveBehaviorAsync : IRetrieveBehaviorAsync
{
    /// <inheritdoc/>
    public virtual Task OnValidateRequestAsync(IRetrieveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnPrepareQueryAsync(IRetrieveRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnBeforeExecuteQueryAsync(IRetrieveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAfterExecuteQueryAsync(IRetrieveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IRetrieveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
