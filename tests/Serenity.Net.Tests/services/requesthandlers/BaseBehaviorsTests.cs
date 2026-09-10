namespace Serenity.Services;

public class BaseBehaviorsTests
{
    private class SaveBehavior : BaseSaveBehavior
    {
    }

    private class SaveBehaviorAsync : BaseSaveBehaviorAsync
    {
    }

    private class SaveDeleteBehavior : BaseSaveDeleteBehavior
    {
    }

    private class SaveDeleteBehaviorAsync : BaseSaveDeleteBehaviorAsync
    {
    }

    private class ListBehavior : BaseListBehavior
    {
    }

    private class ListBehaviorAsync : BaseListBehaviorAsync
    {
    }

    private class RetrieveBehavior : BaseRetrieveBehavior
    {
    }

    private class RetrieveBehaviorAsync : BaseRetrieveBehaviorAsync
    {
    }

    private class DeleteBehavior : BaseDeleteBehavior
    {
    }

    private class DeleteBehaviorAsync : BaseDeleteBehaviorAsync
    {
    }

    private class UndeleteBehavior : BaseUndeleteBehavior
    {
    }

    private class UndeleteBehaviorAsync : BaseUndeleteBehaviorAsync
    {
    }

    [Fact]
    public void BaseSaveBehavior_Methods_Do_Nothing()
    {
        var behavior = new SaveBehavior();
        behavior.OnPrepareQuery(null, null);
        behavior.OnAfterSave(null);
        behavior.OnBeforeSave(null);
        behavior.OnAudit(null);
        behavior.OnReturn(null);
        behavior.OnSetInternalFields(null);
        behavior.OnValidateRequest(null);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public async Task BaseSaveBehaviorAsync_Methods_Complete()
    {
        var behavior = new SaveBehaviorAsync();
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnSetInternalFieldsAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeSaveAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterSaveAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAuditAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public void BaseSaveDeleteBehavior_Methods_Do_Nothing()
    {
        var behavior = new SaveDeleteBehavior();
        behavior.OnPrepareQuery(null, null);
        behavior.OnValidateRequest(null);
        behavior.OnBeforeDelete(null);
        behavior.OnAfterDelete(null);
        behavior.OnAudit(null);
        behavior.OnReturn(null);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public async Task BaseSaveDeleteBehaviorAsync_Methods_Complete()
    {
        var behavior = new SaveDeleteBehaviorAsync();
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeDeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterDeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAuditAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public void BaseListBehavior_Methods_Do_Nothing()
    {
        var behavior = new ListBehavior();
        behavior.OnValidateRequest(null);
        behavior.OnPrepareQuery(null, null);
        behavior.OnApplyFilters(null, null);
        behavior.OnBeforeExecuteQuery(null);
        behavior.OnAfterExecuteQuery(null);
        behavior.OnReturn(null);
    }

    [Fact]
    public async Task BaseListBehaviorAsync_Methods_Complete()
    {
        var behavior = new ListBehaviorAsync();
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnApplyFiltersAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeExecuteQueryAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterExecuteQueryAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
    }

    [Fact]
    public void BaseRetrieveBehavior_Methods_Do_Nothing()
    {
        var behavior = new RetrieveBehavior();
        behavior.OnValidateRequest(null);
        behavior.OnPrepareQuery(null, null);
        behavior.OnApplyFilters(null, null);
        behavior.OnBeforeExecuteQuery(null);
        behavior.OnAfterExecuteQuery(null);
        behavior.OnReturn(null);
    }

    [Fact]
    public async Task BaseRetrieveBehaviorAsync_Methods_Complete()
    {
        var behavior = new RetrieveBehaviorAsync();
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeExecuteQueryAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterExecuteQueryAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
    }

    [Fact]
    public void BaseDeleteBehavior_Methods_Do_Nothing()
    {
        var behavior = new DeleteBehavior();
        behavior.OnPrepareQuery(null, null);
        behavior.OnValidateRequest(null);
        behavior.OnBeforeDelete(null);
        behavior.OnAfterDelete(null);
        behavior.OnAudit(null);
        behavior.OnReturn(null);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public async Task BaseDeleteBehaviorAsync_Methods_Complete()
    {
        var behavior = new DeleteBehaviorAsync();
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeDeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterDeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAuditAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public void BaseUndeleteBehavior_Methods_Do_Nothing()
    {
        var behavior = new UndeleteBehavior();
        behavior.OnPrepareQuery(null, null);
        behavior.OnValidateRequest(null);
        behavior.OnBeforeUndelete(null);
        behavior.OnAfterUndelete(null);
        behavior.OnAudit(null);
        behavior.OnReturn(null);
        behavior.OnException(null, new Exception());
    }

    [Fact]
    public async Task BaseUndeleteBehaviorAsync_Methods_Complete()
    {
        var behavior = new UndeleteBehaviorAsync();
        await behavior.OnPrepareQueryAsync(null, null, TestContext.Current.CancellationToken);
        await behavior.OnValidateRequestAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnBeforeUndeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAfterUndeleteAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnAuditAsync(null, TestContext.Current.CancellationToken);
        await behavior.OnReturnAsync(null, TestContext.Current.CancellationToken);
        behavior.OnException(null, new Exception());
    }
}
