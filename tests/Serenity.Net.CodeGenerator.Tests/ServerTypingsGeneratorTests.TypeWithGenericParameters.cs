using ServerTypingsTest.TypeWithGenericParameters;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void HandlesTypeWithOneGenericParameter()
        {
            var generator = CreateGenerator(typeof(TypeWithOneGenericParameter<>));
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var code = Assert.Single(result).Text;
            Assert.Contains("namespace ServerTypingsTest.TypeWithGenericParameters {", code);
            Assert.True(
                code.Contains("export interface TypeWithOneGenericParameter<T> extends ServiceRequest") ||
                code.Contains("export interface TypeWithOneGenericParameter<T> extends Serenity.ServiceRequest"));
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithOneGenericParameterAndBaseClass()
        {
            var generator = CreateGenerator(typeof(TypeWithOneGenericAndBase<>));
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var code = Assert.Single(result).Text;
            Assert.Contains("namespace ServerTypingsTest.TypeWithGenericParameters {", code);
            Assert.True(
                code.Contains("export interface TypeWithOneGenericAndBase<T> extends RetrieveResponse<string>") ||
                code.Contains("export interface TypeWithOneGenericAndBase<T> extends Serenity.RetrieveResponse<string>"));
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithTwoGenericParameters()
        {
            var generator = CreateGenerator(typeof(TypeWithTwoGenericParameters<,>));
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var code = Assert.Single(result).Text;
            Assert.Contains("namespace ServerTypingsTest.TypeWithGenericParameters {", code);
            Assert.True(
                code.Contains("export interface TypeWithTwoGenericParameters<T1, T2> extends ServiceRequest") ||
                code.Contains("export interface TypeWithTwoGenericParameters<T1, T2> extends Serenity.ServiceRequest"));
            Assert.Contains("SomeList1?: T1[];", code);
            Assert.Contains("SomeList2?: T2[];", code);
            Assert.Contains("SomeDictionary?: { [key: T1]: T2 };", code);
        }
    }
}

namespace ServerTypingsTest.TypeWithGenericParameters
{
    [ScriptInclude]
    public class TypeWithOneGenericParameter<T> : ServiceRequest
    {
        public List<T> SomeList { get; set; }
        public Dictionary<string, T> SomeDictionary { get; set; }
    }

    [ScriptInclude]
    public class TypeWithOneGenericAndBase<T> : RetrieveResponse<string>
    {
        public List<T> SomeList { get; set; }
        public Dictionary<string, T> SomeDictionary { get; set; }
    }

    [ScriptInclude]
    public class TypeWithTwoGenericParameters<T1, T2> : ServiceRequest
    {
        public List<T1> SomeList1 { get; set; }
        public List<T2> SomeList2 { get; set; }
        public Dictionary<T1, T2> SomeDictionary { get; set; }
    }
}