using Serenity.TypeScript;
using System.IO;
using Xunit.Sdk;

namespace Serenity.Tests.CodeGenerator;

public partial class TypeScriptTests
{
    [Fact]
    public void AsyncFunction_WithGenericPromise_TypeOfImport_ReturnAssignment()
    {
        var result = new Parser().ParseSourceFile("test", """"
            var l: any;
            async function x(): Promise<typeof import("l")> {
                return l = await import("x");
            }
            """");
        Assert.NotNull(result);
    }

    [Fact]
    public void AsyncFunction_WithGenericPromise_TypeOfVar_ReturnAssignment()
    {
        var result = new Parser().ParseSourceFile("test", """"
            var l: any;
            async function x(): Promise<typeof l> {
                return l = await import("x");
            }
            """");
        Assert.NotNull(result);
    }
}
