using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    private async Task RunMigrationsAndServiceCall(string connectionString,
        string northwindConnectionString, string providerName)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["Data:Default:ConnectionString"] = connectionString,
                ["Data:Default:ProviderName"] = providerName,
                ["Data:Northwind:ConnectionString"] = northwindConnectionString,
                ["Data:Northwind:ProviderName"] = providerName,
            })
            .Build();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            EnvironmentName = "Development",
            ApplicationName = typeof(Startup).Assembly.GetName().Name!,
            ContentRootPath = tempPath,
            WebRootPath = System.IO.Path.Combine(tempPath, "wwwroot")
        });
        var collection = builder.Services;
        collection.AddSingleton<IConfiguration>(configuration);
        collection.AddApplicationPartsTypeSource();
        collection.AddApplicationPartsFeatureToggles(configuration);
        collection.ConfigureSections(configuration);
        collection.AddSqlConnections();
        collection.Configure<JsonOptions>(options => JSON.Defaults.Populate(options.JsonSerializerOptions));
        collection.Configure<UserEntityOptions>(options => options.RowType = typeof(Administration.UserRow));
        collection.AddSingleton<IPermissionService, AppServices.PermissionService>();
        collection.AddSingleton<IRolePermissionService, AppServices.RolePermissionService>();
        collection.AddUserProvider<AppServices.UserAccessor, AppServices.UserRetrieveService>();
        collection.AddHttpContextItemsAccessor();
        collection.AddServiceHandlers();
        collection.AddUploadStorage();
        collection.AddTransient<IDataMigrations, AppServices.DataMigrations>();

        using var services = collection.BuildServiceProvider();
        var oldRowFieldsProvider = RowFieldsProvider.SetLocalFrom(services);
        try
        {
            var dataMigrations = services.GetRequiredService<IDataMigrations>();
            dataMigrations.Initialize();

            var cancellationToken = TestContext.Current.CancellationToken;
            var userProvider = services.GetRequiredService<IUserProvider>();
            const string adminUsername = "admin";
            userProvider.Impersonate(adminUsername);
            try
            {
                var sqlConnections = services.GetRequiredService<ISqlConnections>();

                using var defaultConnection = sqlConnections.NewFor<Administration.UserRow>();

                var userListHandler = services.GetRequiredService<Administration.IUserListHandler>();
                var users = (await userListHandler.ListAsync(defaultConnection, new()
                {
                    ColumnSelection = ColumnSelection.Details
                }, cancellationToken)).Entities;
                var adminUser = Assert.Single(users);
                Assert.Equal(adminUsername, adminUser.Username);

                using var northwindConnection = sqlConnections.NewFor<Serenity.Demo.Northwind.OrderDetailRow>();

                var orderDetailListHandler = services.GetRequiredService<Serenity.Demo.Northwind.IOrderDetailListHandler>();
                var orderDetails = (await orderDetailListHandler.ListAsync(northwindConnection, new()
                {
                    ColumnSelection = ColumnSelection.Details
                }, cancellationToken)).Entities;

                Assert.Equal(2155, orderDetails.Count);

                // optimistic concurrency: the concurrency version trigger increments RowVersion
                // on update, and a stale RowVersion in the request causes a concurrency conflict
                var productRetrieve = services.GetRequiredService<Serenity.Demo.Northwind.IProductRetrieveHandler>();
                var product = (await productRetrieve.RetrieveAsync(northwindConnection, new()
                {
                    EntityId = 1
                }, cancellationToken)).Entity!;
                var initialVersion = product.RowVersion;
                Assert.NotNull(initialVersion);

                var productSave = services.GetRequiredService<Serenity.Demo.Northwind.IProductSaveHandler>();
                product.UnitsInStock = 0;
                using (var uow = new UnitOfWork(northwindConnection))
                {
                    await productSave.UpdateAsync(uow, new()
                    {
                        EntityId = 1,
                        Entity = product
                    }, cancellationToken);
                    await uow.CommitAsync(cancellationToken);
                }

                var reloaded = (await productRetrieve.RetrieveAsync(northwindConnection, new()
                {
                    EntityId = 1
                }, cancellationToken)).Entity!;
                Assert.Equal(initialVersion + 1, reloaded.RowVersion);

                // product still carries the old RowVersion, so this update must be rejected
                product.UnitsInStock = 1;
                using var staleUow = new UnitOfWork(northwindConnection);
                var ex = await Assert.ThrowsAsync<ValidationError>(() => productSave.UpdateAsync(staleUow, new()
                {
                    EntityId = 1,
                    Entity = product
                }, cancellationToken));
                Assert.Equal("ConcurrencyConflict", ex.ErrorCode);
            }
            finally
            {
                userProvider.UndoImpersonate();
            }
        }
        finally
        {
            RowFieldsProvider.SetLocal(oldRowFieldsProvider);
        }
    }
}
