using MyRow = Serenity.Extensions.Entities.UserPreferenceRow;

namespace Serenity.Extensions.Repositories;

public class UserPreferenceRepositoryTests
{
    private static UserPreferenceRepository Create()
    {
        return new UserPreferenceRepository(new NullRequestContext().AsGuest("12345"));
    }

    [Fact]
    public void Update_Throws_For_Null_Arguments()
    {
        var repository = Create();
        using var connection = new MockDbConnection();
        var uow = new MockUnitOfWork(connection);

        Assert.Throws<ArgumentNullException>(() => repository.Update(uow, null!));
        Assert.Throws<ArgumentNullException>(() => repository.Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t"
        }));
        Assert.Throws<ArgumentNullException>(() => repository.Update(uow, new UserPreferenceUpdateRequest
        {
            Name = "n"
        }));
    }

    [Fact]
    public void Update_Deletes_When_Value_Empty()
    {
        var called = 0;
        using var connection = new MockDbConnection().InterceptExecuteNonQuery(_ =>
        {
            called++;
            return new OptionalValue<long?>(1);
        });
        var uow = new MockUnitOfWork(connection);

        var response = Create().Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = ""
        });

        Assert.NotNull(response);
        Assert.Equal(1, called);
    }

    [Fact]
    public void Update_Updates_Existing_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => new OptionalValue<long?>(1));
        var uow = new MockUnitOfWork(connection);

        var response = Create().Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        });

        Assert.NotNull(response);
    }

    [Fact]
    public void Update_Inserts_When_Update_Affects_No_Rows()
    {
        var calls = 0;
        using var connection = new MockDbConnection().InterceptExecuteNonQuery(_ =>
        {
            calls++;
            return new OptionalValue<long?>(calls == 1 ? 0 : 1);
        });
        var uow = new MockUnitOfWork(connection);

        Create().Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        });

        Assert.Equal(2, calls);
    }

    [Fact]
    public void Update_Retries_Update_When_Insert_Throws()
    {
        var calls = 0;
        using var connection = new MockDbConnection().InterceptExecuteNonQuery(_ =>
        {
            calls++;
            if (calls == 2)
                throw new InvalidOperationException("duplicate");
            return new OptionalValue<long?>(calls == 1 ? 0 : 1);
        });
        var uow = new MockUnitOfWork(connection);

        Create().Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        });

        Assert.Equal(3, calls);
    }

    [Fact]
    public void Update_Rethrows_When_Insert_And_Retry_Fail()
    {
        var calls = 0;
        using var connection = new MockDbConnection().InterceptExecuteNonQuery(_ =>
        {
            calls++;
            if (calls == 2)
                throw new InvalidOperationException("duplicate");
            return new OptionalValue<long?>(0);
        });
        var uow = new MockUnitOfWork(connection);

        Assert.Throws<InvalidOperationException>(() => Create().Update(uow, new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        }));
    }

    [Fact]
    public void Retrieve_Throws_For_Null_Arguments()
    {
        var repository = Create();
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentNullException>(() => repository.Retrieve(connection, null!));
        Assert.Throws<ArgumentNullException>(() => repository.Retrieve(connection, new UserPreferenceRetrieveRequest
        {
            PreferenceType = "t"
        }));
        Assert.Throws<ArgumentNullException>(() => repository.Retrieve(connection, new UserPreferenceRetrieveRequest
        {
            Name = "n"
        }));
    }

    [Fact]
    public void Retrieve_Returns_Empty_When_Not_Found()
    {
        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(null!));

        var response = Create().Retrieve(connection, new UserPreferenceRetrieveRequest
        {
            PreferenceType = "t",
            Name = "n"
        });

        Assert.Null(response.Value);
    }

    [Fact]
    public void Retrieve_Returns_Value_When_Found()
    {
        var row = new MyRow
        {
            UserPreferenceId = 1,
            UserId = 12345,
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        };

        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(row));

        var response = Create().Retrieve(connection, new UserPreferenceRetrieveRequest
        {
            PreferenceType = "t",
            Name = "n"
        });

        Assert.Equal("v", response.Value);
    }
}
