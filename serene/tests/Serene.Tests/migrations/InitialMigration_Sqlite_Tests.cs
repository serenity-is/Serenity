using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Data.Sqlite;
using Serene.Migrations.DefaultDB;
using Serenity.Extensions;
using System.Globalization;

namespace Serene.Tests;

public class InitialMigration_Sqlite_Tests
{
    // MigrationKey stores the versions in yyyyMMdd_HHmm_ss format
    private const long InitialVersion = 20141103_1400_00;

    private static void UsingInitialDatabase(Action<UserEntityOptions> configure, Action<SqliteConnection> test)
    {
        var connectionString = $"Data Source=InitialMigration{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

        // the in-memory database lives as long as at least one connection is open
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using (var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .Configure(configure)
            .Configure<RunnerOptions>(options =>
            {
                options.Tags = [TargetDBAttribute.GetTag(DefaultConnectionAttribute.Key)];
                options.IncludeUntaggedMigrations = true;
            })
            .ConfigureRunner(builder =>
            {
                builder.AddSQLite()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(DefaultDB_20141103_1400_Initial).Assembly).For.Migrations();
                builder.WithRunnerConventions(new MigrationRunnerConventions
                {
                    TypeHasMatchingTags = MigrationTagUtils.CreateMatcher()
                });
            })
            .BuildServiceProvider())
        using (var scope = services.CreateScope())
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp(InitialVersion);

        test(connection);
    }

    private static (string UserId, string InsertUserId) GetAdmin(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT UserId, InsertUserId FROM Users WHERE Username = 'admin'";
        using var reader = command.ExecuteReader();
        Assert.True(reader.Read());
        return (Convert.ToString(reader.GetValue(0), CultureInfo.InvariantCulture)!,
            Convert.ToString(reader.GetValue(1), CultureInfo.InvariantCulture)!);
    }

    [Fact]
    public void AdminIsTheFirstIdentityValueForIntegerIds()
    {
        UsingInitialDatabase(_ => { }, connection =>
        {
            var admin = GetAdmin(connection);
            Assert.Equal("1", admin.UserId);
            Assert.Equal("1", admin.InsertUserId);
        });
    }

    [Fact]
    public void AdminGetsTheSystemUserIdForGuidIds()
    {
        var id = Guid.NewGuid();
        UsingInitialDatabase(options =>
        {
            options.IdFieldType = typeof(GuidField);
            options.SystemUserId = id;
        }, connection =>
        {
            // SQLite stores the GUID parameter as text, so compare the string forms
            var admin = GetAdmin(connection);
            Assert.Equal(id.ToString(), admin.UserId, ignoreCase: true);
            Assert.Equal(id.ToString(), admin.InsertUserId, ignoreCase: true);
        });
    }

    [Fact]
    public void ThrowsWhenSystemUserIdIsNotSetForGuidIds()
    {
        var exception = Assert.ThrowsAny<Exception>(() => UsingInitialDatabase(
            options => options.IdFieldType = typeof(GuidField), _ => { }));

        Assert.Contains(nameof(UserEntityOptions.SystemUserId), exception.ToString());
    }
}
