using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class BaseRoleListServiceTests
{
    [DefaultConnection, TableName("ListRoles")]
    private class RoleRow : Row<RoleRow.RowFields>, IIdRow, INameRow, IRoleKeyRow
    {
        [IdProperty]
        public int? RoleId { get => fields.RoleId[this]; set => fields.RoleId[this] = value; }
        [NameProperty]
        public string? RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }
        public string? RoleKey { get => fields.RoleKey[this]; set => fields.RoleKey[this] = value; }

        StringField IRoleKeyRow.RoleKeyField => fields.RoleKey;

        public class RowFields : RowFieldsBase
        {
            public Int32Field RoleId = null!;
            public StringField RoleName = null!;
            public StringField RoleKey = null!;
        }
    }

    /// <summary>A row with only an id and a name.</summary>
    [DefaultConnection, TableName("ListMinimalRoles")]
    private class MinimalRoleRow : Row<MinimalRoleRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Name = null!;
        }
    }

    /// <summary>A database with only the default connection, as the rows use it.</summary>
    private sealed class MemoryDb : MemoryDbBase
    {
        public MemoryDb()
        {
            System.Data.Common.DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);
            byKey["Default"] = new MemoryDbInstance("BaseRoleListServiceTests_");
        }
    }

    private class RoleListService(ISqlConnections sqlConnections) : BaseRoleListService<RoleRow>(sqlConnections)
    {
    }

    private class MinimalRoleListService(ISqlConnections sqlConnections) : BaseRoleListService<MinimalRoleRow>(sqlConnections)
    {
    }

    private static void WithRoles(Action<RoleListService> action)
    {
        using var db = new MemoryDb();
        db.CreateTable<RoleRow.RowFields>();
        using (var connection = db.NewFor<RoleRow>())
        {
            connection.Insert(new RoleRow { RoleId = 1, RoleName = "Sales Managers", RoleKey = "sales-managers" });
            connection.Insert(new RoleRow { RoleId = 2, RoleName = "Accountants" });
            connection.Insert(new RoleRow { RoleId = 3, RoleName = "Administrators", RoleKey = "" });
            connection.Insert(new RoleRow { RoleId = 4, RoleName = "Support", RoleKey = "help-desk" });
        }

        action(new RoleListService(db));
    }

    [Fact]
    public void Constructor_Throws_For_Null_SqlConnections()
    {
        Assert.Throws<ArgumentNullException>(() => new RoleListService(null!));
    }

    [Fact]
    public void Lists_The_Roles_By_Name_With_Their_Keys()
    {
        WithRoles(service =>
        {
            var roles = service.ListRoles();

            Assert.Equal(["Accountants", "Administrators", "Sales Managers", "Support"], roles.Select(x => x.Name));
            Assert.Equal(["2", "3", "1", "4"], roles.Select(x => x.Id));
            // a blank key is no key
            Assert.Equal([null, null, "sales-managers", "help-desk"], roles.Select(x => x.RoleKey));
        });
    }

    [Fact]
    public void Tells_The_Key_Or_Else_The_Name_As_What_Identifies_The_Role()
    {
        WithRoles(service =>
            Assert.Equal(["Accountants", "Administrators", "sales-managers", "help-desk"],
                service.ListRoles().Select(x => x.GetKeyOrName())));
    }

    [Theory]
    [InlineData("sales", new[] { "Sales Managers" })]
    [InlineData("HELP", new[] { "Support" })]
    [InlineData("  admin ", new[] { "Administrators" })]
    [InlineData("an", new[] { "Accountants", "Sales Managers" })]
    [InlineData("zzz", new string[0])]
    [InlineData("", new[] { "Accountants", "Administrators", "Sales Managers", "Support" })]
    public void Searches_The_Names_And_Keys(string text, string[] expected)
    {
        WithRoles(service =>
            Assert.Equal(expected, service.ListRoles(new RoleListOptions { ContainsText = text }).Select(x => x.Name)));
    }

    [Fact]
    public void Limits_The_Roles_After_Ordering_Them()
    {
        WithRoles(service =>
        {
            Assert.Equal(["Accountants", "Administrators"], service.ListRoles(new RoleListOptions { Take = 2 }).Select(x => x.Name));
            Assert.Equal(4, service.ListRoles(new RoleListOptions { Take = 0 }).Count);
        });
    }

    [Fact]
    public void Lists_The_Roles_Of_A_Row_Without_A_Key()
    {
        using var db = new MemoryDb();
        db.CreateTable<MinimalRoleRow.RowFields>();
        using (var connection = db.NewFor<MinimalRoleRow>())
            connection.Insert(new MinimalRoleRow { Id = 7, Name = "Auditors" });

        var role = Assert.Single(new MinimalRoleListService(db).ListRoles(new RoleListOptions { ContainsText = "audit" }));

        Assert.Equal("7", role.Id);
        Assert.Equal("Auditors", role.Name);
        Assert.Null(role.RoleKey);
        Assert.Equal("Auditors", role.GetKeyOrName());
    }

    [Fact]
    public void GetKeyOrName_Throws_For_A_Null_Role()
    {
        Assert.Throws<ArgumentNullException>(() => RoleDefinitionExtensions.GetKeyOrName(null!));
    }

    [Fact]
    public void AddRoleListService_Registers_The_Service()
    {
        using var db = new MemoryDb();
        var provider = new ServiceCollection()
            .AddSingleton<ISqlConnections>(db)
            .AddRoleListService<RoleListService>()
            .BuildServiceProvider();

        Assert.IsType<RoleListService>(provider.GetRequiredService<IRoleListService>());
    }
}
