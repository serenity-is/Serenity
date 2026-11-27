using System.Threading;

namespace Serenity.Extensions;

public class AccountEventPublisherTests
{
    private sealed class Users(params IUserDefinition[] users) : IUserRetrieveService
    {
        /// <summary>The number of the reads, which would wait for the lock of a transaction that changed the user.</summary>
        public int Reads;
        public IUserDefinition? ById(string id) { Reads++; return users.FirstOrDefault(x => x.Id == id); }
        public IUserDefinition? ByUsername(string username) { Reads++; return users.FirstOrDefault(x => x.Username == username); }
    }

    private sealed class CaptureHandler(bool fail = false) : IAccountEventHandler
    {
        public List<AccountEvent> Events { get; } = [];

        public Task HandleAsync(AccountEvent accountEvent, CancellationToken cancellationToken = default)
        {
            Events.Add(accountEvent);
            if (fail)
                throw new InvalidOperationException("boom");
            return Task.CompletedTask;
        }
    }

    private sealed class TestUnitOfWork : IUnitOfWork
    {
        public IDbConnection Connection => null!;
        public event Action? OnCommit;
        public event Action? OnRollback { add { } remove { } }
        public void Commit() => OnCommit?.Invoke();
    }

    private static readonly Users users = new(
        new MockUserDefinition("1", "admin", "Administrator", "admin@example.test"),
        new MockUserDefinition("5", "nancy", "Nancy Davolio", "nancy@example.test"));

    private static AccountEventPublisher Publisher(string? currentUser, params IAccountEventHandler[] handlers) =>
        Publisher(users, currentUser, handlers);

    private static AccountEventPublisher Publisher(Users retriever, string? currentUser, params IAccountEventHandler[] handlers) =>
        new(handlers, retriever, new MockUserAccessor(() => currentUser!,
            () => currentUser == "admin" ? "1" : currentUser == "nancy" ? "5" : null!));

    [Fact]
    public void Fills_The_User_And_Who_Did_It()
    {
        var handler = new CaptureHandler();
        Publisher("admin", handler).Publish(new AccountEvent { Kind = AccountEventKind.PasswordChanged, UserId = "5" });

        var e = Assert.Single(handler.Events);
        Assert.Equal("nancy", e.Username);
        Assert.Equal("Nancy Davolio", e.DisplayName);
        Assert.Equal("nancy@example.test", e.Email);
        Assert.Equal("1", e.ActorUserId);
        Assert.Equal("Administrator", e.ActorDisplayName);
        Assert.False(e.IsSelf);
        Assert.NotEqual(default, e.Time);
    }

    [Fact]
    public void An_Event_Of_The_Own_Account_Or_Without_A_Signed_In_User_Is_The_Users_Own()
    {
        var handler = new CaptureHandler();
        Publisher("nancy", handler).Publish(new AccountEvent { Kind = AccountEventKind.PasswordChanged, UserId = "5" });
        Publisher(null, handler).Publish(new AccountEvent { Kind = AccountEventKind.PasswordChanged, UserId = "5" });

        Assert.All(handler.Events, e => Assert.True(e.IsSelf));
        Assert.Null(handler.Events[1].ActorUserId);
    }

    [Fact]
    public void Tells_The_Handlers_When_The_Unit_Of_Work_Is_Committed()
    {
        var handler = new CaptureHandler();
        var uow = new TestUnitOfWork();
        var retriever = new Users(new MockUserDefinition("1", "admin", "Administrator"), new MockUserDefinition("5", "nancy", "Nancy"));
        Publisher(retriever, "admin", handler).Publish(new AccountEvent { Kind = AccountEventKind.EmailChanged, UserId = "5" }, uow);
        Assert.Empty(handler.Events);
        // the users are not read in the transaction, which may lock the row of the user that they are read from
        Assert.Equal(0, retriever.Reads);

        uow.Commit();
        var e = Assert.Single(handler.Events);
        Assert.Equal(("nancy", "Administrator"), (e.Username, e.ActorDisplayName));
    }

    [Fact]
    public void A_Failing_Handler_Does_Not_Stop_The_Others_Nor_The_Change()
    {
        var failing = new CaptureHandler(fail: true);
        var other = new CaptureHandler();
        Publisher("admin", failing, other).Publish(new AccountEvent { Kind = AccountEventKind.PasswordChanged, UserId = "5" });

        Assert.Single(failing.Events);
        Assert.Single(other.Events);
    }
}
