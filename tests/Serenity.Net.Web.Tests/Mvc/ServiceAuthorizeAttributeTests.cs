namespace Serenity.Tests.Web.Mvc;

public class ServiceAuthorizeAttributeTests
{
    private const string NotLoggedInErrorCode = "NotLoggedIn";
    private const string AccessDeniedErrorCode = "AccessDenied";

    [Fact]
    public void Passes_If_Permission_Is_Null_And_Context_Is_Logged_In()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute((string) null)
            },
            isLoggedIn: true);

        Assert.Null(sutResult);
    }

    [Fact]
    public void Passes_If_Permission_Is_Not_Null_And_Context_Has_That_Permission()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:Permission")
            },
            permissions: new[] {"Test:Permission"});

        Assert.Null(sutResult);
    }

    [Fact]
    public void Passes_If_OrPermission_Is_Not_Null_And_Context_Has_That_Permission()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new OrPermissionSetAttribute("Test:DoesntHave", "Test:Permission")
            },
            permissions: new[] {"Test:Permission"});

        Assert.Null(sutResult);
    }

    [Fact]
    public void Passes_If_There_Is_Any_ServiceAuthorizeAttribute_With_Override_True_After_This()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:DoesntHave"),
                new ServiceAuthorizeAttribute("Test:Permission")
            },
            permissions: new[] {"Test:Permission"});

        Assert.Null(sutResult);
    }

    [Fact]
    public void Passes_If_There_Is_Any_ServiceAuthorizeAttribute_With_Or_Permission_And_Override_True_After_This()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:DoesntHave"),
                new OrPermissionSetAttribute("Test:DoesntHave2", "Test:Permission")
            },
            permissions: new[] {"Test:Permission"});

        Assert.Null(sutResult);
    }

    [Fact]
    public void Fails_If_There_Is_Any_ServiceAuthorizeAttribute_With_Override_False_After_This()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:DoesntHave"),
                new OrPermissionSetAttribute("Test:DoesntHave2", "Test:Permission", setOverride: false)
            },
            permissions: new[] {"Test:Permission"});

        Assert.Equal(AccessDeniedErrorCode, sutResult.Error.Code);
    }

    [Fact]
    public void Fails_If_Users_Have_No_Permission_Even_On_Overrides()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
        {
            new OrPermissionSetAttribute("Test:DoesntHave2", "Test:DoesntHave3"),
            new ServiceAuthorizeAttribute("Test:DoesntHave"),
        });

        Assert.Equal(AccessDeniedErrorCode, sutResult.Error.Code);
    }

    [Fact]
    public void Fails_If_Permission_Is_Null_And_User_Is_Not_Logged_In_And_Have_No_Permissions()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute((string) null),
            },
            isLoggedIn: false);

        Assert.Equal(NotLoggedInErrorCode, sutResult.Error.Code);
    }

    [Fact]
    public void Passes_If_Permission_Is_Null_And_User_Is_Logged_In_And_Have_No_Permissions()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute((string) null),
            },
            isLoggedIn: true);
        
        Assert.Null(sutResult);
    }

    [Fact]
    public void Fails_With_AccessDenied_If_User_Is_Logged_In()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:Permission")
            },
            isLoggedIn: true);

        Assert.Equal(AccessDeniedErrorCode, sutResult.Error.Code);
    }

    [Fact]
    public void Fails_With_NotLoggedIn_If_User_Is_Not_Logged_In()
    {
        var sutResult = TestResourceFilters(new IResourceFilter[]
            {
                new ServiceAuthorizeAttribute("Test:Permission")
            },
            isLoggedIn: false);

        Assert.Equal(NotLoggedInErrorCode, sutResult.Error.Code);
    }

    [ReadPermission("Test:ReadPermission")]
    [UpdatePermission("Test:UpdatePermission")]
    [DisplayName("Will throw when we set this on constructor's attribute types")]
    private record TypeWithReadPermissionAttribute;

    [Fact]
    public void Constructor_With_Type_Uses_Types_ReadPermissionAttribute_As_Permission()
    {
        var attr = new ServiceAuthorizeAttribute(typeof(TypeWithReadPermissionAttribute));

        Assert.Equal("Test:ReadPermission", attr.Permission);
    }

    [Fact]
    public void Constructor_With_Type_And_AttributeTypes_Tries_To_Find_First_Match_To_Use_As_Permission()
    {
        var attr = new CallConstructorWithTypeAndParamsTypeAttribute(typeof(TypeWithReadPermissionAttribute),
            typeof(InsertPermissionAttribute),
            typeof(UpdatePermissionAttribute));

        Assert.Equal("Test:UpdatePermission", attr.Permission);
    }

    [Fact]
    public void Constructor_With_Type_Throws_If_SourceType_Is_Null()
    {
        Assert.Throws<ArgumentNullException>("sourceType", () => new ServiceAuthorizeAttribute(null));
    }

    [Fact]
    public void Constructor_With_Type_And_Params_Type_List_Throws_If_SourceType_Is_Null()
    {
        Assert.Throws<ArgumentNullException>("attributeTypes", () =>
            new CallConstructorWithTypeAndParamsTypeAttribute(typeof(TypeWithReadPermissionAttribute)));
    }

    [Fact]
    public void Constructor_With_Type_And_Params_Type_List_Throws_If_Attribute_Is_Not_Subclass_Of_PermissionAttributeBase()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CallConstructorWithTypeAndParamsTypeAttribute(typeof(TypeWithReadPermissionAttribute), typeof(DisplayNameAttribute)));

        Assert.Equal(nameof(DisplayNameAttribute) + " is not a subclass of PermissionAttributeBase!", ex.ParamName);
    }

    private record TypeWithoutAnyAttributes;

    [Fact]
    public void Constructor_With_Type_Throws_If_There_Are_No_Attributes_To_Use_As_Permission()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CallConstructorWithTypeAndParamsTypeAttribute(typeof(TypeWithoutAnyAttributes), typeof(ReadPermissionAttribute)));

        Assert.Equal("ServiceAuthorize attribute is created with source type of TypeWithoutAnyAttributes" +
                     ", but it has no ReadPermissionAttribute attribute(s) (Parameter 'sourceType')", ex.Message);
    }

    [Fact]
    public void Constructor_With_Type_Throws_With_Joined_Attribute_Names_If_There_Are_No_Attributes_To_Use_As_Permission()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CallConstructorWithTypeAndParamsTypeAttribute(typeof(TypeWithoutAnyAttributes),
                typeof(ReadPermissionAttribute),
                typeof(UpdatePermissionAttribute)));

        Assert.Equal("ServiceAuthorize attribute is created with source type of TypeWithoutAnyAttributes" +
                     ", but it has no ReadPermissionAttribute OR UpdatePermissionAttribute attribute(s) (Parameter 'sourceType')", ex.Message);
    }

    [Fact]
    public void Constructor_With_Two_Object_Parameters_Joins_Params()
    {
        var attr = new ServiceAuthorizeAttribute("Test", "Permission");
        
        Assert.Equal("Test:Permission", attr.Permission);
    }

    [Fact]
    public void Constructor_With_Three_Object_Parameters_Joins_Params()
    {
        var attr = new ServiceAuthorizeAttribute("Test", "Permission", "LastPart");
        
        Assert.Equal("Test:Permission:LastPart", attr.Permission);
    }
    
    /// <summary>
    /// Runs all the <see cref="IResourceFilter"/>'s on a newly created <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="testFilters"><see cref="IResourceFilter"/>'s to run.</param>
    /// <param name="isLoggedIn">Sets if the user is logged in.</param>
    /// <param name="permissions">Sets the permissions to grant.</param>
    /// <returns>Returns <see cref="ResourceExecutingContext"/>'s result casted as <see cref="ServiceResponse"/>.</returns>
    private ServiceResponse TestResourceFilters(IResourceFilter[] testFilters, bool isLoggedIn = true, IEnumerable<string> permissions = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ITextLocalizer, MockTextLocalizer>();
        serviceCollection.AddSingleton<IPermissionService>(new MockPermissions((perm) => permissions?.Contains(perm) == true));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), isLoggedIn ? "Authenticated" : ""));

        var httpContext = new DefaultHttpContext()
        {
            RequestServices = serviceCollection.BuildServiceProvider(),
            User = principal
        };

        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var modelStateDictionary = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor, modelStateDictionary);

        var filters = new List<IFilterMetadata>(testFilters);
        var valueProviderFactories = new List<IValueProviderFactory>();
        var resourceExecutingContext = new ResourceExecutingContext(actionContext, filters, valueProviderFactories);

        foreach (var filter in testFilters)
            filter.OnResourceExecuting(resourceExecutingContext);

        return (resourceExecutingContext.Result as Result<ServiceResponse>)?.Data;
    }

    private class OrPermissionSetAttribute : ServiceAuthorizeAttribute
    {
        public OrPermissionSetAttribute(string permission, string orPermission, bool setOverride = true) : base(permission)
        {
            OrPermission = orPermission;
            Override = setOverride;
        }
    }

    private class CallConstructorWithTypeAndParamsTypeAttribute : ServiceAuthorizeAttribute
    {
        public CallConstructorWithTypeAndParamsTypeAttribute(Type sourceType, params Type[] attributeTypes)
            : base(sourceType, attributeTypes)
        {
        }
    }
}