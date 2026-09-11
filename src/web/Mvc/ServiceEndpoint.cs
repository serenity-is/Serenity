using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Serenity.Services;

/// <summary>
/// Subclass of controller for service endpoints.
/// </summary>
[HandleServiceException]
public abstract class ServiceEndpoint : ControllerBase, IActionFilter, IAsyncActionFilter, IDisposable, IAsyncDisposable
{
    private IDbConnection? connection;
    private UnitOfWork? unitOfWork;
    private IRequestContext? context;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources currently used by this <see cref="Controller"/> instance.
    /// </summary>
    /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="Dispose()"/> method,
    /// otherwise <c>false</c>.</param>
    protected virtual void Dispose(bool disposing)
    {
        unitOfWork?.Dispose();
        unitOfWork = null;
        connection?.Dispose();
        connection = null;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources currently used by this <see cref="Controller"/> instance.
    /// </summary>
    /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="DisposeAsync()"/> method,
    /// otherwise <c>false</c>.</param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (unitOfWork != null)
        {
            var uow = unitOfWork;
            unitOfWork = null;
            await uow.DisposeAsync();
        }

        if (connection is IAsyncDisposable asyncDisposable)
        {
            connection = null;
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            connection?.Dispose();
            connection = null;
        }
    }

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        var uowParam = context.ActionDescriptor.Parameters.FirstOrDefault(x => x.ParameterType == typeof(IUnitOfWork));
        if (uowParam != null)
        {
            var connectionKey = GetType().GetCustomAttribute<ConnectionKeyAttribute>();
            if (connectionKey == null)
                ArgumentNullException.ThrowIfNull(connectionKey);

            connection = HttpContext.RequestServices.GetRequiredService<ISqlConnections>().NewByKey(connectionKey.Value);

            var attribute = (context.ActionDescriptor as ControllerActionDescriptor)?
                .MethodInfo.GetCustomAttribute<TransactionSettingsAttribute>() ??
                GetType().GetCustomAttribute<TransactionSettingsAttribute>();

            var settings = context.HttpContext?.RequestServices?
                .GetService<IOptions<TransactionSettings>>()?.Value;

            var isolationLevel = attribute?.IsolationLevel ??
                settings?.IsolationLevel ?? IsolationLevel.Unspecified;

            var deferStart = attribute?.HasDeferStart == true ?
                attribute.DeferStart : (settings?.DeferStart ?? false);

            unitOfWork = new UnitOfWork(connection, isolationLevel, deferStart);

            context.ActionArguments[uowParam.Name] = unitOfWork;
            return;
        }

        var cnnParam = context.ActionDescriptor.Parameters.FirstOrDefault(x => x.ParameterType == typeof(IDbConnection));
        if (cnnParam != null)
        {
            var connectionKey = GetType().GetCustomAttribute<ConnectionKeyAttribute>();
            if (connectionKey == null)
                ArgumentNullException.ThrowIfNull(connectionKey);

            connection = HttpContext.RequestServices.GetRequiredService<ISqlConnections>().NewByKey(connectionKey.Value);
            context.ActionArguments[cnnParam.Name] = connection;
            return;
        }
    }

    /// <summary>
    /// Called after the action method is invoked.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    /// <remarks>
    /// This is the synchronous <see cref="IActionFilter"/> implementation and is normally
    /// not called by MVC, as <see cref="ServiceEndpoint"/> also implements
    /// <see cref="IAsyncActionFilter"/> and the async filter is preferred. Unless you are
    /// explicitly invoking the synchronous filter pipeline, override
    /// <see cref="OnActionExecutedAsync"/> instead.
    /// </remarks>
    [NonAction]
    public virtual void OnActionExecuted(ActionExecutedContext context)
    {
        if (unitOfWork != null)
        {
            try
            {
                if (context.Exception != null)
                    unitOfWork.Dispose();
                else
                    unitOfWork.Commit();
            }
            catch (InvalidOperationException)
            {
                // if a DDL error occurs transaction might turn into a zombie
                // and we'll get an error here
            }

            unitOfWork = null;
        }

        connection?.Dispose();
        connection = null;

        context.Result = (context.Result as ActionResult) ?? new Result<object?>(context.Result);
    }

    /// <summary>
    /// Called after the action method is invoked asynchronously.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    /// <remarks>
    /// This is the async <see cref="IAsyncActionFilter"/> implementation and is the one
    /// normally called by MVC. Override this method instead of <see cref="OnActionExecuted"/>.
    /// </remarks>
    [NonAction]
    protected virtual async Task OnActionExecutedAsync(ActionExecutedContext context)
    {
        if (unitOfWork != null)
        {
            try
            {
                if (context.Exception != null)
                    await unitOfWork.DisposeAsync();
                else
                    await unitOfWork.CommitAsync();
            }
            catch (InvalidOperationException)
            {
                // if a DDL error occurs transaction might turn into a zombie
                // and we'll get an error here
            }

            unitOfWork = null;
        }

        if (connection is IAsyncDisposable asyncDisposable)
        {
            connection = null;
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            connection?.Dispose();
            connection = null;
        }

        context.Result = (context.Result as ActionResult) ?? new Result<object?>(context.Result);
    }

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The <see cref="ActionExecutionDelegate"/> to execute. Invoke this delegate in the body
    /// of <see cref="OnActionExecutionAsync" /> to continue execution of the action.</param>
    /// <returns>A <see cref="Task"/> instance.</returns>
    [NonAction]
    public virtual Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(next);

        OnActionExecuting(context);
        if (context.Result == null)
        {
            var task = next();
            if (!task.IsCompletedSuccessfully)
            {
                return Awaited(this, task);
            }

            return OnActionExecutedAsync(task.Result);
        }

        return Task.CompletedTask;

        static async Task Awaited(ServiceEndpoint controller, Task<ActionExecutedContext> task)
        {
            await controller.OnActionExecutedAsync(await task);
        }
    }

    /// <summary>
    /// Gets the request context.
    /// </summary>
    protected IRequestContext Context
    {
        get
        {
            context ??= HttpContext?.RequestServices?.GetRequiredService<IRequestContext>();
            return context ?? throw new InvalidOperationException($"Error reading 'Context' property of {GetType().Name}. The service endpoint has not been initialized.");
        }
        set
        {
            context = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Gets whether a request context (e.g. Context property) is available.
    /// </summary>
    protected bool HasRequestContext => (context ??= HttpContext?.RequestServices?.GetService<IRequestContext>()) != null;

    /// <summary>
    /// Gets the cache from the request context.
    /// </summary>
    protected ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the localizer from the request context.
    /// </summary>
    protected ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets the permission service from the request context.
    /// </summary>
    protected IPermissionService Permissions => Context.Permissions;
}
