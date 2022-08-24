using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Services;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void HandlesTypeWithOneGenericParameter()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "Tests.CodeGenerator.TypeWithOneGenericParameter`1.ts";
            var code = Assert.Single(result, x => x.Filename == expectedFile).Text;
            Assert.Contains("namespace Serenity.Tests.CodeGenerator {", code);
            Assert.True(
                code.Contains("export interface TypeWithOneGenericParameter<T> extends ServiceRequest") ||
                code.Contains("export interface TypeWithOneGenericParameter<T> extends Serenity.ServiceRequest"));
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithOneGenericParameterAndBaseClass()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "Tests.CodeGenerator.TypeWithOneGenericAndBase`1.ts";
            var code = Assert.Single(result, x => x.Filename == expectedFile).Text;
            Assert.Contains("namespace Serenity.Tests.CodeGenerator {", code);
            Assert.True(
                code.Contains("export interface TypeWithOneGenericAndBase<T> extends RetrieveResponse<string>") ||
                code.Contains("export interface TypeWithOneGenericAndBase<T> extends Serenity.RetrieveResponse<string>"));
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithTwoGenericParameters()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "Tests.CodeGenerator.TypeWithTwoGenericParameters`2.ts";
            var code = Assert.Single(result, x => x.Filename == expectedFile).Text;
            Assert.Contains("namespace Serenity.Tests.CodeGenerator {", code);
            Assert.True(
                code.Contains("export interface TypeWithTwoGenericParameters<T1, T2> extends ServiceRequest") ||
                code.Contains("export interface TypeWithTwoGenericParameters<T1, T2> extends Serenity.ServiceRequest"));
            Assert.Contains("SomeList1?: T1[];", code);
            Assert.Contains("SomeList2?: T2[];", code);
            Assert.Contains("SomeDictionary?: { [key: T1]: T2 };", code);
        }
    }

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