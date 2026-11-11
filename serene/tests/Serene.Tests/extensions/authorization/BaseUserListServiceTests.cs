using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class BaseUserListServiceTests
{
    [DefaultConnection, TableName("ListUsers")]
    private class UserRow : Row<UserRow.RowFields>, IIdRow, INameRow, IDisplayNameRow, IEmailRow, IIsActiveRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Username { get => fields.Username[this]; set => fields.Username[this] = value; }
        public string? DisplayName { get => fields.DisplayName[this]; set => fields.DisplayName[this] = value; }
        public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public string? PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }

        StringField IDisplayNameRow.DisplayNameField => fields.DisplayName;
        StringField IEmailRow.EmailField => fields.Email;
        Int16Field IIsActiveRow.IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Username = null!;
            public StringField DisplayName = null!;
            public StringField Email = null!;
            public Int16Field IsActive = null!;
            public StringField PasswordHash = null!;
        }
    }

    /// <summary>A row that keeps the normalized username, which the users are looked up by.</summary>
    [DefaultConnection, TableName("ListNormalizedUsers")]
    private class NormalizedUserRow : Row<NormalizedUserRow.RowFields>, IIdRow, INameRow, INormalizedUsernameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Username { get => fields.Username[this]; set => fields.Username[this] = value; }
        public string? NormalizedUsername { get => fields.NormalizedUsername[this]; set => fields.NormalizedUsername[this] = value; }

        StringField INormalizedUsernameRow.NormalizedUsernameField => fields.NormalizedUsername;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Username = null!;
            public StringField NormalizedUsername = null!;
        }
    }

    /// <summary>A row with only what is needed to tell a user.</summary>
    [DefaultConnection, TableName("ListMinimalUsers")]
    private class MinimalUserRow : Row<MinimalUserRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        [NameProperty]
        public string? Username { get => fields.Username[this]; set => fields.Username[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Username = null!;
        }
    }

    /// <summary>A database with only the default connection, as the rows use it.</summary>
    private sealed class MemoryDb : MemoryDbBase
    {
        public MemoryDb()
        {
            System.Data.Common.DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);
            byKey["Default"] = new MemoryDbInstance("BaseUserListServiceTests_");
        }
    }

    private class UserListService(ISqlConnections sqlConnections) : BaseUserListService<UserRow>(sqlConnections)
    {
        public List<string> Selected { get; } = [];

        protected override void SelectFields(SqlQuery query, UserRow row)
        {
            base.SelectFields(query, row);
            Selected.Add(query.ToString());
        }
    }

    private class NormalizedUserListService(ISqlConnections sqlConnections) : BaseUserListService<NormalizedUserRow>(sqlConnections)
    {
    }

    private class MinimalUserListService(ISqlConnections sqlConnections) : BaseUserListService<MinimalUserRow>(sqlConnections)
    {
    }

    private static void WithUsers(Action<UserListService, MemoryDb> action)
    {
        using var db = new MemoryDb();
        db.CreateTable<UserRow.RowFields>();
        using (var connection = db.NewFor<UserRow>())
        {
            void add(int id, string username, string? displayName, short isActive, string? email = null) =>
                connection.Insert(new UserRow
                {
                    Id = id,
                    Username = username,
                    DisplayName = displayName,
                    Email = email,
                    IsActive = isActive,
                    PasswordHash = "secret-hash"
                });

            add(1, "carol", "Carol Smith", 1, "carol@example.com");
            add(2, "alice", "Alice Jones", 1);
            add(3, "bob", null, 1);
            add(4, "dave", "Dave Disabled", 0);
            add(5, "erin", "Erin Deleted", -1);
            add(6, "frank", "Frank 50%", 1);
        }

        action(new UserListService(db), db);
    }

    [Fact]
    public void Constructor_Throws_For_Null_SqlConnections()
    {
        Assert.Throws<ArgumentNullException>(() => new UserListService(null!));
    }

    [Fact]
    public void Lists_The_Active_Users_By_Username()
    {
        WithUsers((service, _) =>
        {
            var users = service.ListUsers();

            Assert.Equal(["alice", "bob", "carol", "frank"], users.Select(x => x.Username));
            Assert.All(users, x => Assert.Equal(1, x.IsActive));
        });
    }

    [Fact]
    public void Lists_The_Disabled_Users_On_Request_But_Never_The_Deleted_Ones()
    {
        WithUsers((service, _) =>
        {
            var users = service.ListUsers(new UserListOptions { IncludeInactive = true });

            Assert.Equal(["alice", "bob", "carol", "dave", "frank"], users.Select(x => x.Username));
            Assert.Equal(0, users.Single(x => x.Username == "dave").IsActive);
        });
    }

    [Fact]
    public void Gives_The_Properties_Of_A_User_Definition()
    {
        WithUsers((service, _) =>
        {
            var users = service.ListUsers();

            var carol = users.Single(x => x.Username == "carol");
            Assert.Equal("1", carol.Id);
            Assert.Equal("Carol Smith", carol.DisplayName);
            Assert.Equal("carol@example.com", carol.Email);

            // a user without a display name is shown with the username, and without an email has none
            var bob = users.Single(x => x.Username == "bob");
            Assert.Equal("bob", bob.DisplayName);
            Assert.Null(bob.Email);
        });
    }

    [Fact]
    public void Only_Selects_What_A_User_Of_The_List_Has()
    {
        WithUsers((service, _) =>
        {
            service.ListUsers();

            var sql = Assert.Single(service.Selected);
            Assert.DoesNotContain("PasswordHash", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("DisplayName", sql, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Theory]
    [InlineData("ali", new[] { "alice" })]
    [InlineData("SMITH", new[] { "carol" })]
    [InlineData("  bob ", new[] { "bob" })]
    [InlineData("a", new[] { "alice", "carol", "frank" })]
    [InlineData("50%", new[] { "frank" })]
    [InlineData("%", new[] { "frank" })]
    [InlineData("zzz", new string[0])]
    [InlineData("dave", new string[0])]
    [InlineData("", new[] { "alice", "bob", "carol", "frank" })]
    [InlineData("   ", new[] { "alice", "bob", "carol", "frank" })]
    public void Searches_The_Usernames_And_Display_Names(string text, string[] expected)
    {
        WithUsers((service, _) =>
            Assert.Equal(expected, service.ListUsers(new UserListOptions { ContainsText = text }).Select(x => x.Username)));
    }

    [Fact]
    public void Limits_The_Users_After_Ordering_Them()
    {
        WithUsers((service, _) =>
        {
            Assert.Equal(["alice", "bob"], service.ListUsers(new UserListOptions { Take = 2 }).Select(x => x.Username));
            Assert.Equal(4, service.ListUsers(new UserListOptions { Take = 0 }).Count);
            Assert.Equal(4, service.ListUsers(new UserListOptions { Take = -1 }).Count);
        });
    }

    [Fact]
    public void Lists_The_Users_Of_A_Row_With_Only_An_Id_And_A_Name()
    {
        using var db = new MemoryDb();
        db.CreateTable<MinimalUserRow.RowFields>();
        using (var connection = db.NewFor<MinimalUserRow>())
        {
            connection.Insert(new MinimalUserRow { Id = 2, Username = "zed" });
            connection.Insert(new MinimalUserRow { Id = 1, Username = "amy" });
        }

        var users = new MinimalUserListService(db).ListUsers(new UserListOptions { ContainsText = "m" });

        // there is nothing to tell inactive users with, so they are all listed as active
        var amy = Assert.Single(users);
        Assert.Equal("1", amy.Id);
        Assert.Equal("amy", amy.Username);
        Assert.Equal("amy", amy.DisplayName);
        Assert.Equal(1, amy.IsActive);
        Assert.Equal(["amy", "zed"], new MinimalUserListService(db).ListUsers().Select(x => x.Username));
    }

    [Fact]
    public void Lists_The_Users_Asked_For_By_Identifier_Whatever_Their_State()
    {
        WithUsers((service, _) =>
        {
            // to show who the records of another database refer to, even if they are disabled or deleted now
            var users = service.ListUsers(new UserListOptions { UserIds = ["5", "4", "1", "999", "abc", " ", ""] });

            Assert.Equal(["carol", "dave", "erin"], users.Select(x => x.Username));
            Assert.Equal([1, 0, -1], users.Select(x => (int)x.IsActive));
        });
    }

    [Fact]
    public void Lists_The_Users_Asked_For_By_Username()
    {
        WithUsers((service, _) =>
        {
            var users = service.ListUsers(new UserListOptions { Usernames = ["erin", "alice", " bob ", "nobody"] });

            Assert.Equal(["alice", "bob", "erin"], users.Select(x => x.Username));
            Assert.Equal("Alice Jones", users[0].DisplayName);
        });
    }

    [Fact]
    public void Lists_The_Users_Of_Either_When_Both_Are_Asked_For_Once_Each()
    {
        WithUsers((service, _) =>
        {
            var users = service.ListUsers(new UserListOptions { UserIds = ["1", "2"], Usernames = ["alice", "frank"] });

            Assert.Equal(["alice", "carol", "frank"], users.Select(x => x.Username));
        });
    }

    [Fact]
    public void Lists_Nobody_When_An_Empty_List_Is_Asked_For()
    {
        WithUsers((service, _) =>
        {
            Assert.Empty(service.ListUsers(new UserListOptions { UserIds = [] }));
            Assert.Empty(service.ListUsers(new UserListOptions { Usernames = [] }));
            Assert.Empty(service.ListUsers(new UserListOptions { UserIds = ["abc"] }));
            Assert.Empty(service.ListUsers(new UserListOptions { UserIds = [], Usernames = [" "] }));
            // while null is no restriction
            Assert.Equal(4, service.ListUsers(new UserListOptions { UserIds = null, Usernames = null }).Count);
        });
    }

    [Fact]
    public void Narrows_The_Users_Asked_For_With_The_Text_And_The_Limit()
    {
        WithUsers((service, _) =>
        {
            var ids = new[] { "1", "2", "3", "4" };

            Assert.Equal(["alice", "carol", "dave"], service.ListUsers(new UserListOptions { UserIds = ids, ContainsText = "a" })
                .Select(x => x.Username));
            Assert.Equal(["alice", "bob"], service.ListUsers(new UserListOptions { UserIds = ids, Take = 2 })
                .Select(x => x.Username));
        });
    }

    [Fact]
    public void Lists_Many_Users_Asked_For_With_More_Than_One_Query_In_Order()
    {
        using var db = new MemoryDb();
        db.CreateTable<UserRow.RowFields>();
        using (var connection = db.NewFor<UserRow>())
            for (var i = 1; i <= 1100; i++)
                connection.Insert(new UserRow { Id = i, Username = "user" + i.ToString("0000", System.Globalization.CultureInfo.InvariantCulture), IsActive = 1 });

        var service = new UserListService(db);
        var ids = Enumerable.Range(1, 1100).Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

        var users = service.ListUsers(new UserListOptions { UserIds = ids });
        Assert.Equal(1100, users.Count);
        Assert.Equal(users.Select(x => x.Username).Order(StringComparer.Ordinal), users.Select(x => x.Username));

        // the limit is applied to all of them, not to each query
        Assert.Equal(["user0001", "user0002", "user0003"], service.ListUsers(new UserListOptions { UserIds = ids, Take = 3 })
            .Select(x => x.Username));

        var names = ids.Select(x => "user" + x.PadLeft(4, '0'));
        Assert.Equal(1100, service.ListUsers(new UserListOptions { Usernames = names }).Count);
    }

    [Fact]
    public void Looks_Users_Up_By_The_Normalized_Username_When_The_Row_Has_It()
    {
        using var db = new MemoryDb();
        db.CreateTable<NormalizedUserRow.RowFields>();
        using (var connection = db.NewFor<NormalizedUserRow>())
        {
            connection.Insert(new NormalizedUserRow { Id = 1, Username = "Alice", NormalizedUsername = BaseUserRetrieveService.NormalizeUsername("Alice") });
            connection.Insert(new NormalizedUserRow { Id = 2, Username = "Işık", NormalizedUsername = BaseUserRetrieveService.NormalizeUsername("Işık") });
        }

        var service = new NormalizedUserListService(db);

        // whatever the collation of the database does, as for the users that are retrieved by their usernames
        Assert.Equal(["Alice"], service.ListUsers(new UserListOptions { Usernames = ["aLICE"] }).Select(x => x.Username));
        Assert.Equal(["Işık"], service.ListUsers(new UserListOptions { Usernames = ["işık"] }).Select(x => x.Username));

        // and so is the text searched in them
        Assert.Equal(["Alice"], service.ListUsers(new UserListOptions { ContainsText = "LIC" }).Select(x => x.Username));
        Assert.Equal(["Işık"], service.ListUsers(new UserListOptions { ContainsText = "şı" }).Select(x => x.Username));
    }

    [Fact]
    public void AddUserListService_Registers_The_Service()
    {
        using var db = new MemoryDb();
        var provider = new ServiceCollection()
            .AddSingleton<ISqlConnections>(db)
            .AddUserListService<UserListService>()
            .BuildServiceProvider();

        Assert.IsType<UserListService>(provider.GetRequiredService<IUserListService>());
    }
}
