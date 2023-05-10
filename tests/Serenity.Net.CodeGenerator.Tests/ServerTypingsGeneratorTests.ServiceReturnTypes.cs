using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ServerTypingsTest.ServiceReturnTypes;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Return_Types_That_ShouldBe_Generated()
        {
            var controllerType = typeof(ResponseTypesThatShouldBeGenerated);
            var generator = CreateGenerator(controllerType);
            var result = generator.Run();
            var filename = "ServiceReturnTypes." + nameof(ResponseTypesThatShouldBeGenerated) + "Service.ts";
            var code = Assert.Single(result, x => x.Filename == filename).Text;
            var methods = controllerType.GetMethods().Where(x => x.DeclaringType == controllerType);
            foreach (var method in methods)
            {
                Assert.Contains("function " + method.Name, code);
            }
        }

        [Fact]
        public void Return_Types_That_ShouldNotBe_Generated()
        {
            var controllerType = typeof(ResponseTypesThatShouldNotBeGenerated);
            var generator = CreateGenerator(controllerType);
            var result = generator.Run();
            var filename = "ServiceReturnTypes." + nameof(ResponseTypesThatShouldNotBeGenerated) + "Service.ts";
            var code = Assert.Single(result, x => x.Filename == filename).Text;
            var methods = controllerType.GetMethods().Where(x => x.DeclaringType == controllerType);
            foreach (var method in methods)
            {
                Assert.DoesNotContain("function " + method.Name, code);
            }
        }

        [Fact]
        public void Return_Types_That_ShouldBe_Generated_As_Key_Value()
        {
            var controllerType = typeof(ResponseTypesThatShouldBeGeneratedAsKeyValue);
            var generator = CreateGenerator(controllerType);
            var filename = "ServiceReturnTypes." + nameof(ResponseTypesThatShouldBeGeneratedAsKeyValue) + "Service.ts";
            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == filename).Text;
            var methods = controllerType.GetMethods().Where(x => x.DeclaringType == controllerType);
            foreach (var method in methods)
            {
                Assert.True(
                    code.Contains("function " + method.Name + "(request: ListRequest, onSuccess?: (response: { [key: string]: any }) => void, opt?: Q.ServiceOptions<any>)") ||
                    code.Contains("function " + method.Name + "(request: Serenity.ListRequest, onSuccess?: (response: { [key: string]: any }) => void, opt?: Q.ServiceOptions<any>)"));
            }
        }

        [Fact]
        public void Return_Types_That_ShouldBe_Generated_As_List()
        {
            var controllerType = typeof(ResponseTypesThatShouldBeGeneratedAsList);
            var generator = CreateGenerator(controllerType);
            var result = generator.Run();
            var filename = "ServiceReturnTypes." + nameof(ResponseTypesThatShouldBeGeneratedAsList) + "Service.ts";
            var code = Assert.Single(result, x => x.Filename == filename).Text;
            var methods = controllerType.GetMethods().Where(x => x.DeclaringType == controllerType);
            foreach (var method in methods)
            {
                Assert.True(
                    code.Contains("function " + method.Name + "(request: ListRequest, onSuccess?: (response: any[]) => void, opt?: Q.ServiceOptions<any>)") ||
                    code.Contains("function " + method.Name + "(request: Serenity.ListRequest, onSuccess?: (response: any[]) => void, opt?: Q.ServiceOptions<any>)"));
            }
        }
    }
}

namespace ServerTypingsTest.ServiceReturnTypes
{ 
    public class ResponseTypesThatShouldBeGenerated : ServiceEndpoint
    {
        public TestResponseClass Control(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public TestDerivedResponseClass ControlDerived(IDbConnection connection, ListRequest request)
        {
            return null;
        }
    }

    public class ResponseTypesThatShouldNotBeGenerated: ServiceEndpoint
    {

        public IActionResult IActionResultResponse(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public A InheritsFromIActionResult(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public B InheritsFromA(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public C InterfaceInteritsFromIActionResult(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public D InterfaceInteritsFromC(IDbConnection connection, ListRequest request)
        {
            return null;
        }

    }

    public class ResponseTypesThatShouldBeGeneratedAsKeyValue : ServiceEndpoint
    {
        public Dictionary<string, object> HandleDictionaryAsKeyValue(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public IDictionary<string, object> HandleIDictionaryAsKeyValue(IDbConnection connection, ListRequest request)
        {
            return null;
        }
    }

    public class ResponseTypesThatShouldBeGeneratedAsList : ServiceEndpoint
    {
        public List<object> HandleListAsList(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public HashSet<object> HandleHashSetAsList(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public IList<object> HandleIListAsList(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public IEnumerable<object> HandleIEnumerableAsList(IDbConnection connection, ListRequest request)
        {
            return null;
        }

        public ISet<object> HandleISetAsList(IDbConnection connection, ListRequest request)
        {
            return null;
        }
    }

    public class TestResponseClass
    {
        public string Name { get; set; }
    }

    public class TestDerivedResponseClass : TestResponseClass
    {
        public string AnotherThing { get; set; }
    }

    public class A : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class B : A
    {

    }

    public interface C : IActionResult
    {

    }

    public interface D : C
    {

    }
}
