namespace Serenity.Testing;

public class FuncMockUserRetrieveService(Func<string, IUserDefinition> getUserDefinitionFunc) : IUserRetrieveService
{
    public Func<string, IUserDefinition> GetUserDefinitionFunc { get; set; } = getUserDefinitionFunc;

    public IUserDefinition ById(string id)
    {
        return GetUserDefinitionFunc(id);
    }

    public IUserDefinition ByUsername(string username)
    {
        return GetUserDefinitionFunc(username);
    }
}
