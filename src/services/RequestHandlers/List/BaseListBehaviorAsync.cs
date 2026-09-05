namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="IListBehaviorAsync"/>
/// </summary>
public abstract class BaseListBehaviorAsync : IListBehaviorAsync
{
    /// <inheritdoc/>
    public virtual Task OnValidateRequestAsync(IListRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnPrepareQueryAsync(IListRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnApplyFiltersAsync(IListRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnBeforeExecuteQueryAsync(IListRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAfterExecuteQueryAsync(IListRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IListRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
