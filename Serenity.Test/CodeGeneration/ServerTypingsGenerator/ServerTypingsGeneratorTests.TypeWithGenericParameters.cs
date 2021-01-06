using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Services;
using System.Collections.Generic;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void HandlesTypeWithOneGenericParameter()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "CodeGeneration.Test.TypeWithOneGenericParameter`1.ts";
            Assert.Contains(expectedFile, result.Keys);
            var code = result[expectedFile];
            Assert.Contains("namespace Serenity.CodeGeneration.Test {", code);
            Assert.Contains("export interface TypeWithOneGenericParameter<T> extends Serenity.ServiceRequest", code);
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithOneGenericParameterAndBaseClass()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "CodeGeneration.Test.TypeWithOneGenericAndBase`1.ts";
            Assert.Contains(expectedFile, result.Keys);
            var code = result[expectedFile];
            Assert.Contains("namespace Serenity.CodeGeneration.Test {", code);
            Assert.Contains("export interface TypeWithOneGenericAndBase<T> extends Serenity.RetrieveResponse<string>", code);
            Assert.Contains("SomeList?: T[];", code);
            Assert.Contains("SomeDictionary?: { [key: string]: T };", code);
        }

        [Fact]
        public void HandlesTypeWithTwoGenericParameters()
        {
            var generator = CreateGenerator();
            generator.RootNamespaces.Add("Serenity");
            var result = generator.Run();
            var expectedFile = "CodeGeneration.Test.TypeWithTwoGenericParameters`2.ts";
            Assert.Contains(expectedFile, result.Keys);
            var code = result[expectedFile];
            Assert.Contains("namespace Serenity.CodeGeneration.Test {", code);
            Assert.Contains("export interface TypeWithTwoGenericParameters<T1, T2> extends Serenity.ServiceRequest", code);
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