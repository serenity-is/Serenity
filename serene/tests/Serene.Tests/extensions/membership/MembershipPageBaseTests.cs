namespace Serenity.Extensions;

public class MembershipPageBaseTests
{
    private class TestMembershipPage : MembershipPageBase<MockUserRow>
    {
        public ActionResult CallError(string message) => Error(message);
        public string CallGenerateSalt(MembershipSettings settings) => GenerateSalt(settings);
        public string CallCalculateHash(string password, string salt) => CalculateHash(password, salt);
        public int CallGetNonceFor(MockUserRow user) => GetNonceFor(user);
        public string CallGetConnectionKey() => GetConnectionKey();
    }

    private static TestMembershipPage CreatePage()
    {
        return new TestMembershipPage
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public void Error_Returns_ValidationError_View()
    {
        var result = CreatePage().CallError("oops");
        var viewResult = Assert.IsType<ViewResult>(result);
        var error = Assert.IsType<ValidationError>(viewResult.ViewData.Model);
        Assert.Equal("oops", error.Message);
    }

    [Fact]
    public void GenerateSalt_Returns_Configured_Length()
    {
        var salt = CreatePage().CallGenerateSalt(new MembershipSettings { SaltSize = 8 });
        Assert.Equal(8, salt.Length);
    }

    [Fact]
    public void CalculateHash_Is_Deterministic()
    {
        var page = CreatePage();
        var hash1 = page.CallCalculateHash("password", "salt");
        var hash2 = page.CallCalculateHash("password", "salt");
        var hash3 = page.CallCalculateHash("password", "other");

        Assert.Equal(hash1, hash2);
        Assert.NotEqual(hash1, hash3);
    }

    [Fact]
    public void GetDeterministicHashCode_Is_Deterministic_For_Odd_And_Even_Lengths()
    {
        Assert.Equal(MembershipPageBase<MockUserRow>.GetDeterministicHashCode("abc"),
            MembershipPageBase<MockUserRow>.GetDeterministicHashCode("abc"));
        Assert.Equal(MembershipPageBase<MockUserRow>.GetDeterministicHashCode("abcd"),
            MembershipPageBase<MockUserRow>.GetDeterministicHashCode("abcd"));
        Assert.NotEqual(MembershipPageBase<MockUserRow>.GetDeterministicHashCode("abc"),
            MembershipPageBase<MockUserRow>.GetDeterministicHashCode("xyz"));
    }

    [Fact]
    public void GetNonceFor_Uses_UpdateDate_When_Available()
    {
        var user = new MockUserRow
        {
            UpdateDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PasswordHash = "hash",
            PasswordSalt = "salt"
        };

        var nonce = CreatePage().CallGetNonceFor(user);
        Assert.Equal(MembershipPageBase<MockUserRow>.GetDeterministicHashCode(
            user.UpdateDate.Value.ToString("s") + "hash" + "salt"), nonce);
    }

    [Fact]
    public void GetNonceFor_Uses_InsertDate_When_No_UpdateDate()
    {
        var user = new MockUserRow
        {
            InsertDate = new DateTime(2024, 2, 2, 0, 0, 0, DateTimeKind.Utc),
            PasswordHash = "h",
            PasswordSalt = "s"
        };

        var nonce = CreatePage().CallGetNonceFor(user);
        Assert.Equal(MembershipPageBase<MockUserRow>.GetDeterministicHashCode(
            user.InsertDate.Value.ToString("s") + "h" + "s"), nonce);
    }

    [Fact]
    public void GetNonceFor_Falls_Back_To_Today()
    {
        var user = new MockUserRow();
        var nonce = CreatePage().CallGetNonceFor(user);
        Assert.Equal(MembershipPageBase<MockUserRow>.GetDeterministicHashCode(
            DateTime.Today.ToString("s")), nonce);
    }

    [Fact]
    public void GetConnectionKey_Returns_Attribute_Value()
    {
        Assert.Equal("Default", CreatePage().CallGetConnectionKey());
    }
}
